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

        public MainObjectScript UIObject;

        public Text nameText;
        public Text descriptionText;

        public Image artworkImage;

        public Renderer showCase;

        public TextTranslate translate;

    void Update()
        {

            artworkImage.sprite = UIObject.artwork;

            showCase.material.mainTexture = UIObject.ConnectedShowCase;

            if (AppStartBools.dutchLanguage)
            {
                descriptionText.text = UIObject.description_Dutch;
                nameText.text = UIObject.name_Dutch;
            }
            else if (AppStartBools.englishLanguage)
            {
                descriptionText.text = UIObject.description;
                nameText.text = UIObject.name;
            }

        }


    }
}
