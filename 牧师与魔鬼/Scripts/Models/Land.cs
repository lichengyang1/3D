using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land{
    public GameObject land;                   
    public Vector3[] positions;
    public bool isRight;
    public Role[] roles=new Role[6];
    public Land(string name,int type){
        int t=(type==1?1:-1);
        positions=new Vector3[]{
            new Vector3(6.35F*t,2.0F,0),new Vector3(7.35f*t,2.0F,0),new Vector3(8.35f*t,2.0F,0),
            new Vector3(9.35f*t,2.0F,0),new Vector3(10.35f*t,2.0F,0),new Vector3(11.35f*t,2.0F,0)
        };
        land=Object.Instantiate(Resources.Load<GameObject>("Prefabs/Land"),new Vector3(9*t,1,0),Quaternion.identity) as GameObject;
        land.name=name;
        isRight=type==1?true:false;
       
    }
}