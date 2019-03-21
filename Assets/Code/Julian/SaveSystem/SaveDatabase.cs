using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveDatabase : MonoBehaviour
{
    public static SaveDatabase Instance;

    [SerializeField]
    private string url;

    private int m_ID = -1;

    private void Awake()
    {
        Instance = this;
    }
    //
    public void OnSelectScenario(int id)
    {
        StartCoroutine(LoadScenarioUI(id));
    }
    public void OnSaveScenario(string name, string time, int amount)
    {
        StartCoroutine(SaveScenario(name, time, amount));
    }
    public void OnSaveUnit(int id, int scenenumber, string name, float[] position, float[] rotation, float[] scale, int newid)
    {
        StartCoroutine(HandleSaveUnitAsync(id, scenenumber, name, position, rotation, scale, newid));
    }
    public void OnUpdateScenario(int amount, int sceneid, string time)
    {
        StartCoroutine(UpdateScenario(amount, sceneid, time));
    }
    public void OnLoadScenario(int sceneid)
    {
        StartCoroutine(LoadScenario(sceneid));
    }
    public void OnLoadUnits(int sceneid, int id)
    {
        StartCoroutine(GetUnits(sceneid, id));
    }
    public void OnDeleteScenario(int sceneid)
    {
        StartCoroutine(DeleteScenario(sceneid));
    }

    //IEnumartor calls to php
    public IEnumerator HandleSaveAsync(string name, string time, int amount)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "create");
        form.AddField("name", name);
        form.AddField("timetext", time);
        form.AddField("amountunits", amount);
        WWW www = new WWW(url + "createapi.php", form);
        yield return www;
        StartCoroutine(SaveScenarioStorage(name));
    }
    public IEnumerator SaveScenarioStorage(string name)
    {
        yield return new WaitForSeconds(1f);
        WWWForm form = new WWWForm();
        form.AddField("action", "scenarioname");
        form.AddField("scenarioname", name);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text != "create")
        {
            string data = www.text;
            //ID
            int v1 = int.Parse(www.text);
            SceneManager.Instance.SaveStorageData(v1);
        }
    }
    public IEnumerator HandleSaveUnitAsync(int id, int scenenumber, string name, float[] position, float[] rotation, float[] scale, int newid)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "create");
        form.AddField("id", newid);
        form.AddField("unitid", id);
        form.AddField("scenarioid", scenenumber);
        form.AddField("unitname", name);
        form.AddField("posx", position[0].ToString("0.00"));
        form.AddField("posy", position[1].ToString("0.00"));
        form.AddField("posz", position[2].ToString("0.00"));

        form.AddField("rotx", rotation[0].ToString("0.00"));
        form.AddField("roty", rotation[1].ToString("0.00"));
        form.AddField("rotz", rotation[2].ToString("0.00"));

        form.AddField("scalex", scale[0].ToString("0.00"));
        form.AddField("scaley", scale[1].ToString("0.00"));
        form.AddField("scalez", scale[2].ToString("0.00"));
        WWW www = new WWW(url + "createunitapi.php", form);
        yield return www;
        Debug.Log(www.text);
    }
    public IEnumerator LoadScenarioUI(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("scenarioid", id);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text != "")
        {
            string data = www.text;
            string[] values = data.Split(","[0]);
            //ID
            int v1 = int.Parse(values[0]);
            //NAME
            string v2 = values[1];
            //TIME
            string v3 = values[2];

            SceneManager.Instance.LoadScenarios(v1, v2, v3);
        }
    }
    public IEnumerator SaveScenario(string name, string time, int amount)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "scenarioname");
        form.AddField("scenarioname", name);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text == "create")
        {
            StartCoroutine(HandleSaveAsync(name, time, amount));
        }
        else if (www.text != "create")
        {
            int v1 = int.Parse(www.text);
            StartCoroutine(UpdateScenario(amount, v1, time));
            SceneManager.Instance.SaveStorageData(v1);
        }
    }
    public IEnumerator GetUnits(int sceneid, int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "unit");
        form.AddField("scenarioid", sceneid);
        form.AddField("id", id);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text != "")
        {
            string data = www.text;
            string[] values = data.Split(";"[0]);
            //ID
            int v1 = int.Parse(values[0]);
            //Name
            string dbname = values[1];
            //Posx
            float v2 = float.Parse(values[2]);
            //Posy
            float v3 = float.Parse(values[3]);
            //Posz
            float v4 = float.Parse(values[4]);
            //Rotx
            float v5 = float.Parse(values[5]);
            //Roty
            float v6 = float.Parse(values[6]);
            //Rotz
            float v7 = float.Parse(values[7]);
            //Scalex
            float v8 = float.Parse(values[8]);
            //Scaley
            float v9 = float.Parse(values[9]);
            //Scalez
            float v10 = float.Parse(values[10]);

            SceneManager.Instance.LoadUnit(v1, dbname, v2, v3, v4, v5, v6, v7, v8, v9, v10);
        }
    }
    public IEnumerator UpdateScenario(int amount, int sceneid, string time)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("scenarioid", sceneid);
        form.AddField("amount", amount);
        form.AddField("time", time);
        WWW www = new WWW(url + "updateapi.php", form);
        yield return www;
        Debug.Log(www.text);
        SceneManager.Instance.UpdateScenarios(time);
    }
    public IEnumerator LoadScenario(int sceneid)
    {
        WWWForm form = new WWWForm();
        form.AddField("sceneid", sceneid);
        WWW www = new WWW(url + "loadapi.php", form);
        yield return www;
        if (www.text != "")
        {
            string data = www.text;
            string[] values = data.Split(","[0]);
            //Name
            string v1 = values[0];
            //Time
            string v2 = values[1];
            //Amount
            int v3 = int.Parse(values[2]);

            SceneManager.Instance.Load(sceneid, v1, v2, v3);
        }
    }
    public IEnumerator DeleteScenario(int sceneid)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("sceneid", sceneid);
        WWW www = new WWW(url + "deleteapi.php", form);
        yield return www;
    }
}
