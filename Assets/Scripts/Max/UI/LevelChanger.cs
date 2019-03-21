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

        public void CloseBuildANI()
        {
            animator.SetTrigger("Close");
        }

        public void OnCloseBuildComplete()
        {

           objChanger.buildModeUI.SetActive(false);
           print("works");
        }
    }
}