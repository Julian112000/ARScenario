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

    public bool m_IsEnabled;

    public void SetMessage(string text)
    {
        gameObject.SetActive(true);
        m_Text.text = text;
        m_Animator.SetBool("In", true);
        m_IsEnabled = true;
    }
    public void ShowMessage()
    {
        m_Animator.enabled = false;
        GetComponent<CanvasGroup>().alpha = 1;
    }
    public void HideMessage()
    {
        m_Animator.enabled = true;
    }
}
