using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformImplement : PlayTransform
{
    float ini_state = 1.0f;//small
    float MoveSpeed = 0.1f;
    Transform transform;
    public TransformImplement(Transform trans){
        transform=trans;
    }
    public void play_transform(){
        if(ini_state>=0f){
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3 (-1,0,0)* Time.deltaTime * MoveSpeed, Space.World);
                ini_state-=Time.deltaTime;
            }
            
        }
        if(ini_state<=1.0f){
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3 (1,0,0)* Time.deltaTime * MoveSpeed, Space.World);
                ini_state+=Time.deltaTime;
            }        
        }
    }
}
