using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class SaveDatabase : MonoBehaviour
{
    public static SaveDatabase Instance;

    //URL connections to the online database (https://)
    [SerializeField]
    private string url;

    //ID is standard -1, otherwise it will conflict with ids on 0
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
    public void OnSaveScenario(string name, string time, int amount, string lat, string lon)
    {
        StartCoroutine(SaveScenario(name, time, amount, lat, lon));
    }
    public void OnSaveUnit(int id, int scenenumber, string name, string lat, string lon, string posy, float[] rotation, float[] scale, int newid)
    {
        StartCoroutine(HandleSaveUnitAsync(id, scenenumber, name, lat, lon, posy, rotation, scale, newid));
    }
    public void OnUpdateScenario(int amount, int sceneid, string time, string lat, string lon)
    {
        StartCoroutine(UpdateScenario(amount, sceneid, time, lat, lon));
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
    public IEnumerator HandleSaveAsync(string name, string time, int amount, string lat, string lon)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "create");
        form.AddField("name", name);
        form.AddField("timetext", time);
        form.AddField("amountunits", amount);

        #if UNITY_EDITOR
        lat = lat.Replace(".", ","); //Replace , to . for the android build if standalone build
        lon = lon.Replace(".", ","); //Replace , to . for the android build if standalone build
        #endif
        form.AddField("latitude", lat);
        form.AddField("longitude", lon);
        //Link to connection php: createapi.php in the database
        WWW www = new WWW(url + "createapi.php", form);
        yield return www;

        //Start coroutine to save units
        StartCoroutine(SaveScenarioStorage(name));
    }
    public IEnumerator SaveScenarioStorage(string name)
    {
        yield return new WaitForSeconds(1f);
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "scenarioname");
        form.AddField("scenarioname", name);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text != "create")
        {
            string data = www.text; //Store www.text to the data string
            //ID
            int v1 = int.Parse(www.text); //Get ID from php script and save it to int
            SceneManager.Instance.SaveStorageData(v1);
        }
    }
    public IEnumerator HandleSaveUnitAsync(int id, int scenenumber, string name, string lat, string lon, string posy, float[] rotation, float[] scale, int newid)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "create");
        form.AddField("id", newid);                                 //ID of the current order in list
        form.AddField("unitid", id);                                //ID of the unit type (0 = tank, 1 = soldier etc.)
        form.AddField("scenarioid", scenenumber);                   //ID of the scenario linked to it
        form.AddField("unitname", name);                            //Name of the unit
        //Save Position with 5 decimals
        form.AddField("posx", lat);       
        form.AddField("posy", posy);
        form.AddField("posz", lon);
        //Save Rotation with 2 decimals
        form.AddField("rotx", rotation[0].ToString("0.00"));
        form.AddField("roty", rotation[1].ToString("0.00"));
        form.AddField("rotz", rotation[2].ToString("0.00"));
        //Save Scale with 2 decimals
        form.AddField("scalex", scale[0].ToString("0.00"));
        form.AddField("scaley", scale[1].ToString("0.00"));
        form.AddField("scalez", scale[2].ToString("0.00"));
        WWW www = new WWW(url + "createunitapi.php", form);
        yield return www;
    }
    public IEnumerator LoadScenarioUI(int id)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("scenarioid", id);                            //ID of the scanerio linked to it
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
            //Load scenarios with ID,NAME,TIME as parameters
            SceneManager.Instance.LoadScenarios(v1, v2, v3);
        }
    }
    public IEnumerator SaveScenario(string name, string time, int amount, string lat, string lon)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "scenarioname");
        form.AddField("scenarioname", name);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        Debug.Log(www.text + "UPDATE");
        if (www.text == "create")
        {
            //No scenarios in database so save scenario as a new data
            StartCoroutine(HandleSaveAsync(name, time, amount, lat, lon));
        }
        else if (www.text != "create")
        {
            //Already same scenario in database so override data
            int v1 = int.Parse(www.text);
            StartCoroutine(UpdateScenario(amount, v1, time, lat, lon));
            SceneManager.Instance.SaveStorageData(v1); //Delete all units in database and save new ones
        }
    }
    public IEnumerator GetUnits(int sceneid, int id)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "unit");
        form.AddField("scenarioid", sceneid);
        form.AddField("id", id);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        if (www.text != "")
        {
            //Store all www.text data in string data file
            string data = www.text;
            #if UNITY_EDITOR
            data = data.Replace(".", ","); //Replace , to . for the android build if standalone build
            #endif
            string[] values = data.Split(";"[0]); //Split all data with ; involved in it
            //ID
            int db_id = int.Parse(values[0]);           //ID of the unit type
            //Name
            string db_name = values[1];                 //Name of the unit
            //Posx
            float db_posx = float.Parse(values[2]);     //Offset of the player x
            //Posy
            float db_posy = float.Parse(values[3]);     //Position of the unit y
            //Posz
            float db_posz = float.Parse(values[4]);     //Offset of the player z
            //Rotx
            float db_rotx = float.Parse(values[5]);     //Rotation of the unit x
            //Roty
            float db_roty = float.Parse(values[6]);     //Rotation of the unit y
            //Rotz
            float db_rotz = float.Parse(values[7]);     //Rotation of the unit z
            //Scalex
            float db_scalex = float.Parse(values[8]);   //Scale of the unit x
            //Scaley
            float db_scaly = float.Parse(values[9]);    //Scale of the unit y
            //Scalez
            float db_scalez = float.Parse(values[10]);  //Scale of the unit z

            //Load units with the data above
            SceneManager.Instance.LoadUnit(db_id, db_name, 
                db_posx, db_posy, db_posz, 
                db_rotx, db_roty, db_rotz, 
                db_scalex, db_scaly, db_scalez);
        }
    }
    public IEnumerator UpdateScenario(int amount, int sceneid, string time, string lat, string lon)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("scenarioid", sceneid);
        form.AddField("amount", amount);
        form.AddField("time", time);

        #if UNITY_EDITOR
        lat = lat.Replace(".", ",");        //Replace , to . for the android build if standalone build
        lon = lon.Replace(".", ",");        //Replace , to . for the android build if standalone build
        #endif
        form.AddField("latitude", lat);
        form.AddField("longitude", lon);

        WWW www = new WWW(url + "updateapi.php", form);
        yield return www;
        //Update the time of the scenario in database and UI
        SceneManager.Instance.UpdateScenarios(time);
    }
    public IEnumerator LoadScenario(int sceneid)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("sceneid", sceneid);
        WWW www = new WWW(url + "loadapi.php", form);
        yield return www;
        Debug.Log(www.text);
        if (www.text != "")
        {
            //Store data from www.text into local string parameter
            string data = www.text;
            #if UNITY_EDITOR
            data = data.Replace(".", ","); //Replace , to . for the android build if standalone build
            #endif
            string[] values = data.Split(";"[0]);   //Split all data with ; involved in it
            //Name
            string v1 = values[0];                  //Name of the scenario    
            //Time
            string v2 = values[1];                  //Last updated time of the scenario
            //Amount
            int v3 = int.Parse(values[2]);          //Amount of units in the scenario
            //Lat
            float v4 = float.Parse(values[3]);      //Latitude (GPS) of the last saved position in the scenario
            //Lon
            float v5 = float.Parse(values[4]);      //Longitude (GPS) of the last saved position in the scenario

            //Load scenario with the parameters above
            SceneManager.Instance.Load(sceneid, v1, v2, v3, v4, v5);
        }
    }
    public IEnumerator DeleteScenario(int sceneid)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "scenario");
        form.AddField("sceneid", sceneid);
        //Delete scenario from database and UI
        WWW www = new WWW(url + "deleteapi.php", form);
        yield return www;
    }
}
