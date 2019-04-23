using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fennek : Vehicle
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        if (PlayModeOn)
        {
            base.Update();
        }
    }
    //
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
}
