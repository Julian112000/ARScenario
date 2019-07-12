using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField]
    private int id;                         //ID unit type

    public Vector3 snappedPosition;         //Snapped gameobject of the anchor

    [HideInInspector]
    public float latitude;                  //Latitude (GPS) x
    [HideInInspector]
    public float longitude;                 //Longitude (GPS) z
    [HideInInspector]
    public float luditude;                  //Lutitude (GPS) y
                                            //Return ID of the unit type
    public int GetID()
    {
        return id;
    }
    private void Start()
    {
        //Get anchor of the gameobject
        snappedPosition = transform.parent.position;
    }
    private void Update()
    {
        //Reset position of the anchor and snap position to that position
        if (transform.parent.position != snappedPosition)
        {
            transform.parent.position = snappedPosition;
        }
    }
    //Get rotation of the unit
    public Vector3 GetEulerAngles()
    {
        return transform.eulerAngles;
    }
    //Get position of the anchor
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}