using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
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
    private InputAction look;


    //movement
    private Vector2 movementInput;
    private Vector2 lastMoveDirection;
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
    private int maxHealth = 1;
    private int currentHealth;


    //aiming
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject gunarm;
    private bool isAiming = false;
    private Vector2 lookInput;
    private Vector2 crosshairPos;
    private Vector2 characterPos;
    //private float aimAngle;


    //shooting
    public GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = new TezaInput();

        currentHealth = maxHealth;

        Cursor.visible = false;
        crosshair.SetActive(false);
        gunarm.SetActive(false);

    }
    private void OnEnable()
    {
        move = input.Player.Move;
        move.Enable();
        move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        move.canceled += ctx => movementInput = Vector2.zero;

        look = input.Player.Look;
        look.Enable();
        look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();

        dash = input.Player.Dash;
        dash.Enable();
        dash.performed += Dash;

        fire = input.Player.Fire;
        fire.Enable();
        fire.performed += Fire;

        attack = input.Player.Attack;
        attack.Enable();
        attack.performed += Attack;

        aim = input.Player.Aim;
        aim.Enable();
        aim.started += Aim;
        aim.canceled += ctx => isAiming = false;
        aim.canceled += ctx => crosshair.SetActive(false);
        aim.canceled += ctx => gunarm.SetActive(false);
    }
    private void OnDisable()
    {
        move.Disable();
        dash.Disable();
        fire.Disable();
        attack.Disable();
        aim.Disable();
        look.Disable();

    }
    private void UpdateAnimator()
    {
        anim.SetBool("isAiming", isAiming);
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("moveX", lastMoveDirection.x);
        anim.SetFloat("moveY", lastMoveDirection.y);
        anim.SetFloat("timeSinceLastMovement", timeSinceLastMovement);
        
    }

    private void FixedUpdate()
    {
        UpdateAnimator();
        DashSliderManager();
        timeSinceLastMovement += Time.fixedDeltaTime;
        if (isAiming)
        {
            AimLogic();
            return;
        }

        rb.velocity = movementInput * speed * 100; // to * 100 exei na kanei me th klimaka twn sprites (scale*100)
        isMoving = rb.velocity.magnitude > 0.1f;

        if (isMoving)
        {
            lastMoveDirection = movementInput.normalized;
            UpdateAnimator();
            timeSinceLastMovement = 0f;
        }

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
    private void DashSliderManager()
    {
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

    private void Attack(InputAction.CallbackContext context)
    {

        attackPointPosition();

        if (context.performed)
        {
            //Debug.Log("Attack");
            anim.SetTrigger("isAttacking");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                //Debug.Log("Hit");
                IEnemy enemyHealth = enemy.GetComponent<IEnemy>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1);
                }

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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Aim(InputAction.CallbackContext context)
    {
        isAiming = true;
        isMoving = false;
        rb.velocity = Vector2.zero;
        crosshair.SetActive(true);
        gunarm.SetActive(true);
    }
    private void AimLogic()
    {
        //Crosshair

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        crosshairPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        crosshair.transform.position = crosshairPos;
        
        //GunArmRotation
        
        characterPos = transform.position;
        Vector2 crosshairPos2D = new Vector2(crosshairPos.x, crosshairPos.y);
        Vector2 direction = (crosshairPos2D - characterPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunarm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Debug.Log(angle);

        SpriteRenderer gunarmSprite = gunarm.GetComponent<SpriteRenderer>();
        if(angle > 90 || angle < -90)
        {
            gunarmSprite.flipY = true;
        }
        else
        {
             gunarmSprite.flipY = false;
        }

        if(angle >0)
        {
            gunarm.transform.localPosition = new Vector3(-4.40999985f,4.52199984f,0.5f);
        }
        else
        {
            gunarm.transform.localPosition = new Vector3(5.58f,4.52199984f,0);
        }

        anim.SetFloat("aimAngle", angle);
    }
 
    private void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && isAiming)
        {
            Vector2 crosshairPos2D = new Vector2(crosshairPos.x, crosshairPos.y); //Den mporousa na kanw aferesh Vecto3 me Vector2*
            Vector2 direction = (crosshairPos2D - characterPos).normalized;

            GameObject bullet = Instantiate(bulletPrefab, characterPos, Quaternion.identity);
            bullet.GetComponent<Bullet>().velocity = direction * bulletSpeed * 100;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Destroy(bullet, 2f);
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
    void Die()
    {
        anim.SetTrigger("Death");
    }

}
