using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TezaController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    //input
    public TezaInput input;
    private InputAction move;
    private InputAction dash;
    private InputAction fire;
    private InputAction aim;
    private InputAction attack;
    
    //movement
    private Vector2 movementInput;
    private bool isMoving;
    [SerializeField] private float speed;
    private float timeSinceLastMovement;
    //dash
    private bool isDashing;
    private float dashTimer;
    private bool dashExhausted = false;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashCounter;
    private float dashCooldownTimer;
    [SerializeField] private int maxDashes;
    [SerializeField] private float dashCooldownDuration;
    private float timeSinceLastDash;
    [SerializeField] private float DashCounterResetTime;
    //UI
    public Slider DashSlider;
    //Attack
    public Transform attackPoint;
    public float attackRange;
    [SerializeField] private float attackOffset;
    public LayerMask enemyLayer;
    //health
    private int maxHealth =1;
    private int currentHealth;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = new TezaInput();
        
        currentHealth = maxHealth;
    }
    private void OnEnable()
    {
        move = input.Player.Move;
        move.Enable();
        //NEW MOVEMENT*** 
        move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        move.canceled += ctx => movementInput = Vector2.zero;

        dash = input.Player.Dash;
        dash.Enable();
        dash.performed += Dash;

        fire = input.Player.Fire;
        fire.Enable();

        attack = input.Player.Attack;
        attack.Enable();
        attack.performed += Attack;

    }
    private void OnDisable()
    {
        move.Disable();
        dash.Disable();
        fire.Disable();
        attack.Disable();

    }

    private void FixedUpdate()
    {
        //HIGHLY EXCLUSIVE
        //if(isAiming){
        //code for aiming
        //return;
        //}


        //NON EXCLUSIVE
        rb.velocity = movementInput * speed * 100; // to * 100 exei na kanei me th klimaka twn sprites (scale*100)
        isMoving = rb.velocity.magnitude > 0.1f;

        anim.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            anim.SetFloat("moveX", rb.velocity.x / 100 / speed);
            anim.SetFloat("moveY", rb.velocity.y / 100 / speed);
            timeSinceLastMovement = 0f;

        }
        else
        {
            timeSinceLastMovement += Time.fixedDeltaTime;
        }

        anim.SetFloat("timeSinceLastMovement", timeSinceLastMovement);


        //PARTIALLY EXCLUSIVE
        //DASH
        if (isDashing)
        {
            //Debug.Log("isDashing");
            rb.velocity = movementInput.normalized * dashSpeed * 100;
            dashTimer += Time.fixedDeltaTime;

            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                rb.velocity = Vector2.zero;
            }
        }

        //dash slider full
        if (DashSlider.value == 1)
        {
            if (dashExhausted)
            {
                dashCounter = 0;
                dashCooldownTimer = 0;
                dashExhausted = false;
            }
        }
        else if (dashCounter >= maxDashes)
        {
            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.fixedDeltaTime;
            }
        }
        else if (dashCooldownTimer <= 0)
        {
            dashCooldownTimer = dashCooldownDuration;
        }

        timeSinceLastDash += Time.fixedDeltaTime;

        if (timeSinceLastDash >= DashCounterResetTime && dashCounter < maxDashes && dashCounter > 0)
        {
            dashCounter--;
            timeSinceLastDash = 0;
        }


        if (dashExhausted)
        {
            DashSlider.value = Math.Min(1 - dashCooldownTimer / dashCooldownDuration, 1f);
            return;
        }
        DashSlider.value = (maxDashes - dashCounter) / maxDashes;
    }

private void Update()
{
   
}
    private void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && isMoving && dashCounter < maxDashes)
        {
            //Debug.Log("DashPerformed");
            isDashing = true;
            dashTimer = 0f;
            timeSinceLastDash = 0f;
            dashCounter++;
            if (dashCounter == maxDashes) dashExhausted = true;
        }

    }

    private void Attack(InputAction.CallbackContext context)
    {
        float moveX = anim.GetFloat("moveX");
        float moveY = anim.GetFloat("moveY");
        Vector2 attackDirection = new Vector2(moveX, moveY).normalized;

        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * attackOffset;
        attackPoint.position = attackPointPosition;


        if (context.performed)
        {
            Debug.Log("Attack");
            anim.SetTrigger("isAttacking");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Hit");
                IEnemy enemyHealth = enemy.GetComponent<IEnemy>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1);
                }

            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
    void Die()
    {
        anim.SetTrigger("Death");
    }

}
