using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : AIBasics
{
    private float visionRange = 15f;
    private float movementSpeed = 1f;

    public override void Awake()
    {
        base.Awake();
        VisionRange = visionRange;
        MovementSpeed = movementSpeed;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //


    }
}
