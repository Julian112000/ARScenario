using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveScenario : MonoBehaviour
{
    private int m_ScenarioID;
    private string m_ScenarioName;
    private Sprite m_ScenarioSprite;

    [SerializeField]
    private Text m_TimeText;
    [SerializeField]
    private Text m_NameText;
    [SerializeField]
    private Image m_ScenarioImage;


    [SerializeField]
    private Image m_ScenarioBGImage;
    [SerializeField]
    private Color[] m_SelectedColors;

    public int GetScenarioID()
    {
        return m_ScenarioID;
    }
    public void SelectPanel(bool toggle)
    {
        if (!toggle) m_ScenarioBGImage.color = m_SelectedColors[0];
        else m_ScenarioBGImage.color = m_SelectedColors[1];
    }
    public void SetScenarioData(int id, string name, string time)
    {
        m_ScenarioID = id;
        m_NameText.text = name;
        m_TimeText.text = time;
    }
    public void LoadScenario()
    {
        SaveDatabase.Instance.OnLoadScenario(m_ScenarioID);
    }
}
