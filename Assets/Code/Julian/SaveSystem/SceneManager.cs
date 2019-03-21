using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    [SerializeField]
    private List<GameObject> m_UnitRealtime = new List<GameObject>();
    private List<SaveScenario> m_SavedScenarios = new List<SaveScenario>();
    private List<int> m_SavedScenarioIds = new List<int>();
    private SaveScenario m_CurrentScenario;


    [SerializeField]
    private Transform m_ScenarioContent;
    [SerializeField]
    private GameObject m_ScenarioPrefab;
    [SerializeField]
    private GameObject m_ScenarioPanel;
    [SerializeField]
    private List<GameObject> m_UnitPrefabs = new List<GameObject>();
    [SerializeField]
    private int m_MaxSaves;

    private bool m_PanelOpened;
    private int m_Saves;
    public int m_CurrentID;
    public string m_CurrentName;
    public string m_CurrentTime;

    private void Awake()
    {
        Instance = this;
        m_CurrentID = -1;
        m_CurrentName = "TESTSCENE" + UnityEngine.Random.Range(0, 999);
        m_CurrentTime = DateTime.Now.ToString();
        m_Saves = -1;
    }
    private void Start()
    {
        SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
    }

    public void Save()
    {
        FindUnit();
        StartCoroutine(SaveData());
    }
    public void LoadScenarios(int id, string name, string time)
    {
        if (!m_SavedScenarioIds.Contains(id))
        {
            SaveScenario scenario = Instantiate(m_ScenarioPrefab, m_ScenarioContent).GetComponent<SaveScenario>();
            scenario.SetScenarioData(id, name, time);
            m_SavedScenarios.Add(scenario);
            m_SavedScenarioIds.Add(id);
            m_Saves = id;
            //
            for (int i = 0; i < m_SavedScenarios.Count; i++)
            {
                m_SavedScenarios[i].SelectPanel(false);
                if (m_SavedScenarios[i].GetScenarioID() == id)
                {
                    m_CurrentScenario = m_SavedScenarios[i];
                    m_CurrentScenario.SelectPanel(true);
                }
            }
        }
    }
    public void UpdateScenarios(string time)
    {
        m_CurrentScenario.UpdateScenarioData(time);
    }
    public void Load(int number, string name, string time, int amount)
    {
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            Destroy(data.gameObject);
        }
        m_UnitRealtime.Clear();
        //
        m_CurrentID = number;
        m_CurrentName = name;
        m_CurrentTime = time;
        for (int i = 0; i < amount; i++)
        {
            SaveDatabase.Instance.OnLoadUnits(number, i);
        }
        for (int i = 0; i < m_SavedScenarios.Count; i++)
        {
            m_SavedScenarios[i].SelectPanel(false);
            if (m_SavedScenarios[i].GetScenarioID() == m_CurrentID)
            {
                m_CurrentScenario = m_SavedScenarios[i];
                m_CurrentScenario.SelectPanel(true);
            }
        }
    }
    public void LoadUnit(int id, string name, float posx, float posy, float posz, float rotx, float roty, float rotz, float scalex, float scaley, float scalez)
    {
        GameObject unit = Instantiate(m_UnitPrefabs[id], Vector3.zero, Quaternion.identity);
        Vector3 TemporaryPosition = new Vector3();
        TemporaryPosition.x = posx;
        TemporaryPosition.y = posy;
        TemporaryPosition.z = posz;
        Quaternion TemporaryRotation = new Quaternion();
        TemporaryRotation.x = rotx;
        TemporaryRotation.y = roty;
        TemporaryRotation.z = rotz;
        Vector3 TemporaryScale = new Vector3();
        TemporaryScale.x = scalex;
        TemporaryScale.y = scaley;
        TemporaryScale.z = scalez;

        unit.transform.position = TemporaryPosition;
        unit.transform.eulerAngles = new Vector3(TemporaryRotation.x, TemporaryRotation.y, TemporaryRotation.z);
        unit.transform.localScale = TemporaryScale;
        unit.name = name;
    }
    //Find Units
    public void FindUnit()
    {
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            m_UnitRealtime.Add(data.gameObject);
        }
    }
    IEnumerator SaveData()
    {
        yield return new WaitForSeconds(1f);
        //Save General
        SaveSystem.SaveScenario(this.gameObject, m_UnitRealtime.Count);
        Debug.LogError("SCENE SAVED");
    }
    public void SaveStorageData(int sceneid)
    {
        m_CurrentID = sceneid;
        for (int i = 0; i < m_UnitRealtime.Count; i++)
        {
            SaveSystem.SaveStandardUnit(m_UnitRealtime[i], i, m_CurrentID);
        }

        //Update Scenario UI Panel
        m_CurrentTime = DateTime.Now.ToString();

        //Clear unit lists
        if (!m_SavedScenarioIds.Contains(sceneid))
        {
            SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
        }
        else
        {
            SaveDatabase.Instance.OnUpdateScenario(m_UnitRealtime.Count, sceneid, DateTime.Now.ToString());
        }
        m_UnitRealtime.Clear();
        Debug.LogError("STORAGE SAVED");
    }
    public void TogglePanel()
    {
        m_PanelOpened = !m_PanelOpened;
        m_ScenarioPanel.SetActive(m_PanelOpened);
    }
}
