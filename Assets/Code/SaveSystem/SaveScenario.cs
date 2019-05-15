using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.HelloAR;

public class SaveScenario : MonoBehaviour
{
    private int scenarioId;               //ID of the loaded scenario UI panel
    private string scenarioName;          //Name of the loaded scenario UI panel
    private Sprite scenarioSprite;        //Picture of the loaded scenario UI panel

    [SerializeField]
    private Text timeText;                //Text component of the time text
    [SerializeField]
    private Text nameText;                //Text component of the name text
    [SerializeField]
    private Image scenariobgImage;        //Background image of the scenario
    [SerializeField]
    private Color[] selectedColors;       //Equiped - Unequiped colors of the scenario UI panel

    //Get scenario ID from this scenario
    public int GetScenarioID()
    {
        return scenarioId;
    }
    //Select / Unselect panel and change color 
    public void SelectPanel(bool toggle)
    {
        if (!toggle) scenariobgImage.color = selectedColors[0];
        else scenariobgImage.color = selectedColors[1];
    }
    /// <summary>
    /// Set or Update data to the scenario prefab
    /// </summary>
    /// <param ID="id">ID of the saved scenario</param>
    /// <param Name="name">Name of the save scenario</param>
    /// <param Time="time">Normal UCL +1 time of saved scenario</param>
    public void SetScenarioData(int id, string name, string time)
    {
        scenarioId = id;
        scenarioName = name;
        nameText.text = name;
        timeText.text = time;
    }
    /// <summary>
    /// Update Scneario data of the UI element
    /// </summary>
    /// <param Time="time">Normal UCL +1 time</param>
    public void UpdateScenarioData(string time)
    {
        //Update scenario text component with updated save time
        timeText.text = time;
    }
    //Load scenario with the scenarioid from SaveDatabase.cs
    public void LoadScenario()
    {
        SaveDatabase.Instance.OnLoadScenario(scenarioId);
    }
    //Conformation when scenario is loaded and toggle off the loading UI elements
    public void ConfirmLoadScenario()
    {
        ChangeObjectScript.Instance.ToggleLoadingUI(false);
    }
    //Delete scenario from database and unity world
    public void DeleteScenario()
    {
        SaveDatabase.Instance.OnDeleteScenario(scenarioId);
        Destroy(gameObject);
    }
}
