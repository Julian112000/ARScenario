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

        public ChangeUIScript canvasChange;

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
                    if (CanClick)
                    {
                        if (hit.collider.tag == "RightArrow" && change.currentObject <= 4)
                        {
                                StartCoroutine(MoveArrow(1));
                                CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "LeftArrow" && change.currentObject >= 2)
                        {
                                StartCoroutine(MoveArrow(-1));
                                CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "DoneButton")
                        {
                            StartCoroutine(DoneButton());
                            CanClick = false;
                        }
                        else if (hit.collider.gameObject.tag == "confirmChoice")
                        {
                            Debug.Log("Chose");
                        }
                       
                    }
                }
            }
            //Debug.DrawRay(transform.position, hit.point, Color.yellow);
        }

        public IEnumerator MoveArrow(int direction)
        {
            change.currentObject += direction;
            change.ChangeCurrentObject(change.currentObject);
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }
        public IEnumerator DoneButton()
        {
            canvasChange.buildModeUI.SetActive(false);
            canvasChange.mainModeUI.SetActive(true);
            //

            ARController.ChangeModel(change.AllObjects[change.currentObject].ConnectedModel);
            //
            yield return new WaitForSeconds(0.25f);
            CanClick = true;
        }

    }
}
 
