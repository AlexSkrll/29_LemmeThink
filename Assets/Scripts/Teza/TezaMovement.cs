using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TezaMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float dashSpeed;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movementInput;
    private bool isMoving;
    private float timeSinceLastMovement;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

      private void FixedUpdate()
    {
        
        rb.velocity = movementInput * speed;

        isMoving = rb.velocity.magnitude > 0.1f;

        anim.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            anim.SetFloat("moveX", rb.velocity.x);
            anim.SetFloat("moveY", rb.velocity.y);
            timeSinceLastMovement = 0f;
        }
        else
        {
            timeSinceLastMovement += Time.fixedDeltaTime;
        }

        anim.SetFloat("timeSinceLastMovement", timeSinceLastMovement);
        
    }
    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
    private void OnDash(InputValue inputValue)
    {
        
    }

}