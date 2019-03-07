using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleMessageScript : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    public void SetMessage(string text)
    {
        gameObject.SetActive(true);
        m_Text.text = text;
    }
}
