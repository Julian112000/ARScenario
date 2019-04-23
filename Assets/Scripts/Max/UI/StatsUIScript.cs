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

    public void Update()
    {
        speedSlider.value = Mathf.Round(speedSlider.value * 100f) / 100f;

        healthText.text = "Health: " + healthSlider.value;
        ammoText.text = "Ammo: " + ammoSlider.value;
        angleText.text = "Viewing angle: " + angleSlider.value;
        rangeText.text = "Vision Range: " + rangeSlider.value;
        speedText.text = "Movement Speed: " + speedSlider.value;
        accuracyText.text = "Accuracy: " + accuracySlider.value;
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
