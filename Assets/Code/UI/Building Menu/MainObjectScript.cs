using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectCard", menuName = "UISpawning")]
public class MainObjectScript : ScriptableObject
{
    public new string name;                     // The name of the spawnable object (in english).
    public string name_Dutch;                   // The name of the spawnable object (in dutch).

    public string description;                  // The description of the spawnable object (in english).
    public string description_Dutch;            // The description of the spawnable object (in dutch).

    public Sprite artwork;                      // The icon of the spawnable object that will be displayed.

    public GameObject ConnectedModel;           // The 3D Model that will be spawned.

    public RenderTexture ConnectedShowCase;     // The 3D Showcase model (Render texture) that will be previewed in the placing canvas.

}
