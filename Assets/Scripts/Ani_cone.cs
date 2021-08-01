using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ani_cone : MonoBehaviour
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

    // public float ini_state = 1.0f;//small
    // public float MoveSpeed = 0.1f;

    // public void play_scale(){
    //     if(ini_state>=0f){
    //         if (Input.GetKey(KeyCode.W))
    //         {
    //             transform.localScale += new Vector3(Time.deltaTime*MoveSpeed, Time.deltaTime*MoveSpeed, 0);
    //             ini_state-=Time.deltaTime;
    //         }
            
    //     }
    //     if(ini_state<=1.0f){
    //         if (Input.GetKey(KeyCode.S))
    //         {
    //             transform.localScale -= new Vector3(Time.deltaTime*MoveSpeed, Time.deltaTime*MoveSpeed, 0);
    //             ini_state+=Time.deltaTime;
    //         }        
    //     }
    // }
}
