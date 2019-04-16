using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStats : MonoBehaviour
{
    //The STATS of the AI Unit
    [Header("The Unit Stats")]
    [SerializeField]
    protected int Health = 100;
    [SerializeField]
    protected int Ammo = 100;
    [SerializeField]
    protected int Damage = 25;
    [SerializeField]
    protected int ViewingAngle = 90;
    [SerializeField]
    protected float VisionRange = 15f;
    [SerializeField]
    protected float MovementSpeed = 0.75f;
    [SerializeField]
    [Range(1, 100)]
    protected int Accuracy = 75;
    [SerializeField]
    protected float FireRate = 1;
}
