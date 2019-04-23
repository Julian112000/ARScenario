using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    AssaultRifle,
    RPG,
    Sniper
}

public class Human : AIBasics
{
    [Header("Human Related")]
    [SerializeField]
    private Vector3 ChestOffset;
    [SerializeField]
    private bool UsingIkHandling = true;
    [Header("Weapon Related")]
    [SerializeField]
    private WeaponType weaponType = WeaponType.AssaultRifle;
    [SerializeField]
    private List<GameObject> Weapons;
    [SerializeField]
    private List<int> WeaponDamageValues;
    [SerializeField]
    private List<float> WeaponFireRates;
    [SerializeField]
    private List<ParticleSystem> WeaponMuzzleFlashes;
    [SerializeField]
    private List<Transform> IkPositions;
    [SerializeField]
    private GameObject CurrentWeapon;
    [Header("RPG Related")]
    [SerializeField]
    private RPGBulletScript RpgBullet;

    public override void Awake()
    {
        base.Awake();
        unittype = UnitType.Human;
        WeaponUpdate((int)weaponType);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (PlayModeOn)
        {
            base.Update();
        }
           
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
        ActionUpdate();
    }
    //
    public void WeaponUpdate(int WeaponNumber)
    {
        if (WeaponNumber <= Weapons.Count)
        {
            weaponType = (WeaponType)WeaponNumber;
            for (int i = 0; i < Weapons.Count; i++)
            {
                if (i == (int)weaponType)
                {
                    //Debug.Log("Ok");
                    if (CurrentWeapon)
                    {
                        CurrentWeapon.SetActive(false);
                    }
                    Weapons[i].SetActive(true);
                    CurrentWeapon = Weapons[i];
                    Damage = WeaponDamageValues[i];
                    FireRate = WeaponFireRates[i];
                }
            }
        }
        else
        {
            Debug.Log("Weapon Number is not valid weapon does not exist");
        }
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
        if (Waypoints.Count > 1)
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
        animator.SetBool("Aiming", true);
        TrackTarget(FoundTargetScript.VisionPoint.position);
        ShootDelayTimer += Time.deltaTime;
        if (ShootDelayTimer >= FireRate)
        {
            Shoot(FoundTargetScript, Damage, Accuracy, WeaponMuzzleFlashes[(int)weaponType], weaponType, RpgBullet);
            Debug.Log("Shot");
            ShootDelayTimer = 0;
        }
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
        ShootDelayTimer += Time.deltaTime;
        if (ShootDelayTimer >= FireRate)
        {
            Shoot(FoundTargetScript, Damage, Accuracy, WeaponMuzzleFlashes[(int)weaponType], weaponType, RpgBullet);
            Debug.Log("Shot");
            ShootDelayTimer = 0;
        }
    }
    #endregion
    ///
    #endregion
    //Ik handling so that the hand is positioned at the gun
    #region IkHandling

    void OnAnimatorIK()
    {
        if (UsingIkHandling)
        {
            if (animator)
            {
                // Set the right hand target position and rotation, if one has been assigned
                if (IkPositions[(int)weaponType] != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, IkPositions[(int)weaponType].transform.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, IkPositions[(int)weaponType].transform.rotation);
                }
            }
        }
    }

    #endregion
}
