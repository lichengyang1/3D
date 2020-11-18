using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollide : MonoBehaviour{
    public int sign=0;
    private FirstController sceneController;

    private void Start(){
        sceneController=SSDirector.getInstance().currentSceneController as FirstController;
    }

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag=="Player"){
            sceneController.playerSign=sign;
        }
    }
}
