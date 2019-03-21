using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveScenario(GameObject sceneinfo, int amount)
    {
        SceneStats mapstats = new SceneStats(sceneinfo);
        SaveDatabase.Instance.OnSaveScenario(mapstats.scenarioName, DateTime.Now.ToString(), amount);
    }
    public static void LoadScenarios(GameObject sceneinfo, int number)
    {
        for (int i = 0; i < number; i++)
        {
            SaveDatabase.Instance.OnSelectScenario(i);
        }
    }
    public static void SaveStandardUnit(GameObject unit, int number, int scenenumber)
    {
        StandardUnitData unitdata = new StandardUnitData(unit);
        SaveDatabase.Instance.OnSaveUnit(unitdata.m_UnitId, scenenumber, unitdata.m_UnitName, unitdata.m_Position, unitdata.m_Rotation, unitdata.m_Scale, number);
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
            scenarioID = manager.m_CurrentID;
            scenarioName = manager.m_CurrentName;
            scenarioTime = manager.m_CurrentTime;
        }
    }
    [Serializable]
    public class StandardUnitData
    {
        public int m_UnitId;
        public string m_UnitName;
        public float[] m_Position;
        public float[] m_Rotation;
        public float[] m_Scale;

        public StandardUnitData(GameObject unit)
        {
            m_UnitId = unit.GetComponent<SaveData>().GetID();
            //Take over the name
            m_UnitName = unit.name;
            //Take over the position
            m_Position = new float[3];
            m_Position[0] = unit.transform.position.x;
            m_Position[1] = unit.transform.position.y;
            m_Position[2] = unit.transform.position.z;
            //Take over the rotation
            m_Rotation = new float[3];
            m_Rotation[0] = unit.transform.localEulerAngles.x;
            m_Rotation[1] = unit.transform.localEulerAngles.y;
            m_Rotation[2] = unit.transform.localEulerAngles.z;
            //Take over the scale
            m_Scale = new float[3];
            m_Scale[0] = unit.transform.localScale.x;
            m_Scale[1] = unit.transform.localScale.y;
            m_Scale[2] = unit.transform.localScale.z;
        }
    }
}
