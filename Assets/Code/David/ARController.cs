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
        Rotating
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
        [SerializeField]
        private GameObject Model;
        public ControllerState controllerstate;
        [SerializeField]
        private GameObject CurrentplacedObject;
        [SerializeField]
        GameObject GridViewer;
        //
        Vector2 FirstPos;
        Vector2 LastPos;

        // Update is called once per frame
        void Update()
        {
            Debug.Log(Session.Status);
            if (Session.Status != SessionStatus.Tracking)
            {
                Debug.Log("NOT TRACKING");
                return;
            }
            if (controllerstate == ControllerState.Placing)
            {
                ScreenInputPlacing();
                GridViewer.SetActive(true);
            }
            if (controllerstate == ControllerState.Scaling)
            {
                ScreenInputScaling();
                GridViewer.SetActive(false);
            }
            if (controllerstate == ControllerState.Rotating)
            {
                ScreenInputRotating();
                GridViewer.SetActive(false);
            }

        }

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
                    // world evolves.
                    Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    SpawnedObject.transform.parent = anchor.transform;
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
            if(Input.touchCount == 1)
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
                        CurrentplacedObject.transform.Rotate(new Vector3(0, RotateSpeed, 0));

                    }
                }
            }
        }


    }
}
