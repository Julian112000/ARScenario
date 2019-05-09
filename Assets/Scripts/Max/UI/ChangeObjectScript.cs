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
        public static ChangeObjectScript Instance;
        public ObjectChangerScript change;

        public Camera mainCamera;

        public LevelChanger changeBuildANI, changeMainANI, changePlayANI, changePlaceANI, changeRotateANI, changeScanANI, changeSelectANI, changeBehaveANI, changeStatsANI, changeReactANI, changeEquipmentANI, changeWaypointANI, changeRemoveANI;

        [Header("UIModes")]
        public GameObject buildModeUI;

        public GameObject mainModeUI;

        public GameObject placeModeUI;

        public GameObject rotatingModeUI;

        public GameObject scanningModeUI;

        public GameObject behaveModeUI;

        public GameObject selectModeUI;

        public GameObject behaveStatsUI;

        public GameObject behaveEquipmentUI;

        public GameObject behaveReactUI;

        public GameObject waypointPlacementUI;

        public GameObject routeButtonUI;

        public GameObject RemovalUI;

        [SerializeField]
        private GameObject loadingModeUI;

        public StatsUIScript BehaveController;

        private Human currentHuman;
        private AIBasics currentAI;

        private bool CanClick = true;
        private bool ReScaling = false;

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            if (AppStartBools.willLoad == true)
            {
                loadingModeUI.SetActive(true);
            }
            else
            {
                mainModeUI.SetActive(true);
            }

        }

        void Update()
        {
            //Debug.Log(change.currentObject + " Testing");
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CanClick)
                    {
                        switch (touch.phase)
                        {
                            case TouchPhase.Began:
                                break;

                            case TouchPhase.Ended:
                                SelfMadeButton button;
                                button = hit.collider.gameObject.GetComponent<SelfMadeButton>();

                                if (button.myEvent != null)
                                {
                                    button.ButtonClicked();
                                    Debug.Log("Clicked on Selfmade Button");
                                }
                                break;
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CanClick)
                    {
                        SelfMadeButton button;
                        button = hit.collider.gameObject.GetComponent<SelfMadeButton>();

                        if (button.myEvent != null)
                        {
                            button.ButtonClicked();
                            Debug.Log("Clicked on Selfmade Button");
                        }
                    }
                }
            }
        }
        #region All Button voids
        //
        public void MoveArrow(int direction)
        {
            if (change.currentObject <= 6 && change.currentObject >= 2)
            {
                change.currentObject += direction;
                change.ChangeCurrentObject(change.currentObject);
            }
            if (change.currentObject == 7 && direction == -1)
            {
                change.currentObject += direction;
                change.ChangeCurrentObject(change.currentObject);
            }
            if (change.currentObject == 1 && direction == 1)
            {
                change.currentObject += direction;
                change.ChangeCurrentObject(change.currentObject);
            }
        }
        public void DoneButton() //Confirm Build Selection.
        {
            changeBuildANI.CloseBuildANI();
            //

            placeModeUI.SetActive(true);
            //

            ARController.ChangeModel(change.AllObjects[change.currentObject].ConnectedModel);
            ARController.controllerstate = ControllerState.Placing;
            //
        }
        public void ConfirmEditing()
        {
            changeRotateANI.CloseRotateANI();

            //

            //
            if (ReScaling)
            {
                ARController.controllerstate = ControllerState.FullyEditingObject;
                ReScaling = false;
                behaveModeUI.SetActive(true);
            }
            else if (!ReScaling)
            {
                ARController.controllerstate = ControllerState.Placing;
                placeModeUI.SetActive(true);
            }
        }
        public void ConfirmPlacement()
        {
            changePlaceANI.ClosePlaceAni();

            //
            buildModeUI.SetActive(true);

            //
            ARController.controllerstate = ControllerState.Default;
        }
        public void OpenBuildMenu()
        {
            buildModeUI.SetActive(true);

            //
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
        }

        public void BehaveSelection()
        {
            selectModeUI.SetActive(true);

            //
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
            //

            //
            ARController.controllerstate = ControllerState.SelectingObject;
        }
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

        public void HomeButton() //HOME BUTTON.
        {
            if (ARController.controllerstate == ControllerState.FullyEditingObject)
            {
                ARController.CurrentSelectedModel.GetComponent<AIBasics>().TurnOffVisuals();
            }
            changeBuildANI.CloseBuildANI();
            changePlaceANI.ClosePlaceAni();
            changeSelectANI.CloseSelectANI();
            changeBehaveANI.CloseBehaveANI();
            changeScanANI.CloseScanningANI();
            //

            waypointPlacementUI.SetActive(false);

            //
            mainModeUI.SetActive(true);

            //
            ARController.controllerstate = ControllerState.Default;
            //
        }

        public void OpenConsole()
        {
            ConsoleScript.Instance.ToggleConsole();
        }

        public void StartScanning()
        {
            scanningModeUI.SetActive(true);
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
            ARController.controllerstate = ControllerState.Scanning;
        }

        public void UndoWaypoint()
        {
            Debug.Log("Undo");
            ARController.CurrentplacedObject.GetComponent<AIBasics>().UndoLastWaypoint();
        }

        public void ClearWaypoints()
        {
            Debug.Log("Cleared");
            ARController.CurrentplacedObject.GetComponent<AIBasics>().ClearWaypoints();

        }

        public void ConfirmWaypoint()
        {
            changeWaypointANI.CloseWaypointsANI();
            behaveModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.FullyEditingObject;
        }

        public void StopScanning()
        {
            changeScanANI.CloseScanningANI();
            mainModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.Default;
        }

        public void StartWaypointing()
        {
            changeBehaveANI.CloseBehaveANI();
            waypointPlacementUI.SetActive(true);
            ARController.controllerstate = ControllerState.Waypointing;
        }

        public void StartLoading()
        {
            loadingModeUI.SetActive(false);
            mainModeUI.SetActive(true);
        }
        public void EditingObjectStats()
        {
            behaveStatsUI.SetActive(true);
            ARController.controllerstate = ControllerState.EditingStats;
            if (ARController.CurrentSelectedModel != null)
            {
                BehaveController.CurrentAIStats = ARController.CurrentSelectedModel.GetComponent<AIStats>();
                BehaveController.StartCurrentAIStats();
            }
        }
        public void ConfirmEditingObjectStats()
        {
            BehaveController.UpdateCurrentAiStats();
            changeStatsANI.CloseBehaveStatsANI();
            ARController.controllerstate = ControllerState.FullyEditingObject;
            BehaveController.CurrentAIStats = null;
        }
        public void StartSaving()
        {
            SceneManager.Instance.ToggleNewSave();
        }
        //
        public void ChangeWeapon(int WeaponNumber)
        {
            currentHuman.WeaponUpdate(WeaponNumber);
        }
        public void OpenEquipment()
        {
            behaveEquipmentUI.SetActive(true);
            currentHuman = ARController.CurrentSelectedModel.GetComponent<Human>();
        }
        public void CloseEquipment()
        {
            changeEquipmentANI.CloseBehaveReactANI();
            ARController.controllerstate = ControllerState.FullyEditingObject;
            currentHuman = null;
        }
        //
        public void OpenReact()
        {
            behaveReactUI.SetActive(true);
            currentAI = ARController.CurrentSelectedModel.GetComponent<AIBasics>();
        }
        public void CloseReact()
        {
            changeReactANI.CloseBehaveReactANI();
            ARController.controllerstate = ControllerState.FullyEditingObject;
            currentAI = null;
        }
        public void SetReactionEnum(int Number)
        {
            currentAI.Behaviour = (AIBehaviour)Number;
        }
        public void ToggleLoadingUI(bool toggle)
        {
            loadingModeUI.SetActive(toggle);
            mainModeUI.SetActive(true);
        }
        /// <summary>
        /// Remove Related UI Voids
        /// </summary>
        /// 
        ///UI Void that removes the actual Unit (called when pressed YES)
        public void RemoveUnit()
        {
            Destroy(ARController.CurrentSelectedModel.transform.root.gameObject);
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
                //behaveModeUI.SetActive(true);
            }
            else if (!RemovalUI.active)
            {
                RemovalUI.SetActive(true);
                //behaveModeUI.SetActive(false);
            }
        }
        /// <summary>
        /// Trigger the scaling mode when the scale button is pressed
        /// </summary>
        public void TriggerScaling()
        {
            ARController.controllerstate = ControllerState.Editing;
            changeBehaveANI.CloseBehaveANI();
            rotatingModeUI.SetActive(true);
            ReScaling = true;
        }
        #endregion

    }
} 