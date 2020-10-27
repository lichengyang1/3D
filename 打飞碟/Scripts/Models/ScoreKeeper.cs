using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour{
    private float score;
    void Start (){
        score=0;
    }
    public float GetScore(){
        return score;
    }
    public void Reset(){
        score=0;
    }
    public void Record(GameObject disk){
        score += disk.GetComponent<Skeet>().score;
    }    
}