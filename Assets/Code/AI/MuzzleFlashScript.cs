using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashScript : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> particles;
    public void Flash()
    {
        if(particles.Count > 0)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Play();
            }
        }
    }
}
