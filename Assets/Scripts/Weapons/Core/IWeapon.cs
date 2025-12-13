using UnityEngine;

/// <summary>
/// Interface base para todas as armas do jogo.
/// Define o contrato que todas as implementações de armas devem seguir.
/// </summary>
public interface IWeapon
{
    /// <summary>
    /// Inicializa a arma com dados configurados.
    /// </summary>
    /// <param name="weaponData">Dados da arma (ScriptableObject)</param>
    void Initialize(WeaponData weaponData);

    /// <summary>
    /// Executa o ataque da arma.
    /// </summary>
    /// <param name="direction">Direção do ataque (normalizada)</param>
    /// <param name="flipX">Se o sprite está flipado horizontalmente</param>
    void Attack(Vector2 direction, bool flipX);

    /// <summary>
    /// Para o ataque atual e desativa hitboxes.
    /// </summary>
    void StopAttack();

    /// <summary>
    /// Ativa ou desativa a arma visualmente.
    /// </summary>
    /// <param name="active">True para ativar, False para desativar</param>
    void SetActive(bool active);

    /// <summary>
    /// Retorna os dados configurados da arma.
    /// </summary>
    WeaponData GetWeaponData();

    /// <summary>
    /// Verifica se a arma está atualmente ativa.
    /// </summary>
    bool IsActive();

    /// <summary>
    /// Atualiza a posição da arma baseado na direção que o personagem está olhando.
    /// Chamado continuamente durante idle, walk e attack.
    /// </summary>
    /// <param name="direction">Direção do movimento/olhar (normalizada)</param>
    /// <param name="flipX">Se o sprite está flipado horizontalmente</param>
    void UpdateDirectionalPosition(Vector2 direction, bool flipX);
}
