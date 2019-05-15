using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleMessageScript : MonoBehaviour
{
    [SerializeField]
    private Text Text;                    //Text component of the message prefab
    [SerializeField]
    private Animator animator;            //Animator component of the message prefab
    [SerializeField]
    private CanvasGroup canvasAlpha;      //Canvasgroup to handle the fade out animation

    public bool isEnabled;

    //Set text and animations from ConsoleScript.cs script
    public void SetMessage(string text)
    {
        gameObject.SetActive(true);
        Text.text = text;
        animator.SetBool("In", true);
    }
    //Delete message to call fade out
    public void Delete()
    {
        isEnabled = true;
    }
    //Enable - Disable the animation of the message prefab.
    public void ToggleMessage(bool toggle)
    {
        //Disbale - enable animator and reset alpha fade of canvasalpha
        animator.enabled = toggle;

        if (toggle) canvasAlpha.alpha = 1;
    }
}
