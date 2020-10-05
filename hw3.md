# 作业

## 1、简答并用程序验证【建议做】

* 游戏对象运动的本质是什么？
  * 本质就是对象Position、Rotate、Scale这三个属性的改变
* 请用三种方法以上方法，实现物体的抛物线运动。（如，修改Transform属性，使用向量Vector3的方法…）
  * 修改transform属性，水平速度为1，竖直速度每秒+1

    ```c#
    public int t=1;
    void Update(){
        transform.position+=Vector3.right*Time.deltaTime;
        transform.position+=Vector3.down*Time.deltaTime*Time.deltaTime*t;
        t++;
    }
    ```

  * 使用transform.Translate方法

    ```c#
    public int t=1;
    void Update(){
        transform.Translate(Vector3.right*Time.deltaTime+Vector3.down*Time.deltaTime*Time.deltaTime*t);
        t++;
    }
    ```

  * 使用Vector3，设速度为1

    ```c#
    public int t=1;
    void Update(){
        transform.position+=new Vector3(Time.deltaTime,-Time.deltaTime*t, 0);
        t++;
    }
    ```

* 写一个程序，实现一个完整的太阳系， 其他星球围绕太阳的转速必须不一样，且不在一个法平面上。
  * 对象创建
    * 创建星球：创建9个球体，设置大小、位置和材质

        ![0ASUM9.png](https://s1.ax1x.com/2020/09/27/0ASUM9.png)

        ![0ASWqI.png](https://s1.ax1x.com/2020/09/27/0ASWqI.png)

  * 实现旋转
    * 公转：transform.RotateAround (Vector3 point, Vector3 axis, float angle)

        围绕在point的旋转轴axis旋转angle

        ```c#
        Mercury.RotateAround(Sun.transform.position, new Vector3(1,1, 0), 10 * Time.deltaTime);
        Venus.RotateAround(Sun.transform.position, new Vector3(2,3, 0), 30 * Time.deltaTime);
        Earth.RotateAround(Sun.transform.position, new Vector3(5, -10, 0), -30 * Time.deltaTime);
        Mars.RotateAround(Sun.transform.position, new Vector3(3, 10, 0), 24 * Time.deltaTime);
        Jupiter.RotateAround(Sun.transform.position, new Vector3(3, -10, 0), -15 * Time.deltaTime);
        Saturn.RotateAround(Sun.transform.position, new Vector3(7, -10, 0), -10 * Time.deltaTime);
        Uranus.RotateAround(Sun.transform.position, new Vector3(3, 10, 0), 5 * Time.deltaTime);
        Neptune.RotateAround(Sun.transform.position, new Vector3(5, -10, 0), -10 * Time.deltaTime);
        ```

    * 自转：transform.Rotate (Vector3 eulers)

      对象绕eulers旋转

        ```c#
        Sun.Rotate(Vector3.up * Time.deltaTime*2);
        Earth.Rotate(Vector3.up * Time.deltaTime * 20);
        Mercury.Rotate(Vector3.up * Time.deltaTime * 20);
        Venus.Rotate(Vector3.up * Time.deltaTime * 20);
        Mars.Rotate(Vector3.up * Time.deltaTime * 40);
        Jupiter.Rotate(Vector3.up * Time.deltaTime * 60);
        Saturn.Rotate(Vector3.up * Time.deltaTime * 100);
        Uranus.Rotate(Vector3.up * Time.deltaTime * 150);
        Neptune.Rotate(Vector3.up * Time.deltaTime * 150);
        ```

  * 工程部署：一个文件脚本rotate,挂载到空对象上，然后在inspector连接对象

    ![0A9VXj.png](https://s1.ax1x.com/2020/09/27/0A9VXj.png)

  * 成果图：

    ![0AMEgs.gif](https://s1.ax1x.com/2020/09/27/0AMEgs.gif)

    ![0AMaVK.gif](https://s1.ax1x.com/2020/09/27/0AMaVK.gif)


## 2、编程实践-牧师与魔鬼

* **实验内容**：在MVC架构下实现牧师与魔鬼小游戏
  * 实验要求：
    * 列出游戏中提及的事物（Objects）
      * 牧师，恶魔，船，河流，两侧陆地
    * 用表格列出玩家动作表（规则表），注意，动作越少越好

      | 动作 | 前提 | 结果 |
      | :--: | :--: | :--:|
      | 点击角色 |角色在船上 | 角色上同侧岸 |
      | 点击角色 |角色在岸上，与船同侧，船未满 | 角色上船 |
      | 点击船 | 船上至少有一个角色 | 船移动到另一岸 |

    * 请将游戏中对象做成预制

    [![0YFoRI.png](https://s1.ax1x.com/2020/10/05/0YFoRI.png)](https://imgchr.com/i/0YFoRI)
  
    * 在场景控制器 LoadResources 方法中加载并初始化长方形、正方形、球 及其色彩代表游戏中的对象。
    * 整个游戏仅主摄像机和一个 Empty 对象，其他对象必须代码动态生成！！！ 

    [![0YFIJA.png](https://s1.ax1x.com/2020/10/05/0YFIJA.png)](https://imgchr.com/i/0YFIJA) 
    * 整个游戏不许出现 Find 游戏对象，SendMessage这类突破程序结构的 通讯耦合语句。违背本条准则，不给分
    * 请使用课件架构图编程，不接受非 MVC 结构程序
    * 注意细节，例如：船未靠岸，牧师与魔鬼上下船运动中，均不能接受用户事件！*

* **工程部署**：根据MVC架构，项目文件分为3个部分
  * Models：定义所有的游戏对象，包括属性及其构造方法
    * Role.cs-牧师或魔鬼
    * Boat.cs-船
    * Land.cs-陆地
    * River.cs-陆地间的河流
  * Controllers：定义多个控制器，包括控制游戏对象动作的基础控制器和与玩家交互的场景控制器
    * SSDirector.cs-管理游戏全局
    * ISceneController.cs-场景接口，用于多态实现
    * FirstController.cs-实现场景接口，为初始场景，管理该场景所有的游戏对象并响应外部输入
    * IUserAction.cs-用户接口
    * Move.cs-实现对象移动动作
    * Click.cs-实现对象接收用户的点击操作
  * Views
    * UserGUI.cs-渲染GUI


  [![0YZuND.png](https://s1.ax1x.com/2020/10/05/0YZuND.png)](https://imgchr.com/i/0YZuND)
  
  * 将FirstController挂到空对象上即可运行

* **实现过程**：
  * Models
    * Role

      ```c#
      public class Role{ 
          public GameObject role;//游戏对象
          public bool isPriest;//1是牧师,0是恶魔
          public bool isRight;//角色位置,1是右侧陆地，0是左侧陆地
          public bool isInBoat;//1是在船里，0是在岸上
          public Click click;//用于响应鼠标点击
          public Move move;//用于角色移动动作
          public Role(Vector3 position,bool isPriest){//初始化
              this.isPriest=isPriest;
              isRight=true;
              isInBoat=false;
              role=Object.Instantiate(Resources.Load<GameObject>("Prefabs/"+(isPriest?"priest":"devil")),position,Quaternion.Euler(0,-90,0)) as GameObject;
              click=role.AddComponent(typeof(Click)) as Click;
              move=role.AddComponent(typeof(Move)) as Move;
              click.role=this;
          }
      }`
      ```

    * Boat

      ```c#
      public class Boat{
          public GameObject boat;//游戏对象
          public Role[] roles;//船上的角色
          public bool isRight;//船位于哪一侧，1为右侧，0为左侧
          public Vector3[] leftPos;//船位于左侧时，座位的坐标
          public Vector3[] rightPos;//船位于右侧时，座位的坐标
          public Click click;//用于响应鼠标点击
          public Move move;//用于船移动动作
          public Boat(){
              roles=new Role[2];//船最多只能载2个角色
              boat=Object.Instantiate(Resources.Load<GameObject>("Prefabs/Boat"),new Vector3(3.5f,0.5f,0),Quaternion.identity) as GameObject;//船初始位于右侧
              boat.name="boat";
              rightPos=new Vector3[]{new Vector3(3,1,0),new Vector3(4,1,0)};
              leftPos=new Vector3[]{new Vector3(-4,1,0),new Vector3(-3,1,0)};
              move=boat.AddComponent(typeof(Move)) as Move;
              click=boat.AddComponent(typeof(Click)) as Click;
              click.boat=this;
              isRight=true;
          }
      }
      ```

      * land

      ```c#
      public class Land{
          public GameObject land;//游戏对象
          public Vector3[] positions;//岸上角色的位置
          public bool isRight;//陆地位于哪一侧
          public Role[] roles=new Role[6];//岸上的角色
          public Land(string name,int type){
              int t=(type==1?1:-1);
              positions=new Vector3[]{
                  new Vector3(6.35F*t,2.0F,0),new Vector3(7.35f*t,2.0F,0),new Vector3(8.35f*t,2.0F,0),
                  new Vector3(9.35f*t,2.0F,0),new Vector3(10.35f*t,2.0F,0),new Vector3(11.35f*t,2.0F,0)
              };//岸上6个角色的位置
              land=Object.Instantiate(Resources.Load<GameObject>("Prefabs/Land"),new Vector3(9*t,1,0),Quaternion.identity) as GameObject;
              land.name=name;
              isRight=type==1?true:false;
          }
      }
      ```

      * river

      ```c#
      public class River{
          private GameObject river;//游戏对象
          public River(){
              river=Object.Instantiate(Resources.Load("Prefabs/river",typeof(GameObject))) as GameObject;
              river.name="river";
              river.transform.position=Vector3.zero;
          }
      }
      ```

    * Controllers
      * SSDirector：使用单例模式创建，负责场景的运行切换以及游戏的暂停恢复等

      ```c#
      public class SSDirector : System.Object{
        private static SSDirector _instance;
        public ISceneController currentSceneController{ get; set; }
        public static SSDirector getInstance(){
              if (_instance==null){
                  _instance=new SSDirector();
              }
              return _instance;
        }
      }
      ```

      * ISceneController：场景接口，用于多态实现不同场景，本次游戏中只有一个场景且只需要加载资源

      ```c#
      public interface ISceneController
      {
        void LoadResources();
      }
      ```

      * IUserAction：用户接口，用于场景与用户交互，在场景中具体实现

      ```c#
      public interface IUserAction{
              void MoveBoat();//移动船
              void MoveRole(Role role);//移动角色
              void Restart();//重新开始
              int Check();//检查游戏状态，判断胜负
      }
      ```

      * Move：实现对象移动动作

      ```c#
      public class Move : MonoBehaviour{
        float speed=10;//移动速度
        int mode=0;//移动模式
        Vector3 destination;//目标坐标
        Vector3 mid;//中转坐标
        void Update(){
            if(mode==1){//
                transform.position=Vector3.MoveTowards(transform.position,mid,speed*Time.deltaTime);
                if(transform.position==mid){//到达中转地后，x或y相同，则直接移动
                    mode=2;
                }
            }
            else if(mode==2){//直接移动
                transform.position=Vector3.MoveTowards(transform.position,destination,speed*Time.deltaTime);
                if(transform.position==destination){//结束移动
                    mode=0;
                    Click.state=0;
                }
            }
        }
        public void MovePosition(Vector3 position){//判断移动方式
            destination=position;
            if(position.y==transform.position.y){//若初始位置和目的地y相同，则直接移动到目的地，即水平移动
                mode=2;
                return;
            }
            else if(position.y<transform.position.y)//若y不同，则先移动到中转位置，使x或y相同
                mid=new Vector3(position.x,transform.position.y,position.z);
            else
                mid=new Vector3(transform.position.x,position.y,position.z);
            mode=1;
        }
      }
      ```

      * Click：实现角色和船接收用户的点击并进行相关响应

      ```c#
      public class Click : MonoBehaviour{
          IUserAction action;
          public Role role=null;//绑定角色
          public Boat boat=null;//绑定船
          public static int state=0;//用于在移动过程中拒绝用户事件
          void Start(){
              action=SSDirector.getInstance().currentSceneController as IUserAction;
          }
          void OnMouseDown(){
              if(boat==null&&role==null)
                  return;
              if(boat!=null&&state==0){//鼠标点击船
                  state=1;
                  action.MoveBoat();
              } 
              else if(role!=null&&state==0){//鼠标点击角色
                  state=1;
                  action.MoveRole(role);
              }
          }
      }
      ```

      * FirstController：实现场景接口，为初始场景，管理该场景所有的游戏对象并响应外部输入

      ```c#
      public class FirstController : MonoBehaviour,ISceneController,IUserAction{
          public Land rightLand;//右侧陆地
          public Land leftLand;//左侧陆地
          public River river;//河流
          public Boat boat;//船
          public Role[] roles;//角色
          private UserGUI GUI;

          void Awake(){//初始
              SSDirector director=SSDirector.getInstance();
              director.currentSceneController=this;
              GUI=gameObject.AddComponent<UserGUI>() as UserGUI;
              LoadResources();
          }

          ...//省略部分函数

          public void LoadResources(){//创建游戏对象并进行配置
              river=new River();
              rightLand=new Land("left",1);
              leftLand=new Land("right",0);
              boat=new Boat();
              roles=new Role[6];
              for(int i=0;i<3;i++){//配置牧师
                  Role role=new Role(rightLand.positions[GetEmptyPositionOnLand(1)],true);
                  role.role.name=("priest" + i);
                  rightLand.roles[GetEmptyPositionOnLand(1)]=role;
                  roles[i]=role;
              }
              for(int i=0;i<3;i++){//配置恶魔
                  Role role=new Role(rightLand.positions[GetEmptyPositionOnLand(1)],false);
                  role.role.name=("devil" + i);
                  rightLand.roles[GetEmptyPositionOnLand(1)]=role;
                  roles[i+3]=role;
              }
          }

          public void MoveBoat(){//响应鼠标点击船
              bool t=boat.isRight;
          //   Debug.Log(EmptyFullBoat());
              if(EmptyFullBoat()==0||GUI.sign!=0){//若船满或游戏结束，则不动
                  Click.state=0;
                  return;
              }
              if(boat.isRight==false)//通过move移动船到另一侧 
                  boat.move.MovePosition(new Vector3(3.5f,0.5f,0));
              else 
                  boat.move.MovePosition(new Vector3(-3.5f,0.5f,0));
              boat.isRight=(t==true?false:true);
              GUI.sign=Check();//移动后判断游戏状态
          }

          public void MoveRole(Role role){//响应鼠标点击角色
              if(GUI.sign!=0)//游戏结束则不动
                  return;
              if(role.isInBoat){
                  if(boat.isRight==true){//角色上右侧陆地
                      ...
                  }
                  else{//角色上左侧陆地
                      ...
                  }
              }
              else{
                  if(role.isRight==true){//角色在右侧陆地上船
                      ...
                  }
                  else{//角色在左侧陆地上船
                      ...
                  }
              }
              GUI.sign=Check();
          }

          public void Restart(){
              GUI.sign=0;
              Click.state=0;
              SceneManager.LoadScene("Untitled");//重新加载场景
          }

          public int Check(){//统计牧师和恶魔数量来判定胜负
              ...
          }
      }
      ```

  * Views
    * UserGUI：渲染GUI

    ```c#
    public class UserGUI : MonoBehaviour{
      private IUserAction action;
      public int sign=0;//游戏状态标志
      void Start(){
          action=SSDirector.getInstance().currentSceneController as IUserAction;
      }
      void OnGUI(){
          if(sign==1){
              GUI.Label(new Rect(Screen.width/2,Screen.height/2,100,50),"Gameover!");  
          }
          else if(sign==2){
              GUI.Label(new Rect(Screen.width/2,Screen.height/2,100,50),"You Win!");
          }
          if(GUI.Button(new Rect(Screen.width/2-40,Screen.height/2-120,100,50),"Restart")){
              action.Restart();
              sign=0;
          }
      }
    }
    ```

* **遇到的问题和一些细节**
  * 关于预制
    * 若下载的预制带有动画，则需要在动画文件设置循环时间和循环动作
    ![0Y15xU.png](https://s1.ax1x.com/2020/10/05/0Y15xU.png)

    * 使用下载的预制时发现，在游戏运行时对象无法响应鼠标点击，如鼠标点击牧师，牧师并不移动，而使用unity3d的自带对象如随便创建的cube,则可以响应。
      * 解决：在unity上创建的对象会默认挂载collider组件，因此可以响应点击，而下载的资源里并没有，故需要自行添加组件并设置相应参数

      [![0Y3Zz8.png](https://s1.ax1x.com/2020/10/05/0Y3Zz8.png)](https://imgchr.com/i/0Y3Zz8)
  * 实现在船未靠岸，牧师与魔鬼上下船运动时不能接受用户事件
    * 大致思路：在项目中用户事件为点击，故应实现在接受点击到响应点击的过程中不接收其他点击。考虑设置一个全局状态变量state=0,只有为0时才接受点击。在点击后修改其值，拒绝后续点击，在处理完后复原state,重新开放点击。
    * 具体实现：
      * 接收点击时

      ```c#
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
      ```

      * 响应点击后。这里只列出了成功移动后设置state。由于在移动船或角色前需要进行一些条件判断，如船是否满，船上是否有角色等，可能导致移动失败，此时虽然没有移动，但仍需要设置state，否则点击空船或船满时点击角色会导致游戏卡死，对象永远不再接受点击。相关代码有好几处，不再列出，详细可参考FirstController。

      ```c#
      else if(mode==2){
          transform.position=Vector3.MoveTowards(transform.position,destination,speed*Time.deltaTime);
          if(transform.position==destination){
              mode=0;
              Click.state=0;
          }   
      }
      ```

成果展示：

