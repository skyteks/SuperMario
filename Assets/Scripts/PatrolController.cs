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

    protected override void Movement()
    {
        movementSpeed = frozenInAnimation || GameManager.Instance.freeze ? 0f : movementSpeed;

        base.Movement();
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
