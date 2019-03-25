using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitSide
{
    Terrorist,
    Soldier
}
public enum AIStates
{
    AimAtPoint,
    StandStill,
    WalkTowards,
    Patrol
}
public enum AIBehaviour
{
    React,
    Attack,
}

public class AIBasics : MonoBehaviour
{
    //Components and basic needed values
    private Animator animator;
    private UnitSide unittype;
    //Stat based values
    protected float VisionRange;
    protected float MovementSpeed;
    //Variables Based for the waypoint system
    private List<Vector3> Waypoints = new List<Vector3>();
    private int CurrentWaypoint = 0;
    private bool WantsToMove = false;

    //The Awake that is inherited by the child class, Every Unit with an AI element will have this Awake
    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
        DecideType();
    }
    //Void called for in the Awake
    private void DecideType()
    {
        if (gameObject.tag == "Soldier")
        {
            unittype = UnitSide.Soldier;
            AIsceneInfo.Friendlys.Add(this.gameObject);
        }
        else if (gameObject.tag == "Terrorist")
        {
            unittype = UnitSide.Terrorist;
            AIsceneInfo.Enemies.Add(this.gameObject);
        }
        else
        {
            Debug.Log("ERROR: Forgot to set Tag to the units!");
        }
    }
    //
    //The update that is inherited by the child class, Every Unit with an AI element will have this update
    public virtual void Update()
    {
        //Everything related to the vision of the AI
        Vision();
        //
        //This is a check so that the character will iterate when needed
        if (WantsToMove)
        {
            if(Vector3.Distance(transform.position,Waypoints[CurrentWaypoint]) > 0.3f)
            {
                MoveTowardsPoint(Waypoints[CurrentWaypoint]);
            }
            else
            {
                NavigateThroughPath();
                WantsToMove = false;
            }
        }
        //
    }
    //Alles gerelateerd voor de AI zijn visie
    #region Vision Related
    //This void belongs in the update and gets called to make the entire vision work
    public void Vision()
    {
        CheckWhoIsNear();
    }
    //This void, iterates through the list of enemies and friendlys (depending on type of the unit) and checks if they are in range 
    private void CheckWhoIsNear()
    {
        switch (unittype)
        {
            case UnitSide.Terrorist:
                for (int i = 0; i < AIsceneInfo.Friendlys.Count; i++)
                {
                    float DistanceBetween = Vector3.Distance(transform.position, AIsceneInfo.Friendlys[i].transform.position);

                    if (DistanceBetween <= VisionRange)
                    {
                        Debug.Log("Friendly In Range");
                        CheckAngle(AIsceneInfo.Friendlys[i]);
                    }
                }
                break;
            case UnitSide.Soldier:
                for (int i = 0; i < AIsceneInfo.Enemies.Count; i++)
                {
                    float DistanceBetween = Vector3.Distance(transform.position, AIsceneInfo.Enemies[i].transform.position);
                    if (DistanceBetween <= VisionRange)
                    {
                        Debug.Log("Enemy In Range");
                        CheckAngle(AIsceneInfo.Enemies[i]);
                    }
                }
                break;
            default:
                Debug.Log("Set a Unitside!");
                break;
        }
    }
    //This void gets called when a unit is in this unit's range. the purpose of this void is check if it is in a certain viewing angle
    private void CheckAngle(GameObject Target)
    {
        Vector3 TargetDirection = Vector3.Normalize(Target.transform.position - transform.position);
        float Angle = Vector3.Angle(TargetDirection, transform.forward);

        if (Angle < 90)
        {
            CheckForCollision(Target);
        }
    }
    //This void gets called as a last check. that check is checking if there is something inbetween the targeted enemy and this unit. if this is true? a official target has been found
    private void CheckForCollision(GameObject Target)
    {
        if (Physics.Linecast(transform.position + new Vector3(0, 1, 0), Target.transform.position + new Vector3(0, 1, 0)))
        {
            Debug.Log("Blocked");
        }
        else
        {
            Debug.Log("Target In Sight");
            switch (unittype)
            {
                case UnitSide.Terrorist:
                    Debug.DrawLine(transform.position + new Vector3(0, 1, 0), Target.transform.position + new Vector3(0, 1, 0), Color.red);
                    break;
                case UnitSide.Soldier:
                    Debug.DrawLine(transform.position + new Vector3(0, 1, 0), Target.transform.position + new Vector3(0, 1, 0), Color.blue);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
    //Waypoint system voor navigatie
    #region Navigation Related

    private void NavigateThroughPath()
    {
        if (Waypoints != null)
        {
            if (CurrentWaypoint < Waypoints.Count)
            {
                CurrentWaypoint++;
                WantsToMove = true;
            }
            else
            {
                Waypoints.Clear();
                CurrentWaypoint = 0;
                animator.SetBool("Walking", false);
                Debug.Log("End Reached");
            }
        }
    }

    private void MoveTowardsPoint(Vector3 TargetPoint)
    {
        transform.LookAt(TargetPoint);
        animator.SetBool("Walking", true);
        //
        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
    }

    private void PlaceWayPoint(Vector3 Position)
    {
        Waypoints.Add(Position);
    }
    #endregion
}
