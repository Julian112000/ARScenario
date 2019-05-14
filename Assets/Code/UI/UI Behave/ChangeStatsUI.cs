namespace GoogleARCore.Examples.HelloAR
{

    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class ChangeStatsUI : MonoBehaviour
    {
        public Image currentEquipmentImage;
        public Image PreviousEquipmentImage;
        public Camera mainCamera;

        public Color selectedColor;
        public Color normalColor;

        public void Update()
        {

            if (Input.touchCount > 0)
            {
                StartCoroutine(ChangeSelectedStatePhone());
            }
            else if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(ChangeSelectedState());
            }



        }

        public IEnumerator ChangeSelectedState()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject.tag == "StatSelect")
                {
                    PreviousEquipmentImage = currentEquipmentImage;
                    yield return new WaitForSeconds(0.1f);
                    currentEquipmentImage = hit.collider.gameObject.GetComponent<Image>();

                    if (currentEquipmentImage != null)
                    {
                        currentEquipmentImage.color = selectedColor;
                    }
                    if (PreviousEquipmentImage != null)
                    {
                        PreviousEquipmentImage.color = normalColor;
                    }
                }
            }
        }

        public IEnumerator ChangeSelectedStatePhone()
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        break;

                    case TouchPhase.Ended:
                        if (hit.collider.gameObject.tag == "StatSelect")
                        {
                            PreviousEquipmentImage = currentEquipmentImage;
                            yield return new WaitForSeconds(0.1f);
                            currentEquipmentImage = hit.collider.gameObject.GetComponent<Image>();

                            if (currentEquipmentImage != null)
                            {
                                currentEquipmentImage.color = selectedColor;
                            }
                            if (PreviousEquipmentImage != null)
                            {
                                PreviousEquipmentImage.color = normalColor;
                            }
                        }
                        break;
                }
            }
        }


    }
}
