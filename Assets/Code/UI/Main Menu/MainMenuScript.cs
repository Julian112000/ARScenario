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

        //<summary>
        //All the button overlays for visual effect.
        //</summery>
        [Header("Buttons")]
        [SerializeField]
        private GameObject newOverlay;
        [SerializeField]
        private GameObject loadOverlay;
        [SerializeField]
        private GameObject settingsOverlay;

        //<summary>
        // The canvas(2) that are useable in the main screen.
        //</summery>
        [Header("Screens")]
        [SerializeField]
        private GameObject settings;
        [SerializeField]
        private GameObject mainMenu;

        //<summary>
        // All the animations that can play in the main menu.
        //</summery>
        [Header("Animator")]
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Animator settingsAni;
        [SerializeField]
        private Animator mainAni;

        //<summary>
        // The bool that indicates if you are loading or starting a new scenario.
        //</summery>
        public AppStartBools loadScene;

        //<summary>
        // All the interactions witht the UI in the mainMenu.
        //</summery>
        #region MainMenuUI
        void Update()
        {
            //<summary>
            // The interactions for the finger touch.
            //</summery>
            #region Finger touch
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    switch (touch.phase)
                    {
                        //<summary>
                        // Once the players finger touch begins it will deactivate the sprite so it will give a visual feedback that you're clicking the button.
                        //</summery>
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

                        //<summary>
                        // Once the players finger leaves the screen it will activate the buttons.
                        //</summery>
                        case TouchPhase.Ended:
                            if (hit.collider.tag == "NewScenario") //Start a new scenario.
                            {
                                animator.SetTrigger("FadeOutMain"); //Activate the animation that will switch scenes at the end.
                                AppStartBools.willLoad = false;     //Set the bool to FALSE wich will indicate that the loading screen will NOT be loaded.
                                newOverlay.SetActive(true);         //Set the overlay of the button back to true for the visual feedback.
                            }
                            else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario.
                            {
                                animator.SetTrigger("FadeOutMain"); //Activate the animation that will switch scenes at the end.
                                AppStartBools.willLoad = true;      //Set the bool to TRUE wich will indicate that the loading screen WILL be loaded.
                                loadOverlay.SetActive(true);        //Set the overlay of the button back to true for the visual feedback.
                            }
                            else if (hit.collider.gameObject.tag == "SettingsMain") //Open Settings.
                            {
                                settings.SetActive(true);           //Sets the canvas of the settings menu to true (Animation will start automaticly).
                                mainAni.SetTrigger("CloseMainMenu");//Activate the animation that will close the main menu canvas.
                            }
                            else if (hit.collider.gameObject.tag == "CloseSettings") //CloseSettings.
                            {
                                mainMenu.SetActive(true);               //Sets the canvas of the main menu to true (Animation will start automaticly).
                                settingsAni.SetTrigger("CloseSettings");//Activate the animation that will close the settings menu canvas.
                            }
                            else if (hit.collider.gameObject.tag != "NewScenario" && hit.collider.gameObject.tag != "LoadScenario" && hit.collider.gameObject.tag != "SettingsMain") //Set buttons back to normal
                            {
                                newOverlay.SetActive(true);         //
                                loadOverlay.SetActive(true);        //Sets all the overlays back to true for visual effect.
                                settingsOverlay.SetActive(true);    //
                            }
                            break;
                    }

                }
            }
            #endregion

            //<summary>
            // The interactions for the finger touch (Same as finger touch but just for the mouse).
            //</summery>
            #region Mouse touch
            //<summary>
            // Its the same that goes for the finger touch, but for mouse.
            //</summery>
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
        #endregion

        #endregion
    }
}
