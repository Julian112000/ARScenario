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

    public class MainMenuScript : MonoBehaviour
    {

        public Camera mainCamera;


        [Header("Buttons")]
        [SerializeField]
        private GameObject newOverlay;
        [SerializeField]
        private GameObject loadOverlay;
        [SerializeField]
        private GameObject settingsOverlay;

        [Header("Screens")]
        [SerializeField]
        private GameObject settings;
        [SerializeField]
        private GameObject mainMenu;

        [Header("Animator")]
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Animator settingsAni;
        [SerializeField]
        private Animator mainAni;

        public AppStartBools loadScene;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                        switch (touch.phase)
                        {
                            case TouchPhase.Began:
                            if (hit.collider.tag == "NewScenario") 
                            {
                                newOverlay.SetActive(false);
                            }
                            else if (hit.collider.tag == "LoadScenario")
                            {
                                loadOverlay.SetActive(false);
                            }
                            else if (hit.collider.tag == "SettingsMain")
                            {
                                settingsOverlay.SetActive(false);
                            }

                            break;

                            case TouchPhase.Ended:
                            if (hit.collider.tag == "NewScenario") //Start a new scenario
                            {
                                animator.SetTrigger("FadeOutMain");
                                AppStartBools.willLoad = false;
                                newOverlay.SetActive(true);
                            }
                            else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario
                            {
                                animator.SetTrigger("FadeOutMain");
                                AppStartBools.willLoad = true;
                                loadOverlay.SetActive(true);
                            }
                            else if (hit.collider.gameObject.tag == "SettingsMain") //Open Settings
                            {
                                settings.SetActive(true);
                                mainAni.SetTrigger("CloseMainMenu");
                            }
                            else if (hit.collider.gameObject.tag == "CloseSettings") //CloseSettings
                            {
                                mainMenu.SetActive(true);
                                settingsAni.SetTrigger("CloseSettings");
                            }
                            else if(hit.collider.gameObject.tag != "NewScenario" && hit.collider.gameObject.tag != "LoadScenario" && hit.collider.gameObject.tag != "SettingsMain") //Set buttons back to normal
                            {
                                newOverlay.SetActive(true);
                                loadOverlay.SetActive(true);
                                settingsOverlay.SetActive(true);
                            }
                            break;
                        }
                
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "NewScenario") //Start a new scenario
                            {
                                animator.SetTrigger("FadeOutMain");
                                AppStartBools.willLoad = false;
                                newOverlay.SetActive(true);
                            }
                            else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario
                            {
                                animator.SetTrigger("FadeOutMain");
                                AppStartBools.willLoad = true;
                                loadOverlay.SetActive(true);
                            }
                            else if (hit.collider.gameObject.tag == "SettingsMain") //Open Settings
                            {
                                settings.SetActive(true);
                                mainAni.SetTrigger("CloseMainMenu");
                            }
                            else if (hit.collider.gameObject.tag == "CloseSettings") //CloseSettings
                            {
                                mainMenu.SetActive(true);
                                settingsAni.SetTrigger("CloseSettings");
                            }
                            else if (hit.collider.gameObject.tag != "NewScenario" && hit.collider.gameObject.tag != "LoadScenario" && hit.collider.gameObject.tag != "SettingsMain") //Set buttons back to normal
                            {
                                newOverlay.SetActive(true);
                                loadOverlay.SetActive(true);
                                settingsOverlay.SetActive(true);
                            }
                }
            }
        }
    }
}
