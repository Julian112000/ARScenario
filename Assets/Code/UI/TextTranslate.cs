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


        [SerializeField]
        private List<Text> m_TextRealtime = new List<Text>();

        [SerializeField]
        private List<string> m_TextDutch = new List<string>();

        [SerializeField]
        private List<string> m_TextEnglish = new List<string>();

        public void Update()
        {
            if (AppStartBools.dutchLanguage)
            {
                for (int i = 0; i < m_TextRealtime.Count; i++)
                {
                    m_TextRealtime[i].text = m_TextDutch[i];
                }
            }
            else if (AppStartBools.englishLanguage)
            {
                for (int i = 0; i < m_TextRealtime.Count; i++)
                {
                    m_TextRealtime[i].text = m_TextEnglish[i];
                }
            }
        }
    }
}
