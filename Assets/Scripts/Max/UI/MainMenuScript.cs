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

    public class MainMenuScript : MonoBehaviour
    {

        public Camera mainCamera;

        [SerializeField]
        private GameObject newOverlay;
        [SerializeField]
        private GameObject loadOverlay;

        [SerializeField]
        private Animator animator;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                        switch (touch.phase)
                        {
                            case TouchPhase.Began:
                            if (hit.collider.tag == "NewScenario") 
                            {
                                newOverlay.SetActive(false);
                            }
                            else if (hit.collider.tag == "LoadScenario")
                            {
                                loadOverlay.SetActive(false);
                            }
                            break;

                            case TouchPhase.Ended:
                            if (hit.collider.tag == "NewScenario") //Start a new scenario
                            {
                                animator.SetTrigger("FadeOutMain");
                                newOverlay.SetActive(true);
                            }
                            else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario
                            {
                                Debug.Log("Loaded saved Scenario");
                                loadOverlay.SetActive(true);
                            }
                            else if(hit.collider.gameObject.tag != "NewScenario" && hit.collider.gameObject.tag != "LoadScenario") //Set buttons back to normal
                            {
                                newOverlay.SetActive(true);
                                loadOverlay.SetActive(true);
                            }
                            break;
                        }
                
                }
            }
        }
    }
}
