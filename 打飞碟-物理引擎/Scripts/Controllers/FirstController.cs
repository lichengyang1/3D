using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour,ISceneController,IUserAction{
    public FlyActionManager actionManager;
    public Factory factory;
    public UserGUI gui;
    public ScoreKeeper scoreKeeper;
    private int round=1;                                                 
    private int trial=0;                                        
    private bool state=false;
    private int count=0;

    void Start (){
        SSDirector director=SSDirector.getInstance();    
        director.currentSceneController=this;
        gameObject.AddComponent<Factory>();
        gameObject.AddComponent<ScoreKeeper>();
        factory=Singleton<Factory>.Instance;
        scoreKeeper=Singleton<ScoreKeeper>.Instance;
        actionManager=gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
        gui=gameObject.AddComponent<UserGUI>() as UserGUI;
    }

	void Update (){
        if(state){
            count++;
            if (Input.GetButtonDown("Fire1")){
                Vector3 pos=Input.mousePosition;
                Hit(pos);
            }
            if (count >= 150){

                if(round==1){
                    count=0;
                    launchSkeet(1);
                    trial+=1;
                }
                else if(round==2){
                        count=0;
                        if (trial%2==0) 
                            launchSkeet(1);
                        else 
                            launchSkeet(2);
                        trial+=1;
                }
                else if(round==3){
                        count=0;
                        if (trial%3==0) 
                            launchSkeet(1);
                        else if(trial%3==1) 
                            launchSkeet(2);
                        else 
                            launchSkeet(3);
                        trial+=1;
                }
                if (trial==10){
                    if(round==3)
                        state=false;
                    else{
                        round+=1;
                        trial=0;                       
                    }
                }                    
            

            }
            factory.DeleteSkeet();
        }
    }

    public void LoadResources(){
        factory.GetSkeet(round);
        factory.DeleteSkeet();
    }

    private void launchSkeet(int type){
        GameObject skeet=factory.GetSkeet(type);

        float y=0;
        float x=Random.Range(-1f,1f);
        if(x<0)
            x=-1;
        else    
            x=1;

        float power=0;
        float angle=0;
        if (type==1){
            y=Random.Range(1f,3f);
            power=Random.Range(5f,6f);
            angle=Random.Range(30f,40f);
        }
        else if (type==2){
            y=Random.Range(2f,3f);
            power=Random.Range(5f,8f);
            angle=Random.Range(25f,30f);
        }
        else{
            y=Random.Range(5f,6f);
            power=Random.Range(2f,4f);
            angle=Random.Range(15f,25f);
        }
        skeet.transform.position=new Vector3(x*16f,y,0);
    //   actionManager.Fly(skeet,angle,power);
        actionManager.Fly(skeet,power);
    }

    public void Hit(Vector3 pos){
        Ray ray=Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits;
        hits=Physics.RaycastAll(ray);
        for (int i=0;i<hits.Length;i++){
            RaycastHit hit=hits[i];
            if (hit.collider.gameObject.GetComponent<Skeet>()!=null){
                scoreKeeper.Record(hit.collider.gameObject);
                hit.collider.gameObject.transform.position=new Vector3(0,-10,0);
            }
        }
    }

    public float GetScore(){
        return scoreKeeper.GetScore();
    }
    public int GetRound(){
        return round;
    }
    public int GetTrial(){
        return trial;
    }

    public void ReStart(){
        state=true;
        scoreKeeper.Reset();
        factory.Reset();
        round=1;
        trial=1;
    }

    public void GameOver(){
        state=false;
    }
}