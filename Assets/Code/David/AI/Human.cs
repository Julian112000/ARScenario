using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : AIBasics
{
    [Header("Human Related")]
    [SerializeField]
    private Vector3 ChestOffset;

    public override void Awake()
    {
        base.Awake();
        unittype = UnitType.Human;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WantsToMove = true;
        }
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
        ActionUpdate();
    }

    //Actions made for a human (not a vechicle)
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
        animator.SetBool("Aiming", true);
        OnlyYLook(PointToAimAt);
        //
        Transform Chest;
        Chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        Chest.LookAt(PointToAimAt);
        Chest.rotation = Chest.rotation * Quaternion.Euler(ChestOffset);
        //
        Transform Head;
        Head = animator.GetBoneTransform(HumanBodyBones.Head);
        Head.LookAt(PointToAimAt);
        //Debug line
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
        animator.SetBool("Aiming", true);
        TrackTarget(FoundTargetScript.VisionPoint.position);
    }
    private void TrackTarget(Vector3 Targetpos)
    {
        Transform Chest;
        Chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        Chest.LookAt(Targetpos);
        Chest.rotation = Chest.rotation * Quaternion.Euler(ChestOffset);
        //
        Transform Head;
        Head = animator.GetBoneTransform(HumanBodyBones.Head);
        Head.LookAt(Targetpos);
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
    }
    #endregion
    ///
    #endregion
}
