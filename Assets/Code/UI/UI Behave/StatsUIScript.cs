namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

    public class StatsUIScript : MonoBehaviour
    {
        /// <summary>
        /// All the text labels in the stats canvas.
        /// </summary>
        [SerializeField]
        private Text healthText, ammoText, angleText, rangeText, speedText, accuracyText;

        /// <summary>
        /// Al the sliders in the stats canvas.
        /// </summary>
        [SerializeField]
        private Slider healthSlider, ammoSlider, angleSlider, rangeSlider, speedSlider, accuracySlider;

        /// <summary>
        /// The current AI stats of the selected object.
        /// </summary>
        public AIStats CurrentAIStats;

        /// <summary>
        /// The string that desides if the current speed is either: 'stop, slow, normal, fast or max. speed'.
        /// </summary>
        private string currentSpeedState;

        /// <summary>
        /// All the UI Visuals you can see and change in the stats canvas.
        /// </summary>
        #region UIFeedback For stats
        public void Update()
        {
            speedSlider.value = Mathf.Round(speedSlider.value * 2f) / 2f; //The value of the speedslider will always make sure in increases/decreases with 0.5.

            /// <summary>
            /// The english version of the stats.
            /// </summary>
            #region EnglishStats
            if (AppStartBools.englishLanguage)      //If the current selected language (wich is selected in the settings of the main menu) is english.
            {
                if (speedSlider.value == 0)         //Once the speedsliders value reaches 0, the currentspeedstate will be 'stop' (wich means its 0 (THE MIN.))
                {
                    currentSpeedState = "Stop";
                }
                else if (speedSlider.value == 0.5f) //Once the speedsliders value reaches 0.5, the currentspeedstate will be 'slow' (wich means its 0.5)
                {
                    currentSpeedState = "Slow";
                }
                else if (speedSlider.value == 1f)   //Once the speedsliders value reaches 1, the currentspeedstate will be 'normal' (wich means its 1)
                {
                    currentSpeedState = "Normal";
                }
                else if (speedSlider.value == 1.5f) //Once the speedsliders value reaches 1.5, the currentspeedstate will be 'fast' (wich means its 1.5)
                {
                    currentSpeedState = "Fast";
                }
                else if (speedSlider.value == 2f)   //Once the speedsliders value reaches 2, the currentspeedstate will be 'Max. Speed' (wich means its 2 (THE MAX.))
                {
                    currentSpeedState = "Max. Speed";
                }

                //

               /// <summary>
               /// All the text labels will be changed to the correct values.
               /// </summary>
                healthText.text = "Health: " + healthSlider.value;              // Change the health label to the value of the health slider.
                ammoText.text = "Ammo: " + ammoSlider.value + " bullets";       // Change the ammo label to the value of the ammo slider (+ bullet text).
                angleText.text = "Viewing angle: " + angleSlider.value + "°";   // Change the angle label to the value of the angle slider (+ degrees).
                rangeText.text = "Vision Range: " + rangeSlider.value + "m";    // Change the range label to the value of the range slider (+ meters).
                speedText.text = "Movement Speed: " + currentSpeedState;        // Change the speed label to the current string that has been selected by the current speed slider value.
                accuracyText.text = "Accuracy: " + accuracySlider.value + "%";  // Change the accuracy label to the value of the accuracy slider (+ %).
            }
            #endregion

            /// <summary>
            /// The dutch version of the stats.
            /// </summary>
            #region DutchStats
            else if (AppStartBools.dutchLanguage) //If the current selected language (wich is selected in the settings of the main menu) is dutch.
            {
                if (speedSlider.value == 0)         //Once the speedsliders value reaches 0, the currentspeedstate will be 'stop' (wich means its 0 (THE MIN.))
                {
                    currentSpeedState = "Stop";
                }
                else if (speedSlider.value == 0.5f) //Once the speedsliders value reaches 0.5, the currentspeedstate will be 'slow' (wich means its 0.5)
                {
                    currentSpeedState = "Sloom";
                }
                else if (speedSlider.value == 1f)   //Once the speedsliders value reaches 1, the currentspeedstate will be 'normal' (wich means its 1)
                {
                    currentSpeedState = "Normaal";
                }
                else if (speedSlider.value == 1.5f) //Once the speedsliders value reaches 1.5, the currentspeedstate will be 'fast' (wich means its 1.5)
                {
                    currentSpeedState = "Snel";
                }
                else if (speedSlider.value == 2f)   //Once the speedsliders value reaches 2, the currentspeedstate will be 'Max. Speed' (wich means its 2 (THE MAX.))
                {
                    currentSpeedState = "Max. Snelheid";
                }

                //

                /// <summary>
                /// All the text labels will be changed to the correct values.
                /// </summary>
                healthText.text = "Gezondheid: " + healthSlider.value;               // Change the health label to the value of the health slider.
                ammoText.text = "Ammunitie: " + ammoSlider.value + " Kogels";        // Change the ammo label to the value of the ammo slider (+ bullet text).
                angleText.text = "Kijkhoek: " + angleSlider.value + "°";             // Change the angle label to the value of the angle slider (+ degrees).
                rangeText.text = "Afstand zicht: " + rangeSlider.value + "m";        // Change the range label to the value of the range slider (+ meters).
                speedText.text = "Snelheid: " + currentSpeedState;                   // Change the speed label to the current string that has been selected by the current speed slider value.
                accuracyText.text = "Nauwkeurigheid: " + accuracySlider.value + "%"; // Change the accuracy label to the value of the accuracy slider (+ %).
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Once the UI is changed it will link the current UI stats to the AI stats (Update).
        /// </summary>
        #region Link AI stats once changed
        public void UpdateCurrentAiStats()
        {
            ///<summary>
            ///If there is a current selected AI.
            /// </summary>
            if (CurrentAIStats != null) 
            {
                CurrentAIStats.Health = (int)healthSlider.value;        // The health stats of this current selected AI will be set to the new changed stats (as soon as you press confirm).
                CurrentAIStats.Ammo = (int)ammoSlider.value;            // The ammo stats of this current selected AI will be set to the new changed stats (as soon as you press confirm).
                CurrentAIStats.ViewingAngle = (int)angleSlider.value;   // The viewing angle stats of this current selected AI will be set to the new changed stats (as soon as you press confirm).
                CurrentAIStats.VisionRange = (int)rangeSlider.value;    // The vision range stats of this current selected AI will be set to the new changed stats (as soon as you press confirm).
                CurrentAIStats.MovementSpeed = speedSlider.value;       // The movement speed stats of this current selected AI will be set to the new changed stats (as soon as you press confirm).
                CurrentAIStats.Accuracy = (int)accuracySlider.value;    // The accuracy stats of this current selected AI will be set to the new changed stats (as soon as you press confirm).
            }
        }
        #endregion

        /// <summary>
        /// Once the object is selected and the stats canvas appears, link the UI stats to the standard stats of the AI (Start).
        /// </summary>
        #region Link UI stats on start
        public void StartCurrentAIStats()
        {
            ///<summary>
            ///If there is a current selected AI.
            /// </summary>
            if (CurrentAIStats != null)
            {
                healthSlider.value = CurrentAIStats.Health;         // The health stats of this current selected AI will be set to the standard stats.
                ammoSlider.value = CurrentAIStats.Ammo;             // The ammo stats of this current selected AI will be set to the standard stats.
                angleSlider.value = CurrentAIStats.ViewingAngle;    // The viewing angle stats of this current selected AI will be set to the standard stats.
                rangeSlider.value = CurrentAIStats.VisionRange;     // The vision range stats of this current selected AI will be set to the standard stats.
                speedSlider.value = CurrentAIStats.MovementSpeed;   // The movement speed stats of this current selected AI will be set to the standard stats.
                accuracySlider.value = CurrentAIStats.Accuracy;     // The accuracy stats of this current selected AI will be set to the standard stats.
            }
        }
        #endregion

    }
}
