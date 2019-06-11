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
    public static ConsoleScript Instance;         //Static instance to call from other classes (AIBasics.cs, Arcontroller.cs etc.)

    [SerializeField]
    private Image consolePanel;                   //Image component of Consoleview gameobject to change color when selected
    [SerializeField]
    private ScrollRect consoleRect;               //Console Recttransform component of ConsoleView to enable - disable touch input
    [SerializeField]
    private RectTransform consoleRectTransform;   //Console Recttransform component of ConsoleView to change size when selected
    [SerializeField]
    private Transform feedbackParent;             //Transform parent of all feedback messages instantiated
    [SerializeField]
    private GameObject feedbackPrefab;            //Main Feedback prefab with ConsoleMessage.cs attached to it.
    [SerializeField]
    private int maxMessages;                      //Max messages instantiated in the pooling system
    [SerializeField]
    private bool consoleEnabled;                  //Bool to check either the console details is enabled or not.
    [SerializeField]
    private Vector2[] consoleScaling;             //Array of sizes of the console when selected [default 3,3 / 4,4]

    private List<ConsoleMessageScript> messageList = new List<ConsoleMessageScript>();

    private int currentMessage;                   //latest message of the object pool  
    private MessageType messageType;              //Link to enum to call the type of the message [Killmessage, Buildmessage etc.]

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < maxMessages; i++)
        {
            //Instantiate all messages at the start of the game and add them to the pool
            ConsoleMessageScript message = Instantiate(feedbackPrefab, feedbackParent).GetComponent<ConsoleMessageScript>();
            messageList.Add(message);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetFeedback(MessageType.BuildMessage, "test1");
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
        consoleEnabled = !consoleEnabled;
        consoleRect.enabled = consoleEnabled;

        //Show - Hide messages in console panel
        for (int i = 0; i < messageList.Count; i++)
        {
            if (!consoleEnabled) messageList[i].ToggleMessage(true);
            else messageList[i].ToggleMessage(false);
        }
        //Show - Hide console panel color
        if (consoleEnabled)
        {
            //Set new color to background of the console and change scale
            consolePanel.color = new Color(consolePanel.color.r, consolePanel.color.g, consolePanel.color.b, 255);
            consoleRectTransform.localScale = consoleScaling[1];
        }
        else
        {
            //Set new color to background of the console and change scale
            consolePanel.color = new Color(consolePanel.color.r, consolePanel.color.g, consolePanel.color.b, 0);
            consoleRectTransform.localScale = consoleScaling[0];
        }
    }
    //<summery> ToggleFeedback()
    //ToggleFeedback() void : Create and set a new message and enable it
    //</summery>
    private void ToggleFeedback(string message)
    {
        messageList[currentMessage].SetMessage(message);

        //Count the currentmessages up to check if it is lower than the list count
        currentMessage++;

        //If currentmessage count is highter than the list itself reset the number and list
        if (currentMessage > messageList.Count - 1)
        {
            currentMessage = 0;
        }
    }
}
