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
        [SerializeField]
        private Camera mainCamera;

        public Animator animator;

        public TextTranslate translate;

        private void Start()
        {
            AppStartBools.englishLanguage = true;
            AppStartBools.dutchLanguage = false;
        }

        void Update()
        {
            animator.SetBool("ToEnglishBool", AppStartBools.englishLanguage);
            animator.SetBool("ToDutchBool", AppStartBools.dutchLanguage);

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
        }
    }
}
