using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanningDotsScript : MonoBehaviour
{
    [SerializeField]
    private Text dots;

    private float waitTime = 0.5f;

    private bool scanning = true;

    void Start()
    {
        StartCoroutine(DotAnimation());
    }

    IEnumerator DotAnimation()
    {
        while (scanning)
        {
            dots.text = ".";
            yield return new WaitForSeconds(waitTime);
            dots.text = "..";
            yield return new WaitForSeconds(waitTime);
            dots.text = "...";
            yield return new WaitForSeconds(waitTime);
            dots.text = "";
            yield return new WaitForSeconds(waitTime);
        }
    }
}
