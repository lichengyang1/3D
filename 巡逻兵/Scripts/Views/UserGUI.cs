using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour{
    private IUserAction action;
    private GUIStyle score=new GUIStyle();
    private GUIStyle text=new GUIStyle();
    private GUIStyle end=new GUIStyle();
    void Start (){
        action=SSDirector.getInstance().currentSceneController as IUserAction;
        text.fontSize=16;
        score.fontSize=16;
        end.fontSize=25;
    }

    private void OnGUI(){
        GUI.Label(new Rect(Screen.width-100,30,200,50),"分数:",text);
        GUI.Label(new Rect(Screen.width-60,30,200,50),action.GetScore().ToString(),score);
        GUI.Label(new Rect(Screen.width/2-260,30,100,100),"WASD移动，方向键移动视角，成功躲避巡逻兵追捕加1分",text);
        if(action.GetGameover()){
            GUI.Label(new Rect(Screen.width/2-50,Screen.width/2-150,100,100),"游戏结束",end);
            if(GUI.Button(new Rect(Screen.width/2-50,Screen.width/2-250,100,50),"重新开始")){
                action.Restart();
                return;
            }
        }  
    }
}
