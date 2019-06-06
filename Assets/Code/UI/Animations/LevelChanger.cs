namespace GoogleARCore.Examples.HelloAR
{

    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class LevelChanger : MonoBehaviour
    {
        ///<summary>
        /// The animator that will play its animation.
        ///</summary>
        [Header("Animator")]
        public Animator animator;

        ///<summary>
        /// The UIController wich has the script on it that activates the animation.
        ///</summary>
        [Header("Scripts")]
        public ChangeObjectScript objChanger;

        ///<summary>
        ///The int for the level you want to load next.
        ///</summary>
        private int levelToLoad;

        public bool scanning;
        public bool building;
        public bool main;
        public bool placing;
        public bool waypointing;
        public bool scaling;

        ///<summary>
        ///The function that switches to the next scene.
        ///</summary>
        #region SceneSwitchAnimation
        public void OnFadeComplete()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
        #endregion

        ///<summary>
        ///All the functions that will happen after the animation is done (such as disable a canvas after remove animation)
        ///</summary>
        #region OnCompletedAnimation
        //After Animations
        public void LetMeDannyDeletoFade()
        {
            this.gameObject.SetActive(false);
        }

        public void OnCloseBuildComplete()
        {

            objChanger.buildModeUI.SetActive(false);
            if (main)
            {
                objChanger.mainModeUI.SetActive(true);
            }
            else if (placing)
            {
                objChanger.placeModeUI.SetActive(true);
            }
        }

        public void OnCloseMainComplete()
        {
            objChanger.mainModeUI.SetActive(false);
            if (scanning)
            {
                objChanger.scanningModeUI.SetActive(true);
            }
            else if (building)
            {
                objChanger.buildModeUI.SetActive(true);
            }
        }

        public void OnClosePlaceComplete()
        {
            objChanger.placeModeUI.SetActive(false);
            objChanger.buildModeUI.SetActive(true);
        }

        public void OnCloseRotateComplete()
        {
            objChanger.rotatingModeUI.SetActive(false);

            //
            if (objChanger.ReScaling)                                                          //If you are rescaling instead of first time scaling, go back to the behaviour screen of the selected object.
            {
                ARController.controllerstate = ControllerState.FullyEditingObject;
                objChanger.ReScaling = false;
                objChanger.behaveModeUI.SetActive(true);
            }
            else if (!objChanger.ReScaling)                                                    //If you are not rescaling but first time scaling, go back to the placing screen of the spawnable object.
            {
                ARController.controllerstate = ControllerState.Placing;
                objChanger.placeModeUI.SetActive(true);
            }

        }

        public void OnCloseScanningComplete()
        {
            objChanger.scanningModeUI.SetActive(false);
            objChanger.mainModeUI.SetActive(true);
        }

        public void OnCloseBehaveComplete()
        {
            objChanger.behaveModeUI.SetActive(false);
            if (scaling)
            {
                objChanger.rotatingModeUI.SetActive(true);
            }
            else if (waypointing)
            {
                objChanger.waypointPlacementUI.SetActive(true);
            }
            else if (main)
            {
                objChanger.mainModeUI.SetActive(true);
            }
        }

        public void OnCloseStatsComplete()
        {
            objChanger.behaveStatsUI.SetActive(false);
            objChanger.behaveReactUI.SetActive(false);
            objChanger.behaveEquipmentUI.SetActive(false);
            objChanger.waypointPlacementUI.SetActive(false);
            objChanger.RemovalUI.SetActive(false);
            objChanger.behaveModeUI.SetActive(true);
        }

        public void OnCloseLoadComplete()
        {
            objChanger.loadingModeUI.SetActive(false);
            objChanger.mainModeUI.SetActive(true);
        }
        #endregion

        ///<summary>
        ///The function that triggers the specific animation for the canvas switch.
        ///</summary>
        #region StartAnimations
        //Animation Triggers
        public void CloseANI(string animname)
        {
            animator.SetTrigger(animname);
        }
        #endregion



    }
}