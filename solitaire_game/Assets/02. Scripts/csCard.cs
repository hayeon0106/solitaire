using UnityEngine;
using System.Collections;

public class csCard : MonoBehaviour
{
    GameObject card;
    int number;
    float y;
    bool back;

    public csCard() { }
    
    public csCard(GameObject card, int number)
    {
        this.card = card;
        this.number = number;
    }

    public csCard(GameObject card, int number, bool back)
    {
        this.card = card;
        this.number = number;
        this.back = back;
    }

    public GameObject getCard()
    {
        return card;
    }

    public int getNumber()
    {
        return number;
    }

    public float getY()
    {
        return y;
    }

    public void setY(float y)
    {
        this.y = y;
    }

    public void setIsBack(bool back)
    {
        this.back = back;
    }

    public bool isBack()
    {
        return back;
    }

    public void Print()
    {
        Debug.Log("number = "+number+", y = "+y);
    }
}