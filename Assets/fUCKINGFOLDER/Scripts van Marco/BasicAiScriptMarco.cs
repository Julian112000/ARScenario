using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAiScriptMarco : MonoBehaviour
{
    public GameObject MyTarget;
    public Camera Mcamera;

    protected bool NoAmin;
    public bool PlayAnimations;
    private float _Health = 10;
    private bool _IsSelecting;

    public enum UnitType { Tank, Infantery }
    public enum UnitStatus { Idle, Killed, Wounded, Aiming }

    UnitStatus Status;

    public bool IsSelecting
    {
        get
        {
            return _IsSelecting;
        }
        set
        {
            _IsSelecting = value;
        }
    }

    public float Health
    {
        get
        {
            return _Health;
        }

        set
        {
            _Health = value;

            if (_Health <= 0)
            {
                Status = UnitStatus.Killed;
                Debug.Log("Bleh");
                // is weg
            }

        }
    }

    private void Awake()
    {
        Mcamera = Camera.main;
        SceneHandler.SwitchToPlayMode += ToggleAnims;
    }

    public void AimAt(GameObject Target, UnitStatus status)
    {
        transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
    }

    public GameObject CheckCast()
    {
        RaycastHit hit;
        Ray ray = Mcamera.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
    public void Aim(UnitStatus Stat)
    {
        IsSelecting = true;
        if (Input.touchCount > 0)
        {
            MyTarget = CheckCast();
            if (MyTarget != null)
            {
                AimAt(MyTarget, Status);
                IsSelecting = false;
                TestHumanScript.SetNew = false;
            }
        }
    }
    public void Aim(UnitStatus Stat, bool Local, GameObject me)
    {
        IsSelecting = true;
        if (Input.touchCount > 0)
        {
            MyTarget = CheckCast();
            ConsoleScript.Instance.SetFeedback(MessageType.TargetMessage, me.name, MyTarget.name);
            if (MyTarget != null)
            {
                AimAt(MyTarget, Status);
                IsSelecting = false;
                TestHumanScript.SetNew = false;
                Local = false;
            }
        }
    }
    public void ToggleAnims()
    {
        PlayAnimations = true;
    }







}
