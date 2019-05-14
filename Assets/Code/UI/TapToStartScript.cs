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

    public class TapToStartScript : MonoBehaviour
    {

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Application.LoadLevel(1);
            }
        }
    }
}
