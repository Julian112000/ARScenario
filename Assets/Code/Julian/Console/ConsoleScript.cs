using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageType
{
    KillMessage,
    BuildMessage,
    TargetMessage,
    ShotMessage
};
public class ConsoleScript : MonoBehaviour
{
    public static ConsoleScript Instance;

    [SerializeField]
    private GameObject m_ConsolePanel;
    [SerializeField]
    private Transform m_FeedbackParent;
    [SerializeField]
    private GameObject m_FeedbackPrefab;
    [SerializeField]
    private int m_MaxMessages;

    private List<ConsoleMessageScript> m_MessageList = new List<ConsoleMessageScript>();
    private int m_CurrentMessage;
    private bool m_ConsoleEnabled;
    private MessageType m_MessageType;

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
                ToggleFeedback(name1 + " killed " + name2);
                break;
            case MessageType.TargetMessage:
                ToggleFeedback(name1 + " targeted " + name2);
                break;
            case MessageType.ShotMessage:
                ToggleFeedback(name1 + " fired to " + name2);
                break;
        }
    }
    public void SetFeedback(MessageType messagetype, string name1)
    {
        switch (messagetype)
        {
            case MessageType.BuildMessage:
                ToggleFeedback("placed " + name1);
                break;
        }
    }
    public void ToggleConsole()
    {
        m_ConsoleEnabled = !m_ConsoleEnabled;
        m_ConsolePanel.SetActive(m_ConsoleEnabled);
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
