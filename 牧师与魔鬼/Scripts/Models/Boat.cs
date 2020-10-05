using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boat{
    public GameObject boat;               
    public Role[] roles;                  
    public bool isRight;                    
    public Vector3[] leftPos;
    public Vector3[] rightPos;
    public Click click;
    public Move move;
    public Boat(){
        roles=new Role[2];
        boat=Object.Instantiate(Resources.Load<GameObject>("Prefabs/Boat"),new Vector3(3.5f,0.5f,0),Quaternion.identity) as GameObject;
        boat.name="boat";
        rightPos=new Vector3[]{new Vector3(3,1,0),new Vector3(4,1,0)};
        leftPos=new Vector3[]{new Vector3(-4,1,0),new Vector3(-3,1,0)};
        move=boat.AddComponent(typeof(Move)) as Move;
        click=boat.AddComponent(typeof(Click)) as Click;
        click.boat=this;
        isRight=true;
    }
}