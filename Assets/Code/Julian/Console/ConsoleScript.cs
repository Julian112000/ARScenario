using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageType
{
    KillMessage,
    BuildMessage,
    TargetMessage,
    ShotMessage,
    SelectMessage
};
public class ConsoleScript : MonoBehaviour
{
    public static ConsoleScript Instance;

    [SerializeField]
    private Image m_ConsolePanel;
    [SerializeField]
    private ScrollRect m_ConsoleRect;
    [SerializeField]
    private RectTransform m_ConsoleRectTransform;
    [SerializeField]
    private Transform m_FeedbackParent;
    [SerializeField]
    private GameObject m_FeedbackPrefab;
    [SerializeField]
    private int m_MaxMessages;
    [SerializeField]
    private bool m_ConsoleEnabled;
    [SerializeField]
    private Vector2[] m_ConsoleScaling;

    private List<ConsoleMessageScript> m_MessageList = new List<ConsoleMessageScript>();
    private int m_CurrentMessage;
    private MessageType m_MessageType;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < m_MaxMessages; i++)
        {
            ConsoleMessageScript message = Instantiate(m_FeedbackPrefab, m_FeedbackParent).GetComponent<ConsoleMessageScript>();
            m_MessageList.Add(message);
        }
    }
    public void SetFeedback(MessageType messagetype, string name1, string name2)
    {
        switch (messagetype)
        {
            case MessageType.KillMessage:
                ToggleFeedback(name1 + " [killed] " + name2);
                break;
            case MessageType.TargetMessage:
                ToggleFeedback(name1 + " [targeting] " + name2);
                break;
            case MessageType.ShotMessage:
                ToggleFeedback(name1 + " [fired to] " + name2);
                break;
        }
    }
    public void SetFeedback(MessageType messagetype, string name1)
    {
        switch (messagetype)
        {
            case MessageType.BuildMessage:
                ToggleFeedback("[placed] " + name1);
                break;  
            case MessageType.SelectMessage:
                ToggleFeedback("[selected] " + name1);
                break;
        }
    }
    public void ToggleConsole()
    {
        m_ConsoleEnabled = !m_ConsoleEnabled;
        m_ConsoleRect.enabled = m_ConsoleEnabled;

        //Show - Hide messages in console panel
        for (int i = 0; i < m_MessageList.Count; i++)
        {
            if (m_MessageList[i].m_IsEnabled)
            {
                m_MessageList[i].ToggleMessage(m_ConsoleEnabled);
            }
        }
        //Show - Hide console panel color
        if (m_ConsoleEnabled)
        {
            m_ConsolePanel.color = new Color(m_ConsolePanel.color.r, m_ConsolePanel.color.g, m_ConsolePanel.color.b, 255);
            m_ConsoleRectTransform.localScale = m_ConsoleScaling[1];
        }
        else
        {
            m_ConsolePanel.color = new Color(m_ConsolePanel.color.r, m_ConsolePanel.color.g, m_ConsolePanel.color.b, 0);
            m_ConsoleRectTransform.localScale = m_ConsoleScaling[0];
        }
    }
    private void ToggleFeedback(string message)
    {
        m_MessageList[m_CurrentMessage].SetMessage(message);
        m_CurrentMessage++;

        if (m_CurrentMessage > m_MessageList.Count - 1)
        {
            m_CurrentMessage = 0;
        }
    }
}
