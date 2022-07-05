using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class csPlay : MonoBehaviour {

    //시간 측정
    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    public Text message;
    public Text scoreText;
    public Text minuteText;
    public Text secondText;
    GameObject btn;

    string[] messages = { "이동할 수 없습니다.", "빈 공간이 있습니다.", "게임 승리!!" };

    int score = 0;                  //점수 기록
    int successCount = 0;           //완성한 덱 count
    int allCardCount = 13 * 4 * 2;           //전체 카드 수

    //이동에 사용하는 변수
    int hitXIndex = -1;             //hit의 x 위치로 cardList의 인덱스를 구한다.
    bool canMove = false;           //리스트 이동 가능 여부
    bool isCardPull;                //cardPull 여부
    float xInterval = 2.7f;         //카드의 x 값 간격

    //카드 객체 정보
    public GameObject[] cardType;
    public GameObject cardBack;
    int[] nCards = new int[13 * 4 * 2];     //카드 숫자 저장하는 배열
    int deckCount = 4;

    csCardList[] cardList = new csCardList[10];
    csCardList[] cardDeck = new csCardList[5];
    csCardList[] complete = new csCardList[8];
    List<csCard> moveList = new List<csCard>();

    // Use this for initialization
    void Start () {
        btn = GameObject.Find("FinishButton");
        btn.SetActive(false);


        int index = 0;  //nCards의 위치를 표시할 변수
        message.text = "";

        //카드 섞기
        mixCard();

        //---------------처음 카드 셋팅---------------

        //처음 뿌려지는 덱
        for (int i = 0; i < 10; i++)
        {
            if (i < 4)  //6장이 쌓이는 덱
            {
                cardList[i] = createCardList(index, xInterval * i, 0, 0, 6);
                index += 6;
            }
            else    //5장이 쌓이는 덱
            {
                cardList[i] = createCardList(index, xInterval * i, 0, 0, 5);
                index += 5;
            }
            //Debug.Log("카드 배열 생성 " + cardList[i].getListSize());
        }

        //클릭 시 뿌려지는 덱 10개 씩
        for (int i = 0; i < 5; i++)
        {
            cardDeck[i] = createCardList(index, 24.0f, -14.0f, 0.0f);
            index += 10;
            Instantiate(cardBack, new Vector3(24.0f, -14.0f, -0.1f), Quaternion.Euler(0.0f, 180.0f, 0.0f));
        }

        for(int i=0; i<8; i++)
        {
            complete[i] = new csCardList(i * 0.3f, -14f, -0.1f, 0);
        }
    }

    void Update()
    {
        //시간 측정
        int second = (int)(watch.ElapsedMilliseconds / 1000) % 60;
        int minute = (int)(watch.ElapsedMilliseconds / 1000) / 60;

        secondText.text = second.ToString();
        minuteText.text = minute.ToString();

        scoreText.text = score.ToString();

        //마우스가 클릭되었을 때
        if (Input.GetMouseButtonDown(0))
        {
            isCardPull = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("클릭 객체: " + hit.collider.gameObject);
                //시간 측정
                watch.Start();

                //클릭한 오브젝트가 pull 오브젝트일 때
                if (hit.collider.tag == "CardPull")
                {
                    isCardPull = true;
                    Debug.Log("카드 배분");
                    if(cardPull())
                        Destroy(hit.collider.gameObject);
                    else
                    {
                        message.text = messages[1];
                        Invoke("finishMessage", 3f);
                    }
                }

                //그 외
                else
                {
                    //---------------------hit의 위치 정보 얻기----------------------//
                    int hitYIndex = -1;                      //hit의 List 인덱스(y 값)를 저장할 공간
                    csCardList hitCardList;                   //cardList[clickX] 클릭한 객체의 csCardList

                    hitXIndex = (int)(hit.collider.transform.position.x / 2.5f);   //hit의 x 위치로 cardList의 인덱스를 구한다.


                    //Debug.Log("hitXIndex = " + hitXIndex);

                    hitCardList = cardList[hitXIndex];               //hit이 포함된 csCardList

                    int hitCardListSize = hitCardList.getListSize();
                    //Debug.Log("hitCardListSize = " + hitCardListSize);


                    // hit의 y 위치 구하기
                    for (int i = 0; i < hitCardList.getListSize(); i++)
                    {
                        if (!hitCardList.getList()[i].isBack() && hitCardList.getList()[i].getCard().transform.position.y == hit.collider.transform.position.y)
                        {
                            hitYIndex = i;
                            //Debug.Log("hit 위치 검사 끝");
                            //Debug.Log("드래그 시작 위치, x = " + hitXIndex + ", y = " + hitYIndex);

                            //Debug.Log("y가 같은 리스트[" + i + "]: " + hitCardList.getList()[i].getY());
                            //Debug.Log("클릭된 객체의 y = " + hit.collider.transform.position.y);
                            break;
                        }
                    }

                    //Debug.Log("hitYIndex = " + hitYIndex);
                    if (hitYIndex == -1) cardInPlace(hit);      //hit이 뒷면인 카드라면

                    //-----------------------조건 검사---------------------------
                    //이동하려는 오브젝트를 찾은 후 이동이 가능한지 검사
                    canMove = checkCanMove(hitCardList, hitYIndex);
                    Debug.Log("canMove: " + canMove);


                    //---------------moveList에 추가-------------------------
                    if (hitYIndex != -1 && canMove)
                    {
                        //Debug.Log("리스트에 카드 추가 시작");

                        //hitYIndex부터 마지막까지 moveList에 추가한다.
                        //Debug.Log("hitYIndex = " + hitYIndex + "\nhitCardList.getListSize = " + hitCardList.getListSize());
                        addMoveList(hitCardList, hitYIndex);

                        //----------moveList 요소를 hit의 자식으로------------
                        //Debug.Log("hit의 자식으로\nmoveList.Count = " + moveList.Count);
                        //printMoveList();

                        for (int i = 1; i < moveList.Count; i++)
                        {

                            ///Debug.Log(moveList[i] + ", 자식으로");
                            //printMoveList();
                            moveList[i].getCard().transform.parent = moveList[0].getCard().transform;
                        }
                    }
                }
            }
            else if (!Physics.Raycast(ray, out hit))
            {
                finishMessage();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (canMove && !isCardPull)
            {
                //Debug.Log("mouseUP");
                //마우스의 x 좌표
                Vector3 mouseCamera = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
                int indexFromMouse = getMouseIndex(mouseCamera, xInterval);

                //Debug.Log("이동 가능 확인\nmouseCamera.x = " + mouseCamera.x);
                //Debug.Log("이동 가능 확인\nindexFromMouse = " + indexFromMouse);
                //Debug.Log("이동 가능 확인\nhitXIndex = " + hitXIndex);
                //Debug.Log("cardList[indexFromMouse] 크기: " + cardList[indexFromMouse].getListSize());
                //Debug.Log("moveList 크기: " + moveList.Count);

                //printMoveList();

                if (checkStep(cardList[indexFromMouse]))
                {
                    //이동
                    Debug.Log("이동 가능");
                    //printMoveList();

                    //자식 해체
                    for (int i = 1; i < moveList.Count; i++)
                    {
                        Transform parentTrans = moveList[0].getCard().transform;

                        //Debug.Log(moveList[i].getNumber() + ", 부모 해제");
                        //printMoveList();

                        moveList[i].getCard().transform.parent = null;


                        //Debug.Log("부모 위치: x = " + parentTrans.position.x + ", y = " + parentTrans.transform.position.y + ", z = " + parentTrans.transform.position.z);
                        //Debug.Log("지정 위치: x = " + parentTrans.position.x + ", y = " + (parentTrans.transform.position.y - i * 1.0f) + ", z = " + (parentTrans.transform.position.z - i * 0.3f));
                        moveList[i].getCard().transform.position = new Vector3(parentTrans.position.x, parentTrans.transform.position.y - i * 1.0f, parentTrans.transform.position.z - i * 0.3f);
                        //Debug.Log("위치 지정 후: y = "+moveList[i].getCard().transform.position.y+", z = "+moveList[i].transform.position.z);
                    }

                    moveMoveList(cardList[indexFromMouse], 0, 0, -1.0f, -0.3f); //카드 이동

                    //Debug.Log("뒤면 확인\ncardList[hitXIndex] 길이 = "+cardList[hitXIndex].getListSize());
                    //Debug.Log("뒤면 확인\ncardList[hitXIndex]  = " + cardList[hitXIndex].getListSize());
                    //cardList[hitXIndex].PrintList();
                    
                    if (cardList[hitXIndex].getListSize() != 0 && cardList[hitXIndex].last().isBack())
                    {
                        //Debug.Log("카드 회전");
                        cardList[hitXIndex].last().getCard().transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                        cardList[hitXIndex].last().setIsBack(false);
                        cardList[hitXIndex].decreaseBackCount();
                    }
                    
                    //cardLotate(cardList[hitXIndex].last(), cardList[hitXIndex].getListSize());
                    score += 5;

                    successDeck();
                }
                else
                {
                    Debug.Log("이동 불가");
                    //원래 자리로
                    message.text = messages[0];
                    Invoke("finishMessage", 3f);
                    moveMoveList(cardList[hitXIndex], 0, 0, -1.0f, -0.3f);
                    score -= 3;
                }
            }
        }
    }

    //카드를 pull하는 함수
    bool cardPull()
    {
        //cardList[i] 저장할 변수
        csCardList cardListFromCardList;

        //덱에서 나올 카드 오브젝트(활성화, 위치 이동, 회전)
        GameObject cardFromDeck;
        bool pullSuccess = true;

        float y;
        float z;

        //카드가 무조건 하나 이상 있어야 함
        for(int i=0; i<cardList.Length; i++)
        {
            if (cardList[i].getListSize() == 0)
            {
                pullSuccess = false;
                break;
            }
        }

        if (allCardCount < 13)
            pullSuccess = true;

        if (!pullSuccess)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < cardList.Length; i++)
            {
                //cardList[i]
                cardListFromCardList = cardList[i];

                //덱에서 나올 카드
                cardFromDeck = cardDeck[deckCount].getList()[i].getCard();

                //리스트의 마지막 카드의 y위치 - 1.0f
                y = cardList[i].getList()[cardList[i].getList().Count - 1].getCard().transform.position.y - 1.0f;
                z = cardList[i].getListSize() * -0.3f;  //리스트에 포함된 카드 개수 * -0.3

                //cardDeck의 오브젝트를 cardList의 맨 마지막 위치로 이동
                cardFromDeck.SetActive(true);      //오브젝트를 활성화 시킨 후
                cardFromDeck.transform.position = new Vector3(cardListFromCardList.getX(), y, z);  //오브젝트의 위치를 이동시킨다.

                cardList[i].getList().Add(cardDeck[deckCount].getList()[i]);  //해당 리스트에 추가

            }
            cardDeck[deckCount] = null;
            deckCount--;

            successDeck();
            return true;
        }
    }

    //카드 리스트 생성
    private csCardList createCardList(int index, float x, float y, float z, int size)
    {
        csCardList list = new csCardList(x, y, z, size - 1);
        List<csCard> createList = new List<csCard>();
        int card = 0;
        GameObject gameObj = null;

        for (int i = 0; i < size; i++)
        {
            card = nCards[index] % 13;
            if (i + 1 == size)   //앞면 출력
            {
                gameObj = Instantiate(cardType[card], new Vector3(x, i * -0.3f, i * -0.3f), Quaternion.Euler(0.0f, 180.0f, 0.0f)) as GameObject;
                createList.Add(new csCard(gameObj, card, false));       //앞면 표시와 함께 객체 생성
                //Debug.Log("앞면");
            }
            else    //뒷면 출력
            {
                gameObj = Instantiate(cardType[card], new Vector3(x, i * -0.3f, i * -0.3f-0.1f), Quaternion.identity) as GameObject;
                createList.Add(new csCard(gameObj, card, true));
                //Debug.Log("뒷면");
            }
            list.setList(createList);
            index++;
        }
        return list;
    }

    private csCardList createCardList(int index, float x, float y, float z)
    {
        csCardList list = new csCardList(x, y, z, 10);
        List<csCard> createList = new List<csCard>();
        int card = 0;
        GameObject c = null;

        for (int i = 0; i < 10; i++)
        {
            card = nCards[index] % 13;
            c = Instantiate(cardType[card], new Vector3(x, y, z), Quaternion.Euler(0.0f, 180.0f, 0.0f)) as GameObject;   //오브젝트 생성
            c.SetActive(false);     //비활성화로 안 보이게 만든다.
            createList.Add(new csCard(c, card, false));
            //Debug.Log("Pull할 덱");

            list.setList(createList);
            index++;
        }
        return list;
    }

    //카드 섞는 함수
    private void mixCard()
    {
        int randNum = 0;
        int nTemp = 0;

        //0~51까지 범위 내의 수로 지정
        for (int i = 0; i < 13 * 4 * 2; i++)
        {
            nCards[i] = i % 52;
        }

        //랜덤수와 i의 인덱스의 값을 교체
        for (int i = 0; i < 13 * 4 * 2; i++)
        {
            randNum = Random.Range(0, 13 * 4);
            //Debug.Log (randNum);
            nTemp = nCards[i];
            nCards[i] = nCards[randNum];
            nCards[randNum] = nTemp;
        }
    }

    //움직일 수 있는지 판정
    bool checkCanMove(csCardList selectCardList, int selectYIndex)
    {
        bool canMove = false;
        if(selectCardList.getListSize()-1 == selectYIndex)
        {
            return true;
        }
        Debug.Log("checkCanMove\n검사 시작 위치(맨 아래) = " + selectCardList.getListSize());
        for (int i = selectCardList.getListSize() - 1; i >= selectYIndex; i--)
        {
            Debug.Log("checkCanMove\n검사 위치 = " + selectCardList.getListSize());
            Debug.Log("checkCarnMove\n마지막 검사 위치(클릭 위치) = " + selectYIndex);

            //이동 조건: 위 카드와 아래 카드가 연속되는 숫자일 때
            if (checkStep(selectCardList, i))
            {
                Debug.Log("이동 조건에 맞음");
                canMove = true;
                return true;
            }
            else
            {
                Debug.Log("이동 조건에 맞지 않음");
                canMove = false;
                return false;
            }
        }
        return canMove;
    }

    //같은 리스트에서 검사
    bool checkStep(csCardList cardList, int index)
    {
        return cardList.getList()[index].getNumber() + 1 == cardList.getList()[index - 1].getNumber();
    }

    //서로 다른 두 리스트 검사
    bool checkStep(csCardList lastCardList)
    {
        if (lastCardList.getListSize() == 0)
            return true;
        //Debug.Log("checkStep\n리스트의 마지막 = " + lastCardList.last().getNumber());
        //Debug.Log("checkStep\nmoveList의 시작 = " + moveList[0].getNumber());
        return lastCardList.last().getNumber() - 1 == moveList[0].getNumber();
    }

    void moveMoveList(csCardList placeCardList, int startYIndex, float correctionX, float correctionY, float correctionZ)
    {
        Debug.Log("리스트 이동");
        //Debug.Log("moveMoveListCard\nstartYIndex = "+startYIndex);
        //Debug.Log("moveMoveListCard\nmoveList.Count = " + moveList.Count + " ,selectList.Count = " + placeCardList.getList().Count);

        Transform toListTrans;
        if (placeCardList.getList().Count != 0)
        {
            toListTrans = placeCardList.getList()[placeCardList.getList().Count - 1].getCard().transform;
            //Debug.Log("moveMoveListCard\n이동할 덱의 위치(마우스가 떨어지는 곳): y = " + toListTrans.position.y);
            for (int i = startYIndex; i < moveList.Count; i++)
            {
                moveList[i].getCard().transform.position = new Vector3(toListTrans.position.x + correctionX, toListTrans.position.y + (i + 1) * correctionY, toListTrans.position.z + (i + 1) * correctionZ);
                placeCardList.getList().Add(new csCard(moveList[i].getCard(), moveList[i].getNumber(), moveList[i].isBack()));
            }
        }
        else
        {
            for (int i = startYIndex; i < moveList.Count; i++)
            {
                moveList[i].getCard().transform.position = new Vector3(placeCardList.getX() + correctionX, placeCardList.getY() + i * correctionY, placeCardList.getZ() + i * correctionZ - 0.1f);
                placeCardList.getList().Add(new csCard(moveList[i].getCard(), moveList[i].getNumber(), moveList[i].isBack()));
            }
        }
        moveList.Clear();
        Debug.Log("moveMoveListCard\nmoveList 초기화");
    }

    void printMoveList()
    {
        //  moveList 내역 확인 리스트
        for (int i = 0; i < moveList.Count; i++)
        {
            Debug.Log("-------moveList 목록--------");
            moveList[i].Print();
            Debug.Log("--------------------------");
        }
    }

    int getMouseIndex(Vector3 mouseCamera, float num)
    {
        float[] nList = new float[11];

        for (int i = 0; i < 11; i++)
        {
            nList[i] = num * i - num/2;
            //Debug.Log("getMouseIndex\nnList["+i+"] = "+nList[i]);
        }
        
        for(int i=0; i<10; i++)
        {
            if (mouseCamera.x > nList[i] && mouseCamera.x < nList[i + 1])
                return i;
        }
        return -1;
    }

    void cardInPlace(RaycastHit hit)
    {
        Debug.Log("제자리로!");
        Transform hitInPlace = hit.collider.transform;
        hit.collider.transform.position = new Vector3(hitInPlace.position.x, hitInPlace.position.y, hitInPlace.position.z);
    }

    void finishMessage()
    {
            message.text = "";
    }

    void addMoveList(csCardList fromCardList, int selectYIndex)
    {
        for (int i = selectYIndex; i < fromCardList.getListSize(); i++)
        {
            moveList.Add(new csCard(fromCardList.getList()[i].getCard(), fromCardList.getList()[i].getNumber(), fromCardList.getList()[i].isBack()));
            //printMoveList();
        }
        fromCardList.getList().RemoveRange(selectYIndex, fromCardList.getListSize() - selectYIndex);
    }

    void successDeck()
    {
        for (int i = 0; i < 10; i++)
        {
            int size = cardList[i].getListSize();
            Debug.Log("완성 확인\ncardList["+i+"] 길이 = "+size);
            Debug.Log("완성 확인\n뒷면 갯수: "+ cardList[i].getBackCount());
            Debug.Log("완성 확인\n앞면이 13장? = "+ ((size - cardList[i].getBackCount()) >= 13));
            
            if ((size - cardList[i].getBackCount()) >= 13 && checkCanMove(cardList[i], size - 13))
            {
                Debug.Log("successDeck\n이동가능? = " + checkCanMove(cardList[i], size - 13));
                Debug.Log("successDeck\n완성 조건 맞음");
                List<csCard> tmpList = new List<csCard>();
                List<csCard> listFromCardList = cardList[i].getList();
                
                //완성 위치로 이동
                for (int j = size - 1; j > size-14; j--)
                {
                    listFromCardList[j].getCard().transform.position = new Vector3(complete[successCount].getX(), complete[successCount].getY(), complete[successCount].getZ());
                    tmpList.Add(new csCard(listFromCardList[j].getCard(), listFromCardList[j].getNumber(), listFromCardList[j].isBack()));
                }

                cardList[i].PrintList();
                cardList[i].getList().RemoveRange(size-13, 13);
                cardList[i].PrintList();
                
                if (cardList[i].getListSize() != 0 && cardList[i].last().isBack())
                {
                    //Debug.Log("카드 회전");
                    cardList[i].last().getCard().transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    cardList[i].last().setIsBack(false);
                    cardList[i].decreaseBackCount();
                }
                
                complete[successCount].setList(tmpList);
                Debug.Log("완성 판정\nmoveList 초기화");
                successCount++;
                score += 10;
                allCardCount -= 13;
            }
        }
        if (successCount == 8)
        {
            Debug.Log("게임 승리");
            message.text = messages[2];
            watch.Stop();
            btn.SetActive(true);

        }
    }

    void cardLotate(csCard card, int size)
    {
        if (card.isBack() && size != 0)
        {
            Debug.Log("카드 회전");
            card.getCard().transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            card.setIsBack(false);
        }
    }
}
