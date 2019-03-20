using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    private List<int> m_UnitDictionary = new List<int>();
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
    public List<int> GetSceneData()
    {
        return m_UnitDictionary;
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

        }
    }
    public void Load(int number, string name, string time, int amount)
    {
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            Destroy(data.gameObject);
        }
        //
        m_CurrentID = number;
        m_CurrentName = name;
        m_CurrentTime = time;
        for (int i = 0; i < amount; i++)
        {
            SaveDatabase.Instance.OnLoadUnits(number, i);
        }
        //m_CurrentScenario = m_SavedScenarios[m_CurrentID];
        //for (int i = 0; i < m_SavedScenarios.Count; i++)
        //{
        //    m_SavedScenarios[i].SelectPanel(false);
        //}
        //m_CurrentScenario.SelectPanel(true);
    }
    public void TogglePanel()
    {
        m_PanelOpened = !m_PanelOpened;
        m_ScenarioPanel.SetActive(m_PanelOpened);
    }
    public void LoadUnit(int id, float posx, float posy, float posz)
    {
        GameObject unit = Instantiate(m_UnitPrefabs[id], Vector3.zero, Quaternion.identity);
        Vector3 TemporaryPosition = new Vector3();
        TemporaryPosition.x = posx;
        TemporaryPosition.y = posy;
        TemporaryPosition.z = posz;
        //
        unit.transform.position = TemporaryPosition;

        //Quaternion TemporaryRotation = new Quaternion();
        //TemporaryRotation.x = unitdata.m_Rotation[0];
        //TemporaryRotation.y = unitdata.m_Rotation[1];
        //TemporaryRotation.z = unitdata.m_Rotation[2];

        //unit.transform.eulerAngles = new Vector3(TemporaryRotation.x, TemporaryRotation.y, TemporaryRotation.z);
        //unit.name = unitdata.m_UnitName;
    }
    //Find Units
    public void FindUnit()
    {
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            m_UnitDictionary.Add(data.GetID());
            m_UnitRealtime.Add(data.gameObject);
        }
    }
    IEnumerator SaveData()
    {
        yield return new WaitForSeconds(1f);
        //Save General
        SaveSystem.SaveSceneStats(this.gameObject, m_UnitRealtime.Count);
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

        //for (int i = 0; i < m_SavedScenarios.Count; i++)
        //{
        //    m_SavedScenarios[i].SelectPanel(false);
        //}
        //m_CurrentScenario = m_SavedScenarios[m_CurrentID];
        //m_CurrentScenario.SetScenarioData(m_CurrentID, m_CurrentName, m_CurrentTime);
        //m_CurrentScenario.SelectPanel(true);

        //Clear unit lists
        m_UnitDictionary.Clear();
        m_UnitRealtime.Clear();
        Debug.LogError("STORAGE SAVED");
        SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
    }
}
