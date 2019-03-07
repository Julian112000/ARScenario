using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectCard", menuName = "UISpawning")]
public class MainObjectScript : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

    public GameObject ConnectedModel;

}
