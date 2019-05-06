using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIScript : MonoBehaviour
{
    [SerializeField]
    private Text healthText, ammoText, angleText, rangeText, speedText, accuracyText;

    [SerializeField]
    private Slider healthSlider, ammoSlider, angleSlider, rangeSlider, speedSlider, accuracySlider;

    public AIStats CurrentAIStats;

    private string currentSpeedState;

    public void Update()
    {
        speedSlider.value = Mathf.Round(speedSlider.value * 2f) / 2f;

        
        if(speedSlider.value == 0)
        {
            currentSpeedState = "Stop";
        }
        else if(speedSlider.value == 0.5f)
        {
            currentSpeedState = "Slow";
        }
        else if(speedSlider.value == 1f)
        {
            currentSpeedState = "Normal";
        }
        else if (speedSlider.value == 1.5f)
        {
            currentSpeedState = "Fast";
        }
        else if (speedSlider.value == 2f)
        {
            currentSpeedState = "Max. Speed";
        }

        healthText.text = "Health: " + healthSlider.value;
        ammoText.text = "Ammo: " + ammoSlider.value + " bullets";
        angleText.text = "Viewing angle: " + angleSlider.value + "°";
        rangeText.text = "Vision Range: " + rangeSlider.value + "m";
        speedText.text = "Movement Speed: " + currentSpeedState;
        accuracyText.text = "Accuracy: " + accuracySlider.value + "%";
    }
    public void UpdateCurrentAiStats()
    {
        if(CurrentAIStats != null)
        {
            CurrentAIStats.Health = (int)healthSlider.value;
            CurrentAIStats.Ammo = (int)ammoSlider.value;
            CurrentAIStats.ViewingAngle = (int)angleSlider.value;
            CurrentAIStats.VisionRange = (int)rangeSlider.value;
            CurrentAIStats.MovementSpeed = speedSlider.value;
            CurrentAIStats.Accuracy = (int)accuracySlider.value;
        }
    }
}
