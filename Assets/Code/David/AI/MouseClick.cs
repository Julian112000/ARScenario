using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    Vector3 mousepos;
    [SerializeField]
    GameObject waypointprefab;
    [SerializeField]
    LayerMask unitlayer;
    [SerializeField]
    LayerMask WaypointLayer;
    [SerializeField]
    Human prefab;
    [SerializeField]
    WaypointScript waypoint;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (waypoint)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    prefab.RepositionWaypoint(waypoint, hit.point);
                    //prefab.PlaceWayPoint(hit.point);
                    //prefab.AimAtPoint(hit.point);
                }
            }
            else
            {
                if (!prefab)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100.0f, unitlayer))
                    {
                        prefab = hit.collider.gameObject.GetComponent<Human>();
                        prefab.Selected = true;
                        prefab.TurnOnVisuals();
                        //prefab.PlaceWayPoint(hit.point);
                        //prefab.AimAtPoint(hit.point);
                    }
                }
                if (prefab)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        //if(prefab.)
                        Quaternion empty = new Quaternion();
                        empty.eulerAngles = new Vector3(0, 0, 0);
                        GameObject SpawnedObject = Instantiate(waypointprefab, hit.point, empty);
                        WaypointScript script = SpawnedObject.GetComponent<WaypointScript>();
                        prefab.aistate = AIStates.GoTowards;
                        prefab.PlaceWayPoint(hit.point, script);
                        //prefab.AimAtPoint(hit.point);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!waypoint)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f, WaypointLayer))
                {
                    waypoint = hit.collider.gameObject.GetComponent<WaypointScript>();
                    //prefab.PlaceWayPoint(hit.point);
                    //prefab.AimAtPoint(hit.point);
                }
            }
            else if (waypoint)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f, WaypointLayer))
                {
                    if(hit.collider.gameObject == waypoint.gameObject)
                    {
                        waypoint = null;
                    }
                    //prefab.PlaceWayPoint(hit.point);
                    //prefab.AimAtPoint(hit.point);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            prefab.UndoLastWaypoint();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            prefab.ClearWaypoints();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            prefab = null;
            waypoint = null;
            Debug.Log("Stuff cleared");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            prefab.ConnectLastToFirst();
            prefab.aistate = AIStates.Patrol;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            prefab.TurnOnVisuals();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            prefab.TurnOffVisuals();
        }
    }
}
