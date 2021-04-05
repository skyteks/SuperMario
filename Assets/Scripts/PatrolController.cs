using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MovementController
{
    public float startDirectionX = -1f;

    protected override void OnEnable()
    {
        base.OnEnable();
        normalizedHorizontalSpeed = startDirectionX;
    }

    protected override void Movement(float speed)
    {
        speed = frozenInAnimation || GameManager.Instance.freeze ? 0f : speed;

        base.Movement(speed);
    }

    protected override void CheckFacingWall()
    {
        base.CheckFacingWall();
        if (isFacingWall && !frozenInAnimation)
        {
            normalizedHorizontalSpeed *= -1f;
        }
    }
}
