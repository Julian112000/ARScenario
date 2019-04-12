﻿namespace GoogleARCore.Examples.HelloAR
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

    public class ObjectChangerScript : MonoBehaviour
    {

        public List<MainObjectScript> AllObjects;
        //Middle object
        public ObjectDisplayScript o_main;
        public ObjectDisplayScript o_left;
        public ObjectDisplayScript o_right;

        public ObjectDisplayScript o_main_display;

        public int currentObject = 3;

        private void Awake()
        {
            ChangeCurrentObject(currentObject);
        }


        public void ChangeCurrentObject(int CurrentObject)
        {
            int left = currentObject - 1;
            int right = currentObject + 1;

            o_left.UIObject = AllObjects[left];
            o_main.UIObject = AllObjects[CurrentObject];
            o_right.UIObject = AllObjects[right];

            //Feedback for current object that your placing.
            o_main_display.UIObject = AllObjects[CurrentObject];

        }
    }
}
