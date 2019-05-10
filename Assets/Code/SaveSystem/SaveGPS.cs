using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class SaveGPS : MonoBehaviour
{
    public static SaveGPS Instance { set; get; }    //Set Get Static Instance to call from other scripts [ALL saved scripts]       

    private void Start()
    {
        //Set instance to this script
        Instance = this;
    }
    //Return transform position of the camera object
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    /// <summary>
    /// Return distance in vector3 to the unit
    /// x : distance x from firstpersoncamera
    /// y : distance y from firstpersoncamera
    /// z : distance z from firstpersoncamera
    /// </summary>
    /// <param UnitPos="tpos">Unit position in vector3 in unity world</param>
    /// <returns></returns>
    public Vector3 CheckDistance(Vector3 tpos)
    {
        //Calculate values to get lenght from object to camera
        float x = tpos.x - transform.position.x;
        float y = tpos.y - transform.position.y;
        float z = tpos.z - transform.position.z;
        return new Vector3(x, y, z);
    }
}
