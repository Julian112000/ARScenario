namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    using Input = InstantPreviewInput;
#endif

    public class AppStartBools : MonoBehaviour
    {
        public static bool willLoad;

        public static bool dutchLanguage;
        public static bool englishLanguage;
    }
}
