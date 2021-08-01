using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleImplementForSphere : PlayScale
{
    float ini_state = 1.0f;//small
    float MoveSpeed = 0.1f;
    Transform transform;
    public ScaleImplementForSphere(Transform trans){
        transform=trans;
    }
    public void play_scale(){
        if(ini_state>=0f){
            if (Input.GetKey(KeyCode.W))
            {
                transform.localScale += new Vector3(Time.deltaTime*MoveSpeed, Time.deltaTime*MoveSpeed, Time.deltaTime*MoveSpeed);
                ini_state-=Time.deltaTime;
            }
            
        }
        if(ini_state<=1.0f){
            if (Input.GetKey(KeyCode.S))
            {
                transform.localScale -= new Vector3(Time.deltaTime*MoveSpeed, Time.deltaTime*MoveSpeed, Time.deltaTime*MoveSpeed);
                ini_state+=Time.deltaTime;
            }        
        }
    }
}
