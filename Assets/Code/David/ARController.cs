using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ARController : MonoBehaviour {




    private List<DetectedPlane> Trackedplanes = new List<DetectedPlane>();
    [SerializeField]
    private GameObject GridPrefab;
    [SerializeField]
    private UiHandling uihandling;
    [SerializeField]
    private Camera FirstpersonCam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Session.Status != SessionStatus.Tracking)
        {
            Debug.Log("NOT TRACKING");
            return;
        }

        Session.GetTrackables<DetectedPlane>(Trackedplanes, TrackableQueryFilter.New);

        for (int i = 0; i < Trackedplanes.Count; i++)
        {
            GameObject Grid = Instantiate(GridPrefab.gameObject, Vector3.zero, Quaternion.identity, transform);

            Grid.GetComponent<GridVisualiser>().Initialize(Trackedplanes[i]);
        }
        ScreenInput();


    }

    void ScreenInput()
    {
        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstpersonCam.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                GameObject prefab;
                prefab = uihandling.CurrentModel;

                GameObject SpawnedObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                SpawnedObject.transform.Rotate(0, 180, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make Andy model a child of the anchor.
                SpawnedObject.transform.parent = anchor.transform;
            }
        }
    }

}
