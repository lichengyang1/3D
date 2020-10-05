using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class River{
    private GameObject river;              
    public River(){
        river=Object.Instantiate(Resources.Load("Prefabs/river",typeof(GameObject))) as GameObject;
        river.name="river";
        river.transform.position=Vector3.zero;
    }
}