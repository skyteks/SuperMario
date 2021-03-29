using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int lives;
    private int coins;

    public float timer = 250f;

    public SpriteToFont livesText;
    public SpriteToFont cointsText;
    public SpriteToFont timerText;

    public bool freeze;

    private void Update()
    {
        timer = Mathf.Min(0f, timer - Time.deltaTime);

        SetTexts();
    }

    private void SetTexts()
    {
        if (livesText != null)
        {
            livesText.number = lives;
        }
        if (cointsText != null)
        {
            cointsText.number = coins;
        }
        if (timerText != null)
        {
            timerText.number = Mathf.Min(0, Mathf.RoundToInt(timer));
        }
    }

    public void AddLive()
    {
        lives++;
    }

    public bool AddCoin()
    {
        coins++;
        if (coins >= 100)
        {
            coins = 0;
            AddLive();
            return false;
        }
        return true;
    }
}
