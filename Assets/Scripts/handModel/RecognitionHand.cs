using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognitionHand
{
    public enum GestureType
    {
        zhui,qiu,zhu,qu,ping,none
    }

    public static GestureType recognizeHand(double ti,double to,double io,double tp,double ip,double op){
        if(tp<=73){
            if(to<=94){
                if(io<=41){
                    return GestureType.zhui;
                }else{
                    return GestureType.qiu;
                }
            }else{
                return GestureType.qu;
            }
        }else{
            if(op<=62){
                return GestureType.zhu;
            }else{
                return GestureType.ping;
            }
        }
    }
    public static GestureType recognizeHand(List<string> list){
        double ti=double.Parse(list[0]), to=double.Parse(list[1]), io=double.Parse(list[2]), tp=double.Parse(list[3]), ip=double.Parse(list[4]), op=double.Parse(list[5]);

        //Debug.Log(ti+" "+to+" "+io+" "+tp+" "+ip+" "+op);
        
        if(tp<=65){
            if(to<=94){
                if(io<=42){
                    return GestureType.zhu;
                }else{
                    return GestureType.zhui;
                }
            }else{
                return GestureType.qiu;
            }
        }else{
            if(op<=63){
                return GestureType.qu;
            }else{
                if(to<=95){
                    return GestureType.ping;
                }else{
                    return GestureType.qiu;
                }
            }
        }
    }
}
