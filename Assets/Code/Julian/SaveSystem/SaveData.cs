using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField]
    private int m_ID;

    public int GetID()
    {
        return m_ID;
    }
}
