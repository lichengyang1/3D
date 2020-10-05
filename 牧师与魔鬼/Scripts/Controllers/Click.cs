using UnityEngine.SceneManagement;
using UnityEngine;

public class Click : MonoBehaviour{
    IUserAction action;
    public Role role=null;
    public Boat boat=null;
    public static int state=0;
    void Start(){
        action=SSDirector.getInstance().currentSceneController as IUserAction;
    }
    void OnMouseDown(){
        if(boat==null&&role==null)
            return;
        if(boat!=null&&state==0){       
            state=1;
            action.MoveBoat(); 
        } 
        else if(role!=null&&state==0){
            state=1;            
            action.MoveRole(role); 
        }    
    }
}