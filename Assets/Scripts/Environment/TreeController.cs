using System.Collections;
using UnityEngine;

/// <summary>
/// Controla o comportamento visual e sonoro de uma árvore que pode ser cortada.
/// Integra-se com HealthComponent para reagir a dano.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class TreeController : MonoBehaviour
{
    [Header("Shake Animation Settings")]
    [Tooltip("Intensidade do tremor quando a árvore leva dano")]
    [SerializeField] private float shakeIntensity = 0.1f;

    [Tooltip("Duração do tremor em segundos")]
    [SerializeField] private float shakeDuration = 0.2f;

    [Tooltip("Velocidade do tremor (maior = mais rápido)")]
    [SerializeField] private float shakeSpeed = 30f;

    [Header("Audio Settings (Optional)")]
    [Tooltip("Som tocado quando a árvore é atingida")]
    [SerializeField] private AudioClip hitSound;

    [Tooltip("Volume do som de impacto")]
    [SerializeField][Range(0f, 1f)] private float hitVolume = 0.7f;

    [Header("Particle Effects (Optional)")]
    [Tooltip("Sistema de partículas ativado ao tomar dano (ex: folhas caindo)")]
    [SerializeField] private ParticleSystem damageParticles;

    [Header("References")]
    [SerializeField] private HealthComponent healthComponent;
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        // Component Reference Pattern (Unity Best Practice)
        healthComponent ??= GetComponent<HealthComponent>();

        if (healthComponent == null)
        {
            Debug.LogError($"TreeController em {gameObject.name}: HealthComponent não encontrado! Adicione HealthComponent ao GameObject.");
        }

        // Configurar AudioSource se um som foi fornecido
        if (hitSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0.5f; // Som 2D/3D híbrido
            }
        }
    }

    void Start()
    {
        // Salvar posição original para resetar após shake
        originalPosition = transform.position;

        // Subscrever ao evento de dano do HealthComponent
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged.AddListener(HandleDamage);
        }
    }

    void OnDestroy()
    {

        // Cleanup: remover listener para prevenir memory leaks
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged.RemoveListener(HandleDamage);
        }
    }

    /// <summary>
    /// Chamado quando a árvore toma dano via evento OnHealthChanged
    /// </summary>
    /// <param name="currentHealth">Vida atual após o dano</param>
    private void HandleDamage(int currentHealth)
    {
        // Tocar animação de shake
        PlayShakeAnimation();

        // Tocar som de impacto
        PlayHitSound();

        // Ativar partículas de dano
        PlayDamageParticles();

        Debug.Log($"TreeController: {gameObject.name} levou dano! Vida restante: {currentHealth}");
    }

    /// <summary>
    /// Executa animação de tremor na árvore
    /// </summary>
    private void PlayShakeAnimation()
    {
        // Se já existe uma animação de shake rodando, para ela primeiro
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    /// <summary>
    /// Corrotina que executa o efeito de shake
    /// </summary>
    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Calcular offset aleatório baseado na intensidade
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;

            // Aplicar shake apenas no eixo X (árvore balança lateralmente)
            transform.position = originalPosition + new Vector3(offsetX, 0f, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Resetar para posição original
        transform.position = originalPosition;
        shakeCoroutine = null;
    }

    /// <summary>
    /// Toca som de impacto se configurado
    /// </summary>
    private void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound, hitVolume);
        }
    }

    /// <summary>
    /// Ativa sistema de partículas de dano se configurado
    /// </summary>
    private void PlayDamageParticles()
    {
        if (damageParticles != null)
        {
            damageParticles.Play();
        }
    }

    #region Debug Methods (Inspector Testing)

    [ContextMenu("Test Shake Animation")]
    private void TestShake()
    {
        PlayShakeAnimation();
    }

    [ContextMenu("Test Hit Sound")]
    private void TestSound()
    {
        PlayHitSound();
    }

    #endregion
}
