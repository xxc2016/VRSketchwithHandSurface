using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ani_base
{
    Transform trans;
    PlayScale scale;
    PlayRotate rotate;
    PlayTransform transform;
    // public float ini_state = 1.0f;//small
    // public float MoveSpeed = 0.1f;
    public Ani_base(Transform trans){
        this.trans=trans;
        scale = new ScaleImplementForQuad(trans);
        rotate = new RotateImplement(trans);
        transform = new TransformImplement(trans);
    }
    public void play()
    {
        scale.play_scale();
        rotate.play_rotate();
        transform.play_transform();
    }
    
    public void setPlayScale(PlayScale scale){
        this.scale = scale;
    }
    public void setPlayRotate(PlayRotate rotate){
        this.rotate = rotate;
    }
    public void setPlayTransform(PlayTransform transform){
        this.transform = transform;
    }

}
