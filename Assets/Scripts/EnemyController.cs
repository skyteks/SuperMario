using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    public float movementSpeed;
    public LayerMask groundMask;
    private bool isGrounded;
    private SpriteRenderer render;
    private Animator anim;
    public float animSpeed;
    private Rigidbody2D rigid;
    private uint healthPoints;
    public uint maxHealthPoints;
    private Collider2D hitbox;

    [Space]

    public bool turnOnFaceCheck;
    private Transform faceCheckPoint;
    private bool walkRight;
    private bool facingWall;

    void Awake()
    {
        // setup script variables
        render = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        anim = render.transform.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        faceCheckPoint = transform.Find("FaceCheckPoint");
    }

    void Start()
    {
        // set health to max
        healthPoints = maxHealthPoints;

        // set animation speed
        anim?.SetFloat("animSpeed", animSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        if (faceCheckPoint == null)
        {
            faceCheckPoint = transform.Find("FaceCheckPoint");
        }
        if (faceCheckPoint != null && turnOnFaceCheck)
        {
            Gizmos.color = facingWall ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(faceCheckPoint.position, 0.1f);
        }
    }

    void Update()
    {
        // check if grounded
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundMask);

        // facecheck for wall
        if (turnOnFaceCheck)
        {
            facingWall = Physics2D.OverlapCircle(faceCheckPoint.position, 0.1f, groundMask);
            if (facingWall)
            {
                walkRight = !walkRight;
            }
        }

        // move horizontally
        rigid.velocity = new Vector2(movementSpeed * (walkRight ? 1f : -1f), rigid.velocity.y);

        // flip the character
        if (rigid.velocity.x > 0f)
        {
            render.flipX = true;
            faceCheckPoint.localPosition = new Vector3(Mathf.Abs(faceCheckPoint.localPosition.x) * (walkRight ? 1f : -1f), faceCheckPoint.localPosition.y, faceCheckPoint.localPosition.z);
        }
        else if (rigid.velocity.x < 0f)
        {
            render.flipX = false;
            faceCheckPoint.localPosition = new Vector3(Mathf.Abs(faceCheckPoint.localPosition.x) * (walkRight ? 1f : -1f), faceCheckPoint.localPosition.y, faceCheckPoint.localPosition.z);
        }
    }

    public void SufferAttack()
    {
        healthPoints--;

        // die
        if (healthPoints == 0)
        {
            this.enabled = false;
            hitbox.enabled = false;
            rigid.isKinematic = true;
            rigid.velocity = Vector2.zero;
            anim?.SetTrigger("die");
        }
    }

    public void AfterDeathCallback()
    {
        // destroy after death animation is finished
        Destroy(gameObject);
    }
}
