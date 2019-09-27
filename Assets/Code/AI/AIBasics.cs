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
    Tank,
    Fennek
}
public enum AIStates
{
    AimAtPoint,
    StandStill,
    GoTowards,
    Patrol,
    TargetEnemy,
    TargetAndMove,
    Dead
}
public enum AIBehaviour
{
    Passive,
    React,
    Attack,
    Destroy
}
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LineRenderer))]
public abstract class AIBasics : AIStats        //This class inherits from AIstats which holds the information about Health etc.
{
    //Components and basic needed values
    protected Animator animator;                    //The animator of this unit
    protected LineRenderer linerenderer;            //Linerenderer used for waypointing feedback
    [Header("Enums")]
    [SerializeField]
    private UnitSide unitside;                      //Side this unit is of  
    [SerializeField]
    public AIStates aistate;                        //The state this Unit is in
    public UnitType unittype;                       //Type of this unit
    [SerializeField]
    public AIBehaviour Behaviour;                   //The behaviour this Unit is using
    [Header("Needed Objects")]
    [SerializeField]
    public Transform VisionPoint;                   //Point on unit where vision had to come from
    [SerializeField]
    protected GameObject LookingObject;             //Object that rotates towards the point the unit is looking at
    [SerializeField]
    private GameObject WaypointPrefab;          //Prefab for the waypoint that has to be placed when making a route
    [SerializeField]
    private LayerMask LineCastLayers;           //Layermask for the linecasting
    [SerializeField]
    private GameObject SelectedArrow;           //The arrow above the unit when it gets selected
    [SerializeField]
    private List<ParticleSystem> HitParticles;      //List of particles that are used when the unit is hit
    [SerializeField]
    private List<ParticleSystem> ExplosionParticles;        //List of explosion particles used for when the unit gets hit by an explosive
    //Basic Variables
    [Header("Basic Variables")]
    public bool Selected;           //Booleon to check if this unit is selected
    [SerializeField]
    protected GameObject FoundTarget;           //The target found by this unit
    protected AIBasics FoundTargetScript;       //The script of the target found by this unit
    protected float ShootDelayTimer;            //The delay between shots of this unit
    [SerializeField]
    private float TargetDelayTimer;             //Delay needed for checking who is targeted
    [SerializeField]
    protected bool PlayModeOn = false;          //If the game is on playmode (used to call play event)
    /// <summary>
    /// Variables For when the unit is selected to aim at a certain point
    /// The offset when aiming (this is because the animation might not always work)
    /// </summary>
    protected Vector3 PointToAimAt;     //The point the unit has to aim at
    //Variables Based for the waypoint system
    [Header("Waypoint related")]
    [SerializeField]
    protected List<Vector3> Waypoints = new List<Vector3>();        //List of all the waypoint positions
    [SerializeField]
    private List<WaypointScript> waypointscripts = new List<WaypointScript>();      //list of scripts of all the waypoints placed for this unit
    [SerializeField]
    private int CurrentWaypoint = 0;        //The current waypoint being used
    [SerializeField]
    protected bool WantsToMove = false;     //If they unit wants to move along its route


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
        //it's used for route visuals
        linerenderer.SetWidth(0.1f, 0.1f);  
        linerenderer.SetPosition(0, transform.position);
        //Add the first waypoint which is your position
        Waypoints.Add(transform.position);
        //
        Selected = false;
        TurnOffVisuals();
    }
    //Voids called for in the Awake
    #region Awake Voids
    /// <summary>
    /// Void to Decide if this is a Terrorist unit or a friendly Unit
    /// </summary>
    private void DecideType()
    {
        if (gameObject.tag == "Friendly")
        {
            unitside = UnitSide.Friendly;
            AIsceneInfo.Friendlys.Add(this.gameObject);
            AIsceneInfo.FriendlysScripts.Add(this);
        }
        else if (gameObject.tag == "Terrorist")
        {
            unitside = UnitSide.Terrorist;
            AIsceneInfo.Enemies.Add(this.gameObject);
            AIsceneInfo.EnemiesScripts.Add(this);
        }
        else
        {
            Debug.Log("ERROR: Forgot to set Tag to the units!");
        }
    }
    //
    #endregion
    #region EventSubscribed Voids
    /// <summary>
    /// The playmode void which is subscribed to and called when the play event is
    /// </summary>
    public void PlayMode()
    {
        WantsToMove = true;
        TurnOffVisuals();
        PlayModeOn = true;
        Debug.Log("HAPPENEEEEED!");
    }
    #endregion
    /// <summary>
    /// The update that is inherited by the child class, Every Unit with an AI element will have this update
    /// </summary>
    public virtual void Update()
    {
        //Different Types of Updates (peek definition of update to see what it does)
        if(aistate != AIStates.Dead)
        {
            if (PlayModeOn)
            {
                VisionUpdate();
                NavigationUpdate();
            }
        }
    }
    public virtual void LateUpdate()
    {
        //
    }
    /// <summary>
    /// Everything related to the vision of the AI
    /// </summary>
    #region Vision Related
    //This void belongs in the update and gets called to make the entire vision work
    public void VisionUpdate()
    {
        if (FoundTarget == null)
        {
            CheckWhoIsNear();
        }
        else if(FoundTarget != null)
        {
            CheckVisionToTarget();
        }
    }
    //This void, iterates through the list of enemies and friendlys (depending on type of the unit) and checks if they are in range 
    private void CheckWhoIsNear()
    {
        switch (unitside)
        {
            case UnitSide.Terrorist:
                for (int i = 0; i < AIsceneInfo.Friendlys.Count; i++)
                {
                    if (AIsceneInfo.FriendlysScripts[i].aistate != AIStates.Dead)
                    {
                        float DistanceBetween = Vector3.Distance(transform.position, AIsceneInfo.Friendlys[i].transform.position);

                        if (DistanceBetween <= VisionRange)
                        {
                            Debug.Log("Friendly In Range");
                            CheckAngle(AIsceneInfo.Friendlys[i], AIsceneInfo.FriendlysScripts[i]);
                        }
                    }
                }
                break;
            case UnitSide.Friendly:
                for (int i = 0; i < AIsceneInfo.Enemies.Count; i++)
                {
                    if (AIsceneInfo.EnemiesScripts[i].aistate != AIStates.Dead)
                    {
                        float DistanceBetween = Vector3.Distance(transform.position, AIsceneInfo.Enemies[i].transform.position);
                        if (DistanceBetween <= VisionRange)
                        {
                            Debug.Log("Enemy In Range");
                            CheckAngle(AIsceneInfo.Enemies[i], AIsceneInfo.EnemiesScripts[i]);
                        }
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

        if (Angle < ViewingAngle)
        {
            CheckForCollision(Target, TargetScript);
        }
    }
    //This void gets called as a last check. that check is checking if there is something inbetween the targeted enemy and this unit. if this is true? a official target has been found
    private void CheckForCollision(GameObject Target, AIBasics TargetScript)
    {
        RaycastHit hit;
        if (Physics.Linecast(VisionPoint.position, TargetScript.VisionPoint.position, out hit, LineCastLayers))
        {
            Debug.Log("Blocked");
        }
        else
        {
            Debug.Log("Target In Sight");
            TargetDelayTimer += Time.deltaTime;
            if (TargetDelayTimer >= 0.5f)
            {
                //Switchcase for debugging and also for deciding what to do when enemy is spotted
                switch (unitside)
                {
                    case UnitSide.Terrorist:
                        Debug.DrawLine(VisionPoint.position, TargetScript.VisionPoint.position, Color.red);
                        EnemySpotted();
                        FoundTarget = Target;
                        FoundTargetScript = TargetScript;
                        TargetDelayTimer = 0;
                        //LookingObject.transform.LookAt(Target.transform.position);
                        break;
                    case UnitSide.Friendly:
                        Debug.DrawLine(VisionPoint.position, TargetScript.VisionPoint.position, Color.blue);
                        EnemySpotted();
                        FoundTarget = Target;
                        FoundTargetScript = TargetScript;
                        TargetDelayTimer = 0;
                        //LookingObject.transform.LookAt(TargetScript.VisionPoint.position);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    //Check if the currentTarget is still in range and if he is not behind something
    public void CheckVisionToTarget()
    {
        float DistanceBetween = Vector3.Distance(transform.position, FoundTarget.transform.position);

        if (DistanceBetween <= VisionRange)
        {
            CheckAngleFoundTarget();
        }
        else
        {
            ResetTarget();
        }
    }
    //Check if the found target is within the viewingangle of this unit (sight code)
    private void CheckAngleFoundTarget()
    {
        Vector3 TargetDirection = Vector3.Normalize(FoundTarget.transform.position - transform.position);
        float Angle = Vector3.Angle(TargetDirection, transform.forward);

        if (Angle < ViewingAngle)
        {
            CheckCollisionFoundTarget();
        }
        else
        {
            ResetTarget();
        }
    }
    //Check if there is an object inbetween the target and this unit (Check if sight is blocked)
    private void CheckCollisionFoundTarget()
    {
        RaycastHit hit;
        if (Physics.Linecast(VisionPoint.position, FoundTargetScript.VisionPoint.position, out hit, LineCastLayers))
        {
            ResetTarget();
        }

    }
    //Reset the target ( used for when target is out of sight or dead)
    public void ResetTarget()
    {
        switch (aistate)
        {
            case AIStates.TargetEnemy:
                aistate = AIStates.StandStill;
                break;
            case AIStates.TargetAndMove:
                if(Waypoints.Count > 1)
                {
                    aistate = AIStates.GoTowards;
                }
                break;
            default:
                break;
        }
        FoundTarget = null;
        FoundTargetScript = null;
        ResetParameters(animator);
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
                switch (aistate)
                {
                    case AIStates.GoTowards:
                        GoTowardsUpdate();
                        break;
                    case AIStates.Patrol:
                        PatrolUpdate();
                        break;
                    case AIStates.TargetAndMove:
                        TargetAndMoveUpdate();
                        break;
                    case AIStates.Dead:
                        WantsToMove = false;
                        break;
                }
            }
            else
            {
                NavigateThroughPath();
            }
        }
    }
    //When the state is on gotowards this is the update
    private void GoTowardsUpdate()
    {
        if (Vector3.Distance(transform.position, Waypoints[CurrentWaypoint]) > 0.3f)
        {
            //Walk animation gets played
            animator.SetBool("Walking", true);
            //
            MoveTowardsPoint(Waypoints[CurrentWaypoint]);
        }
        else
        {
            NavigateThroughPath();
        }
    }
    //When the state is on Patrol this is the update
    private void PatrolUpdate()
    {
        if (Vector3.Distance(transform.position, Waypoints[CurrentWaypoint]) > 0.3f)
        {
            //Walk animation gets played
            animator.SetBool("Walking", true);
            //
            MoveTowardsPoint(Waypoints[CurrentWaypoint]);
        }
        else
        {
            LoopThroughPath();
        }
    }
    //The update that is called when targeting and moving this handles just the moving of that part
    private void TargetAndMoveUpdate()
    {
        if (Vector3.Distance(transform.position, Waypoints[CurrentWaypoint]) > 0.3f)
        {
            //Walk animation gets played
            animator.SetBool("AimWalk", true);
            //
            MoveTowardsPoint(Waypoints[CurrentWaypoint]);
        }
        else
        {
            NavigateThroughPath();
        }
    }
    //Void that gets called to skip to the next waypoint or cancel walking when you've reached the end
    private void NavigateThroughPath()
    {
        CurrentWaypoint++;
        if (CurrentWaypoint < Waypoints.Count)
        {
            if (CurrentWaypoint > 1)
            {
                Destroy(waypointscripts[0].gameObject);
                waypointscripts.Remove(waypointscripts[0]);
            }
            WantsToMove = true;
        }
        else
        {
            ClearWaypoints();
            switch (aistate)
            {
                case AIStates.TargetAndMove:
                    aistate = AIStates.TargetEnemy;
                    break;
                default:
                    break;
            }
        }
    }
    //Void that is almost the same as navigate throughpath but it resets and repeats the process when at the end
    private void LoopThroughPath()
    {
        CurrentWaypoint++;
        if (CurrentWaypoint < Waypoints.Count)
        {
            WantsToMove = true;
        }
        else
        {
            CurrentWaypoint = 1;
            WantsToMove = true;
        }
    }
    //Void that gets called when you are moving towards a waypoint (this void moves the character forward)
    private void MoveTowardsPoint(Vector3 TargetPoint)
    {
        Vector3 Direction = TargetPoint - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(Direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * (2000 / 360.0f));
        //
        //Make character look at target so that forward is towards the point
        //transform.LookAt(TargetPoint);
        //Actual movement
        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
        //Make line get swallowed by player
        if(aistate == AIStates.GoTowards || aistate == AIStates.TargetAndMove)
        {
            for (int i = 0; i < CurrentWaypoint; i++)
            {
                linerenderer.SetPosition(i, transform.position);
            }
        }

        if (unittype == UnitType.Fennek) SoundManager.Instance.PlayAudioClip("Fennek_Moving", transform);
        //else if (unittype == UnitType.Human) SoundManager.Instance.PlayAudioClip("Human_Walk", 1f);
    }
    //Void that is used when you are done with all the waypoints, Or you want to make a new path so u have to clear the old one
    public void ClearWaypoints()
    {
        WantsToMove = false;
        Waypoints.Clear();
        foreach (var Waypoint in waypointscripts)
        {
            Destroy(Waypoint.gameObject);
        }
        waypointscripts.Clear();
        CurrentWaypoint = 0;
        Waypoints.Add(transform.position);
        linerenderer.positionCount = Waypoints.Count;
        switch (aistate)
        {
            case AIStates.TargetAndMove:
                animator.SetBool("Aiming", true);
                animator.SetBool("AimWalk", false);
                aistate = AIStates.TargetEnemy;
                break;
            default:
                animator.SetBool("Walking", false);
                animator.SetBool("AimWalk", false);
                aistate = AIStates.StandStill;
                //Debug.Log("HAPPEND");
                break;
        }
        Debug.Log("Waypoints Cleared!");
    }
    //Void that gets called in the building phase this is what places the waypoints and adds them to the units list
    public void PlaceWayPoint(Vector3 Position, WaypointScript waypointscript)
    {
        Waypoints.Add(Position);
        waypointscripts.Add(waypointscript);
        //
        linerenderer.positionCount = Waypoints.Count;
        linerenderer.SetPosition(Waypoints.Count - 1, Waypoints[Waypoints.Count - 1]);
    }
    //Undo the last placed waypoint, Useful for when you place a waypoint but did not want to place it there
    public void UndoLastWaypoint()
    {
        if (Waypoints.Count > 1)
        {
            Waypoints.Remove(Waypoints[Waypoints.Count - 1]);
            Destroy(waypointscripts[waypointscripts.Count - 1].gameObject);
            waypointscripts.Remove(waypointscripts[waypointscripts.Count - 1]);
            linerenderer.positionCount = Waypoints.Count;
            Debug.Log("Undo Happened!");
        }
    }
    //Reposition the selected waypoint
    public void RepositionWaypoint(WaypointScript Waypoint, Vector3 NewPosition)
    {
        for (int i = 0; i < waypointscripts.Count; i++)
        {
            if (waypointscripts[i] == Waypoint)
            {
                linerenderer.SetPosition(i + 1, NewPosition);
                Waypoints[i + 1] = NewPosition;
                Waypoint.gameObject.transform.position = NewPosition;
            }
        }
    }
    //Make it so that the last waypoint connects with the first waypoint
    public void ConnectLastToFirst()
    {
        GameObject SpawnedObject = Instantiate(WaypointPrefab, transform.position, waypointscripts[0].gameObject.transform.rotation);
        WaypointScript script = SpawnedObject.GetComponent<WaypointScript>();
        //
        Waypoints.Add(transform.position);
        waypointscripts.Add(script);
        //
        linerenderer.positionCount = Waypoints.Count;
        linerenderer.SetPosition(Waypoints.Count - 1, transform.position);
    }
    //
    #endregion
    //Combat related voids for the AI
    #region Combat Related
    //Void called from the script damaging this object, parameters are given and damage is decided on given parameters
    public void TakeDamage(UnitType ShooterType, AIBasics Shooter, int ShooterDamage)
    {
        switch (unittype)
        {
            case UnitType.Human:
                switch (ShooterType)
                {
                    case UnitType.Human:
                        RemoveHealth(ShooterDamage, Shooter);
                        PlayBulletImpact();
                        Debug.Log("Human Shot Human, Default Damage done");
                        SoundManager.Instance.PlayAudioClip("Grund_" + Random.Range(1, 3), transform);
                        break;
                    case UnitType.Tank:
                        Debug.Log("Tank Shot Human, Human got obliterated");
                        PlayExplosionImpact();
                        SoundManager.Instance.PlayAudioClip("Explosion", transform);
                        RemoveHealth(Health, Shooter);
                        break;
                    case UnitType.Fennek:
                        Debug.Log("Fennek Shot human, Human Died");
                        PlayBulletImpact();
                        RemoveHealth(Health, Shooter);
                        SoundManager.Instance.PlayAudioClip("DeadSound_" + Random.Range(1, 2), transform);
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
                        RemoveHealth(ShooterDamage, Shooter);
                        break;
                    case UnitType.Fennek:
                        Debug.Log("Fennek shot Tank, Tank took 1/5 of the actual damage");
                        RemoveHealth(ShooterDamage / 5, Shooter);
                        break;
                    default:
                        break;
                }
                break;
            case UnitType.Fennek:
                switch (ShooterType)
                {
                    case UnitType.Human:
                        Debug.Log("Human Shot Fennek, 1/10 of damage done to fennek");
                        RemoveHealth(ShooterDamage / 10, Shooter);
                        PlayBulletImpact();
                        break;
                    case UnitType.Tank:
                        Debug.Log("Tank Shot Fennek, Fennek blows up");
                        RemoveHealth(Health, Shooter);
                        PlayExplosionImpact();
                        SoundManager.Instance.PlayAudioClip("Explosion", transform);
                        break;
                    case UnitType.Fennek:
                        Debug.Log("Fennek shot Fennek, Fennek took full damage");
                        RemoveHealth(ShooterDamage, Shooter);
                        PlayBulletImpact();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
    //
    //
    #region particlePlay Voids
    
    //Void that gets called when a bullet makes impact with this unit
    private void PlayBulletImpact()
    {
        int ParticleToPlay = Random.Range(0, HitParticles.Count);
        for (int i = 0; i < HitParticles.Count; i++)
        {
            if (i == ParticleToPlay)
            {
                HitParticles[i].Play();
                SoundManager.Instance.PlayAudioClip(HitParticles[i].name + "_" + Random.Range(1, 4), transform);
            }  
        }
    }
    //Void that gets called when an explosive makes impact with this unit
    public void PlayExplosionImpact()
    {
        int ParticleToPlay = Random.Range(0, ExplosionParticles.Count);
        for (int i = 0; i < ExplosionParticles.Count; i++)
        {
            if (i == ParticleToPlay)
            {
                ExplosionParticles[i].Play();
                SoundManager.Instance.PlayAudioClip("Explosion", transform);
            }
        }
    }
    //
    
    #endregion
    //Actual removehealth this gets called inside the takedamage void
    private void RemoveHealth(int RemoveAmount, AIBasics Shooter)
    { 
        Health = Health - RemoveAmount;
        if(Health <= 0)
        {
            ResetParameters(animator);
            animator.SetTrigger("Dead");
            aistate = AIStates.Dead;
            Shooter.ResetTarget();
            if(Waypoints.Count > 0)
            {
                Waypoints.Clear();
            }
            Debug.Log("Died");
            //
            Health = 0;
        }


    }
    //Shoot void this is a General shoot void for every unit Parameters decide what ammo and damage is linked to the shoot
    protected void Shoot(AIBasics Target, int Damage, int HitChancePercentage, ParticleSystem MuzzleFlash)
    {
        MuzzleFlash.Play();
        SoundManager.Instance.PlayAudioClip(unittype.ToString() + "_Shot", transform);
        //
        int Hitchance = Random.Range(0, 100);
        if (Hitchance <= HitChancePercentage)
        {
            Target.TakeDamage(unittype, this, Damage);
        }
        else
        {
            Debug.Log("Shot Fired, but target missed");
        }

    }
    //Overall void for unit shooting (has loads of parameters because alot of different units use this void)
    protected void Shoot(AIBasics Target, int Damage, int HitChancePercentage, ParticleSystem MuzzleFlash, WeaponType weapontype, RPGBulletScript RPGBullet)
    {
        MuzzleFlash.Play();
        if (unittype == UnitType.Human) { SoundManager.Instance.PlayAudioClip(weapontype.ToString() + "_Shot", transform); }
        else SoundManager.Instance.PlayAudioClip(unittype.ToString() + "_Shot", transform);

        int Hitchance = Random.Range(0, 100);
        switch (weapontype)
        {
            case WeaponType.AssaultRifle:
                if (Hitchance <= HitChancePercentage)
                {
                    Target.TakeDamage(unittype, this, Damage);
                }
                else
                {
                    Debug.Log("Shot Fired, but target missed");
                }
                break;
            case WeaponType.RPG:
                RPGBullet.CanFly = true;
                RPGBullet.Target = Target;
                break;
            case WeaponType.Sniper:
                if (Hitchance <= HitChancePercentage)
                {
                    Target.TakeDamage(unittype, this, Damage);
                }
                else
                {
                    Debug.Log("Shot Fired, but target missed");
                }
                break;
            default:
                break;
        }
        //
        //

    }
    //RPG shooting void
    public void RPGDoDamage()
    {
        FoundTargetScript.TakeDamage(unittype, this, Damage);
    }
    //This void decides how to react on finding an enemy this is based on the Behaviour enum
    private void EnemySpotted()
    {
        switch (Behaviour)
        {
            case AIBehaviour.Passive:
                Passive();
                break;
            case AIBehaviour.React:
                React();
                break;
            case AIBehaviour.Attack:
                Attack();
                break;
            case AIBehaviour.Destroy:
                Destroy();
                break;
            default:
                break;
        }
    }
    #region BehaviourVoids
    //The passive reaction void
    private void Passive()
    {
        Debug.Log("Enemy Spotted but on passive so no reaction");
    }
    //The react void, This makes it so when spotting something you dont take action but when shot at u shoot back
    private void React()
    {

    }
    //The Attack void, this makes it so when spotting an enemy the AI instantly starts aiming and shooting at it
    private void Attack()
    {
        if(Waypoints.Count > 1)
        {
            aistate = AIStates.TargetAndMove;
        }
        else
        {
            aistate = AIStates.TargetEnemy;
        }
        ResetParameters(animator);
    }
    private void Destroy()
    {
        aistate = AIStates.TargetEnemy;
        ResetParameters(animator);
    }
    #endregion
    #endregion
    //Self made voids to make my life helpful
    #region HelpFullVoids
    //A lookat to only rotate the Y axis
    public void OnlyYLook(Vector3 Target)
    {
        Vector3 lookPos = Target - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
    }
    //
    public void OnlyYLook(GameObject ObjectToRotate,Vector3 Target)
    {
        Vector3 lookPos = Target - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        ObjectToRotate.transform.rotation = rotation;
    }

    //A void that resets the entire animator so that everything is false and you are in idle
    private void ResetParameters(Animator animator)
    {
        AnimatorControllerParameter[] parameters = animator.parameters;

        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter.name, parameter.defaultInt);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter.name, parameter.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter.name, parameter.defaultBool);
                    break;
            }
        }

        animator.SetTrigger("Reset");
    }
    //A void to turn off the visuals for playmode
    public void TurnOffVisuals()
    {
        //Waypoint visuals
        foreach (var waypoint in waypointscripts)
        {
            waypoint.gameObject.SetActive(false);
        }
        linerenderer.enabled = false;
        SelectedArrow.SetActive(false);
        Selected = false;
    }
    //Void to turn on the visuals again
    public void TurnOnVisuals()
    {
        //Waypoint visuals
        foreach (var waypoint in waypointscripts)
        {
            waypoint.gameObject.SetActive(true);
        }
        linerenderer.enabled = true;
        SelectedArrow.SetActive(true);
        Selected = true;
    }
    #endregion
}

