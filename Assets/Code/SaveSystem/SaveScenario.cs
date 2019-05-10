using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.HelloAR;

public class SaveScenario : MonoBehaviour
{
    private int m_ScenarioID;               //ID of the loaded scenario UI panel
    private string m_ScenarioName;          //Name of the loaded scenario UI panel
    private Sprite m_ScenarioSprite;        //Picture of the loaded scenario UI panel

    [SerializeField]
    private Text m_TimeText;                //Text component of the time text
    [SerializeField]
    private Text m_NameText;                //Text component of the name text
    [SerializeField]
    private Image m_ScenarioBGImage;        //Background image of the scenario
    [SerializeField]
    private Color[] m_SelectedColors;       //Equiped - Unequiped colors of the scenario UI panel

    //Get scenario ID from this scenario
    public int GetScenarioID()
    {
        return m_ScenarioID;
    }
    //Select / Unselect panel and change color 
    public void SelectPanel(bool toggle)
    {
        if (!toggle) m_ScenarioBGImage.color = m_SelectedColors[0];
        else m_ScenarioBGImage.color = m_SelectedColors[1];
    }
    /// <summary>
    /// Set or Update data to the scenario prefab
    /// </summary>
    /// <param ID="id">ID of the saved scenario</param>
    /// <param Name="name">Name of the save scenario</param>
    /// <param Time="time">Normal UCL +1 time of saved scenario</param>
    public void SetScenarioData(int id, string name, string time)
    {
        m_ScenarioID = id;
        m_NameText.text = name;
        m_TimeText.text = time;
    }
    /// <summary>
    /// Update Scneario data of the UI element
    /// </summary>
    /// <param Time="time">Normal UCL +1 time</param>
    public void UpdateScenarioData(string time)
    {
        //Update scenario text component with updated save time
        m_TimeText.text = time;
    }
    //Load scenario with the scenarioid from SaveDatabase.cs
    public void LoadScenario()
    {
        SaveDatabase.Instance.OnLoadScenario(m_ScenarioID);
    }
    //Conformation when scenario is loaded and toggle off the loading UI elements
    public void ConfirmLoadScenario()
    {
        ChangeObjectScript.Instance.ToggleLoadingUI(false);
    }
    //Delete scenario from database and unity world
    public void DeleteScenario()
    {
        SaveDatabase.Instance.OnDeleteScenario(m_ScenarioID);
        Destroy(gameObject);
    }
}
