using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ani_sphere : MonoBehaviour
{
    Ani_base animator;
    void Start()
    {
        animator = new Ani_base(transform);
        animator.setPlayScale(new ScaleImplementForSphere(transform));
    }

    // Update is called once per frame
    void Update()
    {
        animator.play();
    }


}
