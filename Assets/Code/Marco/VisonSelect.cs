using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisonSelect : MonoBehaviour
{
    public static bool NewTarget;
    public bool IsMouse;
    private Camera Mcamera;
    private void Awake()
    {
        Mcamera = Camera.main;
    }

    public bool SelectObject()
    {
        RaycastHit hit;
        Ray ray = Mcamera.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out hit))
        {
            TestHumanScript human;
            TankAiScriptb tank;
            if (human = hit.collider.gameObject.GetComponent<TestHumanScript>())
            {
                human.SetNewT = true;
                return true;
            }
            if(tank = hit.collider.gameObject.GetComponent<TankAiScriptb>())
            {
                tank.SetNewT = true;
                return true;
            }
        }
        return false;
    }
    public bool SelectObjectM()
    {
        RaycastHit hit;
        Ray ray = Mcamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            TestHumanScript human;
            TankAiScriptb tank;
            if (human = hit.collider.gameObject.GetComponent<TestHumanScript>())
            {
                human.SetNewT = true;
                Debug.Log("Werkt");
                return true;
            }
            if (tank = hit.collider.gameObject.GetComponent<TankAiScriptb>())
            {
                tank.SetNewT = true;
                Debug.Log("Werkt");
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            NewTarget = !NewTarget;
        }
        if (NewTarget && !IsMouse)
        {
            if (!SelectObject())
            {
                return;
            }
            else
            {
                NewTarget = false;
            }
        }
        if(NewTarget && Input.GetMouseButtonDown(0))
        {
            if (!SelectObjectM())
            {
                return;
            }
            else
            {
                NewTarget = false;
            }
        }
        
    }
}
