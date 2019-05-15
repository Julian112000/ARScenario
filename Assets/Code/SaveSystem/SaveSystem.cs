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
        SaveDatabase.Instance.OnSaveScenario(mapstats.scenarioName, DateTime.Now.ToString(), amount, lat, lon, isnew);
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
        SaveDatabase.Instance.OnSaveUnit(unitdata.unitId, scenenumber, unitdata.unitName, unitdata.latitude, unitdata.longitude, unitdata.luditude, unitdata.rotation, unitdata.scale, number);
    }
    [Serializable]
    public class SceneStats
    {
        public List<int> unitData;    //List of all unit ids
        public int scenarioId;        //ID of scenario
        public string scenarioName;   //Name of scenario
        public string scenarioTime;   //Normal UCL +1 time of scenario

        public SceneStats(GameObject sceneinfo)
        {
            SceneManager manager = sceneinfo.GetComponent<SceneManager>();
            //Take over the id
            scenarioId = manager.currentId;
            //Take over the name of the scenario
            scenarioName = manager.currentName;
            //Take over the last updated time
            scenarioTime = manager.currentTime;
        }
    }
    [Serializable]
    public class StandardUnitData
    {
        public int unitId;        //Type of unit [0 = soldier, 1 = fennek etc.]
        public string unitName;   //Name of unit in unityworld
        public string latitude;   //Distance in x from fps camera to unit
        public string luditude;   //Distance in y from fps camera to unit
        public string longitude;  //Distance in z from fps camera to unit
        public float[] rotation;  //Array of localeulerangles of the unit
        public float[] scale;     //Array of localscale of the unit

        public StandardUnitData(GameObject unit)
        {
            SaveData data = unit.GetComponent<SaveData>();

            unitId = data.GetID();
            //Take over the name
            unitName = unit.name;
            //Take over the position
            latitude = data.latitude.ToString();
            longitude = data.longitude.ToString();
            luditude = data.luditude.ToString();
            //Take over the rotation
            rotation = new float[3];
            rotation[0] = unit.transform.localEulerAngles.x;
            rotation[1] = (unit.transform.localEulerAngles.y);
            rotation[2] = unit.transform.localEulerAngles.z;
            //Take over the scale
            scale = new float[3];
            scale[0] = unit.transform.localScale.x;
            scale[1] = unit.transform.localScale.y;
            scale[2] = unit.transform.localScale.z;
        }
    }
}
