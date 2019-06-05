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

    public class TextTranslate : MonoBehaviour
    {

        ///<summary>
        ///The text labels that will be changeable by language.
        ///</summary>
        [SerializeField]
        private List<Text> textRealtime = new List<Text>();

        ///<summary>
        ///The dutch text list of all the changeable labels.
        ///</summary>
        [SerializeField]
        private List<string> textDutch = new List<string>();

        ///<summary>
        ///The english text list of all the changeable labels.
        ///</summary>
        [SerializeField]
        private List<string> textEnglish = new List<string>();


        ///<summary>
        ///The language changer for all labels.
        ///</summary>
        #region LanguageDesider 
        public void Update()
        {
            if (AppStartBools.dutchLanguage)                    //If the dutchlanguage variable (wich is selected in the settingsscreen from the main menu) is true.
            {
                for (int i = 0; i < textRealtime.Count; i++)  //For every label that has been selected.
                {
                    textRealtime[i].text = textDutch[i];    //Replace it by the dutch version in the right order.
                }
            }
            else if (AppStartBools.englishLanguage)             //If the englishlanguage variable (wich is selected in the settingsscreen from the main menu) is true.
            {
                for (int i = 0; i < textRealtime.Count; i++)  //For every label that has been selected.
                {
                    textRealtime[i].text = textEnglish[i];  //Replace it by the english version in the right order.
                }
            }
        }
        #endregion

    }
}
