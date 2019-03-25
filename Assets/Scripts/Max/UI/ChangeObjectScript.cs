﻿namespace GoogleARCore.Examples.HelloAR
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
        

        public LevelChanger changeBuildANI, changeMainANI, changePlayANI;
   

        public GameObject buildModeUI;

        public GameObject mainModeUI;

        [SerializeField]
        private GameObject rotatingModeUI;
        [SerializeField]
        private GameObject targetModeUI;
        [SerializeField]
        private GameObject scanningModeUI;
        [SerializeField]
        private GameObject consoleModeUI;

        private bool CanClick = true;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (CanClick)
                    {
                        if (hit.collider.tag == "RightArrow" && change.currentObject <= 4) //Right arrow
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
                            StartCoroutine(ConfirmRotation());
                            CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "BuildModeButton")
                        {
                            StartCoroutine(OpenBuildMenu());
                            CanClick = false;
                        }
                        else if(hit.collider.gameObject.tag == "PlayMode")
                        {
                            StartCoroutine(StartPlayMode());
                            CanClick = false;
                        }
                        else if(hit.collider.gameObject.tag == "LookAtAni")
                        {
                            CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "TargetModeButton")
                        {
                            StartCoroutine(StartTargetMode());
                            CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "BackButton")
                        {
                            StartCoroutine(StopTargetMode());
                            CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "ConsoleButton")
                        {
                            StartCoroutine(OpenConsole());
                            CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "ScanModeButton")
                        {
                            StartCoroutine(StartScanning());
                            CanClick = false;
                        }
                    }
                }
            }
        }

        private void HandleAllButtons(RaycastHit hit)
        {
            if (hit.collider.tag == "RightArrow" && change.currentObject <= 4) //Right arrow
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
            else if (hit.collider.gameObject.tag == "BuildModeButton")
            {
                StartCoroutine(OpenBuildMenu());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "PlayMode")
            {
                StartCoroutine(StartPlayMode());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "LookAtAni")
            {
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "TargetModeButton")
            {
                StartCoroutine(StartTargetMode());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "BackButton")
            {
                StartCoroutine(StopTargetMode());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ConsoleButton")
            {
                StartCoroutine(OpenConsole());
                CanClick = false;
            }
            else if (hit.collider.gameObject.tag == "ScanModeButton")
            {
                StartCoroutine(StartScanning());
                CanClick = false;
            }
        }
        #region All Enumerators
        public IEnumerator MoveArrow(int direction)
        {
            change.currentObject += direction;
            change.ChangeCurrentObject(change.currentObject);
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator DoneButton()
        {
            mainModeUI.SetActive(true);
            //

            ARController.ChangeModel(change.AllObjects[change.currentObject].ConnectedModel);
            ARController.controllerstate = ControllerState.Placing;
            //

            changeBuildANI.CloseBuildANI();
            //
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
            //
        }
        public IEnumerator ConfirmEditing()
        {
            rotatingModeUI.SetActive(false);
            mainModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.Default;
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
        public IEnumerator StartPlayMode()
        {
            buildModeUI.SetActive(false);
            mainModeUI.SetActive(false);
            targetModeUI.SetActive(false);
            rotatingModeUI.SetActive(false);
            SceneHandler.EnablePlayMode();
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator StartTargetMode()
        {
            targetModeUI.SetActive(true);
            mainModeUI.SetActive(false);
            ARController.controllerstate = ControllerState.Targeting;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }

        public IEnumerator StopTargetMode()
        {
            targetModeUI.SetActive(false);
            scanningModeUI.SetActive(false);
            mainModeUI.SetActive(true);
            ARController.controllerstate = ControllerState.Placing;
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
            mainModeUI.SetActive(false);
            consoleModeUI.SetActive(false);
            ARController.controllerstate = ControllerState.Scanning;
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }

        #endregion

    }
}
 
