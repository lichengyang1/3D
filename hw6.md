# 作业

## 打飞碟小游戏

实验内容：改进打飞碟游戏

* 游戏内容要求：
  * 游戏内容要求：
  * 按 adapter模式 设计图修改飞碟游戏
    * 目的：将一个类的接口转换成客户希望的另外一个接口，使得原本由于接口不兼容而不能一起工作的那些类能一起工作
    * 应用：
      * 之前的打飞碟UML类图大致如下

        [![BTMdhj.png](https://s1.ax1x.com/2020/11/08/BTMdhj.png)](https://imgchr.com/i/BTMdhj)

      * 为实现物理引擎动作，考虑添加PhyFlyAction类，则FirstController不再匹配接口。若此时再添加一个动作管理器PhyFlyActionManager，虽然可行，但系统的可扩展性太低，每多一个运动模式就要多一个对应管理器
      * 若此时使用Adapter模式，使不兼容的两个类能在一起工作

        [![BTM09s.png](https://s1.ax1x.com/2020/11/08/BTM09s.png)](https://imgchr.com/i/BTM09s)
        
      * 修改后的UML图如上，通过将ActionManager设计为适配器，将FirstController和两种运动模式类的接口衔接起来。在FirstController的视角，只需要调用ActionManager中的相关方法即可
  * 使它同时支持物理运动与运动学（变换）运动

</br>

工程部署：根据MVC架构，项目文件分为3个部分。

- Models：定义所有的游戏对象，包括属性及其构造方法

  - Skeet.cs
  - ScoreKeeper.cs
  - Factory.cs

- Controllers：定义多个控制器，包括控制游戏对象动作的基础控制器和与玩家交互的场景控制器

  - FirstController.cs
  - ActionController.cs
  - interface.cs
  - Singleton.cs
  - SSDirector.cs

- Views

  - UserGUI.cs

- 将FirstController挂到空对象上即可运行

  </br>

实现过程：在打飞碟的基础上进行改进，使用物理引擎的动作管理类。

* Controllers:
  * ActionController:包含所有动作管理器，其中大部分复用上个项目的代码，修改函数SSActionManager,SSAction,FlyActionManager,新增PhyFlyAction
    * SSActionManager:通过刚体组件使用物理引擎，但刚体组件不能通过Update()函数来刷新，受到机器性能和被渲染物体的影响，因此新加FixedUpdate函数来是使用物理引擎

    ```c#
    foreach(KeyValuePair<int,SSAction> kv in actions){
        SSAction ac=kv.Value;
        if(ac.destroy)
            waitingDelete.Add(ac.GetInstanceID());
        else if(ac.enable){
            ac.Update();
            ac.FixedUpdate();   //物理引擎更新
        }
            
    }
    ```
    
    * SSAction
      * 同步更新FixedUpdate()

      ```c#
      public virtual void Update(){
          throw new System.NotImplementedException();
      }
      //物理引擎更新
      public virtual void FixedUpdate(){
          throw new System.NotImplementedException();
      }
      ```

    * FlyActionManager:添加PhyFlyAction相关的操作。题目要求实现物理引擎和运动学切换，故重载DiskFly函数，一个用于运动学，一个用于物理引擎

      ```c#
      public class FlyActionManager : SSActionManager{ 
          public FlyAction fly;
          public PhyFlyAction newFly;
          public FirstController sceneController;          

          protected void Start(){
              sceneController=(FirstController)SSDirector.getInstance().currentSceneController;
              sceneController.actionManager=this;    
          }

          public void Fly(GameObject skeet,float angle,float power){
              skeet.GetComponent<Rigidbody>().isKinematic = true;
              int lor=1;
              if (skeet.transform.position.x>0) 
                  lor=-1;
              fly=FlyAction.GetSSAction(lor,angle,power);
              this.RunAction(skeet,fly,this);
          }
          public void Fly(GameObject skeet,float power){
              skeet.GetComponent<Rigidbody>().isKinematic = false;
              int lor=1;
              if (skeet.transform.position.x>0) 
                  lor=-1;
              newFly=PhyFlyAction.GetSSAction(lor,power);
              this.RunAction(skeet,newFly,this);
          }
      }
      ```

    * PhyFlyAction：基于FlyAction进行修改，在FixedUpdate里进行动作相关的操作。在开始时给予给飞碟一个初始力


      ```c#
      public class PhyFlyAction : SSAction {
        private Vector3 start;                              
        public float power;
        private PhyFlyAction(){}
        public static PhyFlyAction GetSSAction(int lor, float power) {
            PhyFlyAction action = CreateInstance<PhyFlyAction>();

            action.start=lor==-1?Vector3.left*power:Vector3.right*power;
            action.power=power;
            return action;
        }

        public override void Update(){}

        public override void FixedUpdate(){
            if (transform.position.y<=-10f){
                gameobject.GetComponent<Rigidbody>().velocity=new Vector3(0,0,0);
                this.destroy=true;
                this.callback.SSActionEvent(this);
            }
        }

        public override void Start(){
            gameobject.GetComponent<Rigidbody>().AddForce(start*3,ForceMode.Impulse);
        }
      }
      ```

  * FirstController：场景控制器
    * launchSkeet：

    ```c#
    //  actionManager.Fly(skeet,angle,power);
        actionManager.Fly(skeet,power);//物理引擎
    ```

* 此外再给所有预制加上刚体组件


</br>

效果图：与上次实验一致