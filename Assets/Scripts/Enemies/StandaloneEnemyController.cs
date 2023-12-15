using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandaloneEnemyController : MonoBehaviour , IEnemy
{
    private Rigidbody2D rb;
    //private Animator anim;
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
    public LayerMask playerLayer;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = player.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector3.Distance(transform.position, targetPosition) <= attackDistance)
        {
            rb.velocity = Vector3.zero;
            Attack();
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
        //anim.SetTrigger("isAttacking");
        
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
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void Die()
    {
        GameManager.instance.IncrementKillCount();
        Destroy(gameObject);
    }
}
