namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    using Input = InstantPreviewInput;
#endif

    public class ChangeObjectScript : MonoBehaviour
    {
        /// <summary>
        /// Al the scripts that are linked to this script.
        /// </summary>
        [Header("Scripts")]
        public static ChangeObjectScript Instance;  
        public ObjectChangerScript change;       
        public StatsUIScript BehaveController;   
        private Human currentHuman;   
        private AIBasics currentAI;

        /// <summary>
        /// The camera that is used for the UI.
        /// </summary>
        [Header("Camera")]
        public Camera mainCamera;

        /// <summary>
        /// Every canvas that is used in the app.
        /// </summary>
        [Header("UIModes")]
        public GameObject mainModeUI;           // The main canvas where you can deside what to do.
        public GameObject scanningModeUI;       // The canvas where you SCAN the area.
        public GameObject buildModeUI;          // The canvas where you can CHOOSE your spawnable object.
        public GameObject placeModeUI;          // The canvas where you can PLACE the spawnable object.
        public GameObject rotatingModeUI;       // The canvas where you can ROTATE your spawned object.
        public GameObject selectModeUI;         // The canvas where you can SELECT a spawned object that you'd like to edit.
        public GameObject behaveModeUI;         // The canvas where you can deside what to do with the current selected object.
        public GameObject behaveStatsUI;        // The canvas where you can edit the STATISTICS of the current selected object.
        public GameObject behaveEquipmentUI;    // The canvas where you can edit the EQUIPMENT of the current selected object.
        public GameObject behaveReactUI;        // The canvas where you can edit the REACTION of the current selected object.
        public GameObject waypointPlacementUI;  // The canvas where you can edit the WAYPOINTS and set a route for the current selected object.
        public GameObject routeButtonUI;        // ??
        public GameObject RemovalUI;            // The canvas where you can REMOVE the current selected object.
        public GameObject loadingModeUI;        // The canvas where you can LOAD a older scenario that you have saved.

        /// <summary>
        /// All the objects that hold the script 'LevelChanger' for canvas change animations.
        /// </summary>
        [Header("Animations")]
        public LevelChanger changeBuildANI;
        public LevelChanger changeMainANI, changePlayANI, changePlaceANI, changeRotateANI, changeScanANI, changeSelectANI, changeBehaveANI, changeStatsANI, changeReactANI, changeEquipmentANI, changeWaypointANI, changeRemoveANI;

        /// <summary>
        /// All the booleans that are used.
        /// </summary>
        [Header("Bools")]
        private bool CanClick = true; // The bool that desides if you are able to click yet (to prevent spamming).
        private bool ReScaling = false; // The bool that desides of you are going to rescale the object that you have selected.

        
        /// <summary>
        /// Sets the var 'Instance' (Wich should hold: ChangeObjectScript) to the object holding this script.
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// The start function that desides if you will open 'load canvas' or 'main canvas'.
        /// </summary>
        #region Main or Load
        void Start()
        {
            if (AppStartBools.willLoad == true) // If you chose 'load new scenario' in the startscreen.
            {
                loadingModeUI.SetActive(true);  // Activate the loading canvas.     (LOAD SCENARIO)
            }
            else                                // If not.
            {
                mainModeUI.SetActive(true);     // Activate the main canvas.        (SAVE SCENARIO)
            }

        }
        #endregion

        /// <summary>
        /// The complete UI controlls for finger and mouse.
        /// </summary>
        #region FingerTouch & Mouseclick
        void Update()
        {
            /// FingerTouch raycast.
            #region FingerTouch
            /// <summary>
            /// Raycast system for the finger touch.
            /// </summary>
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CanClick)   //If you are able to click.
                    {
                        switch (touch.phase)
                        {
                            /// <summary> 
                            /// Once the finger touch begins (when it touches the screen).
                            /// </summary>
                            case TouchPhase.Began:
                                break;

                            ///<summary>
                            /// Once the finger touch ends (when it leaves the screen).
                            /// </summary>
                            case TouchPhase.Ended:
                                SelfMadeButton button;                                              //Create a variable for the script 'SelfMadeButton'.
                                button = hit.collider.gameObject.GetComponent<SelfMadeButton>();    //Sets the variable 'button' to the UI button that you just clicked on.

                                if (button.myEvent != null)     // If there is a current event.
                                {
                                    button.ButtonClicked();     // Activate the function 'ButtonClicked' who will deside wich function to call.
                                }
                                break;
                        }
                    }
                }
            }
            #endregion

            /// MouseClick raycast.
            #region MouseClick
            /// <summary>
            /// Raycast system for the finger touch.
            /// </summary>
            else if (Input.GetMouseButtonUp(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CanClick)   //If you are able to click.
                    {   
                        SelfMadeButton button;                                              //Create a variable for the script 'SelfMadeButton'.
                        button = hit.collider.gameObject.GetComponent<SelfMadeButton>();    //Sets the variable 'button' to the UI button that you just clicked on.

                        if (button.myEvent != null)     // If there is a current event.
                        {
                            button.ButtonClicked();     // Activate the function 'ButtonClicked' who will deside wich function to call.
                        }
                    }
                }
            }
            #endregion

        }
        #endregion



        /// <summary>
        /// All the UI functions for the buttons.
        /// </summary>
        #region All Button voids

        ///<summary>
        /// All functions for the main & scanning buttons.
        /// </summary>
        #region MainMenu/Scanning
            ///<summary>
            /// Opens the scanning canvas and activates the ability to scan.
            /// </summary>
        public void StartScanning()
        {
            scanningModeUI.SetActive(true);                             //Opens the scanning canvas.
            changeMainANI.CloseMainANI();                               //Starts the animation to close the main menu canvas.
            changePlayANI.ClosePlayANI();                               //Starts the animation to close the play button.
            ARController.controllerstate = ControllerState.Scanning;    //Sets the ability to scan the area to true.
        }

        /// <summary>
        /// Closes the scanning canvas and de-activates the ability to scan.
        /// </summary>
        public void StopScanning()
        {
            changeScanANI.CloseScanningANI();                           //Starts the animation to close the scanning canvas.
            mainModeUI.SetActive(true);                                 //Opens the main menu canvas.
            ARController.controllerstate = ControllerState.Default;     //Sets the ability to scan the area to false.
        }
        #endregion

        /// <summary>
        /// All the functions for the building, placing, rotating & choosing buttons.
        /// </summary>
        #region Building/Placing/Rotating/Choosing

        ///<summary>
        /// Opens the build Menu.
        /// </summary>
        public void OpenBuildMenu()
        {
            buildModeUI.SetActive(true);

            //
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
        }

        /// <summary>
        /// Controlles the arrows in the build selection menu. Once you press the left arrow, you go one object to the left. Same goes for the right.
        /// </summary>
        /// <param name="direction"> 1 = RIGHTARROW / -1 = LEFTARROW </param>
        public void MoveArrow(int direction)
        {
            if (change.currentObject <= 6 && change.currentObject >= 2) //If its between the borders of the minimum & maximum amount of spawnable objects.
            {
                change.currentObject += direction;                      //Go to the right or left depending on wich arrow you clicked. 
                change.ChangeCurrentObject(change.currentObject);
            }
            if (change.currentObject == 7 && direction == -1)           //If the current object is at the MAXIMUM and you press the left arrow.
            {
                change.currentObject += direction;                      //Go to the left cause the direction = -1.
                change.ChangeCurrentObject(change.currentObject);
            }
            if (change.currentObject == 1 && direction == 1)            //If the current object is at the MINIMUM and you press the right arrow.
            {
                change.currentObject += direction;                      //Go to the right cause the direction = 1.
                change.ChangeCurrentObject(change.currentObject);
            }
        }

        /// <summary>
        /// Closes the placing canvas and sets it back to mainmenu.
        /// </summary>
        public void ConfirmPlacement()
        {
            changePlaceANI.ClosePlaceAni();

            //
            buildModeUI.SetActive(true);

            //
            ARController.controllerstate = ControllerState.Default;
        }
        #endregion

        /// <summary>
        /// All the functions for the scaling & rescaling buttons.
        /// </summary>
        #region Scaling/Rescaling

        ///<summary>
        /// Close the scaling mode when the confirm button is pressed.
        /// </summary>
        public void ConfirmEditing()
        {
            changeRotateANI.CloseRotateANI();

            //
            if (ReScaling)                                                          //If you are rescaling instead of first time scaling, go back to the behaviour screen of the selected object.
            {
                ARController.controllerstate = ControllerState.FullyEditingObject;
                ReScaling = false;
                behaveModeUI.SetActive(true);
            }
            else if (!ReScaling)                                                    //If you are not rescaling but first time scaling, go back to the placing screen of the spawnable object.
            {
                ARController.controllerstate = ControllerState.Placing;
                placeModeUI.SetActive(true);
            }


        }

        /// <summary>
        /// Trigger the scaling mode when the scale button is pressed.
        /// </summary>
        public void TriggerScaling()
        {
            ARController.controllerstate = ControllerState.Editing;
            changeBehaveANI.CloseBehaveANI();
            rotatingModeUI.SetActive(true);
            ReScaling = true;
        }
        #endregion

        /// <summary>
        /// All the functions for the home, back, done & play buttons.
        /// </summary>
        #region Home/Back/Done/Play
        ///<summary>
        ///Confirms the object you want to spawn and changes the canvas to placing.
        /// </summary>
        public void DoneButton()
        {
            changeBuildANI.CloseBuildANI();

            placeModeUI.SetActive(true);

            ARController.ChangeModel(change.AllObjects[change.currentObject].ConnectedModel); //Changes the model that you will place to the model that is connected to the spawnable object you just chose.
            ARController.controllerstate = ControllerState.Placing;
        }

        /// <summary>
        /// The play button that removes every mode and starts the scenario.
        /// </summary>
        public void StartPlayMode()
        {
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
            buildModeUI.SetActive(false);
            placeModeUI.SetActive(false);
            rotatingModeUI.SetActive(false);
            SceneHandler.EnablePlayMode();
            //DetectedPlaneGenerator.Instance.ToggleVisualizers(false);
        }

        /// <summary>
        /// The home button that will deactivate every canvas and open the main canvas.
        /// </summary>
        public void HomeButton()
        {
            if (ARController.controllerstate == ControllerState.FullyEditingObject)             //If you are in the behaviour mode of a selected object.
            {
                ARController.CurrentSelectedModel.GetComponent<AIBasics>().TurnOffVisuals();    //Turn off all extra visuals of the object
            }
            changeBuildANI.CloseBuildANI();             //
            changePlaceANI.ClosePlaceAni();             //
            changeSelectANI.CloseSelectANI();           //
            changeBehaveANI.CloseBehaveANI();           //  Close every canvas there is by a animation(ony closes it if the canvas is active at the time)
            changeScanANI.CloseScanningANI();           //
            changeWaypointANI.CloseWaypointsANI();      //

            mainModeUI.SetActive(true);

            ARController.controllerstate = ControllerState.Default;                   
        }
        #endregion

        /// <summary>
        /// All the functions for the behave & select buttons.
        /// </summary>
        #region Behave/Select
        ///<summary>
        /// Triggers the selection mode in wich you can select a object to edit.
        /// </summary>
        public void BehaveSelection()
        {
            selectModeUI.SetActive(true);

            //
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
            //

            //
            ARController.controllerstate = ControllerState.SelectingObject;     //(Click on a object to select it)
        }
        #endregion

        /// <summary>
        /// All the functions for the console button.
        /// </summary>
        #region Console
        ///<summary>
        /// Triggers the console to open.
        /// </summary>
        public void OpenConsole()
        {
            ConsoleScript.Instance.ToggleConsole();
        }
        #endregion

        /// <summary>
        /// All the functions for the stats, equipment, reaction & remove button.
        /// </summary>
        #region Stats/Equipment/Reaction/Remove
        ///<summary>
        /// Triggers the stats mode to open.
        /// </summary>
        public void EditingObjectStats()
        {
            behaveStatsUI.SetActive(true);
            ARController.controllerstate = ControllerState.EditingStats;
            if (ARController.CurrentSelectedModel != null)                                                      //If there is a model selected...
            {
                BehaveController.CurrentAIStats = ARController.CurrentSelectedModel.GetComponent<AIStats>();    //Connect the AI stats to the UI stats.
                BehaveController.StartCurrentAIStats();
            }
        }

        /// <summary>
        /// Closes the stats mode.
        /// </summary>
        public void ConfirmEditingObjectStats()
        {
            BehaveController.UpdateCurrentAiStats();
            changeStatsANI.CloseBehaveStatsANI();
            ARController.controllerstate = ControllerState.FullyEditingObject;  //Set the state back to behaviourmode.
            BehaveController.CurrentAIStats = null;
        }

        /// <summary>
        /// Triggers the equipment mode to open.
        /// </summary>
        public void OpenEquipment()
        {
            behaveEquipmentUI.SetActive(true);
            currentHuman = ARController.CurrentSelectedModel.GetComponent<Human>(); //Sets the current human to the model you have selected.
        }

        /// <summary>
        /// Closes the equipment mode.
        /// </summary>
        public void CloseEquipment()
        {
            changeEquipmentANI.CloseBehaveReactANI();
            ARController.controllerstate = ControllerState.FullyEditingObject;
            currentHuman = null;                                                //Set the current human back to nothing.
        }

        /// <summary>
        /// Triggers the react mode to open.
        /// </summary>
        public void OpenReact()
        {
            behaveReactUI.SetActive(true);
            currentAI = ARController.CurrentSelectedModel.GetComponent<AIBasics>(); //Sets the current AI to the models AI you have selected.
        }

        /// <summary>
        /// Closes the trigger mode.
        /// </summary>
        public void CloseReact()
        {
            changeReactANI.CloseBehaveReactANI();
            ARController.controllerstate = ControllerState.FullyEditingObject;
            currentAI = null;                                                   //Set the current AI back to nothing.
        }

        /// <summary>
        /// Remove Related UI Voids
        /// </summary>
        /// 
        ///UI Void that removes the actual Unit (called when pressed YES)
        public void RemoveUnit()
        {
            Destroy(ARController.CurrentSelectedModel.transform.root.gameObject);   //Destroys the current selected model.
            ARController.controllerstate = ControllerState.Default;
            mainModeUI.SetActive(true);
            changeRemoveANI.CloseBehaveReactANI();
            changeBehaveANI.CloseBehaveANI();

        }

        /// <summary>
        /// UI Void that triggers the UI to press yes or no, If already active it turns to false if not it turns active.
        /// </summary>
        public void TriggerRemoveUI()
        {
            if (RemovalUI.active)
            {
                changeRemoveANI.CloseBehaveReactANI();
            }
            else if (!RemovalUI.active)
            {
                RemovalUI.SetActive(true);
            }
        }

        /// <summary>
        /// Updates the current weapon by the number (Number is connected to every UI element).
        /// </summary>
        public void ChangeWeapon(int WeaponNumber)
        {
            currentHuman.WeaponUpdate(WeaponNumber);
        }

        /// <summary>
        /// Updates the current reaction state by the number (Number is connected to every UI element).
        /// </summary>
        public void SetReactionEnum(int Number)
        {
            currentAI.Behaviour = (AIBehaviour)Number;
        }
        #endregion

        /// <summary>
        /// All the functions for the waypoint buttons.
        /// </summary>
        #region Waypoints 
         ///<summary>
         /// Triggers the waypointing mode to open and activates ability to place & edit waypoints.
         /// </summary>
        public void StartWaypointing()
        {
            changeBehaveANI.CloseBehaveANI();
            waypointPlacementUI.SetActive(true);
            ARController.controllerstate = ControllerState.Waypointing;
        }

        /// <summary>
        /// Removes the last placed waypoint.
        /// </summary>
        public void UndoWaypoint()
        {
            ARController.CurrentplacedObject.GetComponent<AIBasics>().UndoLastWaypoint();
        }

        /// <summary>
        /// Resets all the waypoints. (Deletes every waypoint)
        /// </summary>
        public void ClearWaypoints()
        {
            ARController.CurrentplacedObject.GetComponent<AIBasics>().ClearWaypoints();
        }

        /// <summary>
        /// Confirm the waypoints placed and go back to the behaviour mode.
        /// </summary>
        public void ConfirmWaypoint()
        {
            changeWaypointANI.CloseWaypointsANI();
            behaveModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.FullyEditingObject;
        }

        /// <summary>
        /// ??
        /// </summary>
        public void UpdateRouteUI(GameObject selectedobject)
        {
            BoxCollider collider = routeButtonUI.GetComponent<BoxCollider>();
            SpriteRenderer SpriteRenderer = routeButtonUI.transform.GetChild(1).GetComponent<SpriteRenderer>();

            if (selectedobject.tag != "Snipertoren")
            {
                collider.enabled = true;
                SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 1);
            }
            else
            {
                collider.enabled = false;
                SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.2f);
            }
        }

        #endregion

        /// <summary>
        /// All the functions for the save & load buttons.
        /// </summary>
        #region Save/Load
        ///<summary>
        /// Saves everything you did in the scene.
        /// </summary>
        public void StartSaving()
        {
            SceneManager.Instance.ToggleNewSave();
        }

        /// <summary>
        /// Closes the loading mode and opens the mainmode.
        /// </summary>
        public void StartLoading()
        {
            loadingModeUI.SetActive(false);
            mainModeUI.SetActive(true);
        }

        /// <summary>
        /// Opens the loading menu depending if toggle is true.
        /// </summary>
        public void ToggleLoadingUI(bool toggle)
        {
            loadingModeUI.SetActive(toggle);
            mainModeUI.SetActive(true);
        }
        #endregion

        #endregion

    }
} 