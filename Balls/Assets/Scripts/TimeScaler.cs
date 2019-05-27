using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TimeScaler : MonoBehaviour
{
    public Slider slider;

    public void Awake()
    {
        Time.timeScale = slider.value;
        //slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { OnTimeScalserSliderChanged(); });
    }

    public void OnTimeScalserSliderChanged()
    {
        Time.timeScale = slider.value;
        Debug.Log(slider.value);
    }
}
