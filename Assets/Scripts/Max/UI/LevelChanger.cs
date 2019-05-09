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

    public class LevelChanger : MonoBehaviour
    {

        public Animator animator;

        private int levelToLoad;

        public ChangeObjectScript objChanger;

        public void OnFadeComplete()
        {
            int i = Application.loadedLevel;
            Application.LoadLevel(i + 1);
        }


        //After Animations
        public void LetMeDannyDeletoFade()
        {
            this.gameObject.SetActive(false);
        }

        public void OnCloseBuildComplete()
        {

            objChanger.buildModeUI.SetActive(false);
        }

        public void OnCloseMainComplete()
        {
            objChanger.mainModeUI.SetActive(false);
        }

        public void OnClosePlaceComplete()
        {
            objChanger.placeModeUI.SetActive(false);
        }

        public void OnCloseRotateComplete()
        {
            objChanger.rotatingModeUI.SetActive(false);
        }

        public void OnCloseScanningComplete()
        {
            objChanger.scanningModeUI.SetActive(false);
        }

        public void OnCloseSelectComplete()
        {
            objChanger.selectModeUI.SetActive(false);
        }

        public void OnCloseBehaveComplete()
        {
            objChanger.behaveModeUI.SetActive(false);
        }

        public void OnCloseStatsComplete()
        {
            objChanger.behaveStatsUI.SetActive(false);
            objChanger.behaveReactUI.SetActive(false);
            objChanger.behaveEquipmentUI.SetActive(false);
            objChanger.waypointPlacementUI.SetActive(false);
            objChanger.RemovalUI.SetActive(false);
        }

        //83C05A

        //Animation Triggers
        public void CloseBuildANI()
        {
            animator.SetTrigger("Close");
        }

        public void CloseMainANI()
        {
            animator.SetTrigger("CloseM");
        }

        public void ClosePlayANI()
        {
            animator.SetTrigger("CloseP");
        }

        public void ClosePlaceAni()
        {
            animator.SetTrigger("ClosePlacing");
        }

        public void CloseRotateANI()
        {
            animator.SetTrigger("CloseRotate");
        }

        public void CloseScanningANI()
        {
            animator.SetTrigger("CloseScanning");
        }

        public void CloseSelectANI()
        {
            animator.SetTrigger("CloseSelect");
        }

        public void CloseBehaveANI()
        {
            animator.SetTrigger("CloseBehave");
        }

        public void CloseBehaveStatsANI()
        {
            animator.SetTrigger("CloseStats");
        }

        public void CloseBehaveReactANI()
        {
            animator.SetTrigger("CloseReact");
        }

        public void CloseWaypointsANI()
        {
            animator.SetTrigger("CloseWaypoints");
        }


    }
}