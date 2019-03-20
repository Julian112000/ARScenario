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
    public void OnCreateScenario(string name, string time, int amount)
    {
        StartCoroutine(HandleSaveAsync(name, time, amount));
    }
    public void OnSaveUnit(int id, int scenenumber, string name, float[] position, int newid)
    {
        StartCoroutine(HandleSaveUnitAsync(id, scenenumber, name, position, newid));
    }
    public void OnUpdateScenario(int amount, int sceneid)
    {
        StartCoroutine(UpdateScenario(amount, sceneid));
    }
    public void OnLoadScenario(int sceneid)
    {
        StartCoroutine(LoadScenario(sceneid));
    }
    public void OnLoadUnits(int sceneid, int id)
    {
        StartCoroutine(GetUnits(sceneid, id));
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
    public IEnumerator HandleSaveUnitAsync(int id, int scenenumber, string name, float[] position, int newid)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "create");
        form.AddField("id", newid);
        form.AddField("unitid", id);
        form.AddField("scenarioid", scenenumber);
        form.AddField("posx", position[0].ToString("0.00"));
        form.AddField("posy", position[1].ToString("0.00"));
        form.AddField("posz", position[2].ToString("0.00"));
        WWW www = new WWW(url + "createunitapi.php", form);
        yield return www;
    }
    public IEnumerator SaveScenarioStorage(string name)
    {
        yield return new WaitForSeconds(1f);
        WWWForm form = new WWWForm();
        form.AddField("action", "scenarioname");
        form.AddField("scenarioname", name);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text != "")
        {
            string data = www.text;
            //ID
            int v1 = int.Parse(www.text);
            SceneManager.Instance.SaveStorageData(v1);
        }
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
            string[] values = data.Split(","[0]);
            //ID
            int v1 = int.Parse(values[0]);
            //Posx
            float v2 = float.Parse(values[1]);
            //Posy
            float v3 = float.Parse(values[2]);
            //Posz
            float v4 = float.Parse(values[3]);

            SceneManager.Instance.LoadUnit(v1, v2, v3, v4);
        }
    }
    public IEnumerator UpdateScenario(int amount, int sceneid)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("scenarioid", sceneid);
        form.AddField("amount", amount);
        WWW www = new WWW(url + "updateapi.php", form);
        yield return www;
        Debug.Log(www.text);
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
}
