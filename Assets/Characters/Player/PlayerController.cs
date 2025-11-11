using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public FixedJoystick joystick;
    public float joystickSensitivity = 6f;

    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    Animator animator;

    SpriteRenderer spriteRenderer;

    bool canMove = true;

    public SwordAttack swordAttack;

    HealthComponent healthComponent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(HandlePlayerDeath);
        }

    }

    private void FixedUpdate()
    {

        if (!canMove) return;

        // Combinar input do joystick e teclado
        Vector2 joystickInput = (joystick != null) ? joystick.Direction * joystickSensitivity : Vector2.zero;
        Vector2 finalMovementInput = movementInput + joystickInput;


        // Limitar magnitude para evitar velocidade dupla quando ambos são usados
        if (finalMovementInput.magnitude > 1f)
            finalMovementInput = finalMovementInput.normalized;

        // If movement input is not 0, try to move
        if (finalMovementInput != Vector2.zero)
        {
            bool success = TryMove(finalMovementInput);

            if (!success)
            {
                // Try moving on X axis only
                success = TryMove(new Vector2(finalMovementInput.x, 0));

                if (!success)
                {
                    // Try moving on Y axis only
                    success = TryMove(new Vector2(0, finalMovementInput.y));
                }
            }

            // Only play animation if movement was successful
            animator.SetBool("isMoving", success);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        // Update sprite direction based on movement input
        if (finalMovementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (finalMovementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            // Check for potential collisions
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.deltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
                return true;
            }
        }
        return false;
    }



    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack()
    {
        LockMovement();

        if (spriteRenderer.flipX)
        {
            swordAttack.AttackLeft();
        }
        else
        {
            swordAttack.AttackRight();
        }
    }

    public void StopSwordAttack()
    {
        swordAttack.StopAttack();
        UnlockMovement();
    }

    public void LockMovement()
    {
        canMove = false;
    }
    public void UnlockMovement()
    {
        canMove = true;
    }

    void HandlePlayerDeath()
    {
        // TODO: Implement player defeat logic
        // For example: restart level, show game over screen, etc.
    }

    public void TakeDamage(int damage)
    {
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
    }

}
