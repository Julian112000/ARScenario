namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;


    public class EquipmentButtonScript : MonoBehaviour
    {
        SelfMadeButton selfmadebutton;

        private void Awake()
        {
            selfmadebutton = GetComponent<SelfMadeButton>();
        }

        // Update is called once per frame
        void Update()
        {
            if (ARController.SelectedAHuman)
            {
                selfmadebutton.enabled = true;
            }
            else
            {
                selfmadebutton.enabled = false;
            }
        }
    }
}
