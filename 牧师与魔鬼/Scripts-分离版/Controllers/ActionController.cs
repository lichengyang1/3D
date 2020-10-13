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

public class SSMoveToAction:SSAction{
    public Vector3 target; 
    public float speed;  

    private SSMoveToAction(){}
    public static SSMoveToAction GetSSAction(Vector3 _target,float _speed){
        SSMoveToAction action=ScriptableObject.CreateInstance<SSMoveToAction>();
        action.target=_target;
        action.speed=_speed;
        return action;
    }
    public override void Start(){}

    public override void Update(){
        this.transform.position=Vector3.MoveTowards(this.transform.position,target,speed*Time.deltaTime);
        if(this.transform.position==target){
            this.destroy=true;
            this.callback.SSActionEvent(this);      
        }
    }
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

public class SceneActionManager:SSActionManager{
    public FirstController sceneController;
    private SequenceAction boatMove;
    private SequenceAction roleMove;

    protected void Start(){
        sceneController=(FirstController)SSDirector.getInstance().currentSceneController;
        sceneController.actionManager=this;
    }

    protected new void Update(){
        base.Update();
    }

    public void moveBoat(GameObject boat,Vector3 destination,float speed){
        SSAction action1=SSMoveToAction.GetSSAction(destination,speed);
        boatMove=SequenceAction.GetSSAcition(0,0,new List<SSAction>{action1 });
        this.RunAction(boat,boatMove,this);
        Click.state=0;//移动完成后复原state
    }

    public void moveRole(GameObject role,Vector3 mid,Vector3 destination,float speed){
        SSAction action1=SSMoveToAction.GetSSAction(mid,speed);
        SSAction action2=SSMoveToAction.GetSSAction(destination,speed);

        roleMove=SequenceAction.GetSSAcition(1,0,new List<SSAction>{action1,action2 });
        this.RunAction(role,roleMove,this);
        Click.state=0;//移动完成后复原state
    }
}