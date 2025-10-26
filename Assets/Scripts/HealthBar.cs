using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
       
        slider.maxValue = 100;
        slider.value = 100;
    }

    public void TakeDamage(int damage)
    {
        slider.value -= damage;

        if (slider.value < 0)
            slider.value = 0;
    }
}
