using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using System;

public class SceneManager : MonoBehaviour
{
    //Statics
    public static SceneManager Instance;        //Static instance to call from other scripts [Almost all scripts]

    //Serializable
    [SerializeField]
    private List<GameObject> unitPrefabs = new List<GameObject>();    //All unit prefabs that are in the game [soldier, terrorist, fennek etc.]
    [SerializeField]
    private List<GameObject> currentUnits = new List<GameObject>();     //List of all units when scenario is saved
    [SerializeField]
    private Transform[] movedObjects;         //Array of transforms to calculate distance to the unit [Default = fps camera]
    [SerializeField]
    private Transform scenarioContent;        //Parent transform of all UI scenarios panels
    [SerializeField]
    private GameObject scenarioPrefab;        //Prefab of scenario UI panel
    [SerializeField]
    private GameObject scenarioPanel;         //Main scenario panel to enable and disable this object
    [SerializeField]
    private GameObject newsavePanel;          //Screen pops up when new scenario is to be saved
    [SerializeField]
    private Text savingFeedback;              //Text of giving name to the saved scenario

    //Publics
    [SerializeField]
    private int maxSaves;                     //Max saves that will search the database
    public int currentId;                     //Current ID of this scenario
    public string currentName;                //Current name of this scenario
    public string currentTime;                //Current Time of thie scenario

    //Privates
    private List<GameObject> unitRealtime = new List<GameObject>();           //list of realtime unit found by function 'FindUnit()'
    private List<SaveScenario> savedScenarios = new List<SaveScenario>();     //List of saved scenario UI panels 
    private List<int> savedScenarioIds = new List<int>();                     //List of saved scenario Ids
    private SaveScenario currentScenario;                                     //Current opened UI scenario
    private bool panelOpened;                                                 //Bool to check if panel has to be opened or not
    private bool newsaveToggle;                                               //Bool to check if new save panel has to be opened or not

    private void Awake()
    {
        //Set Instance of object to use static variables
        Instance = this;
        //Create start data for the scene, later it will vanish
        currentId = -1;
        //Set current name of the scene to unnamed scene
        currentName = "Unnamed Scene (id: " + UnityEngine.Random.Range(0, 999) + ")";
        //Save current time of realtime
        currentTime = DateTime.Now.ToString();
    }
    private void Start()
    {
        //Load all UI panel scenarios that are listed in mysql
        SaveSystem.LoadScenarios(gameObject, maxSaves);
    }
    /// <summary>
    /// Void that cals when the player clicks on save scenario button and saves a new scenario
    /// </summary>
    /// <param Text Element="isnew">Component of text element of new saved panel</param>
    public void Save(Text isnew)
    {
        //Find Units in the scene and add them to the 'm_UnitRealtime' list
        FindUnit();

        //If scenario is saved from new save panel give it that name - otherwise name it untitled
        if (isnew) StartCoroutine(SaveData(isnew.text));
        else StartCoroutine(SaveData(""));
        //Disable new save panel if opened
        if (newsavePanel.activeSelf)
            newsavePanel.SetActive(false);

        //Give player feedback that the scenario is saving...
        SetSavingFeedback("Saving...", true);
    }
    //Toggle Panel of creating a new save
    public void ToggleNewSave()
    {
        newsaveToggle = !newsaveToggle;
        newsavePanel.SetActive(newsaveToggle); 
    }
    /// <summary>
    /// Load scenario from database
    /// </summary>
    /// <param ID="id">ID from the scenario database</param>
    /// <param Name="name">Name of the scenario database</param>
    /// <param Time="time">Last updated time from the database</param>
    public void LoadScenarios(int id, string name, string time)
    {
        //Load Scenarios as UI panels
        if (!savedScenarioIds.Contains(id))
        {
            //Instantiate UI scenario panel in the content of saveBG
            SaveScenario scenario = Instantiate(scenarioPrefab, scenarioContent).GetComponent<SaveScenario>();
            scenario.SetScenarioData(id, name, time);   //Set scenario data like (id, name, time)
            savedScenarios.Add(scenario);             //Add scenario to savedscenarios list to not get duplicated scenarios
            savedScenarioIds.Add(id);                 //Add scenario id to 'm_SavedScenarioIds' list to not get duplicated scenario ids
            //
            for (int i = 0; i < savedScenarios.Count; i++)
            {
                //Unselect all other panels to select the right id
                savedScenarios[i].SelectPanel(false);
                if (savedScenarios[i].GetScenarioID() == id)
                {
                    //Select - Highlight scenario UI panel if it is the current scenario
                    currentScenario = savedScenarios[i];
                    currentScenario.SelectPanel(true);
                }
            }
        }
    }
    /// <summary>
    /// Load data from database and set it to reality
    /// </summary>
    /// <param ID="number">ID from database</param>
    /// <param Name="name">Name from database</param>
    /// <param Time="time">Time from database</param>
    /// <param Unit amout="amount">Amount of units of scenario from database</param>
    /// <param Latitude="latitude">Latitude from database</param>
    /// <param Longitude="longitude">Longitude from database</param>
    public void Load(int number, string name, string time, int amount, float latitude, float longitude)
    {
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
        for (int i = 0; i < savedScenarios.Count; i++)
        {
            savedScenarios[i].SelectPanel(false);
            if (savedScenarios[i].GetScenarioID() == number)
            {
                //Select - Highlight scenario UI panel if it is the current scenario
                currentScenario = savedScenarios[i];
                currentScenario.SelectPanel(true);
            }
        }
        currentId = number;   //Load ID of the scene from database
        currentName = name;   //Load name of the scene from databse
        currentTime = time;   //Load time of the scene from database
    }
    //Update UI scenario
    public void UpdateScenarios(string time)
    {
        //Update scenario time in the UI text panel
        currentScenario.UpdateScenarioData(time);
    }
    //Load unit with all data from database
    public void LoadUnit(int id, string name, 
        float posx, float posy, float posz, 
        float rotx, float roty, float rotz, 
        float scalex, float scaley, float scalez)
    {
        //Instantiate unit without position or rotation
        GameObject Unit = Instantiate(unitPrefabs[id], Vector3.zero, Quaternion.identity);
        SaveData data = Unit.GetComponent<SaveData>();
        //Set Latitude of unit
        data.latitude = posx;
        //Set Luditude of unit
        data.luditude = posy;
        //Set longitude of unit
        data.longitude = posz;
        //Save position in vector3 
        Vector3 position = (movedObjects[0].position + new Vector3(posx, posy, posz));
        Unit.transform.position = position;

        //Create new pose for the object position
        Pose pose = new Pose(position, Quaternion.identity);
        pose.position = position;
        //Create anchor for the object and arcore
        Anchor anchor = Session.CreateAnchor(pose);
        anchor.transform.position = position;

        //Set other veriables in unit
        data.snappedPosition = position;                                  //Snapped position
        Unit.transform.parent = anchor.transform;                           //Parent unit object to anchor
        Unit.transform.localPosition = Vector3.zero;                        //Set local position to zero
        Unit.transform.localEulerAngles = new Vector3(rotx, roty, rotz);    //Set Rotation in eulerangles of the saved rotation
        Unit.transform.localScale = new Vector3(scalex, scaley, scalez);    //Set Scale of the unit
    }
    /// <summary>
    /// SetSavingFeedback() void to give player feedback what state the saving system is
    /// </summary>
    /// <param Text="text">Text giving to the feedback</param>
    /// <param Toggle="toggle">determine if it has to be closed or not</param>
    public void SetSavingFeedback(string text, bool toggle)
    {
        if (toggle) savingFeedback.transform.parent.gameObject.SetActive(true);
        else StartCoroutine(FadeOutFeedback());
        savingFeedback.transform.GetChild(0).gameObject.SetActive(toggle);
        savingFeedback.text = text;
    }
    IEnumerator FadeOutFeedback()
    {
        yield return new WaitForSeconds(1f);
        savingFeedback.transform.parent.gameObject.SetActive(false);
    }
    /// <summary>
    /// Main function to save a unit
    /// </summary>
    /// <param Unit="unit">Gameobject of the unit in the unity world</param>
    /// <param ID="id">Order of the unit in list</param>
    public void SaveUnit(GameObject unit, int id)
    {
        //Start coroutine for the saved unit
        StartCoroutine(SaveUnitData(unit, id));
    }
    //Main function to find units in the scene with savedata attached
    public void FindUnit()
    {
        //Find all gameobjects with the 'SaveData.cs' script attached to it
        //Add them to the 'm_UnitRealtime''list
        foreach (SaveData data in FindObjectsOfType(typeof(SaveData)))
        {
            //Only if the unitrealtime doesnt contain this gameobject
            if (!unitRealtime.Contains(data.gameObject))
            {
                //Add the gameobject to the list
                unitRealtime.Add(data.gameObject);
            }
        }
    }
    /// <summary>
    /// IEnumerator to store the data of the unit in the database
    /// </summary>
    /// <param Unit="unit">Gameobject of unit in unity world</param>
    /// <param name="id">Order number of unit in list</param>
    /// <returns></returns>
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
        SaveSystem.SaveStandardUnit(unit, id, currentId);
    }
    /// <summary>
    /// IEnumerator To save standard scenario data to the databse
    /// </summary>
    /// <param Name="isnew">New name giving to the scenario</param>
    /// <returns></returns>
    IEnumerator SaveData(string isnew)
    {
        yield return new WaitForSeconds(1f);
        //Save General Data

        //If isnew is not empty or null set the name
        if (!string.IsNullOrEmpty(isnew))
            currentName = isnew;

        //Save scenario with standard lon / lat [Default = (52.5324234, 5.4532432)]
        SaveSystem.SaveScenario(this.gameObject, unitRealtime.Count, "52.5324234", "5.4532432", currentName);
    }
    /// <summary>
    /// Save storage data, such as units
    /// </summary>
    /// <param Scene ID="sceneid">ID of this current scene</param>
    public void SaveStorageData(int sceneid)
    {
        currentId = sceneid;

        //Update Scenario UI Panel
        currentTime = DateTime.Now.ToString();

        for (int i = 0; i < unitRealtime.Count; i++)
        {
            //For every unit found save the data of that unit
            StartCoroutine(SaveUnitData(unitRealtime[i], i));
        }
        //If unit realtime is empty scenario is done saving
        if (unitRealtime.Count <= 0)
        {
            //Give feedback to the player that scenario is done with saving and disable feedback UI
            SetSavingFeedback("SCENARIO SAVED", false);
            StartCoroutine(ToggleOffUI());
        }

        if (!savedScenarioIds.Contains(sceneid))
        {
            //Load scenario when there are no saved scenarios in the list
            SaveSystem.LoadScenarios(gameObject, maxSaves);
        }
        else
        {
            //If scenario is in list (m_SavedScenarioIds) update scenario with lon / lat [Default = (52.5324234, 5.4532432)]
            SaveDatabase.Instance.OnUpdateScenario(unitRealtime.Count, sceneid, DateTime.Now.ToString(), "52.5324234", "5.4532432");
        }

        //Clear UnitRealtime list at the end.
        unitRealtime.Clear();
    }
    //Toggle off ui that contains feedback elements
    public IEnumerator ToggleOffUI()
    {
        yield return new WaitForSeconds(3f);
        savingFeedback.gameObject.SetActive(false);
    }
    //Togglepanel of the main scenario panel
    public void TogglePanel()
    {
        //Toggle the UI panel of the save system
        panelOpened = !panelOpened;
        scenarioPanel.SetActive(panelOpened);
    }
}
