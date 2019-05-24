using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class SelfMadeButton : MonoBehaviour
{
    public UnityEvent myEvent;

    public void ButtonClicked()
    {
        myEvent.Invoke();
    }
}

