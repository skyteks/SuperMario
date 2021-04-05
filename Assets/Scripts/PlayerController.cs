using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MovementController
{
    public enum State : int
    {
        Normal,
        Super,
        Fire,
    }

    public State state;
    public BoxCollider2D normalSize;
    public BoxCollider2D superSize;
    private Bounds normalSizeBounds;
    private Bounds superSizeBounds;
    public float runSpeed = 9f;
    public float sprintSpeed = 12;
    public float jumpHeight = 3f;
    public GameObject projectilePrefab;
    private bool shouldRun;
    private float runningTimer;
    public float runToSprintTime = 2f;
    private Vector2 rawInputMovement;
    private bool rawJumpPressed;

    private Transform firePoint;

    private bool invulnerarble;
    public float growTime = 1f;
    public float invulnerabilityTime = 2f;
    public float knockbackTime = 1f;
    public Vector2 knockBackForce = new Vector2(1f, 0.2f);

    [Space]

    public float camAheadAmount;
    public float camAheadUpOffset;
    public float camAheadSpeed;
    private Transform camTarget;

    [Space]

    public float coyoteTime = 0.1f;
    private float coyoteCounter;

    private Coroutine animCoroutine = null;

    private List<KeyValuePair<float, Vector3>> contactPoints = new List<KeyValuePair<float, Vector3>>();

    protected override void Awake()
    {
        base.Awake();
        // setup script variables
        camTarget = transform.Find("CamTarget");
        firePoint = transform.Find("FirePoint");

        normalSizeBounds = new Bounds(normalSize.offset, normalSize.size);
        superSizeBounds = new Bounds(superSize.offset, superSize.size);
        normalSize.enabled = true;
        superSize.enabled = false;
        Destroy(superSize);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SetSize(state, false);
    }

    protected override void Update()
    {
        CheckGrounded();

        if (!frozenInAnimation)
        {
            normalizedHorizontalSpeed = rawInputMovement.x;

            // manage coyote-time
            coyoteCounter = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);

            runningTimer = shouldRun && Math.Abs(normalizedHorizontalSpeed) > 0.1f ? Mathf.Max(0f, runningTimer - Time.deltaTime) : runToSprintTime;

            FlipCharacter();

            float speed = shouldRun ? (runningTimer == 0f ? sprintSpeed : runSpeed) : walkSpeed;

            Movement(speed);
        }
        Animate();

        MoveCamTarget();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < contactPoints.Count; i++)
            {
                if (contactPoints[i].Key + 2f < Time.time)
                {
                    contactPoints.RemoveAt(i);
                    i--;
                }
                else
                {
                    Gizmos.DrawWireSphere(contactPoints[i].Value, 0.1f);
                }
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collision with: " + collision.collider.gameObject);

        PlatformEffector2D platformEffector = collision.gameObject.GetComponent<PlatformEffector2D>();
        if (platformEffector != null && platformEffector.useOneWay)
        {
            float angle = Vector2.Angle(Vector2.up, collision.relativeVelocity);
            if (angle < (platformEffector.rotationalOffset - platformEffector.surfaceArc * 0.5f))
            {
                print("return");
                return;
            }
        }

        TilemapManager manager = collision.transform.GetComponent<TilemapManager>();
        if (manager == null)
        {
            manager = collision.transform.GetComponentInParent<TilemapManager>();
        }
        if (manager != null)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.contacts[i].normal.y != 1f)
                {
                    float sign = -1f;
                    do
                    {
                        sign *= -1f;
                        Vector3 hitPosition = Vector3.zero;
                        hitPosition.x = collision.contacts[i].point.x - 0.01f * collision.contacts[i].normal.x;
                        hitPosition.y = collision.contacts[i].point.y - 0.01f * collision.contacts[i].normal.y;

                        Vector3Int cellPosition = manager.WorldToCell(hitPosition + Vector3.right * sign * groundCheckOffset);
                        manager.HitTile(cellPosition, collision.contacts[0].normal);
                    } while (sign > 0f);
                }
                contactPoints.Add(new KeyValuePair<float, Vector3>(Time.time, collision.contacts[0].point));
            }
        }

        TriggerObject trigger = collision.transform.GetComponent<TriggerObject>();
        Vector2 normal = (collision.transform.position - transform.position).normalized;
        if (trigger != null)
        {
            trigger.Trigger(this, normal);
        }
    }

    protected override void FlipCharacter()
    {
        if (normalizedHorizontalSpeed > 0.01f)
        {
            normalizedHorizontalSpeed = 1f;
            if (render.flipX)
            {
                runningTimer = runToSprintTime;
            }
            render.flipX = false;
            firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
            firePoint.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (normalizedHorizontalSpeed < -0.01f)
        {
            normalizedHorizontalSpeed = -1f;
            if (!render.flipX)
            {
                runningTimer = runToSprintTime;
            }
            render.flipX = true;
            firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x) * -1f, firePoint.localPosition.y, firePoint.localPosition.z);
            firePoint.localRotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            normalizedHorizontalSpeed = 0f;
        }
    }

    protected override void Animate()
    {
        base.Animate();
        if (anim != null)
        {
            anim.SetBool("duck", rawInputMovement.y < -0.01f);
        }
    }

    public void OnMovement(InputAction.CallbackContext callbackContext)
    {
        if (!enabled)
        {
            return;
        }
        rawInputMovement = callbackContext.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (!enabled)
        {
            return;
        }
        if (!frozenInAnimation)
        {
            if (callbackContext.started)
            {
                rawJumpPressed = true;
                if (isGrounded && coyoteCounter > 0f)
                {
                    Bounce();
                }
            }

            if (callbackContext.canceled && rigid.velocity.y > 0f)
            {
                rawJumpPressed = false;
                rigid.velocity = rigid.velocity.ToWithY(rigid.velocity.y * 0.5f);
            }
        }
    }

    public void OnSprint(InputAction.CallbackContext callbackContext)
    {
        if (!enabled)
        {
            return;
        }
        if (callbackContext.ReadValueAsButton())
        {
            shouldRun = true;
        }
        else if (callbackContext.canceled)
        {
            shouldRun = false;
        }
        runningTimer = runToSprintTime;
    }

    public IEnumerator Grow()
    {
        invulnerarble = true;
        ToggleFreeze(true);
        GameManager.Instance.freeze = true;
        yield return new WaitForSeconds(growTime);
        GameManager.Instance.freeze = false;
        ToggleFreeze(false);
        invulnerarble = false;
        animCoroutine = null;
    }

    public IEnumerator Invulnerability()
    {
        invulnerarble = true;
        float stopAt = Time.time + invulnerabilityTime;

        // blinking invulnerability
        while (Time.time < stopAt)
        {
            render.enabled = !render.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        render.enabled = true;
        invulnerarble = false;
        animCoroutine = null;
    }

    public IEnumerator Knockback(Vector2 direction)
    {
        //knockback
        ToggleFreeze(true);
        GameManager.Instance.freeze = true;
        rigid.velocity = new Vector2(Mathf.Sign(direction.x) * knockBackForce.x, knockBackForce.y);
        yield return new WaitForSeconds(knockbackTime);
        GameManager.Instance.freeze = false;
        ToggleFreeze(false);
    }

    public void Bounce()
    {
        rigid.velocity = rigid.velocity.ToWithY(Mathf.Sqrt((rawJumpPressed ? 4f : 2f) * jumpHeight * -Physics2D.gravity.y));

        coyoteCounter = 0f;

        GameManager.Instance.PlaySound(state == State.Normal ? "jump normal" : "jump super");
    }

    public void OnAnimationCallback(int i)
    {
        if (!enabled)
        {
            return;
        }
        // choose animation event callback by parameter
        switch (i)
        {
            case 0:
                SpawnProjectile();
                break;
            case 1:
                frozenInAnimation = false;
                break;
        }
    }

    public bool GetDamaged(Vector2 normal)
    {
        if (!invulnerarble)
        {
            State newState = State.Normal;
            switch (state)
            {
                case State.Normal:
                    Die();
                    return true;
                case State.Super:
                    newState = State.Normal;
                    break;
                case State.Fire:
                    newState = State.Super;
                    break;
            }
            SetSize(newState, false);

            StartCoroutine(Knockback(normal));
            animCoroutine = StartCoroutine(Invulnerability());
            return true;
        }
        return false;
    }

    public void CollectPowerUp(State givenState)
    {
        if (state != State.Normal && state != State.Super && givenState == State.Super)
        {
            return;
        }
        else
        {
            SetSize(givenState, true);
            GameManager.Instance.PlaySound("powerup");
            animCoroutine = StartCoroutine(Grow());
        }
    }

    private void SetSize(State newState, bool doAnim)
    {
        if (newState == State.Normal)
        {
            normalSize.offset = normalSizeBounds.center;
            normalSize.size = normalSizeBounds.size;
        }
        else
        {
            normalSize.offset = superSizeBounds.center;
            normalSize.size = superSizeBounds.size;
        }

        anim?.SetFloat("state", (int)newState);

        if (state == State.Normal && newState == State.Super)
        {
            anim?.SetTrigger("grow");
        }

        state = newState;
    }

    public void Die()
    {
        ToggleFreeze(true);
        anim?.SetTrigger("die");
        GameManager.Instance.PlaySound("die");

        normalSize.enabled = false;

        enabled = false;

        GameManager.Instance.QuitSoon();
    }

    private void SpawnProjectile()
    {
        // spawn projectile
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // flip projectile
        projectileInstance.GetComponent<Projectile>().spriteRenderer.flipX = render.flipX;
    }

    private void MoveCamTarget()
    {
        if (camTarget != null && rawInputMovement.x != 0f)
        {
            float lerp = Mathf.Lerp(camTarget.localPosition.x, camAheadAmount * rawInputMovement.x, camAheadSpeed * Time.deltaTime);
            camTarget.localPosition = new Vector2(lerp, camAheadUpOffset);
        }
    }
}
