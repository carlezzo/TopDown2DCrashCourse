using UnityEngine;
using TMPro; // Se usar TextMeshPro; senão, use UnityEngine.UI para Text normal

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Arraste o Text aqui no Inspector
    public float updateInterval = 0.5f; // Atualiza a cada 0.5s pra não piscar muito

    private float accum = 0f;
    private int frames = 0;
    private float timeLeft;

    void Start()
    {
        // Força o app a rodar a 60 FPS
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (fpsText == null)
            fpsText = GetComponent<TextMeshProUGUI>(); // Se o script estiver no mesmo GO do Text

        timeLeft = updateInterval;
    }

    void Update()
    {
        float deltaTime = Time.unscaledDeltaTime; // Usa tempo real, ignora pausas
        accum += 1.0f / deltaTime;
        frames++;

        timeLeft -= deltaTime;
        if (timeLeft <= 0f)
        {
            float fps = accum / frames;
            fpsText.text = "FPS: " + Mathf.RoundToInt(fps).ToString();

            // Opcional: Muda cor baseado no FPS (verde >60, amarelo 30-60, vermelho <30)
            if (fps >= 60) fpsText.color = Color.green;
            else if (fps >= 30) fpsText.color = Color.yellow;
            else fpsText.color = Color.red;

            accum = 0f;
            frames = 0;
            timeLeft = updateInterval;
        }
    }
}