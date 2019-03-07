using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAiScriptb : BasicAiScriptMarco
{

    UnitType MyType;
    UnitStatus UnitStat;

    public bool PlayAnimations;
    public Transform Turret;
    public Animator MyAnim;

    private void Start()
    {
        MyType = UnitType.Tank;
        UnitStat = UnitStatus.Idle;
        if(MyAnim != null)
        {
            MyAnim.SetBool("Start", false);
            NoAmin = false;
        }
        else
        {
            NoAmin = true;
        }
    }

    public void PlayAnimation()
    {
        PlayAnimations = true;

        switch (UnitStat)
        {
            case UnitStatus.Aiming:
                Debug.Log("I am aiming");
                MyAnim.SetBool("Start", false);
                break;
            case UnitStatus.Idle:
                Debug.Log("Idle");
                MyAnim.SetBool("Start", true);
                break;
            default:
                Debug.Log("Geen geldige informatie");
                break;
        }
    }
    private void Update()
    {
        if (IsSelecting)
        {
            Aim(UnitStat);
        }
    }

}
