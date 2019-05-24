using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGBulletScript : MonoBehaviour
{
    public bool CanFly = false;
    public Vector3 Startpos;
    public Quaternion StartRotation;
    public Transform starter;
    public AIBasics Target;
    public ParticleSystem FlyParticle;
    [SerializeField]
    private AIBasics RootBasic;

    private void Awake()
    {
        Startpos = this.transform.localPosition;
        StartRotation = this.transform.localRotation;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (CanFly)
        {
            Fly();
        }
    }

    public void Reset()
    {
        transform.localPosition = Startpos;
        transform.localRotation = StartRotation;
        CanFly = false;
        FlyParticle.Stop();
        RootBasic.RPGDoDamage();
    }
    private void Fly()
    {
        FlyParticle.Play();
        transform.LookAt(Target.transform);
        transform.position += transform.forward * 1f;
        if(Vector3.Distance(transform.position,Target.transform.position) < 1)
        {
            Reset();
            Target.PlayExplosionImpact();
        }
    }
}
