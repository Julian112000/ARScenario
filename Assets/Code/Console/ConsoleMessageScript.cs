using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleMessageScript : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;                    //Text component of the message prefab
    [SerializeField]
    private Animator m_Animator;            //Animator component of the message prefab
    [SerializeField]
    private CanvasGroup m_CanvasAlpha;      //Canvasgroup to handle the fade out animation

    public bool m_IsEnabled;

    //Set text and animations from ConsoleScript.cs script
    public void SetMessage(string text)
    {
        gameObject.SetActive(true);
        m_Text.text = text;
        m_Animator.SetBool("In", true);
    }
    //Delete message to call fade out
    public void Delete()
    {
        m_IsEnabled = true;
    }
    //Enable - Disable the animation of the message prefab.
    public void ToggleMessage(bool toggle)
    {
        //Disbale - enable animator and reset alpha fade of canvasalpha
        m_Animator.enabled = toggle;

        if (toggle) m_CanvasAlpha.alpha = 1;
    }
}
