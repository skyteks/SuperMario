using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MovementController
{
    public enum State
    {
        Normal,
        Super,
        Fire,
    }

    public State state;
    public float runSpeed = 8f;
    public float jumpHeight = 3f;
    public GameObject projectilePrefab;
    private bool isSprinting;
    private Vector2 rawInputMovement;

    private Transform firePoint;

    public float invulnerabilityTime = 2f;
    public float knockbackTime = 0.5f;
    public Vector2 knockBackForce = new Vector2(1f, 0.2f);

    [Space]

    public float camAheadAmount;
    public float camAheadUpOffset;
    public float camAheadSpeed;
    private Transform camTarget;

    [Space]

    public float coyoteTime = 0.1f;
    private float coyoteCounter;

    private Coroutine injuredCoroutine = null;

    protected override void Awake()
    {
        base.Awake();
        // setup script variables
        camTarget = transform.Find("CamTarget");
        firePoint = transform.Find("FirePoint");
    }

    protected override void Update()
    {
        if (!frozenInAnimation)
        {
            normalizedHorizontalSpeed = rawInputMovement.x;

            // manage coyote-time
            coyoteCounter = controller.isGrounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);

            FlipCharacter();

            float speed = isSprinting ? runSpeed : walkSpeed;
            Movement(speed);

        }
        Animate();

        MoveCamTarget();
    }

    private void MoveCamTarget()
    {
        if (camTarget != null && rawInputMovement.x != 0f)
        {
            float lerp = Mathf.Lerp(camTarget.localPosition.x, camAheadAmount * rawInputMovement.x, camAheadSpeed * Time.deltaTime);
            camTarget.localPosition = new Vector2(lerp, camAheadUpOffset);
        }
    }

    protected override void OnControllerCollision(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
        {
            return;
        }

        TilemapManager manager = hit.transform.GetComponent<TilemapManager>();
        if (manager != null)
        {
            Vector3 hitPosition = Vector3.zero;
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
            Vector3Int cellPosition = manager.WorldToCell(hitPosition);
            manager.HitTile(cellPosition, hit.normal);
        }
    }

    /*
    void OnCollisionStay2D(Collision2D collision)
    {
        // get injured on enemy touch
        EnemyController enemy = collision.transform.GetComponent<EnemyController>();
        if (enemy != null && injuredCoroutine == null)
        {
            StartCoroutine(Knockback(transform.position - collision.transform.position));
            injuredCoroutine = StartCoroutine(Invulnerability());
        }
    }
    */

    protected override void FlipCharacter()
    {
        if (normalizedHorizontalSpeed > 0.01f)
        {
            normalizedHorizontalSpeed = 1f;
            render.flipX = false;
            firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
            firePoint.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (normalizedHorizontalSpeed < -0.01f)
        {
            normalizedHorizontalSpeed = -1f;
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
            anim.SetBool("sprint", isSprinting);
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
        // jump in the air
        if (callbackContext.started && controller.isGrounded)// && coyoteCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -Physics2D.gravity.y);

            coyoteCounter = 0f;
        }

        if (callbackContext.canceled && controller.velocity.y > 0f)
        {
            controller.velocity = controller.velocity.ToWithY(controller.velocity.y * 0.5f);
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
            isSprinting = true;
        }
        else if (callbackContext.canceled)
        {
            isSprinting = false;
        }
    }

    public IEnumerator Invulnerability()
    {
        float stopAt = Time.time + invulnerabilityTime;

        // blinking invulnerability
        while (Time.time < stopAt)
        {
            render.enabled = !render.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        render.enabled = true;

        injuredCoroutine = null;
    }

    public IEnumerator Knockback(Vector3 direction)
    {
        //knockback
        frozenInAnimation = true;
        controller.velocity = new Vector2(Mathf.Sign(direction.x) * knockBackForce.x, knockBackForce.y);
        yield return new WaitForSeconds(knockbackTime);
        frozenInAnimation = false;
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

    private void SpawnProjectile()
    {
        // spawn projectile
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // flip projectile
        projectileInstance.GetComponent<Projectile>().spriteRenderer.flipX = render.flipX;
    }
}
