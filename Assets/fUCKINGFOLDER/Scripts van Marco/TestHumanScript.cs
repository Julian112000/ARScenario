using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHumanScript : BasicAiScriptMarco
{

    UnitType MyType;
    UnitStatus UnitStat;
    public static bool SetNew;
    public bool SetNewT;
    public Animation MyAnimations;
    private Animator animator;
    private void Start()
    {
        MyType = UnitType.Infantery;

        if (animator = GetComponent<Animator>())
        {
            NoAmin = false;
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
                animator.SetBool("Aiming", true);
                //MyAnimations.Blend("Aiming", 1.0f, 0.3f);

                break;
            case UnitStatus.Idle:
                animator.SetBool("Aiming", false);

                break;
            default:
                Debug.Log("No anim");
                break;
        }
    }


    private void Update()
    {
        if (PlayAnimations)
        {
            PLayAnimation();
        }

        if (SetNew || SetNewT)
        {
            UnitStat = UnitStatus.Aiming;

            if (SetNewT)
            {
                Aim(UnitStat, SetNewT, this.gameObject);
            }
            else
            {
                Aim(UnitStat);
            }
            if (!NoAmin)
            {
                PLayAnimation();
            }

        }
        if (Input.GetKeyDown(KeyCode.Space) && !NoAmin)
        {
            Health--;
            if (UnitStat == UnitStatus.Aiming)
            {
                UnitStat = UnitStatus.Idle;
            }
            else
            {
                UnitStat = UnitStatus.Aiming;
            }
            PLayAnimation();
        }
    }







}
