using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject{
    public bool enable=true;                     
    public bool destroy=false;                

    public GameObject gameobject;              
    public Transform transform;                    
    public ISSActionCallback callback;            

    protected SSAction(){ }

    public virtual void Start(){
        throw new System.NotImplementedException();
    }

    public virtual void Update(){
        throw new System.NotImplementedException();
    }

    public virtual void FixedUpdate(){
        throw new System.NotImplementedException();
    }
}
public class SSActionManager : MonoBehaviour{
    private Dictionary<int,SSAction> actions=new Dictionary<int,SSAction>();  
    private List<SSAction> waitingAdd=new List<SSAction>();                
    private List<int> waitingDelete=new List<int>();                                       

    protected void Update(){
        foreach (SSAction ac in waitingAdd){
            actions[ac.GetInstanceID()]=ac;
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int,SSAction> kv in actions){
            SSAction ac=kv.Value;
            if (ac.destroy){
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable){
                //ac.Update();
                ac.FixedUpdate();
            }
        }

        foreach (int key in waitingDelete){
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

}
public class GuardActionManager : SSActionManager,ISSActionCallback{
    private GuardPatrolAction patrol;
    private GameObject player;
    public void GuardPatrol(GameObject pa,GameObject pl){
        player=pl;
        patrol=GuardPatrolAction.GetSSAction(pa.transform.position);
        this.RunAction(pa,patrol,this);
    }

    public void SSActionEvent(
        SSAction source,SSActionEventType events=SSActionEventType.Competeted,
        int intParam=0,GameObject objectParam=null){
        if (intParam==0){//追逐
            FollowAction follow=FollowAction.GetSSAction(player);
            this.RunAction(objectParam,follow,this);
        } 
        else{//巡逻           
            GuardPatrolAction move=GuardPatrolAction.GetSSAction(objectParam.gameObject.GetComponent<Patrol>().start_position);
            this.RunAction(objectParam,move,this);
            Singleton<GameEventManager>.Instance.PlayerEscape();
        }
    }
}
public class FollowAction : SSAction{
    private GameObject player;        
    private Patrol patrol;
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec; // 平面移动向量
    private float speed;

    private FollowAction(){}

    public override void Start(){
        patrol=gameobject.GetComponent<Patrol>();
        anim=gameobject.GetComponent<Animator>();
        rigid=gameobject.GetComponent<Rigidbody>();
        speed=patrol.runSpeed;
        anim.SetFloat("forward",2.0f);
    }

    public static FollowAction GetSSAction(GameObject player){
        FollowAction action=CreateInstance<FollowAction>();
        action.player=player;
        return action;
    }

    public override void Update(){
        //保留供物理引擎调用
        planarVec=gameobject.transform.forward * speed;
    }

    public override void FixedUpdate(){
        transform.LookAt(player.transform.position);
        rigid.velocity=new Vector3(planarVec.x,rigid.velocity.y,planarVec.z);
        
        //如果玩家脱离该区域则继续巡逻
        if (patrol.playerSign!=patrol.sign){
            this.destroy=true;
            this.callback.SSActionEvent(this,SSActionEventType.Competeted,1,this.gameobject);
        }
    }
}
public class GuardPatrolAction : SSAction{
    private enum Dirction{EAST,NORTH,WEST,SOUTH};
    private float pos_x,pos_z;                 
    private float move_length;                 
    private bool move_sign=true;              
    private Dirction dirction=Dirction.EAST;  

    private Patrol patrol;
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec; // 平面移动向量
    private GuardPatrolAction(){}

    public override void Start(){
        patrol=gameobject.GetComponent<Patrol>();
        anim=gameobject.GetComponent<Animator>();
        rigid=gameobject.GetComponent<Rigidbody>();
        anim.SetFloat("forward",1.0f);
    }
    public static GuardPatrolAction GetSSAction(Vector3 location){
        GuardPatrolAction action=CreateInstance<GuardPatrolAction>();
        action.pos_x=location.x;
        action.pos_z=location.z;
        //设定移动矩形的边长
        action.move_length=Random.Range(5,6);
        return action;
    }

    public override void Update(){
        //保留供物理引擎调用
        planarVec=gameobject.transform.forward * patrol.walkSpeed;
    }

    public override void FixedUpdate(){
        //巡逻
        Gopatrol();
        //玩家进入该区域，巡逻结束，开始追逐
        if (patrol.playerSign==patrol.sign){
            this.destroy=true;
            this.callback.SSActionEvent(this,SSActionEventType.Competeted,0,this.gameobject);
        }
    }

    void Gopatrol(){
        if (move_sign){
            //不需要转向则设定一个目的地，按照矩形移动
            switch (dirction){
                case Dirction.EAST:
                    pos_x-=move_length;
                    break;
                case Dirction.NORTH:
                    pos_z+=move_length;
                    break;
                case Dirction.WEST:
                    pos_x+=move_length;
                    break;
                case Dirction.SOUTH:
                    pos_z-=move_length;
                    break;
            }
            move_sign=false;
        }
        this.transform.LookAt(new Vector3(pos_x,0,pos_z));
        float distance=Vector3.Distance(transform.position,new Vector3(pos_x,0,pos_z));

        if (distance>0.9){
            rigid.velocity=new Vector3(planarVec.x,rigid.velocity.y,planarVec.z);
        } 
        else{
            dirction=dirction+1;
            if(dirction>Dirction.SOUTH){
                dirction=Dirction.EAST;
            }
            move_sign=true;
        }
    }
}