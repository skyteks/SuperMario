using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool facingRight;
    public float speed = 5f;
    private float input;
    public Range jumpHeight = new Range(0.8f, 4.5f);
    private Range jumpVelocity;
    public float jumpDuration = 0.5f;

    private Rigidbody2D rigid;

    private bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rigid.gravityScale = rigid.gravityScale * jumpHeight.max / Mathf.Pow(jumpDuration, 2f);
        jumpVelocity.max = -Mathf.Sqrt(2f * rigid.gravityScale * jumpHeight.max);
        jumpVelocity.min = -Mathf.Sqrt(2f * rigid.gravityScale * jumpHeight.min);
    }

    void FixedUpdate()
    {
        input = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(input * speed, rigid.velocity.y);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, whatIsGround);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpVelocity.max);
        }
        if (Input.GetButtonUp("Jump") && rigid.velocity.y < jumpVelocity.min)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpVelocity.min);
        }
    }

    private void Jump()
    {
        //JUMP
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}
