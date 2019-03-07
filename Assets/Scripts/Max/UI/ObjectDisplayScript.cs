using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDisplayScript : MonoBehaviour
{

    public MainObjectScript UIObject;

    public Text nameText;
    public Text descriptionText;

    public Image artworkImage;

    void Update()
    {
        nameText.text = UIObject.name;
        descriptionText.text = UIObject.description;

        artworkImage.sprite = UIObject.artwork;
    }


}
