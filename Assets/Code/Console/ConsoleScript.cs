using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageType
{
    KillMessage,    //[Killed]
    BuildMessage,   //[Placed]
    TargetMessage,  //[Spotted]
    ShotMessage,    //[Fired to]
    SelectMessage   //[Selected]
};
public class ConsoleScript : MonoBehaviour
{
    public static ConsoleScript Instance;           //Static instance to call from other classes (AIBasics.cs, Arcontroller.cs etc.)

    [SerializeField]
    private Image m_ConsolePanel;                   //Image component of Consoleview gameobject to change color when selected
    [SerializeField]
    private ScrollRect m_ConsoleRect;               //Console Recttransform component of ConsoleView to enable - disable touch input
    [SerializeField]
    private RectTransform m_ConsoleRectTransform;   //Console Recttransform component of ConsoleView to change size when selected
    [SerializeField]
    private Transform m_FeedbackParent;             //Transform parent of all feedback messages instantiated
    [SerializeField]
    private GameObject m_FeedbackPrefab;            //Main Feedback prefab with ConsoleMessage.cs attached to it.
    [SerializeField]
    private int m_MaxMessages;                      //Max messages instantiated in the pooling system
    [SerializeField]
    private bool m_ConsoleEnabled;                  //Bool to check either the console details is enabled or not.
    [SerializeField]
    private Vector2[] m_ConsoleScaling;             //Array of sizes of the console when selected [default 3,3 / 4,4]

    private List<ConsoleMessageScript> m_MessageList = new List<ConsoleMessageScript>();

    private int m_CurrentMessage;                   //latest message of the object pool  
    private MessageType m_MessageType;              //Link to enum to call the type of the message [Killmessage, Buildmessage etc.]

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < m_MaxMessages; i++)
        {
            //Instantiate all messages at the start of the game and add them to the pool
            ConsoleMessageScript message = Instantiate(m_FeedbackPrefab, m_FeedbackParent).GetComponent<ConsoleMessageScript>();
            m_MessageList.Add(message);
        }
    }
    //<summery> SetFeedback()
    //SetFeedback() void to create a new message and add them to the console
    //messageType : type of the message
    //name1 : first name in the message
    //name2 : last name in the message
    //Order Example:: "[name1] + [messagetype] + [name2]"
    //</summery>
    public void SetFeedback(MessageType messagetype, string name1, string name2)
    {
        switch (messagetype)
        {
            case MessageType.KillMessage:
                ToggleFeedback(name1 + " [killed] " + name2);
                break;
            case MessageType.TargetMessage:
                ToggleFeedback(name1 + " [spotted] " + name2);
                break;
            case MessageType.ShotMessage:
                ToggleFeedback(name1 + " [fired to] " + name2);
                break;
        }
    }
    //<summery> SetFeedback()
    //SetFeedback() void to create a new message and add them to the console
    //messageType : type of the message
    //name1 : first name in the message
    //Order Example:: "[messagetype] + [name1]"
    //</summery>
    public void SetFeedback(MessageType messagetype, string name1)
    {
        switch (messagetype)
        {
            case MessageType.BuildMessage:
                ToggleFeedback(" [placed] " + name1);
                break;  
            case MessageType.SelectMessage:
                ToggleFeedback(" [selected] " + name1);
                break;
        }
    }
    //<summery> ToggleConsole()
    //ToggleConsole() void is to toggle the console on and off when highlighting the console
    //</summery>
    public void ToggleConsole()
    {
        //Enable console if console isn't opened - disable console if console is opened
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
            //Set new color to background of the console and change scale
            m_ConsolePanel.color = new Color(m_ConsolePanel.color.r, m_ConsolePanel.color.g, m_ConsolePanel.color.b, 255);
            m_ConsoleRectTransform.localScale = m_ConsoleScaling[1];
        }
        else
        {
            //Set new color to background of the console and change scale
            m_ConsolePanel.color = new Color(m_ConsolePanel.color.r, m_ConsolePanel.color.g, m_ConsolePanel.color.b, 0);
            m_ConsoleRectTransform.localScale = m_ConsoleScaling[0];
        }
    }
    //<summery> ToggleFeedback()
    //ToggleFeedback() void : Create and set a new message and enable it
    //</summery>
    private void ToggleFeedback(string message)
    {
        m_MessageList[m_CurrentMessage].SetMessage(message);

        //Count the currentmessages up to check if it is lower than the list count
        m_CurrentMessage++;

        //If currentmessage count is highter than the list itself reset the number and list
        if (m_CurrentMessage > m_MessageList.Count - 1)
        {
            m_CurrentMessage = 0;
        }
    }
}
