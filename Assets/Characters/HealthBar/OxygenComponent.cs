using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OxygenComponent : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygenTime = 10f;
    public OxygenBarController oxygenBar;

    [Header("Events")]
    public UnityEvent<float> OnOxygenChanged;
    public UnityEvent OnOxygenDepleted;

    private float currentOxygen;
    private Coroutine oxygenConsumptionCoroutine;
    private bool isConsumingOxygen = false;

    void Start()
    {
        currentOxygen = maxOxygenTime;
        if (oxygenBar != null)
            oxygenBar.SetMaxOxygen(maxOxygenTime);
        
        StartOxygenConsumption();
    }

    public void StartOxygenConsumption()
    {
        if (!isConsumingOxygen)
        {
            isConsumingOxygen = true;
            oxygenConsumptionCoroutine = StartCoroutine(ConsumeOxygenOverTime());
        }
    }

    public void StopOxygenConsumption()
    {
        if (isConsumingOxygen)
        {
            isConsumingOxygen = false;
            if (oxygenConsumptionCoroutine != null)
            {
                StopCoroutine(oxygenConsumptionCoroutine);
                oxygenConsumptionCoroutine = null;
            }
        }
    }

    private IEnumerator ConsumeOxygenOverTime()
    {
        while (isConsumingOxygen && currentOxygen > 0)
        {
            yield return new WaitForSeconds(1f);
            ConsumeOxygen(1f);
        }
    }

    public void ConsumeOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygenTime);

        if (oxygenBar != null)
            oxygenBar.SetOxygen(currentOxygen);

        OnOxygenChanged?.Invoke(currentOxygen);

        if (currentOxygen <= 0f)
        {
            StopOxygenConsumption();
            OnOxygenDepleted?.Invoke();
        }
    }

    public void RestoreOxygen(float amount)
    {
        SetOxygen(currentOxygen + amount);
    }

    public void SetOxygen(float oxygen)
    {
        currentOxygen = Mathf.Clamp(oxygen, 0f, maxOxygenTime);
        
        if (oxygenBar != null)
            oxygenBar.SetOxygen(currentOxygen);
        
        OnOxygenChanged?.Invoke(currentOxygen);

        if (currentOxygen > 0f && !isConsumingOxygen)
        {
            StartOxygenConsumption();
        }
    }

    public void RefillOxygen()
    {
        SetOxygen(maxOxygenTime);
        if (!isConsumingOxygen)
            StartOxygenConsumption();
    }

    public float GetCurrentOxygen() => currentOxygen;
    public float GetMaxOxygen() => maxOxygenTime;
    public bool IsConsumingOxygen() => isConsumingOxygen;

    void OnDestroy()
    {
        StopOxygenConsumption();
    }
}