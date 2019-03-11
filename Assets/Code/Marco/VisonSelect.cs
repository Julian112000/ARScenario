using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisonSelect : MonoBehaviour
{
    public static bool NewTarget;
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
            if (human = hit.collider.gameObject.GetComponent<TestHumanScript>())
            {
                human.SetNewT = true;
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (NewTarget)
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
        
    }
}
