using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveScenario(GameObject sceneinfo, int amount, string lat, string lon, string isnew)
    {
        //Create new SceneStats and apply the data
        SceneStats mapstats = new SceneStats(sceneinfo);
        SaveDatabase.Instance.OnSaveScenario(mapstats.scenarioName, DateTime.Now.ToString(), amount, lat, lon, isnew);
    }
    public static void LoadScenarios(GameObject sceneinfo, int number)
    {
        //Load all UI panels (standard number = 100)
        for (int i = 0; i < number; i++)
        {
            SaveDatabase.Instance.OnSelectScenario(i);
        }
    }
    public static void SaveStandardUnit(GameObject unit, int number, int scenenumber)
    {
        //Create new StandardUnitData script and add data to it
        StandardUnitData unitdata = new StandardUnitData(unit);
        SaveDatabase.Instance.OnSaveUnit(unitdata.m_UnitId, scenenumber, unitdata.m_UnitName, unitdata.m_Latitude, unitdata.m_Longitude, unitdata.m_Latitude, unitdata.m_Rotation, unitdata.m_Scale, number);
    }
    [Serializable]
    public class SceneStats
    {
        public List<int> unitdata;
        public string scenarioName;
        public int scenarioID;
        public string scenarioTime;

        public SceneStats(GameObject sceneinfo)
        {
            SceneManager manager = sceneinfo.GetComponent<SceneManager>();
            //Take over the id
            scenarioID = manager.m_CurrentID;
            //Take over the name of the scenario
            scenarioName = manager.m_CurrentName;
            //Take over the last updated time
            scenarioTime = manager.m_CurrentTime;
        }
    }
    [Serializable]
    public class StandardUnitData
    {
        public int m_UnitId;
        public string m_UnitName;
        public string m_Latitude;
        public string m_Luditude;
        public string m_Longitude;
        public float[] m_Rotation;
        public float[] m_Scale;

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
