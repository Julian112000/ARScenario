namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif
    public class ObjectDisplayScript : MonoBehaviour
    {
        /// <summary>
        /// All the other scripts that are connected to this script.
        /// </summary>
        [Header("Scripts")]
        public MainObjectScript UIObject;
        public TextTranslate translate;

        /// <summary>
        /// The name and the description text that will be changed by the scriptable object.
        /// </summary>
        [Header("Spawnable Object Text")]
        public Text nameText;
        public Text descriptionText;

        /// <summary>
        /// The artwork image that will be changed by the scriptable object.
        /// </summary>
        [Header("Artwork image")]
        public Image artworkImage;

        /// <summary>
        /// The showcase rendered that will be changed by the scriptable object.
        /// </summary>
        [Header("Showcase Renderer")]
        public Renderer showCase;

    void Update()
        {
            artworkImage.sprite = UIObject.artwork;                         //Change the artwork of the UI to the connected scriptable object's artwork.

            showCase.material.mainTexture = UIObject.ConnectedShowCase;     //Change the texture of the showcase to the render texture of the connected scriptable object's.

            if (AppStartBools.dutchLanguage)                                //If the current language is dutch, change the name & the description to the dutch versions of the connected scriptable object.
            {
                descriptionText.text = UIObject.description_Dutch;          
                nameText.text = UIObject.name_Dutch;
            }
            else if (AppStartBools.englishLanguage)                          //If the current language is english, change the name & the description to the english versions of the connected scriptable object.
            {
                descriptionText.text = UIObject.description;
                nameText.text = UIObject.name;
            }

        }


    }
}
