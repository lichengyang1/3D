using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour{
    private List<Skeet> runList=new List<Skeet>();
    private List<Skeet> freeList=new List<Skeet>();

    public GameObject GetSkeet(int type){
        GameObject skeet=null;
        if (freeList.Count>0){
            for(int i=0;i<freeList.Count;i++){
                if (freeList[i].type==type){
                    skeet=freeList[i].gameObject;
                    freeList.Remove(freeList[i]);
                    break;
                }
            }     
        }
        
        if (skeet==null){
            string t=type.ToString();
            skeet=Instantiate(Resources.Load<GameObject>("Prefabs/skeet"+t),new Vector3(0,-10f,0),Quaternion.identity);
        }
        runList.Add(skeet.GetComponent<Skeet>());
        skeet.SetActive(true);
        return skeet;
    }

    public void DeleteSkeet(){
        for(int i=0;i<runList.Count;i++){
            if (runList[i].gameObject.transform.position.y<= -10f){
                freeList.Add(runList[i]);
                runList.Remove(runList[i]);
            }
        }          
    }

    public void Reset(){
        DeleteSkeet();
    }
}