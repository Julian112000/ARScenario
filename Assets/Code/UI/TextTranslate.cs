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
        private List<Text> m_TextRealtime = new List<Text>();

        ///<summary>
        ///The dutch text list of all the changeable labels.
        ///</summary>
        [SerializeField]
        private List<string> m_TextDutch = new List<string>();

        ///<summary>
        ///The english text list of all the changeable labels.
        ///</summary>
        [SerializeField]
        private List<string> m_TextEnglish = new List<string>();


        ///<summary>
        ///The language changer for all labels.
        ///</summary>
        #region LanguageDesider 
        public void Update()
        {
            if (AppStartBools.dutchLanguage)                    //If the dutchlanguage variable (wich is selected in the settingsscreen from the main menu) is true.
            {
                for (int i = 0; i < m_TextRealtime.Count; i++)  //For every label that has been selected.
                {
                    m_TextRealtime[i].text = m_TextDutch[i];    //Replace it by the dutch version in the right order.
                }
            }
            else if (AppStartBools.englishLanguage)             //If the englishlanguage variable (wich is selected in the settingsscreen from the main menu) is true.
            {
                for (int i = 0; i < m_TextRealtime.Count; i++)  //For every label that has been selected.
                {
                    m_TextRealtime[i].text = m_TextEnglish[i];  //Replace it by the english version in the right order.
                }
            }
        }
        #endregion

    }
}
