using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ani_arcquad : MonoBehaviour
{
    // Start is called before the first frame update
    Ani_base animator;
    void Start()
    {
        animator = new Ani_base(transform);
    }

    // Update is called once per frame
    void Update()
    {
        animator.play();
    }

}
