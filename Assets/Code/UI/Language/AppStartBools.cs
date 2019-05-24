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
        //<summary>
        // The boolean that indicates if your going to load or create a scenario.
        //</summery>
        public static bool willLoad;

        //<summary>
        // The booleans that will set the language from the Startscene to the mainscene. 
        //</summery>
        public static bool dutchLanguage;
        public static bool englishLanguage;
    }
}
