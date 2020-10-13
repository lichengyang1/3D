# 作业

## 1、基本操作演练【建议做】

* 下载 Fantasy Skybox FREE， 构建自己的游戏场景
  * 天空盒
    * 导入Fantasy Skybox后，创建skybox,在6个面中放入合适的贴图

        ![061RER.png](https://s1.ax1x.com/2020/10/10/061RER.png)

    * 然后在Camera中添加部件Skybox，然后拖入，这样该摄像头就使用天空盒

        [![061z28.png](https://s1.ax1x.com/2020/10/10/061z28.png)](https://imgchr.com/i/061z28)

    * 效果：

        ![06YilV.png](https://s1.ax1x.com/2020/10/10/06YilV.png)

  * 地形
    * 创建Terrain对象，然后在Terrain组件中使用各种工具进行绘制

        [![063uMF.png](https://s1.ax1x.com/2020/10/10/063uMF.png)](https://imgchr.com/i/063uMF)

    * 通过升高、平滑、铺地、种草等一系列操作，成果如下

        ![063dqH.png](https://s1.ax1x.com/2020/10/10/063dqH.png)

  * 玩家降临
    * 为了更好的观察游戏场景，在此实现玩家降临。先导入官方资源Standard Assets，使用预制ThirdPersonController，然后令Camera作为其子对象，并将Camera移至其头部。

        ![068iQO.png](https://s1.ax1x.com/2020/10/10/068iQO.png)

    *运行游戏

    [![068f1K.gif](https://s1.ax1x.com/2020/10/10/068f1K.gif)](https://imgchr.com/i/068f1K)

* 写一个简单的总结，总结游戏对象的使用
  * 游戏对象是unity3d的基础元素。它们的不同组合产生了各种各样的模型和预制，由此构成缤纷多彩的3D游戏世界。
    * Empty对象并没有实体，往往成为脚本的载体和子对象的容器
    * 3D对象诸如立方体、球体等用于搭建场景
    * Camera是在场景中定义视图的对象，是观察游戏世界的窗口，
    * Light是游戏的光源，提升亮度的同时也能制造阴影
    * Audio提供声效，为用户带来听觉体验
  * 通过向游戏对象添加多种组件，由此实现多样化的功能
    * transform组件改变对象的大小，位置，实现旋转
    * light组件设置光照参数，改变阴影效果
    * Camera组件调整摄像位置、角度和视图
    * Terrain组件提供一套强大的制作工具，可以创建地形
    * Collider组件提供碰撞判定，可以实现与用户互动
    * Animation组件定义动作，形成动画效果

## 2、牧师与魔鬼-动作分离

* **实验内容**：在小游戏-牧师与魔鬼的基础上实现动作分离
  * 实验要求：场记不仅要处理用户交互，还要加载游戏对象、实现游戏规则等。这样场记显得太臃肿，条理不清晰。因此考虑将动作分离出来，由动作控制器来单独管理。这样程序更能适应需求变化和易于维护。
    * 设计一个裁判类，当游戏达到结束条件时，通知场景控制器游戏结束
  
* **工程部署**：根据MVC架构，项目文件分为3个部分。
  * Models：定义所有的游戏对象，包括属性及其构造方法
    * Role.cs-牧师或魔鬼
    * Boat.cs-船
    * Land.cs-陆地
    * River.cs-陆地间的河流
  * Controllers：定义多个控制器，包括控制游戏对象动作的基础控制器和与玩家交互的场景控制器
    * SSDirector.cs-管理游戏全局
    * ISceneController.cs-场景接口，用于多态实现
    * FirstController.cs-实现场景接口，为初始场景，管理该场景所有的游戏对象并响应外部输入
    * ActionController.cs-包含了与动作管理器相关的类
    * Check.cs-裁判类
    * ISSActionCallback.cs-事件处理接口，所有事件管理者必须实现
    * IUserAction.cs-用户接口
    * Click.cs-实现对象接收用户的点击操作
  * Views
    * UserGUI.cs-渲染GUI
  * 将FirstController挂到空对象上即可运行

* **实现过程**：
  * Models：在role类和boat类中移除move并添加属性speed
  * Controllers：删除move
    * ISSActionCallback：回调函数，通过SSActionEvent向其他函数通信的接口，

    ```c#
    public enum SSActionEventType : int { Started, Competeted }

    public interface ISSActionCallback {
        void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,int intParam = 0, string strParam = null, Object objectParam = null);
    }
    ```
    * actionController：

    ```c#
    //动作基类，代表一个动作
    public class SSAction:ScriptableObject{
        public bool enable=true;//进行
        public bool destroy=false;//删除
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
    //移动子类，实现移动到目的地
    public class SSMoveToAction:SSAction{
        public Vector3 target;//目标位置
        public float speed;//速度

        private SSMoveToAction(){}
        public static SSMoveToAction GetSSAction(Vector3 _target,float _speed){
            SSMoveToAction action=ScriptableObject.CreateInstance<SSMoveToAction>();
            action.target=_target;
            action.speed=_speed;
            return action;
        }
        //作为父类虚函数供子类重写
        public override void Start(){}
        //作为父类虚函数供子类重写
        public override void Update(){
            this.transform.position=Vector3.MoveTowards(this.transform.position,target,speed*Time.deltaTime);
            //完成动作通知动作管理器或动作序列
            if(this.transform.position==target){
                this.destroy=true;
                this.callback.SSActionEvent(this);
            }
        }
    }
    //动作序列，实现多个动作
    public class SequenceAction:SSAction,ISSActionCallback{
        public List<SSAction> sequence;//动作列表
        public int repeat=-1;//循环次数
        public int start=0;//当前动作

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
            source.destroy=false;//由于可能会无限循环所以先不删除
            this.start++;
            if(this.start>=sequence.Count){
                this.start=0;
                if(repeat>0)
                    repeat--;
                if(repeat==0){//循环完毕
                    this.destroy=true;
                    this.callback.SSActionEvent(this);  
                }
            }
        }
        void OnDestroy(){}
    }
    //动作管理基类，负责管理所有动作或动作序列
    public class SSActionManager:MonoBehaviour,ISSActionCallback{
        private Dictionary<int,SSAction> actions=new Dictionary<int,SSAction>();//字典存有执行动作
        private List<SSAction> waitingAdd=new List<SSAction>();//等待执行的动作
        private List<int> waitingDelete=new List<int>();//等待删除的动作

        protected void Update(){
          //将等待列表加入字典
            foreach(SSAction ac in waitingAdd)
                actions[ac.GetInstanceID()]=ac;

            waitingAdd.Clear();

            //根据键值对进行执行或删除
            foreach(KeyValuePair<int,SSAction> kv in actions){
                SSAction ac=kv.Value;
                if(ac.destroy)
                    waitingDelete.Add(ac.GetInstanceID());
                else if(ac.enable)
                    ac.Update();
            }
            //删除完成动作
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
    //牧师与魔鬼的动作管理类
    public class SceneActionManager:SSActionManager{
        public FirstController sceneController;
        private SequenceAction boatMove;//船移动
        private SequenceAction roleMove;//角色移动

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
    ```

    * FirstController：删除check()函数并引进SceneActionManager

    ```c#
    public class FirstController : MonoBehaviour,ISceneController,IUserAction{
      public Land rightLand;
      public Land leftLand;
      public River river;
      public Boat boat;
      public Role[] roles;
      public SceneActionManager actionManager;
      public Check check;
      private UserGUI GUI;

      void Awake(){
          SSDirector director=SSDirector.getInstance();
          director.currentSceneController=this;
          GUI=gameObject.AddComponent<UserGUI>() as UserGUI;
          actionManager=gameObject.AddComponent<SceneActionManager>() as SceneActionManager;
          check=gameObject.AddComponent<Check>() as Check;
          LoadResources();
      }

      ...//省略部分函数

      public void LoadResources(){
          river=new River();
          rightLand=new Land("left",1);
          leftLand=new Land("right",0);
          boat=new Boat();
          roles=new Role[6];
          for(int i=0;i<3;i++){
              Role role=new Role(rightLand.positions[GetEmptyPositionOnLand(1)],true);
              role.role.name=("priest" + i);
              rightLand.roles[GetEmptyPositionOnLand(1)]=role;
              roles[i]=role;
          }
          for(int i=0;i<3;i++){
              Role role=new Role(rightLand.positions[GetEmptyPositionOnLand(1)],false);
              role.role.name=("devil" + i);
              rightLand.roles[GetEmptyPositionOnLand(1)]=role;
              roles[i+3]=role;
          }
      }

      public void MoveBoat(){//移动船
          Vector3 destination;
          bool t=boat.isRight;
      //   Debug.Log(EmptyFullBoat());
          if(EmptyFullBoat()==0||GUI.sign!=0){
              Click.state=0;//移动失败后复原state
              return;
          }
          if(boat.isRight==false)
              destination=new Vector3(3.5f,0.5f,0);
          else
              destination=new Vector3(-3.5f,0.5f,0);
          boat.isRight=(t==true?false:true);
          actionManager.moveBoat(boat.boat,destination,boat.speed);//使用动作管理器进行移动
          GUI.sign=check.CheckGame();
      }
      
      public void MoveRole(Role role){//移动角色
          if(GUI.sign!=0) 
              return;
          Vector3 mid,destination;
          if(role.isInBoat){
              if(boat.isRight==true){
                  deleteRoleInBoat(role.role.name);
                  destination=rightLand.positions[GetEmptyPositionOnLand(1)];
                  role.role.transform.parent=null;
                  role.isInBoat=false;
                  role.isRight=true;
                  rightLand.roles[GetEmptyPositionOnLand(1)]=role;
              }
              else{
                  deleteRoleInBoat(role.role.name);
                  destination=leftLand.positions[GetEmptyPositionOnLand(0)];
                  role.role.transform.parent=null;
                  role.isInBoat=false;
                  role.isRight=false;
                  leftLand.roles[GetEmptyPositionOnLand(0)]=role;
              }
              mid=new Vector3(role.role.transform.position.x,destination.y,destination.z);
              actionManager.moveRole(role.role,mid,destination,role.speed);//使用动作管理器进行移动
          }
          else{
              if(role.isRight==true){
                  if(EmptyFullBoat()==2||rightLand.isRight!= boat.isRight){
                      Click.state=0; //移动失败后复原state
                      return;
                  } 
                  deleteRoleOnLand(role.role.name,1);
                  destination=boat.rightPos[GetEmptyPositionInBoat(1)];
                  role.role.transform.parent=boat.boat.transform;
                  role.isInBoat=true;
                  boat.roles[GetEmptyPositionInBoat(1)]=role;
              }
              else{
                  if(EmptyFullBoat()==2||leftLand.isRight!=boat.isRight){
                      Click.state=0; //移动失败后复原state
                      return;
                  } 
                  deleteRoleOnLand(role.role.name,0);
                  destination=boat.leftPos[GetEmptyPositionInBoat(0)];
                  role.role.transform.parent=boat.boat.transform;
                  role.isInBoat=true;
                  boat.roles[GetEmptyPositionInBoat(0)]=role;
              }
              mid=new Vector3(destination.x,role.role.transform.position.y,destination.z);
              actionManager.moveRole(role.role,mid,destination,role.speed);//使用动作管理器进行移动
          }
          GUI.sign=check.CheckGame();
      }

      public void Restart(){
          GUI.sign=0;
          Click.state=0;
          SceneManager.LoadScene("Untitled");
      }
    }
    ```

    * Check-实现裁判类，用于判定游戏胜负。就是将原来FirstController的check()函数移至该处

      ```c#
      public class Check:MonoBehaviour{
        public FirstController sceneController;

        protected void Start(){
            sceneController=(FirstController)SSDirector.getInstance().currentSceneController;
            sceneController.check=this;
        }

        public int CheckGame(){
            int[] count1 ={0,0};
            int[] count2 ={0,0};
            int[] count3 ={0,0};
            for(int i=0;i<6;i++){
                if(sceneController.rightLand.roles[i]!=null){
                    if(sceneController.rightLand.roles[i].isPriest==true) 
                        count1[0]++;
                    else 
                        count1[1]++;
                }
                if(sceneController.leftLand.roles[i]!=null){
                    if(sceneController.leftLand.roles[i].isPriest==true) 
                        count2[0]++;
                    else 
                        count2[1]++;
                }
                if(i<2&&sceneController.boat.roles[i]!=null){
                    if(sceneController.boat.roles[i].isPriest==true) 
                        count3[0]++;
                    else 
                        count3[1]++;
                }
            }
            int leftPriests=count1[0];
            int leftDevils=count1[1];
            int rightPriests=count2[0];
            int rightDevils=count2[1];
            if(rightPriests+rightDevils==6) 
                return 2;
            if(sceneController.boat.isRight==true){
                leftPriests+=count3[0];
                leftDevils+=count3[1];
            }
            else{
                rightPriests+=count3[0];
                rightDevils+=count3[1];
            }
            if((rightPriests>0&&rightPriests<rightDevils)||(leftPriests>0&&leftPriests<leftDevils)){
                return 1;
            }
            return 0;
        }
      }
      ```

