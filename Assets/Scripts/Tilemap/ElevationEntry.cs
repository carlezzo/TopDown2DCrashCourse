using UnityEngine;

/// <summary>
/// Sistema de transição de elevação.
/// Adicione este script ao GameObject da escada/trigger de transição.
///
/// SETUP NO INSPECTOR:
/// 1. Adicione BoxCollider2D ao GameObject
/// 2. Marque "Is Trigger" = true
/// 3. Posicione o collider para cobrir a área da escada
/// 4. Configure os layers de origem e destino abaixo
/// </summary>
public class ElevationEntry : MonoBehaviour
{
    [Header("Configuração de Elevação")]
    [Tooltip("Layer de origem (ex: ElevationLevel0)")]
    public int fromLayer = 6; // ElevationLevel0

    [Tooltip("Layer de destino (ex: ElevationLevel1)")]
    public int toLayer = 7; // ElevationLevel1

    [Tooltip("Sorting order quando no nível superior (para renderizar por cima)")]
    public int elevatedSortingOrder = 10;

    [Tooltip("Sorting order quando no nível inferior")]
    public int groundSortingOrder = 0;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar se é o Player
        if (collision.gameObject.tag != "Player")
        {
            return;
        }

        // Verificar layer atual do player e alternar
        bool isOnGroundLevel = collision.gameObject.layer == fromLayer;

        if (isOnGroundLevel)
        {
            // Player está no chão (Level 0) → subir para Level 1
            collision.gameObject.layer = toLayer;

            SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = elevatedSortingOrder;
            }

            if (showDebugLogs)
            {
                Debug.Log($"[ElevationEntry] Player SUBIU para layer {toLayer} (sortingOrder: {elevatedSortingOrder})");
            }
        }
        else
        {
            // Player está no topo (Level 1) → descer para Level 0
            collision.gameObject.layer = fromLayer;

            SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = groundSortingOrder;
            }

            if (showDebugLogs)
            {
                Debug.Log($"[ElevationEntry] Player DESCEU para layer {fromLayer} (sortingOrder: {groundSortingOrder})");
            }
        }

        // Atualizar filtro de movimento do PlayerController
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.OnElevationChanged();
        }
    }
}
