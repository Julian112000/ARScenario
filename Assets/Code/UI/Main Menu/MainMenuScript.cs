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
        // All the animations that can play in the main menu.
        //</summery>
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Animator startMenu;

        //<summary>
        // The bool that indicates if you are loading or starting a new scenario.
        //</summery>
        public AppStartBools loadScene;

        private bool touchToBegin;

        //<summary>
        // All the interactions witht the UI in the mainMenu.
        //</summery>
        #region MainMenuUI

        private void Start()
        {
            touchToBegin = true;    
        }

        void Update()
        {
            //<summary>
            // The interactions for the finger touch.
            //</summery>
            #region Finger touch
            if (Input.touchCount > 0)
            {
                if (touchToBegin)
                {
                    startMenu.SetTrigger("StartMenu");
                    touchToBegin = false;
                }
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
                            break;

                        //<summary>
                        // Once the players finger leaves the screen it will activate the buttons.
                        //</summery>
                        case TouchPhase.Ended:
                            if (!touchToBegin)
                            {
                                if (hit.collider.tag == "NewScenario") //Start a new scenario.
                                {
                                    startMenu.SetTrigger("LoadNew");
                                    AppStartBools.willLoad = false;     //Set the bool to FALSE wich will indicate that the loading screen will NOT be loaded.
                                }
                                else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario.
                                {
                                    startMenu.SetTrigger("LoadNew");
                                    AppStartBools.willLoad = true;      //Set the bool to TRUE wich will indicate that the loading screen WILL be loaded.
                                }
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
                if (touchToBegin)
                {
                    startMenu.SetTrigger("StartMenu");
                    touchToBegin = false;
                }
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (!touchToBegin)
                    {
                        if (hit.collider.tag == "NewScenario") //Start a new scenario
                        {
                            startMenu.SetTrigger("LoadNew");
                            AppStartBools.willLoad = false;
                        }
                        else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario
                        {
                            startMenu.SetTrigger("LoadNew");
                            AppStartBools.willLoad = true;
                        }
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
