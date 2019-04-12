using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class SaveGPS : MonoBehaviour
{
    public static SaveGPS Instance { set; get; }
    private Vector3 m_SnappedPosition;

    private void Start()
    {
        Instance = this;
    }
    public Vector3 GetPosition()
    {
        //Return transform position of the camera object
        return transform.position;
    }
    public Vector3 CheckDistance(Vector3 tpos)
    {
        //Return distance in vector3 to the unit
        float x = tpos.x - transform.position.x;
        float y = tpos.y - transform.position.y;
        float z = tpos.z - transform.position.z;
        return new Vector3(x, y, z);
    }
}
