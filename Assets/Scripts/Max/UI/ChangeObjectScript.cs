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

    public class ChangeObjectScript : MonoBehaviour
    {
        public ObjectChangerScript change;

        public Camera mainCamera;

        
        public LevelChanger changeBuildANI, changeMainANI, changePlayANI, changePlaceANI, changeRotateANI, changeScanANI, changeSelectANI, changeBehaveANI;

        [Header("UIModes")]
        public GameObject buildModeUI;

        public GameObject mainModeUI;

        public GameObject placeModeUI;

        public GameObject rotatingModeUI;

        public GameObject scanningModeUI;

        public GameObject behaveModeUI;

        public GameObject selectModeUI;

        [SerializeField]
        private GameObject consoleModeUI;
        [SerializeField]
        private GameObject waypointPlacementUI;

        [Header("Object")]
        public GameObject CurrentSelectedObject;

        private bool CanClick = true;

        void Update()
        {
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
                                Debug.Log("yeet");
                                break;

                            case TouchPhase.Ended:
                                Debug.Log("yoot");
                                HandleAllButtons(hit);
                                break;
                        }
                    }
                }
            }
        }

        private void HandleAllButtons(RaycastHit hit)
        {
            if (hit.collider.tag == "RightArrow" && change.currentObject <= 7) //Right arrow
            {
                StartCoroutine(MoveArrow(1));
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "LeftArrow" && change.currentObject >= 2) //Left arrow
            {
                StartCoroutine(MoveArrow(-1));
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "DoneButton") //Close build mode
            {
                StartCoroutine(DoneButton());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ConfirmRotate") //Close rotate/scale mode
            {
                StartCoroutine(ConfirmEditing());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "BuildModeButton") //Open build selectment
            {
                StartCoroutine(OpenBuildMenu());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "PlayMode") //Start scenario
            {
                StartCoroutine(StartPlayMode());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "LookAtAni") //??
            {
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "TargetModeButton") //Start build placement.
            {
                StartCoroutine(StartTargetMode());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "BackButton") //Go to home and close all other UI tabs.
            {
                StartCoroutine(HomeButton());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ConsoleButton") //Open & close console.
            {
                StartCoroutine(OpenConsole());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ScanModeButton") //Open scanmode
            {
                StartCoroutine(StartScanning());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ScanModeDoneButton") //Close scan mode
            {
                StartCoroutine(StopScanning());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ConfirmWaypoint") //Confirm waypointS
            {
                StartCoroutine(ConfirmWaypoint());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ClearWaypoint") //Clear all waypoints
            {
                StartCoroutine(ClearWaypoints());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "UndoWaypoint") //Undo last waypoint.
            {
                StartCoroutine(UndoWaypoint());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ConfirmPlacement") //Go back to the build choose UI;
            {
                StartCoroutine(ConfirmPlacement());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "BehaviorSelect") //Open the select target menu.
            {
                StartCoroutine(BehaveSelection());
                CanClick = false;
            }
            else if(hit.collider.gameObject.tag == "SkipSelect") //Om het selecten te skippen, TIJDELIJK.
            {
                StartCoroutine(SkipSelect());
                CanClick = false;
            }
        }
        #region All Enumerators

        public IEnumerator SkipSelect()
        {
            changeSelectANI.CloseSelectANI();
            behaveModeUI.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }

        public IEnumerator MoveArrow(int direction)
        {
            change.currentObject += direction;
            change.ChangeCurrentObject(change.currentObject);
            yield return new WaitForSeconds(0.25f);
            CanClick = true; 
        }
        public IEnumerator DoneButton() //Confirm Build Selection.
        {
            changeBuildANI.CloseBuildANI();
            //

            placeModeUI.SetActive(true);
            //mainModeUI.SetActive(true);
            //

            ARController.ChangeModel(change.AllObjects[change.currentObject].ConnectedModel);
            ARController.controllerstate = ControllerState.Placing;
            //
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
            //
        }
        public IEnumerator ConfirmEditing()
        {
            changeRotateANI.CloseRotateANI();

            //
            placeModeUI.SetActive(true);

            //
            ARController.controllerstate = ControllerState.Default;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator ConfirmPlacement()
        {
            changePlaceANI.ClosePlaceAni();

            //
            buildModeUI.SetActive(true);

            //
            ARController.controllerstate = ControllerState.Editing;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator OpenBuildMenu()
        {
            buildModeUI.SetActive(true);

            //
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();

            //
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }

        public IEnumerator BehaveSelection()
        {
            selectModeUI.SetActive(true);

            //
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();

            //
            ARController.controllerstate = ControllerState.SelectingObject;
            //
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator StartPlayMode()
        {
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
            buildModeUI.SetActive(false);
            placeModeUI.SetActive(false);
            rotatingModeUI.SetActive(false);
            SceneHandler.EnablePlayMode();
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator StartTargetMode() 
        {
            placeModeUI.SetActive(true);
            mainModeUI.SetActive(false);
            ARController.controllerstate = ControllerState.Targeting;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator HomeButton() //HOME BUTTON.
        {
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
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator OpenConsole()
        {
            ConsoleScript.Instance.ToggleConsole();
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator StartScanning()
        {
            scanningModeUI.SetActive(true);
            changeMainANI.CloseMainANI();
            changePlayANI.ClosePlayANI();
            consoleModeUI.SetActive(false);
            ARController.controllerstate = ControllerState.Scanning;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator UndoWaypoint()
        {
            Debug.Log("Undo");
            ARController.CurrentplacedObject.GetComponent<AIBasics>().UndoLastWaypoint();
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator ClearWaypoints()
        {
            Debug.Log("Cleared");
            ARController.CurrentplacedObject.GetComponent<AIBasics>().ClearWaypoints();
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator ConfirmWaypoint()
        {
            waypointPlacementUI.SetActive(false);
            mainModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.Default;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator StopScanning()
        {
            changeScanANI.CloseScanningANI();
            mainModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.Default;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        #endregion

    }
} 