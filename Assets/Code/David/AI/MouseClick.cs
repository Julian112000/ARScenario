using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    Vector3 mousepos;
    [SerializeField]
    Human prefab;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                prefab.PlaceWayPoint(hit.point);
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
    }
}
