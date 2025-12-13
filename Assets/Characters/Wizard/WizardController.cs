using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class WizardController : MonoBehaviour
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

    // Armazena última direção para manter animação idle correta
    private Vector2 lastDirection = Vector2.down; // Default: front

    [Header("Weapon System")]
    [SerializeField] private WeaponManager weaponManager;

    HealthComponent healthComponent;

    ElevationState elevationState;

    [System.NonSerialized] OxygenComponent oxygenComponent;
    
    // Sistema de interação com NPCs
    private NPCController nearbyNPC;

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

        oxygenComponent = GetComponent<OxygenComponent>();
        if (oxygenComponent != null)
        {
            oxygenComponent.OnOxygenDepleted.AddListener(StartSuffocation);
        }

        // Obter componente de elevação
        elevationState = GetComponent<ElevationState>();
        if (elevationState == null)
        {
            Debug.LogError("[PlayerController] ElevationState component not found! Add ElevationState component to Player.");
        }
        else
        {
            // Subscrever ao evento de mudança de elevação
            elevationState.OnElevationChanged.AddListener(OnElevationChangedHandler);
        }

        // Configurar filtro de movimento baseado na elevação atual
        UpdateMovementFilter();

        // Inicializar weapon manager
        weaponManager ??= GetComponentInChildren<WeaponManager>();
        if (weaponManager == null)
        {
            Debug.LogWarning("[WizardController] WeaponManager não encontrado! Sistema de armas não funcionará. Adicione WeaponSlot child com WeaponManager component.");
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

        // Atualizar última direção quando há movimento
        if (finalMovementInput.magnitude > 0.1f)
        {
            lastDirection = finalMovementInput.normalized;
        }

        // Enviar direção para o Animator (sempre, mesmo parado)
        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);

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

        // Flip sprite baseado na última direção (para animações side)
        if (lastDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (lastDirection.x > 0)
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

    /// <summary>
    /// Atualiza o filtro de movimento para colidir apenas com layers apropriados
    /// baseado na elevação atual do player (ElevationState).
    /// </summary>
    void UpdateMovementFilter()
    {
        if (elevationState == null)
        {
            Debug.LogWarning("[PlayerController] Cannot update movement filter - ElevationState is null!");
            return;
        }

        // Configurar filtro para usar layer mask
        movementFilter.useLayerMask = true;

        // Obter o layer de colisão apropriado baseado na elevação atual
        int collisionLayer = elevationState.GetCollisionLayer();

        // Criar layer mask que inclui APENAS o layer de colisão da elevação atual
        // Usa bit shift (1 << layer) para criar uma máscara com apenas esse layer ativo
        LayerMask mask = 1 << collisionLayer;

        movementFilter.SetLayerMask(mask);

        Debug.Log($"[PlayerController] Movement filter atualizado para elevação: {elevationState.CurrentElevation} (Collision Layer: {LayerMask.LayerToName(collisionLayer)}, Mask: {mask.value})");
    }



    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
            Debug.Log("OnFire() chamado! Disparando trigger swordAttack"); // ADICIONE ESTA LINHA

        animator.SetTrigger("swordAttack");
    }
    
    void OnInteract()
    {
        if (nearbyNPC != null)
        {
            nearbyNPC.TryInteract();
        }
    }

    /// <summary>
    /// Executa ataque com a arma equipada.
    /// Animation Event - chamado pela animação de ataque do corpo.
    /// </summary>
    public void SwordAttack()
    {
        LockMovement();

        if (weaponManager != null && weaponManager.HasWeaponEquipped())
        {
            weaponManager.Attack(lastDirection, spriteRenderer.flipX);
        }
        else
        {
            Debug.LogWarning("[WizardController] Tentou atacar sem arma equipada!");
        }
    }

    /// <summary>
    /// Para o ataque atual.
    /// Animation Event - chamado pela animação de ataque do corpo.
    /// </summary>
    public void StopSwordAttack()
    {
        weaponManager?.StopAttack();
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

    /// <summary>
    /// Equipa uma arma usando WeaponData.
    /// Chamado pelo sistema de inventário ou para testes.
    /// </summary>
    /// <param name="weaponData">Dados da arma a equipar</param>
    public void EquipWeapon(WeaponData weaponData)
    {
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponData);
        }
        else
        {
            Debug.LogError("[WizardController] WeaponManager não encontrado! Não é possível equipar arma.");
        }
    }

    /// <summary>
    /// Remove a arma atualmente equipada.
    /// </summary>
    public void UnequipWeapon()
    {
        weaponManager?.UnequipCurrentWeapon();
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



    /// <summary>
    /// Handler para o evento OnElevationChanged do ElevationState.
    /// Atualiza automaticamente o filtro de colisão quando a elevação muda.
    /// </summary>
    void OnElevationChangedHandler(ElevationState.ElevationLevel previousLevel, ElevationState.ElevationLevel newLevel)
    {
        Debug.Log($"[PlayerController] Elevação mudou de {previousLevel} para {newLevel}");
        UpdateMovementFilter();
    }

    /// <summary>
    /// Método público para atualizar o filtro quando o player mudar de elevação.
    /// DEPRECATED: Use o evento OnElevationChanged do ElevationState ao invés deste método.
    /// Mantido para compatibilidade com código legado.
    /// </summary>
    public void OnElevationChanged()
    {
        UpdateMovementFilter();
    }

    private void StartSuffocation()
    {
        // Inicia Coroutine de dano por sufocamento                                                                                              
        StartCoroutine(SuffocationDamage());
    }

    private IEnumerator SuffocationDamage()
    {
        while (oxygenComponent.GetCurrentOxygen() <= 0)
        {
            yield return new WaitForSeconds(1f); // 1 dano por segundo                                                                           
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(1);
            }
        }
    }
    
    /// <summary>
    /// Define o NPC próximo ao jogador para interação.
    /// Chamado pelo NPCController quando o jogador entra no range.
    /// </summary>
    public void SetNearbyNPC(NPCController npc)
    {
        nearbyNPC = npc;
    }
    
    /// <summary>
    /// Remove o NPC próximo ao jogador.
    /// Chamado pelo NPCController quando o jogador sai do range.
    /// </summary>
    public void ClearNearbyNPC(NPCController npc)
    {
        if (nearbyNPC == npc)
        {
            nearbyNPC = null;
        }
    }

}
