using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction:ScriptableObject{
    public bool enable=true;
    public bool destroy=false;
    public GameObject gameobject;
    public Transform transform;
    public ISSActionCallback callback;

    protected SSAction(){}                      

    public virtual void Start(){
        throw new System.NotImplementedException();
    }

    public virtual void Update(){
        throw new System.NotImplementedException();
    }
}

public class FlyAction : SSAction{
    public float gravity=-2;                            
    private Vector3 start;                      
    private Vector3 acceleration=Vector3.zero;         
    private Vector3 angle=Vector3.zero;          
    private float time;                                  

    private FlyAction(){ }
    public static FlyAction GetSSAction(int lor,float angle,float power){
        FlyAction action=CreateInstance<FlyAction>();
        if (lor==-1){
            action.start=Quaternion.Euler(new Vector3(0,0,-angle))*Vector3.left*power;
        }
        else{
            action.start=Quaternion.Euler(new Vector3(0,0,angle))*Vector3.right*power;
        }
        return action;
    }

    public override void Update(){
        time+=Time.fixedDeltaTime;
        acceleration.y=gravity*time;

        transform.position+=(start+acceleration)*Time.fixedDeltaTime;
        angle.z=Mathf.Atan((start.y+acceleration.y)/start.x)*Mathf.Rad2Deg;
        transform.eulerAngles=angle;

        if (this.transform.position.y<-10){
            this.destroy=true;
            this.callback.SSActionEvent(this);     
        }
    }

    public override void Start(){ }
}

public class SequenceAction:SSAction,ISSActionCallback{
    public List<SSAction> sequence;
    public int repeat=-1;
    public int start=0;

    public static SequenceAction GetSSAcition(int repeat,int start,List<SSAction> sequence){
        SequenceAction action=ScriptableObject.CreateInstance<SequenceAction>();
        action.sequence=sequence;
        action.repeat=repeat;
        action.start=start;
        return action;
    }

    public override void Start(){
        foreach(SSAction action in sequence){
            action.gameobject=this.gameobject;
            action.transform=this.transform;
            action.callback=this;
            action.Start();
        }
    }

    public override void Update(){
        if(sequence.Count==0) 
            return;
        if(start<sequence.Count)
            sequence[start].Update();
    }
    public void SSActionEvent(SSAction source,SSActionEventType events=SSActionEventType.Competeted,int intParam=0,string strParam=null,Object objectParam=null){
        source.destroy=false;                   
        this.start++;                             
        if(this.start>=sequence.Count){
            this.start=0;
            if(repeat>0) 
                repeat--;            
            if(repeat==0){                     
                this.destroy=true;               
                this.callback.SSActionEvent(this); 
            }
        }
    }
    void OnDestroy(){}
}

public class SSActionManager:MonoBehaviour,ISSActionCallback{
    private Dictionary<int,SSAction> actions=new Dictionary<int,SSAction>();
    private List<SSAction> waitingAdd=new List<SSAction>();
    private List<int> waitingDelete=new List<int>();            

    protected void Update(){
        foreach(SSAction ac in waitingAdd)
            actions[ac.GetInstanceID()]=ac;                                      

        waitingAdd.Clear();

        foreach(KeyValuePair<int,SSAction> kv in actions){
            SSAction ac=kv.Value;
            if(ac.destroy)
                waitingDelete.Add(ac.GetInstanceID());
            else if(ac.enable)
                ac.Update();
        }
        foreach(int key in waitingDelete){
            SSAction ac=actions[key];
            actions.Remove(key);
            Object.Destroy(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject,SSAction action,ISSActionCallback manager){
        action.gameobject=gameobject;
        action.transform=gameobject.transform;
        action.callback=manager;
        waitingAdd.Add(action);
        action.Start();
    }
    public void SSActionEvent(SSAction source,SSActionEventType events=SSActionEventType.Competeted,int intParam=0,string strParam=null,Object objectParam=null){}
}

public class FlyActionManager : SSActionManager{
    public FlyAction fly; 
    public FirstController sceneController;          

    protected void Start(){
        sceneController=(FirstController)SSDirector.getInstance().currentSceneController;
        sceneController.actionManager=this;    
    }

    public void Fly(GameObject skeet,float angle,float power){
        int lor=1;
        if (skeet.transform.position.x>0) 
            lor=-1;
        fly=FlyAction.GetSSAction(lor,angle,power);
        this.RunAction(skeet,fly,this);
    }
}