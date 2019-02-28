using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Models
{
    Tank,
    Truck,
    Jeep
}
public class UiHandling : MonoBehaviour {

    public GameObject CurrentModel;
    public Models models = Models.Tank;
    [SerializeField]
    private List<GameObject> ModelList;

    private void Awake()
    {
        CurrentModel = ModelList[0];
    }

    public void SwitchModels()
    {
        models = models + 1;
        CurrentModel = ModelList[(int)models];
        if((int)models > ModelList.Count)
        {
            models = 0;
            CurrentModel = ModelList[(int)models];
        }
    }
}
