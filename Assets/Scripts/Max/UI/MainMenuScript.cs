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
        private Animator animator;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "NewScenario") //Start a new scenario
                    {
                        Debug.Log("Started new scenario");
                        animator.SetTrigger("FadeOutMain");
                    }
                    else if (hit.collider.gameObject.tag == "LoadScenario") //Load a saved scenario
                    {
                        Debug.Log("Loaded saved Scenario");
                    }
                }
            }
        }
    }
}
