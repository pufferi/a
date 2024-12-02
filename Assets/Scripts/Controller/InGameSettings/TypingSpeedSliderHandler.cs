using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypingSpeedSliderHandler : MonoBehaviour
{
    public Slider mySlider;

    public float TypingSpeed { private set; get; }
    void Start()
    {
        if (mySlider != null)
        {
            mySlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        TypingSpeed = value;
    }

    void OnDestroy()
    {
        if (mySlider != null)
        {
            mySlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}
