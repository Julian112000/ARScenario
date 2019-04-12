using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveScenario : MonoBehaviour
{
    private int m_ScenarioID;
    private string m_ScenarioName;
    private Sprite m_ScenarioSprite;

    //Text component of the time text
    [SerializeField]
    private Text m_TimeText;
    //Text component of the name text
    [SerializeField]
    private Text m_NameText;

    //Background image of the scenario
    [SerializeField]
    private Image m_ScenarioBGImage;
    //Equiped / Unequiped colors of the scenario UI panel
    [SerializeField]
    private Color[] m_SelectedColors;

    public int GetScenarioID()
    {
        return m_ScenarioID;
    }
    public void SelectPanel(bool toggle)
    {
        //Select / Unselect panel and change color 
        if (!toggle) m_ScenarioBGImage.color = m_SelectedColors[0];
        else m_ScenarioBGImage.color = m_SelectedColors[1];
    }
    public void SetScenarioData(int id, string name, string time)
    {
        //Set data to the scenario prefab
        m_ScenarioID = id;
        m_NameText.text = name;
        m_TimeText.text = time;
    }
    public void UpdateScenarioData(string time)
    {
        //Update scenario text component with updated save time
        m_TimeText.text = time;
    }
    public void LoadScenario()
    {
        //Load scenario with the scenarioid
        SaveDatabase.Instance.OnLoadScenario(m_ScenarioID);
    }
    public void DeleteScenario()
    {
        //Delete scenario from database and unity world
        SaveDatabase.Instance.OnDeleteScenario(m_ScenarioID);
        Destroy(gameObject);
    }
}
