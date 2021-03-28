using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CharacterController2D))]
public abstract class MovementController : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    protected Vector3 velocity;
    protected float normalizedHorizontalSpeed;

    protected CharacterController2D controller;
    protected SpriteRenderer render;
    protected Animator anim;
    private Rigidbody2D rigid;

    public float animSpeed = 0.2f;
    public bool frozenInAnimation { get; set; }

    protected virtual void Awake()
    {
        render = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        anim = render?.transform.GetComponent<Animator>();
        controller = GetComponent<CharacterController2D>();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        //anim?.SetFloat("animSpeed", animSpeed);
        controller.onControllerCollidedEvent += OnControllerCollision;
    }

    protected virtual void Update()
    {
        if (!frozenInAnimation)
        {
            FlipCharacter();

        }
        Movement(walkSpeed);

        Animate();
    }

    protected virtual void Movement(float speed)
    {
        speed = frozenInAnimation ? 0f : speed;

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        float smoothedMovementFactor = controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * smoothedMovementFactor);

        velocity.y += Physics2D.gravity.y * Time.deltaTime;
        controller.move(velocity * Time.deltaTime);
        velocity = controller.velocity;
    }

    protected virtual void FlipCharacter()
    {
        if (normalizedHorizontalSpeed > 0.01f)
        {
            normalizedHorizontalSpeed = 1f;
            render.flipX = false;
        }
        else if (normalizedHorizontalSpeed < -0.01f)
        {
            normalizedHorizontalSpeed = -1f;
            render.flipX = true;
        }
        else
        {
            normalizedHorizontalSpeed = 0f;
        }
    }

    protected virtual void Animate()
    {
        if (anim != null)
        {
            anim.SetFloat("xSpeed", Mathf.Abs(controller.velocity.x));
            anim.SetFloat("ySpeed", controller.velocity.y);
            anim.SetBool("grounded", controller.isGrounded);
        }
    }

    protected abstract void OnControllerCollision(RaycastHit2D hit);

    public void ToggleFreeze(bool toggle)
    {
        frozenInAnimation = toggle;
        //rigid?.Freeze(toggle);
    }
}
