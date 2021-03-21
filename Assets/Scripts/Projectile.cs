using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float movementSpeed;

    private Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer { get; private set; }

    void Awake()
    {
        // setup script variables
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // add force to fly in straight line
        rigid.velocity = new Vector2(movementSpeed * (spriteRenderer.flipX ? -1f : 1f), 0f);

        // destroy if out of world to minimize lag
        Destroy(gameObject, 10f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // damage enemy on impact
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.SufferAttack();
        }

        // selfdestruct on impact
        Destroy(gameObject);
    }
}
