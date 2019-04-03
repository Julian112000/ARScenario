using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitSide
{
    Terrorist,
    Friendly
}
public enum UnitType
{
    Human,
    Tank
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
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LineRenderer))]
public abstract class AIBasics : AIStats
{
    //Components and basic needed values
    private Animator animator;
    private LineRenderer linerenderer;
    private UnitSide unitside;
    private AIStates aistate;
    protected UnitType unittype;
    [SerializeField]
    public Transform VisionPoint;
    [SerializeField]
    public GameObject LookingObject;
    [SerializeField]
    public GameObject Spine;
    //Stat based values
    protected float VisionRange;
    protected float MovementSpeed;
    //Variables Based for the waypoint system
    [SerializeField]
    private List<Vector3> Waypoints = new List<Vector3>();
    private int CurrentWaypoint = 0;
    protected bool WantsToMove = false;
    //Variables For when the unit is selected to aim at a certain point
    //The offset when aiming (this is because the animation might not always work)
    [SerializeField]
    private Vector3 ChestOffset;
    private Vector3 PointToAimAt;
    public GameObject Target;
    public bool Selected;


    //Constructor of this class
    public AIBasics()
    {

    }
    //The Awake that is inherited by the child class, Every Unit with an AI element will have this Awake
    public virtual void Awake()
    {
        //GetComponents
        animator = GetComponent<Animator>();
        linerenderer = GetComponent<LineRenderer>();
        //Voids
        DecideType();
        //Subscribe to play event
        SceneHandler.SwitchToPlayMode += PlayMode;
        //Linerenderer
        linerenderer.SetWidth(0.1f, 0.1f);
        linerenderer.SetPosition(0, transform.position);
        //
        Waypoints.Add(transform.position);
    }
    //Voids called for in the Awake
    #region Awake Voids
    //Void to Decide if this is a Terrorist unit or a friendly Unit
    private void DecideType()
    {
        if (gameObject.tag == "Friendly")
        {
            unitside = UnitSide.Friendly;
            AIsceneInfo.Friendlys.Add(this.gameObject);
            AIsceneInfo.FriendlysScripts.Add(this);
            ChooseLineColor(Color.blue);
        }
        else if (gameObject.tag == "Terrorist")
        {
            unitside = UnitSide.Terrorist;
            AIsceneInfo.Enemies.Add(this.gameObject);
            AIsceneInfo.EnemiesScripts.Add(this);
            ChooseLineColor(Color.red);
        }
        else
        {
            Debug.Log("ERROR: Forgot to set Tag to the units!");
        }
    }
    //Void to choose which color the Line will get
    private void ChooseLineColor(Color color)
    {
        linerenderer.startColor = color;
        linerenderer.endColor = color;
        linerenderer.material.color = color;
    }
    //
    #endregion
    #region EventSubscribed Voids
    public void PlayMode()
    {
        WantsToMove = true;
    }
    #endregion
    //The update that is inherited by the child class, Every Unit with an AI element will have this update
    public virtual void Update()
    {
        //Different Types of Updates (peek definition of update to see what it does)
        VisionUpdate();
        NavigationUpdate();
    }
    public virtual void LateUpdate()
    {
        //
        ActionUpdate();
    }
    //Everything related to the vision of the AI
    #region Vision Related
    //This void belongs in the update and gets called to make the entire vision work
    public void VisionUpdate()
    {
        CheckWhoIsNear();
    }
    //This void, iterates through the list of enemies and friendlys (depending on type of the unit) and checks if they are in range 
    private void CheckWhoIsNear()
    {
        AIBasics targetAIscript;
        switch (unitside)
        {
            case UnitSide.Terrorist:
                for (int i = 0; i < AIsceneInfo.Friendlys.Count; i++)
                {
                    float DistanceBetween = Vector3.Distance(transform.position, AIsceneInfo.Friendlys[i].transform.position);

                    if (DistanceBetween <= VisionRange)
                    {
                        Debug.Log("Friendly In Range");
                        CheckAngle(AIsceneInfo.Friendlys[i], AIsceneInfo.FriendlysScripts[i]);
                    }
                }
                break;
            case UnitSide.Friendly:
                for (int i = 0; i < AIsceneInfo.Enemies.Count; i++)
                {
                    float DistanceBetween = Vector3.Distance(transform.position, AIsceneInfo.Enemies[i].transform.position);
                    if (DistanceBetween <= VisionRange)
                    {
                        Debug.Log("Enemy In Range");
                        CheckAngle(AIsceneInfo.Enemies[i], AIsceneInfo.EnemiesScripts[i]);
                    }
                }
                break;
            default:
                Debug.Log("Set a Unitside!");
                break;
        }
    }
    //This void gets called when a unit is in this unit's range. the purpose of this void is check if it is in a certain viewing angle
    private void CheckAngle(GameObject Target, AIBasics TargetScript)
    {
        Vector3 TargetDirection = Vector3.Normalize(Target.transform.position - transform.position);
        float Angle = Vector3.Angle(TargetDirection, transform.forward);

        if (Angle < 90)
        {
            CheckForCollision(Target, TargetScript);
        }
    }
    //This void gets called as a last check. that check is checking if there is something inbetween the targeted enemy and this unit. if this is true? a official target has been found
    private void CheckForCollision(GameObject Target, AIBasics TargetScript)
    {
        RaycastHit hit;
        if (Physics.Linecast(VisionPoint.position, TargetScript.VisionPoint.position, out hit))
        {
            Debug.Log("Blocked");
        }
        else
        {
            Debug.Log("Target In Sight");
            //Switchcase for debugging and also for deciding what to do when enemy is spotted
            switch (unitside)
            {
                case UnitSide.Terrorist:
                    Debug.DrawLine(VisionPoint.position, TargetScript.VisionPoint.position, Color.red);
                    //LookingObject.transform.LookAt(Target.transform.position);
                    break;
                case UnitSide.Friendly:
                    Debug.DrawLine(VisionPoint.position, TargetScript.VisionPoint.position, Color.blue);
                    //LookingObject.transform.LookAt(TargetScript.VisionPoint.position);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
    //Waypoint system for navigation of the AI
    #region Navigation Related
    //The update that handles Navigation
    private void NavigationUpdate()
    {
        //This is a check so that the character will iterate when needed
        if (WantsToMove)
        {
            if (Waypoints.Count > 0)
            {
                if (Vector3.Distance(transform.position, Waypoints[CurrentWaypoint]) > 0.3f)
                {
                    MoveTowardsPoint(Waypoints[CurrentWaypoint]);
                }
                else
                {
                    NavigateThroughPath();
                }
            }
            else
            {
                NavigateThroughPath();
            }
        }
        //Check if there is a waypoint And if there is the linerender will be setactive
        LineRendererUpdate();
    }
    //Void that gets called to skip to the next waypoint or cancel walking when you've reached the end
    private void NavigateThroughPath()
    {
        CurrentWaypoint++;
        if (CurrentWaypoint < Waypoints.Count)
        {
            WantsToMove = true;
        }
        else
        {
            ClearWaypoints();
        }
    }
    //Void that gets called when you are moving towards a waypoint (this void moves the character forward)
    private void MoveTowardsPoint(Vector3 TargetPoint)
    {
        //Walk animation gets played
        animator.SetBool("Walking", true);
        //Make character look at target so that forward is towards the point
        transform.LookAt(TargetPoint);
        //Actual movement
        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
        //Make line get swallowed by player
        for (int i = 0; i < CurrentWaypoint; i++)
        {
            linerenderer.SetPosition(i, transform.position);
        }
    }
    //Void that is used when you are done with all the waypoints, Or you want to make a new path so u have to clear the old one
    public void ClearWaypoints()
    {
        WantsToMove = false;
        Waypoints.Clear();
        CurrentWaypoint = 0;
        animator.SetBool("Walking", false);
        Waypoints.Add(transform.position);
        linerenderer.positionCount = Waypoints.Count;
        Debug.Log("Waypoints Cleared!");
    }
    //Void that gets called in the building phase this is what places the waypoints and adds them to the units list
    public void PlaceWayPoint(Vector3 Position)
    {
        Waypoints.Add(Position);
        linerenderer.positionCount = Waypoints.Count;
        linerenderer.SetPosition(Waypoints.Count - 1, Waypoints[Waypoints.Count - 1]);
    }
    //Undo the last placed waypoint, Useful for when you place a waypoint but did not want to place it there
    public void UndoLastWaypoint()
    {
        if (Waypoints.Count > 1)
        {
            Waypoints.Remove(Waypoints[Waypoints.Count - 1]);
            linerenderer.positionCount = Waypoints.Count;
            Debug.Log("Undo Happened!");
        }
    }
    //

    //LINERENDERER RELATED ONLY
    private void LineRendererUpdate()
    {
        if (Selected)
        {
            TurnRendererOn();
        }
        else if (!Selected)
        {
            TurnRendererOff();
        }
    }
    //turn off linerenderer
    public void TurnRendererOff()
    {
        linerenderer.enabled = false;
    }
    //Turn on linerenderer
    public void TurnRendererOn()
    {
        linerenderer.enabled = true;
    }
    #endregion
    //Combat related voids for the AI
    #region Combat Related
    //Void called from the script damaging this object, parameters are given and damage is decided on given parameters
    public void TakeDamage(UnitType ShooterType, int ShooterDamage)
    {
        switch (unittype)
        {
            case UnitType.Human:
                switch (ShooterType)
                {
                    case UnitType.Human:
                        Debug.Log("Human Shot Human");
                        break;
                    case UnitType.Tank:
                        Debug.Log("Tank Shot Human");
                        break;
                    default:
                        break;
                }
                break;
            case UnitType.Tank:
                switch (ShooterType)
                {
                    case UnitType.Human:
                        Debug.Log("Human Shot Tank, No Effects");
                        break;
                    case UnitType.Tank:
                        Debug.Log("Tank Shot Tank");
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
    //Actual removehealth this gets called inside the takedamage void
    private void RemoveHealth(int RemoveAmount)
    {
        Health = Health - RemoveAmount;
    }
    //Shoot void this is a General shoot void for every unit Parameters decide what ammo and damage is linked to the shoot
    private void Shoot()
    {

    }
    #endregion
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
                break;
            case AIStates.WalkTowards:
                break;
            case AIStates.Patrol:
                break;
            default:
                break;
        }
    }
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
    }
    public void StopAimingAtPoint()
    {
        animator.SetBool("Aiming", false);
    }

    #endregion
    //Self made voids to make my life helpful
    #region HelpFullVoids
    //A lookat to only rotate the Y axis
    private void OnlyYLook(Vector3 Target)
    {
        Vector3 lookPos = Target - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
    }
    #endregion
}

