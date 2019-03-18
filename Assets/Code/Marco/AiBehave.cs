using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBehave : MonoBehaviour
{

    public delegate void Respond(Vector3 MyPos, Vector3 EnemyPos);
    public static event Respond React;

    public static List<GameObject> BadGuys = new List<GameObject>();

    private void Start()
    {
        React += LookForEnemy;
    }


    public static void LookForEnemy(Vector3 MyPos, Vector3 EnemyPos)
    {
        
    }

}
