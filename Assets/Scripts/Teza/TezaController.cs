using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TezaController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator tezaArm;

    //input
    public TezaInput input;
    private InputAction move;
    private InputAction dash;
    private InputAction fire;
    private InputAction aim;
    private InputAction attack;
    private InputAction look;
    private InputAction block;

    //movement
    private Vector2 movementInput;
    private Vector2 lastMoveDirection;
    private bool isMoving;
    [SerializeField] private float speed;
    private float timeSinceLastMovement;

    //UI
    public Slider DashSlider;
    public Slider AttackSlider;
    public LayerMask enemyLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = new TezaInput();

        currentHealth = maxHealth;

        Cursor.visible = false;
        crosshair.SetActive(false);
        gunarm.SetActive(false);
        bulletCount = maxbulletCount;

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

        block = input.Player.Block;
        block.Enable();
        block.performed += Block;


        timeSinceLastAttack = 0.1f;
        timeSinceLastBlock = 5f;
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
        DashCounterManager();
        AttackCounterManager();
        timeSinceLastMovement += Time.fixedDeltaTime;
        timeSinceLastBlock += Time.fixedDeltaTime;
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

        if (isBlocking)
        {

            Debug.Log(isBlocking);
            blockTimer += Time.fixedDeltaTime;

            if (blockTimer >= blockDuration)
            {
                isBlocking = false;
                shield.SetActive(false);
                timeSinceLastBlock = 0;
            }
        }

    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //dash
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
    private void DashCounterManager()
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Attack
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Transform attackPoint;
    public float attackRange;
    [SerializeField] private float attackOffset;
    //AttackCounter
    [SerializeField] int maxAttacks;
    private float attackCounter;
    private bool attackExhausted = false;
    private float attackCooldownTimer;
    [SerializeField] float attackCooldownDuration;
    private float timeSinceLastAttack;
    [SerializeField] private float attackCounterResetTime;
    private bool isAttacking;
    private void Attack(InputAction.CallbackContext context)
    {

        attackPointPosition();

        if (context.performed && !attackExhausted && !isAiming)
        {
            //Debug.Log("Attack");
            tezaArm.SetTrigger("Attacking");
            anim.SetTrigger("Attacking");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                //Debug.Log("Hit");
                IEnemy enemyHealth = enemy.GetComponent<IEnemy>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1);
                    if(bulletCount< maxbulletCount) bulletCount++;
                    break;// ama den bei break skotonei osous einai sto area!!
                }

            }

            attackCounter++;
            timeSinceLastAttack = 0;
            if (attackCounter == maxAttacks) attackExhausted = true;

            
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
        attackPoint.position = new Vector2(attackPoint.position.x, attackPoint.position.y - 10f);
        if (attackY == 0)
        {
            attackPoint.position = new Vector2(attackPoint.position.x, attackPoint.position.y + 10f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }
    private void AttackCounterManager()
    {
        if (AttackSlider.value == 1)
        {
            if (attackExhausted)
            {
                attackCounter = 0;
                attackCooldownTimer = 0;
                attackExhausted = false;
            }
        }
        else if (attackCounter >= maxAttacks)
        {
            if (attackCooldownTimer >= 0)
            {
                attackCooldownTimer -= Time.fixedDeltaTime;
            }
        }
        else if (attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldownDuration;
        }

        timeSinceLastAttack += Time.fixedDeltaTime;

        if (timeSinceLastAttack >= attackCounterResetTime && attackCounter < maxAttacks && attackCounter > 0)
        {
            attackCounter = 0;
            timeSinceLastAttack = 0;
        }

        if (attackExhausted)
        {
            AttackSlider.value = Math.Min(1 - attackCooldownTimer / attackCooldownDuration, 1f);
            return;
        }
        AttackSlider.value = (maxAttacks - attackCounter) / maxAttacks;

        //ammocounter 
        AmmoText.text = "Ammo: " + bulletCount.ToString();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                        aiming
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject gunarm;
    private bool isAiming = false;
    private Vector2 lookInput;
    private Vector2 crosshairPos;
    private Vector2 characterPos;

    private float aimAngle;
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
        //Debug.Log(angle);

        SpriteRenderer gunarmSprite = gunarm.GetComponent<SpriteRenderer>();
        if (angle > 90 || angle < -90)
        {
            gunarmSprite.flipY = true;
        }
        else
        {
            gunarmSprite.flipY = false;
        }

        if (angle > 0)
        {
            gunarm.transform.localPosition = new Vector3(-4.40999985f, 4.52199984f, 0.5f);
        }
        else
        {
            gunarm.transform.localPosition = new Vector3(5.58f, 4.52199984f, -1f);
        }

        anim.SetFloat("aimAngle", angle);
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //shooting
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public GameObject bulletPrefab;
    private int bulletCount;
    public int maxbulletCount;
    public TextMeshProUGUI AmmoText;
    [SerializeField] private float bulletSpeed;
    private void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && isAiming && bulletCount >= 1f)
        {
            Vector2 crosshairPos2D = new Vector2(crosshairPos.x, crosshairPos.y); //Den mporousa na kanw aferesh Vecto3 me Vector2*
            Vector2 direction = (crosshairPos2D - characterPos).normalized;
            Vector3 bulletStartPosition = gunarm.transform.position;
            float offset = 1.0f;
            bulletStartPosition += new Vector3(direction.x * offset, direction.y * offset, 0);
            GameObject bullet = Instantiate(bulletPrefab, bulletStartPosition, Quaternion.identity);
            bullet.GetComponent<Bullet>().velocity = direction * bulletSpeed * 100;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Destroy(bullet, 2f);
            bulletCount--;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //block
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private bool isBlocking;
    private float blockDuration = 4;
    private float blockTimer;
    private float timeSinceLastBlock;
    [SerializeField] private GameObject shield;

    private void Block(InputAction.CallbackContext context)
    {
        if (context.performed && timeSinceLastBlock >5f)
        {
            isBlocking = true;
            shield.SetActive(true);
            blockTimer = 0f;
            //Debug.Log("Blocking");
        }
    }

    //health
    private int maxHealth = 1;
    private int currentHealth;
    public void TakeDamage(int damageAmount)
    {
        if (!isBlocking)
        {

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }

        }
    }
    public GameObject Enemies;

    void Die()
    {
        //anim.SetTrigger("Death");
        GameManager.instance.ToggleDeathScreen();
    }

}
