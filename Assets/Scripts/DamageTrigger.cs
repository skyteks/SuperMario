using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public bool playerOnly;
    public bool instantKill = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        MovementController controller = collision.transform.GetComponent<MovementController>();
        if (controller != null)
        {
            if (playerOnly && !(controller is PlayerController))
            {
                return;
            }

            if (instantKill)
            {
                controller.Die();
            }
            else
            {
                controller.GetDamaged();
            }
        }
    }
}
