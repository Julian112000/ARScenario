﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : AIBasics
{
    [Header("Vehicle Related")]
    [SerializeField]
    private GameObject AimingObject;
    [SerializeField]
    private ParticleSystem BrokenVehicleSmoke;
    [SerializeField]
    private ParticleSystem Muzzleflash;

    [SerializeField]
    private Material[] m_BrokenMaterials;
    [SerializeField]
    private MeshRenderer[] m_BrokenParts_1;
    [SerializeField]
    private MeshRenderer[] m_BrokenParts_2;

    private bool m_Broken;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        if (PlayModeOn)
        {
            base.Update();
            if (aistate == AIStates.Dead && !m_Broken)
            {
                BrokenVehicleSmoke.Play();
                foreach (MeshRenderer m in m_BrokenParts_1)
                {
                    m.material = m_BrokenMaterials[0];
                }
                foreach (MeshRenderer m in m_BrokenParts_2)
                {
                    m.material = m_BrokenMaterials[1];
                }
                m_Broken = true;
            }
        }
    }
    public override void LateUpdate()
    {
        if (PlayModeOn)
        {
            base.LateUpdate();
            ActionUpdate();
        }
    }

    //Actions made for a Vehicle (not a Vehicle)
    //Action Voids
    #region Actions
    public void ActionUpdate()
    {
        switch (aistate)
        {
            case AIStates.AimAtPoint:
                AimingAtPoint();
                break;
            case AIStates.StandStill:
                StandStill();
                break;
            case AIStates.GoTowards:
                break;
            case AIStates.Patrol:
                break;
            case AIStates.TargetEnemy:
                TargetEnemy();
                break;
            case AIStates.TargetAndMove:
                TargetAndMove();
                break;
            default:
                break;
        }
    }
    //Everything related to the aimatpoint state
    #region AimAtPoint Related
    public void AimAtPoint(Vector3 Point)
    {
        aistate = AIStates.AimAtPoint;
        PointToAimAt = Point;
    }
    private void AimingAtPoint()
    {
        //
        AimingObject.transform.LookAt(PointToAimAt);
        Debug.DrawLine(VisionPoint.position, PointToAimAt, Color.green);

    }
    public void StopAimingAtPoint()
    {
        animator.SetBool("Aiming", false);
    }
    #endregion
    //Everything related to the StandStill state
    #region Standstill Related
    private void StandStill()
    {
        //ResetParameters(animator);
        if(Waypoints.Count > 1)
        {
            aistate = AIStates.GoTowards;
        }
    }
    #endregion
    //Everything related to the Walktowards state
    #region Gotowards Related

    #endregion
    //Everything related to the Patrol state
    #region Patrol Related

    #endregion
    //Everything related to the targetEnemy state
    #region Target Enemy Related
    private void TargetEnemy()
    {
        //OnlyYLook(LookingObject, PointToAimAt);
        TrackTarget(FoundTargetScript.VisionPoint.position);
        ShootDelayTimer += Time.deltaTime;
        if (ShootDelayTimer >= FireRate)
        {
            Shoot(FoundTargetScript, Damage, Accuracy, Muzzleflash);
            Debug.Log("Shot");
            ShootDelayTimer = 0;
        }
        //Shoot every "So called seconds"

        //Shoot(FoundTargetScript, Damage, Accuracy);
    }
    private void TrackTarget(Vector3 Targetpos)
    {
        //OnlyYLook(LookingObject, Targetpos);
        //
        Vector3 DirectionLookingObject = Targetpos - LookingObject.transform.position;
        DirectionLookingObject.y = 0.0f;
        Quaternion LookRotationLookingObject = Quaternion.LookRotation(DirectionLookingObject);
        LookingObject.transform.rotation = Quaternion.Lerp(LookingObject.transform.rotation, LookRotationLookingObject, Time.deltaTime * (2000 / 360.0f));
        //
        Vector3 Direction = Targetpos - AimingObject.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(Direction);
        AimingObject.transform.rotation = Quaternion.Lerp(AimingObject.transform.rotation, lookRotation, Time.deltaTime * (2000 / 360.0f));
        //
        //AimingObject.transform.LookAt(Targetpos);
        //
        //Debug line
        Debug.DrawLine(VisionPoint.position, Targetpos, Color.green);
        //Check if target is still in range


    }
    #endregion
    //Everything related to the Target and Move state
    #region Target And Move Related
    private void TargetAndMove()
    {
        TrackTarget(FoundTargetScript.VisionPoint.position);
        ShootDelayTimer += Time.deltaTime;
        if (ShootDelayTimer >= FireRate)
        {
            Shoot(FoundTargetScript, Damage, Accuracy, Muzzleflash);
            Debug.Log("Shot");
            ShootDelayTimer = 0;
        }
    }
    #endregion
    ///
    #endregion
}
