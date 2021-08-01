using System.Linq.Expressions;
// using System.Diagnostics;
// using System.Numerics;
using System.IO;
using System.Globalization;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    // Start is called before the first frame update
    //加载预设体资源
    public int state=0;
    private GameObject go=null;
    private Camera theCamera;
    private Transform tx;
    void Start()
    {
         if ( !theCamera )
        {
            theCamera = Camera.main;
        }
        tx = theCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            state+=1;
            if(state==6)
                state=0;
            // print(state);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            go=spawn(go);
        }
    }

    public GameObject spawn(GameObject go){
        if(go!=null)
            Destroy(go);
        Vector3 position = tx.position + tx.forward*0.5f + tx.up*(Random.Range(-0.5f,0.5f));
        Quaternion rotation = Quaternion.Euler(new Vector3(Random.Range(-90f,90f),Random.Range(-90f,90f),Random.Range(-90f,90f)));

        Debug.Log(position+" "+rotation);

        if(state==0){
            return (GameObject)Instantiate(Resources.Load("Prefabs/ArcQuad"),position,rotation);
        }else if(state==1){
            return (GameObject)Instantiate(Resources.Load("Prefabs/Cone"),position,rotation);
        }else if(state==2){
            // return (GameObject)Instantiate(Resources.Load("Prefabs/Cube"));
            return null;
        }else if(state==3){
            return (GameObject)Instantiate(Resources.Load("Prefabs/Cylinder"),position,rotation);
        }else if(state==4){
            return (GameObject)Instantiate(Resources.Load("Prefabs/Quad"),position,rotation);
        }else if(state==5){
            return (GameObject)Instantiate(Resources.Load("Prefabs/Sphere"),position,rotation);
        }else{
            return null;
        }
        
    }
}
