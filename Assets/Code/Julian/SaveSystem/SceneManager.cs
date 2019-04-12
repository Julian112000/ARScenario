using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using System;
using Mapbox.Unity.Ar;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Unity.Map;

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
    [SerializeField]
    private Text m_SaveInfoText;
    [SerializeField]
    private Transform[] m_MovedObjects;
    private Component m_LastPlacedAnchor = null;


    private bool m_PanelOpened;
    private int m_Saves;
    public int m_CurrentID;
    public string m_CurrentName;
    public string m_CurrentTime;
    //
    public Transform mapRootTransform;

    [SerializeField]
    private List<GameObject> currentUnits = new List<GameObject>();
    [SerializeField]
    private AbstractMap _map;
    [HideInInspector]
    public bool deviceAuthenticated = false;
    private bool gotInitialAlignment = false;

    public double m_Latitude;
    public double m_Longitude;
    private Vector3 _targetPosition;

    public Mapbox.Unity.Location.DeviceLocationProvider deviceLocation;

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
        //Load all UI panel scenarios that are listed in mysql
        SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
    }

    public void Save()
    {
        //Find Units in the scene and add them to the 'm_UnitRealtime' list
        FindUnit();
        StartCoroutine(SaveData());
    }
    public void LoadScenarios(int id, string name, string time)
    {
        //Load Scenarios as UI panels
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

    public void Load(int number, string name, string time, int amount, float latitude, float longitude)
    {
        //store Vector2 position from the gps location
        m_Latitude = latitude;
        m_Longitude = longitude;
        _targetPosition = _map.Root.TransformPoint(Conversions.GeoToWorldPosition(m_Latitude, m_Longitude, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
        for (int i = 0; i < m_MovedObjects.Length; i++)
        {
            m_MovedObjects[i].transform.position = _targetPosition;
        }

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
                m_CurrentScenario = m_SavedScenarios[i];
                m_CurrentScenario.SelectPanel(true);
            }
        }

        m_CurrentID = number;
        m_CurrentName = name;
        m_CurrentTime = time;
    }
    public void UpdateScenarios(string time)
    {
        //Update scenario time in the UI text panel
        m_CurrentScenario.UpdateScenarioData(time);
    }
    public void SetSaveInfo(string text)
    {
        //This is text for testing purposes... IGNORE
        m_SaveInfoText.text = text;
    }

    public void LoadUnit(int id, string name, 
        float posx, float posy, float posz, 
        float rotx, float roty, float rotz, 
        float scalex, float scaley, float scalez)
    {
        GameObject Unit = Instantiate(m_UnitPrefabs[id], Vector3.zero, Quaternion.identity);
        SaveData data = Unit.GetComponent<SaveData>();

        data.latitude = posx;
        data.longitude = posz;

        Unit.transform.localEulerAngles = new Vector3(rotx, roty, rotz);
        Unit.transform.localScale = new Vector3(scalex, scaley, scalez);

        Vector3 position = _targetPosition + new Vector3((float)posx, posy, (float)posz);
        Unit.transform.position = position;

        Pose pose = new Pose(position, Quaternion.identity);
        pose.position = position;
        Anchor anchor = Session.CreateAnchor(pose);

        Unit.transform.parent = anchor.transform;
        Unit.transform.localPosition = Vector3.zero;
    }

    public void SaveUnit(GameObject unit)
    {
        StartCoroutine(SaveUnitData(unit));
    }
    //Find Units
    public void FindUnit()
    {
        //Find all gameobjects with the 'SaveData.cs' script attached to it
        //Add them to the 'm_UnitRealtime''list
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            m_UnitRealtime.Add(data.gameObject);
        }
    }
    IEnumerator SaveUnitData(GameObject unit)
    {
        yield return new WaitForSeconds(0);
        SaveData data = unit.GetComponent<SaveData>();

        float lat = SaveGPS.Instance.CheckDistance(unit.transform.parent.position).x;
        float lon = SaveGPS.Instance.CheckDistance(unit.transform.parent.position).z;
        data.latitude = lat;
        data.longitude = lon;
        SaveSystem.SaveStandardUnit(unit, 0, m_CurrentID);
    }
    IEnumerator SaveData()
    {
        yield return new WaitForSeconds(1f);
        //Save General Data

        double lat = deviceLocation._currentLocation.LatitudeLongitude.x;
        double lon = deviceLocation._currentLocation.LatitudeLongitude.y;
        m_Latitude = lat;
        m_Longitude = lon;

        SaveSystem.SaveScenario(this.gameObject, m_UnitRealtime.Count, lat.ToString(), lon.ToString());
        Debug.LogWarning("SCENE SAVED...");
    }
    public void SaveStorageData(int sceneid)
    {
        m_CurrentID = sceneid;

        //Update Scenario UI Panel
        m_CurrentTime = DateTime.Now.ToString();

        for (int i = 0; i < m_UnitRealtime.Count; i++)
        {
            StartCoroutine(SaveUnitData(m_UnitRealtime[i]));
        }
        if (!m_SavedScenarioIds.Contains(sceneid))
        {
            SaveSystem.LoadScenarios(gameObject, m_MaxSaves);
        }
        else
        {
            //Update scenario data if there is already a scenario in the MYSQL database.
            double lat = deviceLocation._currentLocation.LatitudeLongitude.x;
            double lon = deviceLocation._currentLocation.LatitudeLongitude.y;
            m_Latitude = lat;
            m_Longitude = lon;
            SaveDatabase.Instance.OnUpdateScenario(m_UnitRealtime.Count, sceneid, DateTime.Now.ToString(), lat.ToString(), lon.ToString());
        }
        Debug.LogWarning("STORAGE SAVED...");
    }
    public void TogglePanel()
    {
        //Toggle the UI panel of the save system
        m_PanelOpened = !m_PanelOpened;
        m_ScenarioPanel.SetActive(m_PanelOpened);
    }
}
