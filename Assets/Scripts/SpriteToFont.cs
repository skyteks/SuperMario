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
        bool show = false;
        for (int digid = digids.Length - 1; digid >= 0; digid--)
        {
            int pow = (int)Mathf.Pow(10, digid);
            int rest;
            int num;
            if (temp < pow)
            {
                num = 0;
            }
            else
            {
                rest = temp % pow;
                num = (temp - rest) / pow;
            }
            show = num != 0 || digid == 0 || show;
            SetDigid(digid, num, !show);
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
