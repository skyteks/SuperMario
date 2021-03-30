using System.Collections;
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
    public bool playPowerupSpawnSound;

    public readonly float minTriggerNormalY = -0.7f;

    void Start()
    {
        if (playPowerupSpawnSound)
        {
            GameManager.Instance.PlaySound("powerup spawn");
        }
    }

    public void Trigger(PlayerController player, Vector2 normal)
    {
        switch (triggerType)
        {
            case TriggerTypes.Add1Live:
                GameManager.Instance.AddLive();
                Destroy(gameObject);
                break;
            case TriggerTypes.Add1Coin:
                GameManager.Instance.AddCoin();
                Destroy(gameObject);
                break;
            case TriggerTypes.PowerUp:
                player.CollectPowerUp(powerUpType);
                Destroy(gameObject);
                break;
            case TriggerTypes.Damage:
                if (normal.y < minTriggerNormalY)
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
