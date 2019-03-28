using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : AIBasics
{
    private float visionRange = 15f;
    private float movementSpeed = 0.75f;

    public override void Awake()
    {
        base.Awake();
        VisionRange = visionRange;
        MovementSpeed = movementSpeed;
        unittype = UnitType.Human;


    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
