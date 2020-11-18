# 作业

## 打飞碟小游戏

实验内容：实现智能巡逻兵

- 游戏设计要求：
  * 创建一个地图和若干巡逻兵(使用动画)；
  * 每个巡逻兵走一个3~5个边的凸多边型，位置数据是相对地址。即每次确定下一个目标位置，用自己当前位置为原点计算；
  * 巡逻兵碰撞到障碍物，则会自动选下一个点为目标；
  * 巡逻兵在设定范围内感知到玩家，会自动追击玩家；
  * 失去玩家目标后，继续巡逻；
  * 计分：玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束；
- 程序设计要求：
  - 必须使用订阅与发布模式传消息
    - subject：OnLostGoal
    - Publisher: ?
    - Subscriber: ?
  - 工厂模式生产巡逻兵


</br>

工程部署：根据MVC架构，项目文件分为3个部分。

- Models：定义所有的游戏对象，包括属性及其构造方法。此项目中定义了巡逻兵及其工厂和记分员。

  - Patrol.cs
  - ScoreRecorder.cs
  - PatrolFactory.cs
- Controllers：定义多个控制器，包括控制游戏对象动作的基础控制器和与玩家交互的场景控制器。在此项目中，关键内容为玩家输入、摄像机跟随以及动作控制
  - PlayerController
    - ActorController.cs
    - CameraController.cs
    - PlayerInput.cs
  - FirstController.cs
  - ActionController.cs
  - AreaCollide.cs
  - PlayerCollide.cs
  - GameEventManager.cs
  - interface.cs
  - Singleton.cs
  - SSDirector.cs
- Views：定义GUI
  - UserGUI.cs

</br>

脚本挂载
* 在场景中创建一个空对象，挂载FirstController,GameEventManager,ScoreRecorder和PatrolFactory
* 在Player预制上挂载PlayerCollide,PlayerInput
  * cameraPos挂载CameraController
* 在Patrol预制上挂载Patrol
* 在Plane预制中为每个区域挂载AreaCollide
* 
</br>

游戏玩法基本介绍：

* 游戏地图为九宫格，玩家出生在其中一格，其他八格每格都有巡逻兵巡逻
* 玩家通过WSAD控制人物走动，按住left shift开始奔跑，使用方向键调整视角方向
* 当玩家进入其他格时，该格的巡逻兵就会开始追逐玩家，若玩家成功离开该区域，则代表甩开巡逻兵，巡逻兵停止追逐并分数加一；若玩家与巡逻兵触碰则游戏结束。

</br>

实现过程：

* 人物模型预制和动画方面

  * 动画

    * 实验中，玩家和巡逻兵只有三个动作，分别为站立、行走和奔跑

    * 动画器PlayerAnmiator结构如下

      [![DnkQgg.png](https://s3.ax1x.com/2020/11/18/DnkQgg.png)](https://imgchr.com/i/DnkQgg)

      其中ground为混合树，内部结构如下，

      [![DnkM8S.png](https://s3.ax1x.com/2020/11/18/DnkM8S.png)](https://imgchr.com/i/DnkM8S)

    * 混合树Blend Tree常用于混合行走和奔跑动画，允许通过不同程度合并多个动画来使动画平滑混合。这里混合了站立、行走和奔跑三种动画

    * 其中参数为forward，用于混合树中行走奔跑的过渡

  * 本次项目使用预制如下

    [![DnFEkV.png](https://s3.ax1x.com/2020/11/18/DnFEkV.png)](https://imgchr.com/i/DnFEkV)

    * Player：代表玩家的模型

      [![DnVSNq.png](https://s3.ax1x.com/2020/11/18/DnVSNq.png)](https://imgchr.com/i/DnVSNq)

      其中bot为外部预制，cameraPos用于摄像机跟踪

    * Patrol：代表巡逻兵模型

      ![ ](https://s3.ax1x.com/2020/11/18/DnEvHs.png)  

      其中bot为外部预制

    * Plane：场地布置

      ![DnECOH.png](https://s3.ax1x.com/2020/11/18/DnECOH.png)

      其中Plane为地面，Wall为墙，Sensor用于判定角色是否进入某格

    * Player和Patrol都使用了PlayAnimator动画器。Player使用WSAD进行行走，left shift进行奔跑，方向键转动视角。Patrol运动由代码控制，追逐时奔跑，其他时候行走

* 具体实现：下面分析关键部分，部分代码与上次实验相同，故直接略过

  * Models：

    - Patrol.cs：定义巡逻兵的基本属性
    
      ```c#
      public GameObject model;
      public float walkSpeed=1.2f;//行走速度
      public float runSpeed=2.5f;//奔跑速度
      public int sign;//标志巡逻兵所在区域
      public bool isFollow=false;//表示巡逻兵是否追逐玩家
      public int playerSign=-1;//表示玩家所在区域
      public Vector3 start_position;//当前巡逻兵初始位置 
      ```
    
    - ScoreRecorder.cs：实现分数统计
    
      ```c#
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
      ```
    
    - PatrolFactory.cs：实现工厂模式生产巡逻兵
    
      ```c#
      public List<GameObject> GetPatrols(){
          int[] pos_x ={-6,4,13};
          int[] pos_z ={-4,6,-13};
          int index=0;
          for(int i=0;i<3;i++){
              for(int j=0;j<3;j++){
                  vec[index]=new Vector3(pos_x[i],0,pos_z[j]);//获得巡逻兵初始位置
                  index++;
              }
          }
          for(int i=0;i<8;i++){
              patrol=Instantiate(Resources.Load<GameObject>("Prefabs/Patrol"));//生产Patrol
              patrol.transform.position=vec[i];
              patrol.GetComponent<Patrol>().sign=i+1;
              patrol.GetComponent<Patrol>().start_position=vec[i];
              patrol.GetComponent<Animator>().SetFloat("forward",1);
              used.Add(patrol);//加入使用列表
          }   
          return used;
      }
      }
      ```
    
      
    
  * Controllers：

    - PlayerController

      - ActorController.cs：控制动画参数

        ```c#
        void Update(){
        
              float targetRunSpeed=input.run?2.0f:1.0f;
                anim.SetFloat("forward",input.Dmag*Mathf.Lerp(anim.GetFloat("forward"),targetRunSpeed,0.3f));//动作切换补帧
                
           
            if(input.Dmag>0.01f){
                Vector3 targetForward=Vector3.Slerp(model.transform.forward,input.Dvec,0.2f);//旋转补帧
                model.transform.forward=targetForward;
            }
            move=input.Dmag*model.transform.forward*walkSpeed*(input.run?runSpeed:1.0f);//更新平面移动
        }
        
        private void FixedUpdate(){
            rigid.velocity=new Vector3(move.x,rigid.velocity.y,move.z) ;
        }
        ```

        

      - CameraController.cs：用于摄像头跟踪，实现第三人称

        ```c#
        void FixedUpdate(){
                Vector3 tempModelEuler=model.transform.eulerAngles;
                playerHandle.transform.Rotate(Vector3.up,input.Jright*horizontalSpeed*Time.fixedDeltaTime);
                tempEulerX-=input.Jup*verticalSpeed*Time.fixedDeltaTime;
                tempEulerX=Mathf.Clamp(tempEulerX,-35,30);
                cameraHandle.transform.localEulerAngles=new Vector3(tempEulerX,0,0);
                model.transform.eulerAngles=tempModelEuler;
        
                camera.transform.position=Vector3.SmoothDamp(
                    camera.transform.position,transform.position, 
                    ref cameraDampVelocity,cameraDampValue);
                camera.transform.eulerAngles=transform.eulerAngles;
            }
        ```

        

      - PlayerInput.cs：实现玩家输入。通过将矩形坐标转换为圆坐标实现组合按键效果，使得多个方向同时按下时速度不同

        ```c#
        public class PlayerInput : MonoBehaviour{
            public string keyUp="w";
            public string keyDown="s";
            public string keyLeft="a";
            public string keyRight="d";
            public string keyRun="left shift";
            public string keyJUp="up";
            public string keyJDown="down";
            public string keyJLeft="left";
            public string keyJRight="right";
            //角色方向
            public float Dup;
            public float Dright;
            public float Dmag;
            
            public Vector3 Dvec;//速度
            //镜头方向
            public float Jup;
            public float Jright;
            public bool run;//奔跑参数
            private float targetDup;
            private float targetDright;
            private float velocityDup;
            private float velocityDright;
        
            void Start(){}
        
            void Update(){
                Jup=(Input.GetKey(keyJUp))?1.0f:0-(Input.GetKey(keyJDown)?1.0f:0);
                Jright=(Input.GetKey(keyJRight))?1.0f:0-(Input.GetKey(keyJLeft)?1.0f:0);
        
                targetDup=(Input.GetKey(keyUp)?1.0f:0)-(Input.GetKey(keyDown)?1.0f:0);
                targetDright=(Input.GetKey(keyRight)?1.0f:0)-(Input.GetKey(keyLeft)?1.0f:0);
        
                //平滑变动
                Dup=Mathf.SmoothDamp(Dup,targetDup,ref velocityDup,0.1f);
                Dright=Mathf.SmoothDamp(Dright,targetDright,ref velocityDright,0.1f);
                
                Vector2 tempDAxis=SquareToCircle(new Vector2(Dup,Dright));
                float Dup2=tempDAxis.x;
                float Dright2=tempDAxis.y;
        
                Dmag=Mathf.Sqrt((Dup2*Dup2)+(Dright2*Dright2));
                Dvec=Dright*transform.right+Dup*transform.forward;
                run=Input.GetKey(keyRun);
            }
        
            //矩形坐标转圆坐标
            private Vector2 SquareToCircle(Vector2 input){
                Vector2 output=Vector2.zero;
                output.x=input.x*Mathf.Sqrt(1-(input.y*input.y)/2.0f);
                output.y=input.y*Mathf.Sqrt(1-(input.x*input.x)/2.0f);
                return output;
            }
        }
        
        ```

        

    - FirstController.cs：初始化场景

      ```c#
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
      ```

      

    - ActionController.cs

      - FollowAction：实现巡逻兵追逐

        ```c#
        public override void Update(){
            planarVec=gameobject.transform.forward*speed;
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
        ```

        

      - GuardPatrolAction：实现巡逻动作

        ```c#
        public override void FixedUpdate(){
            Gopatrol();//训练
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
        ```

        

      - GuardActionManager：实现巡逻兵动作切换

        ```c#
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
        ```

        

    - AreaCollide.cs：实现区域碰撞，作为追逐条件

      ```c#
      public class AreaCollide : MonoBehaviour{
          public int sign=0;
          private FirstController sceneController;
      
          private void Start(){
              sceneController=SSDirector.getInstance().currentSceneController as FirstController;
          }
      
          void OnTriggerEnter(Collider collider){
              if(collider.gameObject.tag=="Player"){
                  sceneController.playerSign=sign;
              }
          }
      }
      ```

    - PlayerCollide.cs：判断玩家与巡逻兵相撞

      ```c#
      public class PlayerCollide : MonoBehaviour{
          void OnCollisionEnter(Collision other){
              if (other.gameObject.tag=="Guard"){
                  Singleton<GameEventManager>.Instance.PlayerGameover();
              }
          }
      }
      ```

* 订阅与发布模式

  * GameEventManager发布消息

    ```c#
    public void PlayerEscape(){
        if(ScoreChange!=null){
            ScoreChange();
        }
    }
    
    public void PlayerGameover(){
        if(GameoverChange!=null){
            GameoverChange();
        }
    }
    ```

  * FirstController订阅消息

    ```c#
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
    ```

    

  ​      

</br>



效果图：

[![Dn3Spj.gif](https://s3.ax1x.com/2020/11/18/Dn3Spj.gif)](https://imgchr.com/i/Dn3Spj)

[![Dn3ajI.gif](https://s3.ax1x.com/2020/11/18/Dn3ajI.gif)](https://imgchr.com/i/Dn3ajI)