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
    public class ChangeUIScript : MonoBehaviour
    {

        public RaycastHit hit;

        public GameObject buildModeUI;
        public GameObject mainModeUI;

        public Camera mainCamera;

        private bool CanClick = true;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "BuildModeButton")
                    {
                        StartCoroutine(OpenBuildMenu());
                    }
                }
            }
            Debug.DrawRay(transform.position, hit.point, Color.yellow);
        }

        public IEnumerator OpenBuildMenu()
        {
            buildModeUI.SetActive(true);
            mainModeUI.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
    }
} 
