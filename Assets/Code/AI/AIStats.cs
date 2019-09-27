using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStats : MonoBehaviour
{
    //The STATS of the AI Unit
    [Header("The Unit Stats")]
    [SerializeField]
    public int Health = 100;        //Health of this unit (0 means death)
    [SerializeField]
    public int Ammo = 100;          //Ammo of this unit (0 means shooting is no longer possible)
    [SerializeField]
    public int ViewingAngle = 90;       //Viewing angle of this unit (This of this as a cone that the player uses to view)
    [SerializeField]
    public float VisionRange = 15f;     //The length the "Viewing cone" is (so the distance the unit can look)
    [SerializeField]
    public float MovementSpeed = 0.75f;     //The speed the unit moves at
    [SerializeField]
    [Range(1, 100)]
    public int Accuracy = 75;           //Shooting accuracy 100 means 10/10 hits, 50 means 5/10 hits
    //
    protected float FireRate = 1;       //The speed the unit shoots at
    protected int Damage = 25;          //The damage the unit does 25 means 25 out of for example 100 health (could change if the target getting shot at has armor)
}
