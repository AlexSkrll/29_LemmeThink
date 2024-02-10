using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemy
{
    private Rigidbody2D rb;
    private Animator anim;
    public AudioManager audioManager;
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
    private float timesincelastattack;
    [SerializeField] private float attackDelay;
    public LayerMask playerLayer;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawner = transform.parent.GetComponent<EnemySpawner>();
        audioManager = AudioManager.FindObjectOfType<AudioManager>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        timesincelastattack +=Time.deltaTime;
    }
    void FixedUpdate()
    {
        Vector3 targetPosition = player.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * speed;
        anim.SetFloat("moveX", direction.x);
        anim.SetFloat("moveY", direction.y);

        if (Vector3.Distance(transform.position, targetPosition) <= attackDistance)
        {
            rb.velocity = Vector3.zero;
            if(timesincelastattack >= attackDelay)anim.SetTrigger("isAttacking");
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
        attackPointPosition();
        audioManager.AttackSFX();

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            //Debug.Log("EnemyHit");
            TezaController playerHealth = player.GetComponent<TezaController>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            timesincelastattack = 0;
        }
    }
    private void attackPointPosition()
    {
        float attackX = anim.GetFloat("moveX");
        float attackY = anim.GetFloat("moveY");
        Vector2 attackDirection = new Vector2(attackX, attackY).normalized;
        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * attackOffset;
        attackPoint.position = attackPointPosition;
        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void Die()
    {
        spawner.EnemyDestroyed();
        audioManager.EnemyDeathSFX();
        GameManager.instance.IncrementKillCount();
        Destroy(gameObject);
    }
}
