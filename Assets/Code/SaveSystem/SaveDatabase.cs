using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class SaveDatabase : MonoBehaviour
{
    public static SaveDatabase Instance;    //Static Instance to call from other classes

    [SerializeField]
    private string url;                     //URL connections to the online database (https://)

    private int id = -1;                  //ID is standard -1, otherwise it will conflict with ids on 0

    private void Awake()
    {
        //Set instance to this script
        Instance = this;
    }
    /// <summary>
    /// Load all Scenarios from database in UI
    /// </summary>
    /// <param Scenario ID="id"></param>
    public void OnSelectScenario(int id)
    {
        StartCoroutine(LoadScenarioUI(id));
    }
    /// <summary>
    /// Save Scenario to the database
    /// </summary>
    /// <param Name="name">name of the scenario you named it when saving</param>
    /// <param Time="time">current UCS +1 time when scenario was last saved</param>
    /// <param Amount of units="amount">amount of the units in the scenario</param>
    /// <param Latitude="lat">gps saved position x of the place where the scenario is saved</param>
    /// <param Longitude="lon">gps saved position z of the place where the scenario is saved</param>
    /// <param New name="isnew">new name of the scenario</param>
    public void OnSaveScenario(string name, string time, int amount, string lat, string lon, string isnew)
    {
        StartCoroutine(SaveScenario(name, time, amount, lat, lon, isnew));
    }
    /// <summary>
    /// Save unit to the database from scenario
    /// </summary>
    /// <param Type="id">Type of the unit [0 = soldier], [1 = fennek] etc.</param>
    /// <param Scenario ID="scenenumber">ID of the scenario where unit was saved</param>
    /// <param Name="name">Name of the unit</param>
    /// <param Latitude="lat">Distance in X from the firstpersoncamera component</param>
    /// <param Longitude="lon">Distance in Z from the firstpersoncamera component</param>
    /// <param Lutitude="lud">Distance in Y from the firstpersoncamera component</param>
    /// <param Rotation Array="rotation">Array of Rotations [X / Y / Z] of the unit</param>
    /// <param Scene Array="scale">Array of Sclaes [X / Y / Z] of the unit</param>
    /// <param ID="newid">ID of the order number in the units list</param>
    public void OnSaveUnit(int id, int scenenumber, string name, string lat, string lon, string lud, float[] rotation, float[] scale, int newid)
    {
        StartCoroutine(HandleSaveUnitAsync(id, scenenumber, name, lat, lon, lud, rotation, scale, newid));
    }
    /// <summary>
    /// This void is called if a scenario is already in the database
    /// Scenario will update the amount of units, time, lat etc.
    /// </summary>
    /// <param amount of units="amount">New amount of units in the saved scenario</param>
    /// <param ID of scenario="sceneid">Current ID of the scenario from the database</param>
    /// <param Time="time">Standard UCL +1 Time of the updated saved scenario</param>
    /// <param Latitude="lat">New latitude (x) GPS position of the saved scenario</param>
    /// <param Longitude="lon">New longitude (z) GPS position of the saved scenario</param>
    public void OnUpdateScenario(int amount, int sceneid, string time, string lat, string lon)
    {
        StartCoroutine(UpdateScenario(amount, sceneid, time, lat, lon));
    }
    /// <summary>
    /// Void to call if the player desides to load the scenario
    /// With the load button
    /// </summary>
    /// <param scene id="sceneid">ID that desides which scenario to load from mysql database</param>
    public void OnLoadScenario(int sceneid)
    {
        StartCoroutine(LoadScenario(sceneid));
    }
    /// <summary>
    /// Void to call and start GetUnits ienumerator
    /// </summary>
    /// <param Scene ID="sceneid">ID from scenario in mysql database</param>
    /// <param ID="id">Type of unit ID</param>
    public void OnLoadUnits(int sceneid, int id)
    {
        StartCoroutine(GetUnits(sceneid, id));
    }
    /// <summary>
    /// Delete scenario from the php mysql database
    /// </summary>
    /// <param Scenario ID="sceneid">id of the soon be deleted scenario from the database</param>
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
            int db_id = int.Parse(www.text); //Get ID from php script and save it to int
            SceneManager.Instance.SaveStorageData(db_id);
        }
    }
    /// <summary>
    /// Save unit to the database from scenario
    /// </summary>
    /// <param Type="id">Type of the unit [0 = soldier], [1 = fennek] etc.</param>
    /// <param Scenario ID="scenenumber">ID of the scenario where unit was saved</param>
    /// <param Name="name">Name of the unit</param>
    /// <param Latitude="lat">Distance in X from the firstpersoncamera component</param>
    /// <param Longitude="lon">Distance in Z from the firstpersoncamera component</param>
    /// <param Lutitude="lud">Distance in Y from the firstpersoncamera component</param>
    /// <param Rotation Array="rotation">Array of Rotations [X / Y / Z] of the unit</param>
    /// <param Scene Array="scale">Array of Sclaes [X / Y / Z] of the unit</param>
    /// <param ID="newid">ID of the order number in the units list</param>
    public IEnumerator HandleSaveUnitAsync(int id, int scenenumber, string name, string lat, string lon, string lud, float[] rotation, float[] scale, int newid)
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
        form.AddField("posy", lud);
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
        SceneManager.Instance.SetSavingFeedback("SCENARIO SAVED", false);
        StartCoroutine(SceneManager.Instance.ToggleOffUI());
        yield return www;
    }
    /// <summary>
    /// Load all Scenarios from database in UI
    /// </summary>
    /// <param Scenario ID="id"></param>
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
            int db_id = int.Parse(values[0]);
            //NAME
            string db_name = values[1];
            //TIME
            string db_time = values[2];
            //Load scenarios with ID,NAME,TIME as parameters
            SceneManager.Instance.LoadScenarios(db_id, db_name, db_time);
        }
    }
    /// <summary>
    /// Save Scenario to the database
    /// </summary>
    /// <param Name="name">name of the scenario you named it when saving</param>
    /// <param Time="time">current UCS +1 time when scenario was last saved</param>
    /// <param Amount of units="amount">amount of the units in the scenario</param>
    /// <param Latitude="lat">gps saved position x of the place where the scenario is saved</param>
    /// <param Longitude="lon">gps saved position z of the place where the scenario is saved</param>
    /// <param New name="isnew">new name of the scenario</param>
    public IEnumerator SaveScenario(string name, string time, int amount, string lat, string lon, string isnew)
    {
        //Create new WWWForm and add the form data to it
        WWWForm form = new WWWForm();
        form.AddField("action", "scenarioname");
        form.AddField("scenarioname", name);
        WWW www = new WWW(url + "api.php", form);
        yield return www;
        //If return value is create, save scenario as a new object
        if (!string.IsNullOrEmpty(isnew) || www.text == "create")
        {
            StartCoroutine(HandleSaveAsync(isnew, time, amount, lat, lon));
        }
        //If return value is different than create update scenario
        else if (string.IsNullOrEmpty(isnew) || www.text != "create")
        {
            //Already same scenario in database so override data
            int v1 = int.Parse(www.text);
            StartCoroutine(UpdateScenario(amount, v1, time, lat, lon));
            SceneManager.Instance.SaveStorageData(v1); //Delete all units in database and save new ones
        }
    }
    /// <summary>
    /// Void to call and start GetUnits ienumerator
    /// </summary>
    /// <param Scene ID="sceneid">ID from scenario in mysql database</param>
    /// <param ID="id">Type of unit ID</param>
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
    /// <summary>
    /// This void is called if a scenario is already in the database
    /// Scenario will update the amount of units, time, lat etc.
    /// </summary>
    /// <param amount of units="amount">New amount of units in the saved scenario</param>
    /// <param ID of scenario="sceneid">Current ID of the scenario from the database</param>
    /// <param Time="time">Standard UCL +1 Time of the updated saved scenario</param>
    /// <param Latitude="lat">New latitude (x) GPS position of the saved scenario</param>
    /// <param Longitude="lon">New longitude (z) GPS position of the saved scenario</param>
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
    /// <summary>
    /// Void to call if the player desides to load the scenario
    /// With the load button
    /// </summary>
    /// <param scene id="sceneid">ID that desides which scenario to load from mysql database</param>
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
            data = data.Replace(".", ",");              //Replace , to . for the android build if standalone build
            #endif
            string[] values = data.Split(";"[0]);       //Split all data with ; involved in it
            //Name
            string db_name = values[0];                 //Name of the scenario    
            //Time
            string db_time = values[1];                 //Last updated time of the scenario
            //Amount
            int db_amount = int.Parse(values[2]);       //Amount of units in the scenario
            //Lat
            float db_lat = float.Parse(values[3]);      //Latitude (GPS) of the last saved position in the scenario
            //Lon
            float db_lon = float.Parse(values[4]);      //Longitude (GPS) of the last saved position in the scenario

            //Load scenario with the parameters above
            SceneManager.Instance.Load(sceneid, db_name /* name */, db_time /* time */, db_amount /* amount */, db_lat /* latitude */, db_lon /* longitude */);
        }
    }
    /// <summary>
    /// Delete scenario from the php mysql database
    /// </summary>
    /// <param Scenario ID="sceneid">id of the soon be deleted scenario from the database</param>
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
