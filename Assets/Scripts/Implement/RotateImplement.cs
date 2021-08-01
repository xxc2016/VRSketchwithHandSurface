// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImplement : PlayRotate
{
    float ini_state = 90.0f;//smallslef
    float MoveSpeed = 30.0f;
    Transform transform;
    public RotateImplement(Transform trans){
        this.transform=trans;
    }
    
    public void play_rotate(){
        if(ini_state>=0f){
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // Debug.Log(transform);
                transform.Rotate(new Vector3 (-1,0,0)* Time.deltaTime * MoveSpeed, Space.Self);
                ini_state-=Time.deltaTime* MoveSpeed;
            }
            
        }
        if(ini_state<=90.0f){
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(new Vector3 (1,0,0)* Time.deltaTime * MoveSpeed, Space.Self);
                ini_state+=Time.deltaTime* MoveSpeed;
            }        
        }
    }
}
