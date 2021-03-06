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

    public class DisableCanvas : MonoBehaviour
    {
        /// <summary>
        /// Every canvas in the scene.
        /// </summary>
        [Header("Screens")]
        [SerializeField]
        private GameObject settings;
        [SerializeField]
        private GameObject mainMenu;

        public void OnSettingsClosed()  //The void for closing the settings canvas.
        {
            settings.SetActive(false);  
        }
        public void OnMainClosed()      //The void for closing the main canvas.
        {
            mainMenu.SetActive(false);
        }
    }
}
