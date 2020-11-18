using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder : MonoBehaviour{
    public FirstController sceneController;
    public int score=0;

    void Start(){
        sceneController=(FirstController)SSDirector.getInstance().currentSceneController;
        sceneController.recorder=this;
    }
    public int GetScore(){
        return score;
    }
    public void AddScore(){
        score++;
    }
}

