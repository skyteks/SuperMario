﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class MovementController : MonoBehaviour
{
    public float walkSpeed = 6f;
    protected float normalizedHorizontalSpeed;
    public LayerMask groundMask;
    public LayerMask facingMask;
    public float groundCheckOffset = 0.25f;
    protected bool isGrounded;
    protected bool isFacingWall;

    protected SpriteRenderer render;
    protected Animator anim;
    protected Rigidbody2D rigid;

    public float animSpeed = 0.2f;
    public bool frozenInAnimation { get; set; }

    protected virtual void Awake()
    {
        render = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        anim = render?.transform.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        if (anim != null)// for some reason using ?. doesn't work here
        {
            anim.SetFloat("animSpeed", animSpeed);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Vector3 pos;
        Gizmos.color = isGrounded ? Color.blue : Color.yellow;
        pos = transform.position + Vector3.right * -groundCheckOffset + Vector3.up * 0.05f;
        Gizmos.DrawRay(pos, Vector3.down * 0.1f);
        pos = transform.position + Vector3.right * groundCheckOffset + Vector3.up * 0.05f;
        Gizmos.DrawRay(pos, Vector3.down * 0.1f);

        if (render != null)
        {
            render = GetComponentInChildren<SpriteRenderer>();
        }
        if (render != null)
        {
            Gizmos.color = isFacingWall ? Color.blue : Color.yellow;
            pos = transform.position + Vector3.right * -render.flipX.ToSignFloat() * (groundCheckOffset + 0.1f) + Vector3.up * 0.5f;
            Gizmos.DrawRay(pos, Vector2.right * -render.flipX.ToSignFloat() * 0.1f);
        }
    }

    protected virtual void Update()
    {
        CheckGrounded();
        CheckFacingWall();

        if (!frozenInAnimation)
        {
            FlipCharacter();
        }
        Movement(walkSpeed);

        Animate();
    }

    protected void CheckGrounded()
    {
        Vector3 pos = transform.position + Vector3.right * -groundCheckOffset + Vector3.up * 0.05f;
        isGrounded = Physics2D.Raycast(pos, Vector3.down, 0.1f, groundMask);//Physics2D.OverlapCircle(transform.position + Vector3.right * -groundCheckOffset, 0.1f, groundMask);
        if (!isGrounded)
        {
            pos = transform.position + Vector3.right * groundCheckOffset + Vector3.up * 0.05f;
            isGrounded = Physics2D.Raycast(pos, Vector3.down, 0.1f, groundMask);//Physics2D.OverlapCircle(transform.position + Vector3.right * groundCheckOffset, 0.1f, groundMask);
        }
    }

    protected virtual void CheckFacingWall()
    {
        Vector3 pos = transform.position + Vector3.right * -render.flipX.ToSignFloat() * (groundCheckOffset + 0.1f) + Vector3.up * 0.5f;
        isFacingWall = Physics2D.Raycast(pos, Vector2.right * -render.flipX.ToSignFloat(), 0.1f, facingMask);
    }

    protected virtual void Movement(float speed)
    {
        speed = frozenInAnimation ? 0f : speed;

        rigid.velocity = rigid.velocity.ToWithX(normalizedHorizontalSpeed * speed * Time.deltaTime * 200f);

        rigid.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
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
            anim.SetFloat("xSpeed", Mathf.Abs(rigid.velocity.x));
            anim.SetFloat("ySpeed", rigid.velocity.y);
            anim.SetBool("grounded", isGrounded);
        }
    }

    public void ToggleFreeze(bool toggle)
    {
        frozenInAnimation = toggle;
        rigid?.Freeze(toggle);
    }
}
