using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    //Serializable
    [SerializeField]
    private List<GameObject> m_UnitPrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> currentUnits = new List<GameObject>();
    [SerializeField]
    private Transform[] m_MovedObjects;
    [SerializeField]
    private Transform m_ScenarioContent;
    [SerializeField]
    private GameObject m_ScenarioPrefab;
    [SerializeField]
    private GameObject m_ScenarioPanel;
    [SerializeField]
    private GameObject m_NewSavePanel;

    //Publics
    [SerializeField]
    private int m_MaxSaves;
    public int m_CurrentID;
    public double m_Latitude;
    public double m_Longitude;
    public string m_CurrentName;
    public string m_CurrentTime;
    public bool deviceAuthenticated = false;

    //Privates
    private List<GameObject> m_UnitRealtime = new List<GameObject>();       //list of realtime unit found by function 'FindUnit()'
    private List<SaveScenario> m_SavedScenarios = new List<SaveScenario>(); //List of saved scenario UI panels 
    private List<int> m_SavedScenarioIds = new List<int>();                 //List of saved scenario Ids
    private SaveScenario m_CurrentScenario;
    private bool m_PanelOpened;
    private bool gotInitialAlignment = false;
    private Vector3 _targetPosition;
    private Component m_LastPlacedAnchor = null;
    private bool m_NewSaveToggle;

    private void Awake()
    {
        //Set Instance of object to use static variables
        Instance = this;
        //Create start data for the scene, later it will vanish
        m_CurrentID = -1;
        //Set current name of the scene to unnamed scene
        m_CurrentName = "Unnamed Scene (id: " + UnityEngine.Random.Range(0, 999) + ")";
        //Save current time of realtime
        m_CurrentTime = DateTime.Now.ToString();
    }
    private void Start()
    {
        //Load all UI panel scenarios that are listed in mysql
        SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
    }

    public void Save(Text isnew)
    {
        //Find Units in the scene and add them to the 'm_UnitRealtime' list
        FindUnit();

        if (isnew) StartCoroutine(SaveData(isnew.text));
        else StartCoroutine(SaveData(""));
        //
        if (m_NewSavePanel.activeSelf)
            m_NewSavePanel.SetActive(false);
    }
    public void ToggleNewSave()
    {
        //Toggle Panel of creating a new save
        m_NewSaveToggle = !m_NewSaveToggle;
        m_NewSavePanel.SetActive(m_NewSaveToggle); 
    }
    public void LoadScenarios(int id, string name, string time)
    {
        //Load Scenarios as UI panels
        if (!m_SavedScenarioIds.Contains(id))
        {
            //Instantiate UI scenario panel in the content of saveBG
            SaveScenario scenario = Instantiate(m_ScenarioPrefab, m_ScenarioContent).GetComponent<SaveScenario>();
            scenario.SetScenarioData(id, name, time);   //Set scenario data like (id, name, time)
            m_SavedScenarios.Add(scenario);             //Add scenario to savedscenarios list to not get duplicated scenarios
            m_SavedScenarioIds.Add(id);                 //Add scenario id to 'm_SavedScenarioIds' list to not get duplicated scenario ids
            //
            for (int i = 0; i < m_SavedScenarios.Count; i++)
            {
                //Unselect all other panels to select the right id
                m_SavedScenarios[i].SelectPanel(false);
                if (m_SavedScenarios[i].GetScenarioID() == id)
                {
                    //Select - Highlight scenario UI panel if it is the current scenario
                    m_CurrentScenario = m_SavedScenarios[i];
                    m_CurrentScenario.SelectPanel(true);
                }
            }
        }
    }

    public void Load(int number, string name, string time, int amount, float latitude, float longitude)
    {
        //store Vector2 position from the gps location
        m_Latitude = latitude;
        m_Longitude = longitude;

        //Destroy every unit in the scene to be loaded again
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            Destroy(data.transform.parent.gameObject);
        }
        for (int i = 0; i < amount; i++)
        {
            SaveDatabase.Instance.OnLoadUnits(number, i);
        }
        //Update data of the new scenario
        for (int i = 0; i < m_SavedScenarios.Count; i++)
        {
            m_SavedScenarios[i].SelectPanel(false);
            if (m_SavedScenarios[i].GetScenarioID() == number)
            {
                //Select - Highlight scenario UI panel if it is the current scenario
                m_CurrentScenario = m_SavedScenarios[i];
                m_CurrentScenario.SelectPanel(true);
            }
        }
        m_CurrentID = number;   //Load ID of the scene from database
        m_CurrentName = name;   //Load name of the scene from databse
        m_CurrentTime = time;   //Load time of the scene from database
    }
    public void UpdateScenarios(string time)
    {
        //Update scenario time in the UI text panel
        m_CurrentScenario.UpdateScenarioData(time);
    }

    public void LoadUnit(int id, string name, 
        float posx, float posy, float posz, 
        float rotx, float roty, float rotz, 
        float scalex, float scaley, float scalez)
    {
        //Instantiate unit without position or rotation
        GameObject Unit = Instantiate(m_UnitPrefabs[id], Vector3.zero, Quaternion.identity);
        SaveData data = Unit.GetComponent<SaveData>();
        //Set Latitude of unit
        data.latitude = posx;
        //Set Luditude of unit
        data.luditude = posy;
        //Set longitude of unit
        data.longitude = posz;
        //Save position in vector3 
        Vector3 position = (m_MovedObjects[0].position + new Vector3(posx, posy, posz));
        Unit.transform.position = position;

        //Create new pose for the object position
        Pose pose = new Pose(position, Quaternion.identity);
        pose.position = position;
        //Create anchor for the object and arcore
        Anchor anchor = Session.CreateAnchor(pose);
        anchor.transform.position = position;

        //Set other veriables in unit
        data.m_SnappedPosition = position;                                  //Snapped position
        Unit.transform.parent = anchor.transform;                           //Parent unit object to anchor
        Unit.transform.localPosition = Vector3.zero;                        //Set local position to zero
        Unit.transform.localEulerAngles = new Vector3(rotx, roty, rotz);    //Set Rotation in eulerangles of the saved rotation
        Unit.transform.localScale = new Vector3(scalex, scaley, scalez);    //Set Scale of the unit
    }

    public void SaveUnit(GameObject unit, int id)
    {
        //Start coroutine for the saved unit
        StartCoroutine(SaveUnitData(unit, id));
    }
    //Find Units
    public void FindUnit()
    {
        //Find all gameobjects with the 'SaveData.cs' script attached to it
        //Add them to the 'm_UnitRealtime''list
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            if (!m_UnitRealtime.Contains(data.gameObject))
            {
                m_UnitRealtime.Add(data.gameObject);
            }
        }
    }
    IEnumerator SaveUnitData(GameObject unit, int id)
    {
        yield return new WaitForSeconds(0);
        SaveData data = unit.GetComponent<SaveData>();

        //Save distance in lat from fps camera to unit
        float lat = SaveGPS.Instance.CheckDistance(unit.transform.parent.position).x;
        //Save distance in lud from fps camera to unit
        float lud = SaveGPS.Instance.CheckDistance(unit.transform.parent.position).y;
        //Save distance in lon from fps camera to unit
        float lon = SaveGPS.Instance.CheckDistance(unit.transform.parent.position).z;
        data.latitude = lat;
        data.longitude = lon;
        data.luditude = lud;
        //Save unit with the currentid and unit object inside
        SaveSystem.SaveStandardUnit(unit, id, m_CurrentID);
    }
    IEnumerator SaveData(string isnew)
    {
        yield return new WaitForSeconds(1f);
        //Save General Data

        if (!string.IsNullOrEmpty(isnew))
            m_CurrentName = isnew;

        SaveSystem.SaveScenario(this.gameObject, m_UnitRealtime.Count, "52.5324234", "5.4532432", m_CurrentName);
        Debug.LogWarning("SCENE SAVED...");
    }
    public void SaveStorageData(int sceneid)
    {
        m_CurrentID = sceneid;

        //Update Scenario UI Panel
        m_CurrentTime = DateTime.Now.ToString();

        for (int i = 0; i < m_UnitRealtime.Count; i++)
        {
            //For every unit found save the data of that unit
            StartCoroutine(SaveUnitData(m_UnitRealtime[i], i));
        }
        if (!m_SavedScenarioIds.Contains(sceneid))
        {
            //Load scenario when there are no saved scenarios in the list
            SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
        }
        else
        {
            SaveDatabase.Instance.OnUpdateScenario(m_UnitRealtime.Count, sceneid, DateTime.Now.ToString(), "52.5324234", "5.4532432");
        }
        Debug.LogWarning("STORAGE SAVED...");
        m_UnitRealtime.Clear();
    }
    public void TogglePanel()
    {
        //Toggle the UI panel of the save system
        m_PanelOpened = !m_PanelOpened;
        m_ScenarioPanel.SetActive(m_PanelOpened);
    }
}
