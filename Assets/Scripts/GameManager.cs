using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerController player;

    private int lives;
    private int coins;

    public float timer = 250f;

    public SpriteToFont livesText;
    public SpriteToFont cointsText;
    public SpriteToFont timerText;

    public bool freeze;

    private AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
    }

    void Update()
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
        PlaySound("1-up");
    }

    public void AddCoin()
    {
        coins++;
        if (coins >= 100)
        {
            coins = 0;
            AddLive();
        }
        else
        {
        PlaySound("coin");
        }
    }

    public void PlaySound(string soundCommand)
    {
        audioPlayer?.Play(soundCommand);
    }

    public void StopPlayerMovementY()
    {
        player.StopMovementY();
    }
}
