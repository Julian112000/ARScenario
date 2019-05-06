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

        public LevelChanger changeBuildANI, changeMainANI, changePlayANI, changePlaceANI, changeRotateANI, changeScanANI, changeSelectANI, changeBehaveANI, changeStatsANI, changeReactANI, changeEquipmentANI, changeWaypointANI;

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
        [SerializeField]
        private GameObject loadingModeUI;

        public StatsUIScript BehaveController;

        private Human currentHuman;
        private AIBasics currentAI;

        private bool CanClick = true;

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            if(SimpleLoadBool.willLoad == true)
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

                                if(button.myEvent != null)
                                {
                                    button.ButtonClicked();
                                    Debug.Log("Clicked on Selfmade Button");
                                }
                                break;
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0))
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
            if(change.currentObject <= 7 && change.currentObject >= 2)
            {
                change.currentObject += direction;
                change.ChangeCurrentObject(change.currentObject);
            }
            if(change.currentObject == 8 && direction == -1)
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
            placeModeUI.SetActive(true);

            //
            ARController.controllerstate = ControllerState.Placing;
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
            ARController.controllerstate = ControllerState.SelectingObject;
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
           if(ARController.controllerstate == ControllerState.FullyEditingObject)
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
            if(ARController.CurrentSelectedModel != null)
            {
                BehaveController.CurrentAIStats = ARController.CurrentSelectedModel.GetComponent<AIStats>();
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
        #endregion

    }
} 