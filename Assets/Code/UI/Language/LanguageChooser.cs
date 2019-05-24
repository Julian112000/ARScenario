namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class LanguageChooser : MonoBehaviour
    {
        /// <summary>
        /// The camera thats used for the UI.
        /// </summary>
        [Header("Cameras")]
        [SerializeField]
        private Camera mainCamera;

        /// <summary>
        /// The animator for the change animation.
        /// </summary>
        [Header("Animator")]
        public Animator animator;

        /// <summary>
        /// The script that changes the language.
        /// </summary>
        [Header("Language")]
        public TextTranslate translate;

        /// <summary>
        /// Sets the language to english once the app is opened.
        /// </summary>
        private void Start()
        {
            AppStartBools.englishLanguage = true;
            AppStartBools.dutchLanguage = false;
        }

        void Update()
        {
            ///<summary>
            ///Sets the animators bool to the current language.
            /// </summary>
            animator.SetBool("ToEnglishBool", AppStartBools.englishLanguage);
            animator.SetBool("ToDutchBool", AppStartBools.dutchLanguage);

            ///<summary>
            /// Change the language on the phone with a raycast.
            /// </summary>
            #region FingerTouch
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "DutchLang")        
                    {
                        AppStartBools.dutchLanguage = true;
                        AppStartBools.englishLanguage = false;
                    }
                    else if (hit.collider.tag == "EnglishLang")
                    {
                        AppStartBools.dutchLanguage = false;
                        AppStartBools.englishLanguage = true;
                    }
                }
            }
            #endregion

            ///<summary>
            /// Change the language on the PC with a raycast.
            /// </summary>
            #region MouseClick
            else if (Input.GetMouseButtonUp(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "DutchLang")
                    {
                        AppStartBools.dutchLanguage = true;
                        AppStartBools.englishLanguage = false;
                    }
                    else if (hit.collider.tag == "EnglishLang")
                    {
                        AppStartBools.dutchLanguage = false;
                        AppStartBools.englishLanguage = true;
                    }
                }
            }
            #endregion
        }
    }
}
