using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStats : MonoBehaviour
{
    //The STATS of the AI Unit
    [Header("The Unit Stats")]
    [SerializeField]
    public int Health = 100;
    [SerializeField]
    public int Ammo = 100;
    [SerializeField]
    public int ViewingAngle = 90;
    [SerializeField]
    public float VisionRange = 15f;
    [SerializeField]
    public float MovementSpeed = 0.75f;
    [SerializeField]
    [Range(1, 100)]
    public int Accuracy = 75;
    //
    protected float FireRate = 1;
    protected int Damage = 25;
}
