namespace GoogleARCore.Examples.HelloAR
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif
    public class TestHumanScript : BasicAiScriptMarco
    {

        UnitType MyType;
        UnitStatus UnitStat;
        public static bool SetNew;
        public bool SetNewT, IsMouse;
        private Animator animator;
        private void Start()
        {
            MyType = UnitType.Infantery;

            if (animator = GetComponent<Animator>())
            {
                NoAmin = false;
            }
            else
            {
                NoAmin = true;
            }

        }

        public void SelectTarget()
        {
            IsSelecting = true;
        }

        public void SelectAnimatie(int AniNumber)
        {
            switch (AniNumber)
            {
                case 0:
                    UnitStat = UnitStatus.Idle;
                    break;
                case 1:
                    UnitStat = UnitStatus.Aiming;
                    break;
                default:
                    UnitStat = UnitStatus.Wounded;
                    break;
            }
        }

        public void PLayAnimation()
        {
            PlayAnimations = true;
            switch (UnitStat)
            {
                case UnitStatus.Aiming:
                    animator.SetBool("Aiming", true);
                    //MyAnimations.Blend("Aiming", 1.0f, 0.3f);

                    break;
                case UnitStatus.Idle:
                    animator.SetBool("Aiming", false);

                    break;
                default:
                    Debug.Log("No anim");
                    break;
            }
        }

    void TestDel()
    {
        Vector3 pos = transform.position;

        private void Update()
        {
            if (PlayAnimations)
            {
                PLayAnimation();
            }

            if (SetNewT && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                SelectedImg.SetActive(true);
                UnitStat = UnitStatus.Aiming;
                if (SetNewT && !IsMouse)
                {
                    //Aim(UnitStat, SetNewT, this.gameObject);
                    if (Input.touchCount > 0)
                    {
                        SetNewT = !Aim(UnitStat, SetNewT, this.gameObject);
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetNewT = !AimM(UnitStat, SetNewT, this.gameObject);
                    }
                }
                if (!NoAmin)
                {
                    PLayAnimation();
                }

            }
            if (!SetNewT)
            {
                SelectedImg.SetActive(false);
            }
        }
    }
}
