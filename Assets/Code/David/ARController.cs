namespace GoogleARCore.Examples.HelloAR
{
    //
#if UNITY_EDITOR
    // NOTE:
    // - InstantPreviewInput does not support `deltaPosition`.
    // - InstantPreviewInput does not support input from
    //   multiple simultaneous screen touches.
    // - InstantPreviewInput might miss frames. A steady stream
    //   of touch events across frames while holding your finger
    //   on the screen is not guaranteed.
    // - InstantPreviewInput does not generate Unity UI event system
    //   events from device touches. Use mouse/keyboard in the editor
    //   instead.
    using Input = InstantPreviewInput;
#endif

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GoogleARCore;
    using GoogleARCoreInternal;
    using GoogleARCore.Examples.Common;
    //
    public enum ControllerState
    {
        Default,
        Scanning,
        Placing,
        Editing,
        SelectingObject,
        FullyEditingObject,
        EditingStats,
        Waypointing,
        Targeting,
        Playing
    }



    public class ARController : MonoBehaviour
    {
        [SerializeField]
        private float ScaleSpeed = 0.02f;
        [SerializeField]
        private float RotateSpeed = 1f;
        //
        [SerializeField]
        private Camera FirstpersonCam;
        private static GameObject Model;
        [SerializeField]
        public static ControllerState controllerstate;
        public static GameObject CurrentplacedObject;
        [SerializeField]
        GameObject GridViewer;
        //
        Vector2 FirstPos;
        Vector2 LastPos;
        //
        [SerializeField]
        LayerMask unitlayer;
        [SerializeField]
        GameObject WaypointPrefab;
        //
        [SerializeField]
        GameObject BuildCanvas;
        [SerializeField]
        GameObject SelectBuildCanvas;
        [SerializeField]
        GameObject ObjectEditCanvas;
        [SerializeField]
        GameObject ScalingFeedback;
        [SerializeField]
        GameObject RotateFeedback;
        [SerializeField]
        GameObject SelectingObjectCanvas;
        [SerializeField]
        GameObject FullyEditingCanvas;
        [SerializeField]
        GameObject StatsChangeCanvas;
        [SerializeField]
        public static GameObject CurrentSelectedModel;
        public static bool SelectedAHuman = false;
        public static bool Scanning = false;

        public bool canPlace = false;

        private void Awake()
        {
            SceneHandler.SwitchToPlayMode += Playmode;
        }

        private void Start()
        {
            canPlace = false;
        }
        void Update()
        {
            if (SelectBuildCanvas.activeSelf || ObjectEditCanvas.activeSelf)
            {
                canPlace = false;
            }
            else
            {
                StartCoroutine(SetBuildMode());
            }
            SessionHandling();
            Debug.Log(controllerstate + " Controllerstate");
        }

        private void SessionHandling()
        {
            //If you are not Tracking with the camera nothing works!
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }
            //Switch state handling almost the entire ARController
            switch (controllerstate)
            {
                case ControllerState.Default:
                    Default();
                    break;
                case ControllerState.Scanning:
                    ScanningUpdate();
                    break;
                case ControllerState.Placing:
                    if (canPlace) Placing();
                    break;
                case ControllerState.Editing:
                    Editing();
                    break;
                case ControllerState.SelectingObject:
                    SelectingObject();
                    break;
                case ControllerState.FullyEditingObject:
                    FullyEditingObject();
                    break;
                case ControllerState.EditingStats:
                    EditingUnitStats();
                    break;
                case ControllerState.Waypointing:
                    PlacingWaypoints();
                    break;
                case ControllerState.Targeting:
                    break;
                case ControllerState.Playing:
                    break;
                default:
                    break;
            }
        }
        //The voids linked to the main ENUM
        #region EnumVoids
        //Called when the enum is in Defaultmode
        private void Default()
        {
            Scanning = false;
            //GridViewer.SetActive(true);
        }
        //Called when the enum is on Scanning (when you want to scan in the ARCore grid WITHOUT ANY PLACE INPUT)
        private void ScanningUpdate()
        {
            Scanning = true;
            //GridViewer.SetActive(true);
        }
        //Called when enum is on Placing (you can still scan while placing)
        private void Placing()
        {
            Scanning = false;
            ScreenInputPlacing();
            //
            ObjectEditCanvas.SetActive(false);
            //GridViewer.SetActive(true);
        }
        //Called when enum is on Editing (this is when wanting to rotate or scale but no input has been found yet
        private void Editing()
        {
            BuildCanvas.SetActive(false);
            ObjectEditCanvas.SetActive(true);
            Scanning = false;
            //GridViewer.SetActive(true);
            //
            if (Input.touchCount == 1)
            {
                Rotating();

            }
            if (Input.touchCount == 2)
            {
                Scaling();
            }
            if (Input.touchCount == 0)
            {
                ScalingFeedback.SetActive(true);
                RotateFeedback.SetActive(true);
            }
        }
        //This void handles everything needed when wanting to select an object to edit it afterwards
        private void SelectingObject()
        {
            ScreenInputSelectingObject();
            //
            SelectingObjectCanvas.SetActive(true);
            //
            Scanning = false;
            //GridViewer.SetActive(false);
            ScalingFeedback.SetActive(false);
            RotateFeedback.SetActive(false);
            //
        }
        //This void is when you have an object selected and now want to choose what to do with it.
        private void FullyEditingObject()
        {
            FullyEditingCanvas.SetActive(true);
            SelectingObjectCanvas.SetActive(false);
        }
        //This void is the update of when you are editing a selected units Stats
        private void EditingUnitStats()
        {
            Scanning = false;
            //StatsChangeCanvas.SetActive(true);
        }
        //This void is called when you are editing and have one finger on the touchscreen (Rotating)
        private void Rotating()
        {
            ScreenInputRotating();
            Scanning = false;
            //GridViewer.SetActive(false);
            ScalingFeedback.SetActive(false);
            RotateFeedback.SetActive(true);
        }
        //This void is called when you are editing and have two fingers on the touchscreen (Scaling)
        private void Scaling()
        {
            ScreenInputScaling();
            Scanning = false;
            //GridViewer.SetActive(false);
            ScalingFeedback.SetActive(true);
            RotateFeedback.SetActive(false);
        }
        //This void is called when you are done editing and are ready to place waypoints for the Unit's path
        private void PlacingWaypoints()
        {
            Scanning = false;
            //GridViewer.SetActive(true);
            ScreenInputWaypointing();
        }
        #endregion
        //The voids that handle the MobileInput
        #region Input Voids
        //Screen Input when in build mode and Prefab of unit has been selected to place
        void ScreenInputPlacing()
        {
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstpersonCam.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    GameObject SpawnedObject = Instantiate(Model, hit.Pose.position, hit.Pose.rotation);
                    SpawnedObject.transform.Rotate(0, 180, 0, Space.Self);
                    CurrentplacedObject = SpawnedObject;
                    //ConsoleScript.Instance.SetFeedback(MessageType.BuildMessage, CurrentplacedObject.name);
                    //
                    Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    SpawnedObject.transform.parent = anchor.transform;
                    //Switch Editor State to scaling
                    controllerstate = ControllerState.Editing;
                    //

                }
            }
        }
        //Screen input for Scaling this is pinching with two fingers to make a object smaller or unpinching to make it bigger
        void ScreenInputScaling()
        {
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                //
                if (CurrentplacedObject.transform.localScale.x > 0)
                    CurrentplacedObject.transform.localScale += new Vector3(CurrentplacedObject.transform.localScale.x * deltaMagnitudeDiff * ScaleSpeed, CurrentplacedObject.transform.localScale.y * deltaMagnitudeDiff * ScaleSpeed, CurrentplacedObject.transform.localScale.z * deltaMagnitudeDiff * ScaleSpeed);

                // Clamp the field of view to make sure it's between 0 and 180.
                FirstpersonCam.fieldOfView = Mathf.Clamp(FirstpersonCam.fieldOfView, 0.1f, 179.9f);
            }

        }
        //Screen input for rotating this is swiping left or right to turn the object
        void ScreenInputRotating()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("began");
                    FirstPos = touch.position;
                    LastPos = touch.position;
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    LastPos = touch.position;
                    if (FirstPos.x < LastPos.x)
                    {
                        Debug.Log("Swiping right");
                        CurrentplacedObject.transform.Rotate(new Vector3(0, RotateSpeed, 0));

                    }
                    else if (LastPos.x < FirstPos.x)
                    {
                        Debug.Log("Swiping Left");
                        CurrentplacedObject.transform.Rotate(new Vector3(0, -RotateSpeed, 0));

                    }
                }
            }
        }
        //Screen input for Placing waypoints so that the player gets a path
        void ScreenInputWaypointing()
        {
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            //
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstpersonCam.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    GameObject SpawnedObject = Instantiate(WaypointPrefab, hit.Pose.position, hit.Pose.rotation,CurrentSelectedModel.transform);
                    WaypointScript script = SpawnedObject.GetComponent<WaypointScript>();
                    CurrentSelectedModel.GetComponent<AIBasics>().PlaceWayPoint(hit.Pose.position, script);
                }
            }
        }
        //Screen Input when in Select mode. this handles it so that you can tap on an object and edit it
        void ScreenInputSelectingObject()
        {
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }
            //
            //TrackableHit hit;
            //TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            //    TrackableHitFlags.FeaturePointWithSurfaceNormal;
            Ray ray = FirstpersonCam.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, unitlayer))
            {
                CurrentSelectedModel = hit.collider.gameObject;
                if(hit.collider.gameObject.GetComponent<AIBasics>().unittype == UnitType.Human)
                {
                    SelectedAHuman = true;
                }
                else
                {
                    SelectedAHuman = false;
                }
                CurrentSelectedModel.GetComponent<AIBasics>().Selected = true;
                CurrentSelectedModel.GetComponent<AIBasics>().TurnOnVisuals();
                //
                controllerstate = ControllerState.FullyEditingObject;
            }
            //if (Physics.Raycast(touch.position, FirstpersonCam.transform.forward,out hit, UnitLayer))
            //{
            //    CurrentSelectedModel = hit.collider.gameObject;
            //    CurrentSelectedModel.GetComponent<AIBasics>().Selected = true;
            //    //
            //    controllerstate = ControllerState.FullyEditingObject;
                
            //    //{

            //    //    GameObject SpawnedObject = Instantiate(Model, hit.Pose.position, hit.Pose.rotation);
            //    //    SpawnedObject.transform.Rotate(0, 180, 0, Space.Self);
            //    //    CurrentplacedObject = SpawnedObject;
            //    //    ConsoleScript.Instance.SetFeedback(MessageType.BuildMessage, CurrentplacedObject.name);
            //    //    //
            //    //    Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            //    //    SpawnedObject.transform.parent = anchor.transform;
            //    //    //Switch Editor State to scaling
            //    //    controllerstate = ControllerState.Editing;
            //    //    //

            //    //}
            //}
        }
        #endregion

        //Void that is subscribed to the playmode event
        public void Playmode()
        {
            DetectedPlaneGenerator.instance.ToggleVisualizers(false);
            //GridViewer.SetActive(false);
            controllerstate = ControllerState.Playing;
            //
        }
        //Static void to change model from another script
        public static void ChangeModel(GameObject modelpar)
        {
            Model = modelpar;
        }
        //Enumarator to make sure you dont place when pressing a button
        public IEnumerator SetBuildMode()
        {
            yield return new WaitForSeconds(1f);
            canPlace = true;
        }
    }
}
