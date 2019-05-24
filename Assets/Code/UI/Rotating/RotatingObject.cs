using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    /// <summary>
    /// A simple int for the amount of speed the object is rotating in.
    /// </summary>
    [SerializeField]
    private int rotateSpeed;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Rotate the objects y-axis by the amount of speed given my the int (rotateSpeed).
    /// </summary>
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
