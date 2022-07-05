using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class csCardList : MonoBehaviour {
    List<csCard> cardList;
    float x, y, z;
    int backCount;

    public csCardList() { }

    public csCardList(float x, float y, float z, int backCount)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.backCount = backCount;
    }

    public void setList(List<csCard> cardList)
    {
        this.cardList = cardList;
    }

    public List<csCard> getList()
    {
        return cardList;
    }

    public float getX()
    {
        return x;
    }

    public float getY()
    {
        return y;
    }

    public float getZ()
    {
        return z;
    }

    public float getBackCount()
    {
        return backCount;
    }

    public int getListSize()
    {
        return cardList.Count;
    }

    public csCard last()
    {
        return cardList[cardList.Count - 1];
    }

    public void addCard(csCard card)
    {
        cardList.Add(card);
    }

    public void decreaseBackCount()
    {
        backCount--;
    }

    public void PrintList()
    {
        foreach(csCard c in cardList)
        {
            c.Print();
        }
        Debug.Log("x = "+x+", y = "+y+", z = "+z);
    }
}
