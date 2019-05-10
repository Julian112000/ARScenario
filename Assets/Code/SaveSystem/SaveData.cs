using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField]
    private int m_ID;                       //ID unit type

    public Vector3 m_SnappedPosition;       //Snapped gameobject of the anchor

    [HideInInspector]
    public float latitude;                  //Latitude (GPS) x
    [HideInInspector]
    public float longitude;                 //Longitude (GPS) z
    [HideInInspector]
    public float luditude;                  //Lutitude (GPS) y

    private void Start()
    {
        //Get anchor of the gameobject
        m_SnappedPosition = transform.parent.position;
    }
    private void Update()
    {
        //Reset position of the anchor and snap position to that position
        if (transform.parent.position != m_SnappedPosition)
        {
            transform.parent.position = m_SnappedPosition;
        }
    }
    //Return ID of the unit type
    public int GetID()
    {
        return m_ID;
    }
    //Get position of the anchor
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    //Get rotation of the unit
    public Vector3 GetEulerAngles()
    {
        return transform.eulerAngles;
    }
}
