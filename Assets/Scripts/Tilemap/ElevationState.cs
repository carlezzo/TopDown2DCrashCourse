using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente que gerencia o estado de elevação de uma entidade (Player, Enemy, NPC, etc).
/// Separa o conceito de "Layer do GameObject" (usado para identificação e colisão física)
/// do "Estado de Elevação" (usado para lógica de gameplay como detecção e interação).
///
/// Uso:
/// - Attach no Player, inimigos e NPCs que mudam de elevação
/// - Configure currentElevation no Inspector para o nível inicial
/// - Use SetElevation() para mudar de nível (chamado por ElevationEntry)
/// - Subscreva OnElevationChanged para reagir a mudanças
/// </summary>
public class ElevationState : MonoBehaviour
{
    /// <summary>
    /// Enum que define os níveis de elevação disponíveis no jogo
    /// </summary>
    public enum ElevationLevel
    {
        Level0 = 0,  // Chão/Ground
        Level1 = 1,  // Torre/Elevated
        Level2 = 2,  // [FUTURO] Níveis superiores
        Level3 = 3   // [FUTURO] Níveis superiores
    }

    [Header("Estado Atual")]
    [Tooltip("Nível de elevação atual desta entidade")]
    [SerializeField] private ElevationLevel currentElevation = ElevationLevel.Level0;

    [Header("Eventos")]
    [Tooltip("Evento disparado quando a elevação muda. Retorna (nivelAnterior, nivelNovo)")]
    public UnityEvent<ElevationLevel, ElevationLevel> OnElevationChanged;

    /// <summary>
    /// Propriedade pública para acessar o nível de elevação atual
    /// </summary>
    public ElevationLevel CurrentElevation => currentElevation;

    /// <summary>
    /// Retorna o layer de colisão correspondente ao nível de elevação atual.
    /// Usado para configurar collision filters dinamicamente.
    /// </summary>
    public int GetCollisionLayer()
    {
        switch (currentElevation)
        {
            case ElevationLevel.Level0:
                return LayerMask.NameToLayer("Collision_Level0");
            case ElevationLevel.Level1:
                return LayerMask.NameToLayer("Collision_Level1");
            case ElevationLevel.Level2:
                return LayerMask.NameToLayer("Collision_Level2");
            default:
                Debug.LogWarning($"[ElevationState] Nível de elevação {currentElevation} não possui layer de colisão mapeado. Usando Level0 como fallback.");
                return LayerMask.NameToLayer("Collision_Level0");
        }
    }

    /// <summary>
    /// Muda o nível de elevação desta entidade.
    /// Dispara o evento OnElevationChanged se o nível mudou.
    /// </summary>
    /// <param name="newElevation">Novo nível de elevação</param>
    public void SetElevation(ElevationLevel newElevation)
    {
        if (currentElevation != newElevation)
        {
            ElevationLevel previousElevation = currentElevation;
            currentElevation = newElevation;

            Debug.Log($"[ElevationState] {gameObject.name} mudou de {previousElevation} para {currentElevation}");

            // Dispara evento para outros componentes reagirem (PlayerController, Enemy, etc)
            OnElevationChanged?.Invoke(previousElevation, currentElevation);
        }
    }

    /// <summary>
    /// Verifica se esta entidade está no mesmo nível de elevação que outra entidade
    /// </summary>
    /// <param name="other">Outro ElevationState para comparar</param>
    /// <returns>True se estão no mesmo nível</returns>
    public bool IsOnSameLevel(ElevationState other)
    {
        if (other == null)
        {
            Debug.LogWarning($"[ElevationState] Tentando comparar elevação com referência nula em {gameObject.name}");
            return false;
        }

        return currentElevation == other.currentElevation;
    }

    /// <summary>
    /// Retorna uma representação legível do estado de elevação
    /// </summary>
    public override string ToString()
    {
        return $"{gameObject.name} - Elevação: {currentElevation}";
    }

    // Visualização no Inspector para debug
    private void OnValidate()
    {
        // Atualiza nome do GameObject no hierarchy para mostrar elevação (apenas em modo de edição)
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Força atualização do Inspector quando valores mudam
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}
