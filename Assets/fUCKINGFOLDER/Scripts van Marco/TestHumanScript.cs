using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHumanScript : BasicAiScriptMarco
{
   
    UnitType MyType;
    UnitStatus UnitStat;

    public bool PlayAnimations;
    public Animation MyAnimations;
    public AnimationClip Idle, Aiming;

    private void Start()
    {
        MyType = UnitType.Infantery;

        if(MyAnimations = GetComponent<Animation>())
        {
            NoAmin = false;
        }
        else
        {
            NoAmin = true;
        }

        if (!PlayAnimations && !NoAmin)
        {
            MyAnimations.AddClip(Idle, "Idle");
            MyAnimations.AddClip(Aiming, "Aiming");
            
        }
        else
        {
            NoAmin = true;
        }
        
    }

    public void SelectTarget()
    {
        IsSelecting = true;
    }

    public void SelectAnimatie(int AniNumber)
    {
        switch (AniNumber)
        {
            case 0:
                UnitStat = UnitStatus.Idle;
                break;
            case 1:
                UnitStat = UnitStatus.Aiming;
                break;
            default:
                UnitStat = UnitStatus.Wounded;
                break;
        }
    }

    public void PLayAnimation()
    {
        PlayAnimations = true;
        switch (UnitStat)
        {
            case UnitStatus.Aiming:
                MyAnimations.Play("Aiming");
                //MyAnimations.Blend("Aiming", 1.0f, 0.3f);
                
                break;
            case UnitStatus.Idle:
                MyAnimations.Blend("Idle", 1.0f, 0.3f);
                //MyAnimations.Play("Idle");

                break;
            default:
                Debug.Log("No anim");
                break;
        }
    }


    private void Update()
    {
        if (IsSelecting)
        {
            Aim(UnitStat);
        }
        if (Input.GetKey(KeyCode.Space) && !NoAmin)
        {
            Health--;
            if(UnitStat == UnitStatus.Aiming)
            {
                UnitStat = UnitStatus.Idle;
            }
            else
            {
                UnitStat = UnitStatus.Aiming;
            }
            PLayAnimation();
        }
        if (!NoAmin)
        {
            if (!MyAnimations.isPlaying && PlayAnimations)
            {
                PLayAnimation();
            }
        }
    }







}
