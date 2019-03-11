using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleMessageScript : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private CanvasGroup m_CanvasAlpha;

    public bool m_IsEnabled;

    public void SetMessage(string text)
    {
        gameObject.SetActive(true);
        m_Text.text = text;
        m_Animator.SetBool("In", true);
    }
    public void Delete()
    {
        m_IsEnabled = true;
    }
    public void ToggleMessage(bool toggle)
    {
        if (toggle)
        {
            m_Animator.enabled = false;
            m_CanvasAlpha.alpha = 1;
        }
        else
        {
            m_Animator.enabled = true;
        }
    }
}
