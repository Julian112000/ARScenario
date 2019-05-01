using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    //ID unit type
    [SerializeField]
    private int m_ID;

    //Snapped gameobject of the anchor
    public Vector3 m_SnappedPosition;

    [HideInInspector]
    public float latitude;
    [HideInInspector]
    public float longitude;
    [HideInInspector]
    public float luditude;

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
    public int GetID()
    {
        //Return ID of the unit type
        return m_ID;
    }
    public Vector3 GetPosition()
    {
        //Get position of the anchor
        return transform.position;
    }
    public Vector3 GetEulerAngles()
    {
        //Get rotation of the unit
        return transform.eulerAngles;
    }
}
