using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Normal,
        Super,
        Fire,
    }

    public State state;
    public float walkSpeed, runSpeed, jumpForce;
    public GameObject projectilePrefab;
    public LayerMask groundMask;
    public float groundCheckOffset = 0.25f;
    private bool isGrounded;
    private bool wasGrounded;
    private Vector2 lastVelocity;
    private bool isSprinting;
    private Vector2 rawInputMovement;

    private SpriteRenderer render;
    private Animator anim;
    public float animSpeed = 0.2f;

    private Rigidbody2D rigid;
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

    private bool frozenInAnimation;
    private Coroutine injuredCoroutine;

    void Awake()
    {
        // setup script variables
        render = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        anim = render.transform.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        camTarget = transform.Find("CamTarget");
        firePoint = transform.Find("FirePoint");
    }

    void Start()
    {
        // set animation speed
        anim?.SetFloat("animSpeed", animSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Physics2D.OverlapCircle(transform.position + Vector3.right * -groundCheckOffset, 0.1f, groundMask) ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * -groundCheckOffset, 0.1f);
        Gizmos.color = Physics2D.OverlapCircle(transform.position + Vector3.right * groundCheckOffset, 0.1f, groundMask) ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * groundCheckOffset, 0.1f);
    }

    void Update()
    {
        if (!frozenInAnimation)
        {
            // move horizontally
            rigid.velocity = new Vector2(rawInputMovement.x * (isSprinting ? runSpeed : walkSpeed), rigid.velocity.y);

            // check if grounded
            isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.right * -groundCheckOffset, 0.1f, groundMask);
            isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.right * groundCheckOffset, 0.1f, groundMask) || isGrounded;

            // manage coyote-time
            coyoteCounter = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteCounter - Time.deltaTime);

            // flip the character
            if (rigid.velocity.x > 0f)
            {
                render.flipX = false;
                firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
                firePoint.localRotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (rigid.velocity.x < 0f)
            {
                render.flipX = true;
                firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x) * -1f, firePoint.localPosition.y, firePoint.localPosition.z);
                firePoint.localRotation = Quaternion.Euler(0f, -90f, 0f);
            }

            // animate character
            if (anim != null)
            {
                anim.SetFloat("xSpeed", Mathf.Abs(rigid.velocity.x));
                anim.SetFloat("ySpeed", rigid.velocity.y);
                anim.SetBool("grounded", isGrounded);
                anim.SetBool("sprint", isSprinting);
                anim.SetBool("duck", rawInputMovement.y < -0.01f);
            }

            // shoot
            if (projectilePrefab != null && false)//TODO
            {
                rigid.velocity = Vector2.zero;
                anim?.SetTrigger("shoot");
                frozenInAnimation = true;
            }
        }

        // move camera point
        if (camTarget != null && rawInputMovement.x != 0f)
        {
            float lerp = Mathf.Lerp(camTarget.localPosition.x, camAheadAmount * rawInputMovement.x, camAheadSpeed * Time.deltaTime);
            camTarget.localPosition = new Vector2(lerp, camAheadUpOffset);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        InteractableBlock block = collision.transform.GetComponent<InteractableBlock>();
        if (block != null && Mathf.Abs(collision.transform.position.x - transform.position.x) < 0.8f)
        {
            if (!isGrounded && lastVelocity.y > 0f)
            {
                block.HitBlock(collision.GetContact(0).point);
            }
        }
    }

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

    void LateUpdate()
    {
        wasGrounded = isGrounded;
        lastVelocity = rigid.velocity;
    }

    public void OnMovement(InputAction.CallbackContext callbackContext)
    {
        rawInputMovement = callbackContext.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        // jump in the air
        if (callbackContext.started && coyoteCounter > 0f)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
            coyoteCounter = 0f;
        }

        if (callbackContext.canceled && rigid.velocity.y > 0f)
        {
            rigid.velocity = rigid.velocity.ToWithY(rigid.velocity.y * 0.5f);
        }
    }

    public void OnSprint(InputAction.CallbackContext callbackContext)
    {
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
        rigid.velocity = new Vector2(Mathf.Sign(direction.x) * knockBackForce.x, knockBackForce.y);
        yield return new WaitForSeconds(knockbackTime);
        frozenInAnimation = false;
    }

    public void OnAnimationCallback(int i)
    {
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
