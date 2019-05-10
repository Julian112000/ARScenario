using System.Collections.Generic;
using UnityEngine;
using System;

public static class SaveSystem
{
    /// <summary>
    /// Save Scenario to the database
    /// </summary>
    /// <param Gameobject="sceneinfo">gameobject of scenemanger in unity world</param>
    /// <param Amount of units="amount">amount of the units in the scenario</param>
    /// <param Latitude="lat">gps saved position x of the place where the scenario is saved</param>
    /// <param Longitude="lon">gps saved position z of the place where the scenario is saved</param>
    /// <param New name="isnew">new name of the scenario</param>
    public static void SaveScenario(GameObject sceneinfo, int amount, string lat, string lon, string isnew)
    {
        //Create new SceneStats and apply the data
        SceneStats mapstats = new SceneStats(sceneinfo);
        SaveDatabase.Instance.OnSaveScenario(mapstats.m_ScenarioName, DateTime.Now.ToString(), amount, lat, lon, isnew);
    }
    /// <summary>
    /// Void to call if the player desides to load the scenario
    /// With the load button
    /// </summary>
    /// <param Gameobject="sceneinfo">gameobject of scenemanger in unity world</param>
    /// <param scene id="number">Number of times to check if there is the loaded scenario in database</param>
    public static void LoadScenarios(GameObject sceneinfo, int number)
    {
        //Load all UI panels [Default Number = 100]
        for (int i = 0; i < number; i++)
        {
            SaveDatabase.Instance.OnSelectScenario(i);
        }
    }
    /// <summary>
    /// Save unit to the database from scenario
    /// </summary>
    /// <param Gameobject="unit">Unit gameobject in unity world</param>
    /// <param Order Number="number">Number of order of unit in list</param>
    /// <param Scenario ID="scenenumber">ID of scenario where the unit is saved in</param>
    public static void SaveStandardUnit(GameObject unit, int number, int scenenumber)
    {
        //Create new StandardUnitData script and add data to it
        StandardUnitData unitdata = new StandardUnitData(unit);
        SaveDatabase.Instance.OnSaveUnit(unitdata.m_UnitId, scenenumber, unitdata.m_UnitName, unitdata.m_Latitude, unitdata.m_Longitude, unitdata.m_Luditude, unitdata.m_Rotation, unitdata.m_Scale, number);
    }
    [Serializable]
    public class SceneStats
    {
        public List<int> m_Unitdata;    //List of all unit ids
        public int m_ScenarioID;        //ID of scenario
        public string m_ScenarioName;   //Name of scenario
        public string m_ScenarioTime;   //Normal UCL +1 time of scenario

        public SceneStats(GameObject sceneinfo)
        {
            SceneManager manager = sceneinfo.GetComponent<SceneManager>();
            //Take over the id
            m_ScenarioID = manager.m_CurrentID;
            //Take over the name of the scenario
            m_ScenarioName = manager.m_CurrentName;
            //Take over the last updated time
            m_ScenarioTime = manager.m_CurrentTime;
        }
    }
    [Serializable]
    public class StandardUnitData
    {
        public int m_UnitId;        //Type of unit [0 = soldier, 1 = fennek etc.]
        public string m_UnitName;   //Name of unit in unityworld
        public string m_Latitude;   //Distance in x from fps camera to unit
        public string m_Luditude;   //Distance in y from fps camera to unit
        public string m_Longitude;  //Distance in z from fps camera to unit
        public float[] m_Rotation;  //Array of localeulerangles of the unit
        public float[] m_Scale;     //Array of localscale of the unit

        public StandardUnitData(GameObject unit)
        {
            SaveData data = unit.GetComponent<SaveData>();

            m_UnitId = data.GetID();
            //Take over the name
            m_UnitName = unit.name;
            //Take over the position
            m_Latitude = data.latitude.ToString();
            m_Longitude = data.longitude.ToString();
            m_Luditude = data.luditude.ToString(); 
            //Take over the rotation
            m_Rotation = new float[3];
            m_Rotation[0] = unit.transform.localEulerAngles.x;
            m_Rotation[1] = (unit.transform.localEulerAngles.y);
            m_Rotation[2] = unit.transform.localEulerAngles.z;
            //Take over the scale
            m_Scale = new float[3];
            m_Scale[0] = unit.transform.localScale.x;
            m_Scale[1] = unit.transform.localScale.y;
            m_Scale[2] = unit.transform.localScale.z;
        }
    }
}
