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
        [Header("Images")]
        [SerializeField]
        private Image currentEquipmentImage;    //The image for the selected equipment.
        [SerializeField]
        private Image PreviousEquipmentImage;   //The image for the previous selected equipment.

        [Header("UI Camera")]
        [SerializeField]
        private Camera mainCamera;              //The camera used for the UI.

        [Header("Colors")]
        [SerializeField]
        private Color selectedColor;            //The color for the selected equipment.
        [SerializeField]
        private Color normalColor;              //The color for the previous selected equipment.

        public void Update()
        {
            if (Input.touchCount > 0)                           //If you touch with 1 finger on the screen it will start the coroutine for the PHONE.
            {
                StartCoroutine(ChangeSelectedStatePhone());
            }
            else if (Input.GetMouseButtonDown(0))               //If you click one time with the mouse it will start the coroutine for the PC.
            {
                StartCoroutine(ChangeSelectedState());
            }
        }

        /// <summary>
        /// The functions that will change the color of the current selected equipment in the UI.
        /// </summary>
        #region ChangeSelectedColor

        ///<summary>
        /// Change the color for the PC.
        /// </summary>
        #region MouseClick
        public IEnumerator ChangeSelectedState()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject.tag == "StatSelect")
                {
                    PreviousEquipmentImage = currentEquipmentImage;                         //Change the previous image to the current image.
                    yield return new WaitForSeconds(0.1f);                                  //Wait 0.1 seconds.
                    currentEquipmentImage = hit.collider.gameObject.GetComponent<Image>();  //Change the current image to the image you clicked on.

                    if (currentEquipmentImage != null)
                    {
                        currentEquipmentImage.color = selectedColor;                        //Change the color of the current image.
                    }
                    if (PreviousEquipmentImage != null)
                    {
                        PreviousEquipmentImage.color = normalColor;                         //Change the color of the previous image.
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Change the color for the PHONE.
        /// </summary>
        #region FingerTouch
        public IEnumerator ChangeSelectedStatePhone()
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (touch.phase)
                {
                    //Once the finger enters the screen.
                    case TouchPhase.Began:
                        break;

                    //Once the finger leaves the screen.
                    case TouchPhase.Ended:
                        if (hit.collider.gameObject.tag == "StatSelect")
                        {
                            PreviousEquipmentImage = currentEquipmentImage;                         //Change the previous image to the current image.
                            yield return new WaitForSeconds(0.1f);                                  //Wait 0.1 seconds.
                            currentEquipmentImage = hit.collider.gameObject.GetComponent<Image>();  //Change the current image to the image you clicked on.

                            if (currentEquipmentImage != null)
                            {
                                currentEquipmentImage.color = selectedColor;                        //Change the color of the current image.
                            }
                            if (PreviousEquipmentImage != null)
                            {
                                PreviousEquipmentImage.color = normalColor;                         //Change the color of the previous image.
                            }
                        }
                        break;
                }
            }
        }
        #endregion
        
        #endregion

    }
}
