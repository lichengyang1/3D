using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstController : MonoBehaviour, IUserAction, ISceneController{
    public PatrolFactory patrol_factory;                        
    public ScoreRecorder recorder;                                  
    public GuardActionManager action_manager;                     
    public int playerSign=-1;                                 
    public GameObject player;                                
    public UserGUI gui;                                     

    private List<GameObject> patrols;                         
    private bool game_over=false;                           

    
    void Awake(){
        SSDirector director=SSDirector.getInstance();
        director.currentSceneController=this;
        patrol_factory=Singleton<PatrolFactory>.Instance;
        action_manager=gameObject.AddComponent<GuardActionManager>() as GuardActionManager;
        gui=gameObject.AddComponent<UserGUI>() as UserGUI;
        LoadResources();
        recorder=Singleton<ScoreRecorder>.Instance;
    }

    void Update(){
        for (int i=0;i<patrols.Count;i++){
            patrols[i].gameObject.GetComponent<Patrol>().playerSign=playerSign;
        }
    }


    public void LoadResources(){
        Instantiate(Resources.Load<GameObject>("Prefabs/Plane"));
        player=Instantiate(
            Resources.Load("Prefabs/Player"), 
            new Vector3(13,0,-13), Quaternion.identity) as GameObject;
        patrols=patrol_factory.GetPatrols();

        for (int i=0;i<patrols.Count;i++){
            action_manager.GuardPatrol(patrols[i], player);
        }
    }

    public int GetScore(){
        return recorder.GetScore();
    }

    public bool GetGameover(){
        return game_over;
    }

    public void Restart(){
        SceneManager.LoadScene("Scenes/mySence");
    }

    void OnEnable(){
        GameEventManager.ScoreChange+=AddScore;
        GameEventManager.GameoverChange+=Gameover;
    }
    void OnDisable(){
        GameEventManager.ScoreChange-=AddScore;
        GameEventManager.GameoverChange-=Gameover;
    }

    void AddScore(){
        recorder.AddScore();
    }

    void Gameover(){
        game_over=true;
    }
}
