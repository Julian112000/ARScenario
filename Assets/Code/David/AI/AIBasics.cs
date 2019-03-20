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
    private Animator animator;
    private UnitSide unittype;
    protected float VisionRange;
    protected float MovementSpeed;
    //Variables Based for the waypoint system
    private List<Vector3> Waypoints = new List<Vector3>();
    private int CurrentWaypoint = 0;
    private bool WantsToMove = false;

    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
        DecideType();
    }

    public virtual void Update()
    {
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
        //Test input for placing waypoints
        if (Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            pos.z = 45;
            pos = Camera.main.ScreenToWorldPoint(pos);
            PlaceWayPoint(pos);
        }
    }
    
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

    #region Vision Related
    //De uiteindelijke void die gecalled word in de update voor vision control
    public void Vision()
    {
        CheckWhoIsNear();
    }
    //Deze void loopt door de lijst van of enemies of friendlys heen en kijkt of er units die niet bij hem horen in zijn range staan
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
    //Deze void word gecalled als er een unit in een unit zijn range staat. vervolgens kijkt deze void of dat die unit in een bepaalde angel van hem staat
    private void CheckAngle(GameObject Target)
    {
        Vector3 TargetDirection = Vector3.Normalize(Target.transform.position - transform.position);
        float Angle = Vector3.Angle(TargetDirection, transform.forward);

        if (Angle < 90)
        {
            CheckForCollision(Target);
        }
    }
    //Deze void word gecalled als er een unit binnen de targetrange staat en binnen de juiste angle staat. Zo ja dan doe ik een Linecast die checkt of er een object tussen hun twee in staat. Staat er niks? dan is hij officieel in zicht
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


    private void NavigateThroughPath()
    {
        if(Waypoints != null)
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
}
