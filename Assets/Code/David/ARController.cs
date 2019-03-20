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
    //
    public enum ControllerState
    {
        Placing,
        Scaling,
        Rotating,
        Targeting,
        Playing
    }



    public class ARController : MonoBehaviour
    {

        //
        [SerializeField]
        private float ScaleSpeed = 0.05f;
        [SerializeField]
        private float RotateSpeed = 0.5f;
        //
        [SerializeField]
        private Camera FirstpersonCam;
        private static GameObject Model;
        public static ControllerState controllerstate;
        [SerializeField]
        private GameObject CurrentplacedObject;
        [SerializeField]
        GameObject GridViewer;
        //
        Vector2 FirstPos;
        Vector2 LastPos;
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

        public bool canPlace = false;

        private void Awake()
        {
            SceneHandler.SwitchToPlayMode += TurnOffBuildGrid;
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
        }

        private void SessionHandling()
        {
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }
            //
            if (controllerstate == ControllerState.Placing && canPlace)
            {
                Debug.Log("Allowed");
                //BuildCanvas.SetActive(true);
                ObjectEditCanvas.SetActive(false);
                //
                ScreenInputPlacing();
                GridViewer.SetActive(true);
            }
            //
            if (controllerstate != ControllerState.Placing && controllerstate != ControllerState.Targeting && controllerstate != ControllerState.Playing)
            {
                BuildCanvas.SetActive(false);
                ObjectEditCanvas.SetActive(true);
                if (Input.touchCount == 1)
                {
                    controllerstate = ControllerState.Rotating;
                    ScreenInputRotating();
                    GridViewer.SetActive(false);
                    ScalingFeedback.SetActive(false);
                    RotateFeedback.SetActive(true);

                }
                if(Input.touchCount == 2)
                {
                    controllerstate = ControllerState.Scaling;
                    //
                    ScreenInputScaling();
                    GridViewer.SetActive(false);
                    ScalingFeedback.SetActive(true);
                    RotateFeedback.SetActive(false);
                }
                if(Input.touchCount == 0)
                {
                    ScalingFeedback.SetActive(true);
                    RotateFeedback.SetActive(true);
                }
            }
        }
        public IEnumerator SetBuildMode()
        {
            yield return new WaitForSeconds(1f);
            canPlace = true;
        }
        #region Input Voids
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
                    ConsoleScript.Instance.SetFeedback(MessageType.BuildMessage, CurrentplacedObject.name);
                    //
                    Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    SpawnedObject.transform.parent = anchor.transform;
                    //Switch Editor State to scaling
                    controllerstate = ControllerState.Rotating;
                    //

                }
            }
        }

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

                CurrentplacedObject.transform.localScale += new Vector3(CurrentplacedObject.transform.localScale.x * deltaMagnitudeDiff * ScaleSpeed, CurrentplacedObject.transform.localScale.y * deltaMagnitudeDiff * ScaleSpeed, CurrentplacedObject.transform.localScale.z * deltaMagnitudeDiff * ScaleSpeed);

                // Clamp the field of view to make sure it's between 0 and 180.
                FirstpersonCam.fieldOfView = Mathf.Clamp(FirstpersonCam.fieldOfView, 0.1f, 179.9f);
            }

        }

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
        #endregion

        //
        public void TurnOffBuildGrid()
        {
            GridViewer.SetActive(false);
            controllerstate = ControllerState.Playing;
        }

        //Static void to change model from another script
        public static void ChangeModel(GameObject modelpar)
        {
            Model = modelpar;
        }

        public static void ChangeToBuild()
        {
            controllerstate = ControllerState.Placing;
        }
    }
}
