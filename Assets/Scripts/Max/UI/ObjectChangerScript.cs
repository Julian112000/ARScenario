using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectChangerScript : MonoBehaviour
{

    public List<MainObjectScript> AllObjects;
    //Middle object
    public ObjectDisplayScript o_main;
    public ObjectDisplayScript o_left;
    public ObjectDisplayScript o_right;

    public int currentObject = 3;

    private void Awake()
    {
        Debug.Log(currentObject);
        ChangeCurrentObject(currentObject);
    }


    public void ChangeCurrentObject(int CurrentObject)
    {
        int left = currentObject - 1;
        int right = currentObject + 1;

        o_left.UIObject = AllObjects[left];
        o_main.UIObject = AllObjects[CurrentObject];
        o_right.UIObject = AllObjects[right];

    }
}
