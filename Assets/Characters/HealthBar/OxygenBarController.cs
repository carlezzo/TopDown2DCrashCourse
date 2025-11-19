using UnityEngine;
using UnityEngine.UI;

public class OxygenBarController : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        // Hide();
    }

    public void SetMaxOxygen(float oxygen)
    {
        slider.maxValue = oxygen;
        slider.value = oxygen;
    }

    public void SetOxygen(float oxygen)
    {
        slider.value = oxygen;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}