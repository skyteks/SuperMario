﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    public enum TriggerTypes
    {
        Add1Live,
        Add1Coin,
        PowerUp,
        Damage,
    }

    public TriggerTypes triggerType;

    public PlayerController.State powerUpType;

    public void Trigger(PlayerController player, Vector2 normal)
    {
        switch (triggerType)
        {
            case TriggerTypes.Add1Live:
                print("TODO: add extra life");
                Destroy(gameObject);
                break;
            case TriggerTypes.Add1Coin:
                print("TODO: add coin");
                Destroy(gameObject);
                break;
            case TriggerTypes.PowerUp:
                player.CollectPowerUp(powerUpType);
                Destroy(gameObject);
                break;
            case TriggerTypes.Damage:
                if (normal.y < -0.8f)
                {
                    player.Bounce();
                    Destroy(gameObject);
                }
                else
                {
                    player.GetDamaged(normal);
                }
                break;
        }
    }
}
