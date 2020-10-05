using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role{ 
    public GameObject role;     
    public bool isPriest;        
    public bool isRight;            
    public bool isInBoat;             
    public Click click;
    public Move move;
    public Role(Vector3 position,bool isPriest){
        this.isPriest=isPriest;
        isRight=true;
        isInBoat=false;
        role=Object.Instantiate(Resources.Load<GameObject>("Prefabs/"+(isPriest?"priest":"devil")),position,Quaternion.Euler(0,-90,0)) as GameObject;
        click=role.AddComponent(typeof(Click)) as Click;
        move=role.AddComponent(typeof(Move)) as Move;
        click.role=this;
    }
}