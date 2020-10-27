using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour{
    private IUserAction action;
    GUIStyle text_style=new GUIStyle();
    private bool start=false;

    void Start (){
        action=SSDirector.getInstance().currentSceneController as IUserAction;
    }
	
	void OnGUI (){
        text_style.normal.textColor=new Color(0,0,0,1);
        text_style.fontSize=16;

        if (start){
            GUI.Label(new Rect(Screen.width-150,13,200,50),"分数:"+ action.GetScore().ToString(),text_style);
            GUI.Label(new Rect(100,13,50,50),"Round:"+action.GetRound().ToString(),text_style);
            GUI.Label(new Rect(180,13,50,50),"Trial:"+action.GetTrial().ToString(),text_style);

            if (action.GetRound()==3&&action.GetTrial()==10){
                GUI.Label(new Rect(Screen.width/2-40,Screen.height/2,100,100),"游戏结束",text_style);
                GUI.Label(new Rect(Screen.width/2-40,Screen.height/2-100,50,50),"你的分数:"+action.GetScore().ToString(),text_style);
                if (GUI.Button(new Rect(Screen.width/2-40,Screen.height/2-200,100,50),"重新开始")){
                    action.ReStart();
                    return;
                }
                action.GameOver();
            }
        }
        else{
            GUI.Label(new Rect(Screen.width/2-50,100,100,100),"打飞碟",text_style);
            if (GUI.Button(new Rect(Screen.width/2-50,200,100,50),"游戏开始")){
                start=true;
                action.ReStart();
            }
        }
    }
   
}