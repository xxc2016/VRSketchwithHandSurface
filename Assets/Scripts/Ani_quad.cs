using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ani_quad : MonoBehaviour
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

    // public override void play_scale(){
    //     if(base.ini_state>=0f){
    //         if (Input.GetKey(KeyCode.W))
    //         {
    //             transform.localScale += new Vector3(Time.deltaTime*base.MoveSpeed, Time.deltaTime*MoveSpeed, 0);
    //             base.ini_state-=Time.deltaTime;
    //         }
            
    //     }
    //     if(base.ini_state<=1.0f){
    //         if (Input.GetKey(KeyCode.S))
    //         {
    //             transform.localScale -= new Vector3(Time.deltaTime*base.MoveSpeed, Time.deltaTime*MoveSpeed, 0);
    //             base.ini_state+=Time.deltaTime;
    //         }        
    //     }
    // }

}
