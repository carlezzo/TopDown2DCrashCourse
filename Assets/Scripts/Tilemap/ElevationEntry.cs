using UnityEngine;

/// <summary>
/// Sistema de transição de elevação usando ElevationState.
/// Adicione este script ao GameObject da escada/trigger de transição.
///
/// SETUP NO INSPECTOR:
/// 1. Adicione BoxCollider2D ao GameObject
/// 2. Marque "Is Trigger" = true
/// 3. Posicione o collider para cobrir a área da escada
/// 4. Configure os níveis de origem e destino abaixo
/// 5. IMPORTANTE: A entidade que entra no trigger DEVE ter o componente ElevationState
/// </summary>
public class ElevationEntry : MonoBehaviour
{
    [Header("Configuração de Elevação")]
    [Tooltip("Nível de origem (ex: Level0)")]
    public ElevationState.ElevationLevel fromLevel = ElevationState.ElevationLevel.Level0;

    [Tooltip("Nível de destino (ex: Level1)")]
    public ElevationState.ElevationLevel toLevel = ElevationState.ElevationLevel.Level1;

    [Tooltip("Sorting order quando no nível superior (para renderizar por cima)")]
    public int elevatedSortingOrder = 11;

    [Tooltip("Sorting order quando no nível inferior")]
    public int groundSortingOrder = 1;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Obter o componente ElevationState da entidade que entrou no trigger
        ElevationState elevationState = collision.gameObject.GetComponent<ElevationState>();

        if (elevationState == null)
        {
            // Só loga warning se for o player (outras entidades podem não ter elevação)
            if (collision.CompareTag("Player"))
            {
                Debug.LogWarning($"[ElevationEntry] {collision.gameObject.name} não possui componente ElevationState! Adicione o componente para usar o sistema de elevação.");
            }
            return;
        }

        // Verificar elevação atual e alternar entre fromLevel e toLevel
        bool isOnFromLevel = elevationState.CurrentElevation == fromLevel;

        if (isOnFromLevel)
        {
            // Entidade está no nível de origem → subir para nível de destino
            elevationState.SetElevation(toLevel);

            // Atualizar sorting order para renderização correta
            SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = elevatedSortingOrder;
            }

            if (showDebugLogs)
            {
                Debug.Log($"[ElevationEntry] {collision.gameObject.name} SUBIU de {fromLevel} para {toLevel} (sortingOrder: {elevatedSortingOrder})");
            }
        }
        else
        {
            // Entidade está no nível de destino → descer para nível de origem
            elevationState.SetElevation(fromLevel);

            // Atualizar sorting order para renderização correta
            SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = groundSortingOrder;
            }

            if (showDebugLogs)
            {
                Debug.Log($"[ElevationEntry] {collision.gameObject.name} DESCEU de {toLevel} para {fromLevel} (sortingOrder: {groundSortingOrder})");
            }
        }

        // Notificar outros componentes sobre mudança de elevação
        // O evento OnElevationChanged do ElevationState já vai disparar automaticamente
        // PlayerController e inimigos vão escutar esse evento para atualizar collision filters
    }
}
