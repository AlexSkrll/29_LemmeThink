using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemy
{
    private Rigidbody2D rb;
    private Animator anim;
    public EnemySpawner spawner;
    private Transform player;
    //movement
    [SerializeField] private float speed;
    [SerializeField] private float attackDistance;
    //health
    private int maxHealth = 1;
    private int currentHealth;
    //Attack
    public Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackOffset;
    public LayerMask playerLayer;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawner = transform.parent.GetComponent<EnemySpawner>();

        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = player.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * speed;
        anim.SetTrigger("Running");

        if (Vector3.Distance(transform.position, targetPosition) <= attackDistance)
        {
            rb.velocity = Vector3.zero;
            anim.SetTrigger("isAttacking");
        }
    }
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Attack()
    {

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            //Debug.Log("EnemyHit");
            TezaController playerHealth = player.GetComponent<TezaController>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

        }

    }
    private void attackPointPosition()
    {
        float attackX = anim.GetFloat("moveX");
        float attackY = anim.GetFloat("moveY");
        if (attackX != 0 && attackY != 0)
        {
            attackX = 0;
        }
        Vector2 attackDirection = new Vector2(attackX, attackY).normalized;
        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * attackOffset;
        attackPoint.position = attackPointPosition;
        if (attackY == 0)
        {
            attackPoint.position = new Vector2(attackPoint.position.x, attackPoint.position.y - 10f);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void Die()
    {
        spawner.EnemyDestroyed();
        GameManager.instance.IncrementKillCount();
        Destroy(gameObject);
    }
}
