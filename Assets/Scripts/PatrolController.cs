using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MovementController
{
    public float startDirectionX = -1f;

    protected override void Start()
    {
        base.Start();
        normalizedHorizontalSpeed = startDirectionX;
    }

    protected override void OnControllerCollision(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
        {
            return;
        }

        //Debug.Log("flags: " + controller.collisionState + ", hit.normal: " + hit.normal);
        if (!frozenInAnimation && hit.normal.y == 0f && (hit.normal.x < 0f || hit.normal.x > 0f))
        {
            normalizedHorizontalSpeed *= -1f;
        }
    }
}
