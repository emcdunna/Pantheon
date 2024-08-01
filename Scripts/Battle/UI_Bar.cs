
using UnityEngine;
using UnityEngine.UI;

public class UI_Bar : MonoBehaviour
{
    private Image HealthBarImage;

    // Sets the health bar value
    public void SetHealthBarValue(float value)
    {
        TryToFindImage();
        float oldValue = GetHealthBarValue();
        if(value > 1f)
        {
            value = 1f;
        }
        else if(value < 0f)
        {
            value = 0f;
        }
        HealthBarImage.fillAmount = Mathf.Lerp(oldValue, value, 0.08f);
    }

    public void SetHealthBarImmediate(float value)
    {
        TryToFindImage();
        float oldValue = GetHealthBarValue();
        if (value > 1f)
        {
            value = 1f;
        }
        else if (value < 0f)
        {
            value = 0f;
        }
        HealthBarImage.fillAmount = value;
    }


    public float GetHealthBarValue()
    {
        return HealthBarImage.fillAmount;
    }


    // Sets the health bar color
    public void SetHealthBarColor(Color healthColor)
    {
        HealthBarImage.color = healthColor;
    }


    private void Start()
    {
        TryToFindImage();
    }

    public void TryToFindImage()
    {
        HealthBarImage = GetComponent<Image>();
    }
}