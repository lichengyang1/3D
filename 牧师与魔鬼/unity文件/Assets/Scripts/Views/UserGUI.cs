using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour{
    private IUserAction action;
    public int sign=0;
    void Start(){
        action=SSDirector.getInstance().currentSceneController as IUserAction;
    }
    void OnGUI(){
        if(sign==1){
            GUI.Label(new Rect(Screen.width/2,Screen.height/2,100,50),"Gameover!");  
        }
        else if(sign==2){
            GUI.Label(new Rect(Screen.width/2,Screen.height/2,100,50),"You Win!");
        }
        if(GUI.Button(new Rect(Screen.width/2-40,Screen.height/2-120,100,50),"Restart")){
            action.Restart();
            sign=0;
        }
    }
}