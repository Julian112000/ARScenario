using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectCard", menuName = "UISpawning")]
public class MainObjectScript : ScriptableObject
{
    public new string name;
    public string name_Dutch;

    public string description;
    public string description_Dutch;

    public Sprite artwork;

    public GameObject ConnectedModel;

    public RenderTexture ConnectedShowCase;

}
