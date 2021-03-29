using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteToFont : MonoBehaviour
{
    public SpriteToFontData data;
    public Image[] digids;

    public int number;
    private int lastNum;

    void Start()
    {
        Set();
    }

    void LateUpdate()
    {
        Set();
    }

    public void Set()
    {
        if (lastNum != number)
        {
            SetDigids();
            lastNum = number;
        }
    }

    private void SetDigids()
    {
        int temp = number;
        bool stillZero = false;
        for (int digid = digids.Length - 1; digid >= 0; digid--)
        {
            int pow = (int)Mathf.Pow(10, digid);
            int rest;
            int num;
            if (temp < pow)
            {
                stillZero = digid > 0;
                num = 0;
            }
            else
            {
                stillZero = false;
                rest = temp % pow;
                num = (temp - rest) / pow;
            }
            SetDigid(digid, num, stillZero);
            temp -= pow * num;
        }
    }

    private void SetDigid(int digid, int num, bool toNothing)
    {
        if (data != null && digids.Length >= digid + 1)
        {
            digids[digid].sprite = toNothing ? null : data.numbers[num];
            digids[digid].enabled = !toNothing;
        }
    }
}
