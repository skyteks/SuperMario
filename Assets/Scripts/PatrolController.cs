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

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        float smoothedMovementFactor = controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * smoothedMovementFactor);

        velocity.y += Physics2D.gravity.y * Time.deltaTime;
        if (controller.enabled)
        {
            controller.move(velocity * Time.deltaTime);
        }
        velocity = controller.velocity;
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
