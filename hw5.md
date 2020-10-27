# 作业

## 打飞碟小游戏

实验内容：在小游戏-牧师与魔鬼的基础上实现动作分离

- 游戏内容要求：

  1. 游戏有 n 个 round，每个 round 都包括10 次 trial；

     * 本次实验设计了3个round

  2. 每个 trial 的飞碟的色彩、大小、发射位置、速度、角度、同时出现的个数都可能不同。它们由该 round 的 ruler 控制；

     * 设计了3种飞碟

       [![BMRAbQ.png](https://s1.ax1x.com/2020/10/27/BMRAbQ.png)](https://imgchr.com/i/BMRAbQ)

  3. 每个 trial 的飞碟有随机性，总体难度随 round 上升；

     * 飞碟发射的角度，速度随机产生，round上升后飞碟的发射速度变快

  4. 鼠标点中得分，得分规则按色彩、大小、速度不同计算，规则可自由设定。

     * 1号飞碟计1分，2号飞碟计3分，3号飞碟计5分

* 游戏的要求：
  * 使用带缓存的工厂模式管理不同飞碟的生产与回收，该工厂必须是场景单实例的！具体实现见参考资源 Singleton 模板类
  * 近可能使用前面 MVC 结构实现人机交互与游戏模型分离

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

  - UserGUI.cs\

- 将FirstController挂到空对象上即可运行

  </br>

实现过程：

* Models:

  * Skeet：飞碟类，定义飞碟属性-种类和分数

  ```c#
  public class Skeet : MonoBehaviour {
      public int type=1;
      public int score=1;                                    
  }
  ```

  * ScoreKeeper：记分员，记录得分

  ```c#
  public class ScoreKeeper : MonoBehaviour{
      private float score;
      void Start (){
          score=0;
      }
      public float GetScore(){
          return score;
      }
      public void Reset(){
          score=0;
      }
      public void Record(GameObject disk){
          score += disk.GetComponent<Skeet>().score;
      }    
  }
  ```

  * Factory：飞碟工厂，产生和回收飞碟
    * 当场景需要飞碟时，先从freeList中寻找飞碟，然后再实例化生成新的飞碟，最后加入runList
    * 当飞碟掉出屏幕后，则将其加入freeList回收

  ```c#
  public class Factory : MonoBehaviour{
      private List<Skeet> runList=new List<Skeet>();
      private List<Skeet> freeList=new List<Skeet>();
  
      public GameObject GetSkeet(int type){
          GameObject skeet=null;
          if (freeList.Count>0){//回收列表不为空
              for(int i=0;i<freeList.Count;i++){
                  if (freeList[i].type==type){//种类匹配
                      skeet=freeList[i].gameObject;
                      freeList.Remove(freeList[i]);
                      break;
                  }
              }     
          }
          
          if (skeet==null){
              string t=type.ToString();
              skeet=Instantiate(Resources.Load<GameObject>("Prefabs/skeet"+t),new Vector3(0,-10f,0),Quaternion.identity);
          }
          runList.Add(skeet.GetComponent<Skeet>());
          skeet.SetActive(true);
          return skeet;
      }
  
      public void DeleteSkeet(){
          for(int i=0;i<runList.Count;i++){
              if (runList[i].gameObject.transform.position.y<= -10f){//掉出屏幕
                  freeList.Add(runList[i]);
                  runList.Remove(runList[i]);
              }
          }          
      }
  
      public void Reset(){
          DeleteSkeet();
      }
  ```

* Controllers:

  * ActionController:包含所有动作管理器，其中大部分复用上个项目的代码，新增FlyAction和FlyActionManager

    * FlyAction：通过位置变化和角度变化模拟飞碟的飞行动作

    ```c#
    public class FlyAction : SSAction{
        public float gravity=-2;//重力                            
        private Vector3 start;//初速度                      
        private Vector3 acceleration=Vector3.zero;//加速度         
        private Vector3 angle=Vector3.zero;//欧拉角          
        private float time;//时间                                  
    
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
    		//动作结束
            if (this.transform.position.y<-10){
                this.destroy=true;
                this.callback.SSActionEvent(this);     
            }
        }
    ```

    * FlyActionManager：管理飞行动作，当场景控制器发射飞碟时调用Fly进行飞行

    ```c#
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
    ```

  * Singleton：单例模式

    ```c#
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{
        protected static T instance;
        public static T Instance{
            get{
                if (instance==null){
                    instance=(T)FindObjectOfType(typeof(T));
                    if (instance==null){
                        Debug.LogError("An instance of "+typeof(T)
                           +" is needed in the scene,but there is none.");
                    }
                }
                return instance;
            }
        }
    }
    ```

  * FirstController：场景控制器

    * Update：每一帧检测鼠标点击并发射飞碟，然后调整难度

      ```c#
      void Update (){
          if(state){
              count++;
              if (Input.GetButtonDown("Fire1")){//检测鼠标点击
                  Vector3 pos=Input.mousePosition;
                  Hit(pos);
              }
              if (count >= 300){//设置间隔
      
                  if(round==1){//round1只发射1号飞碟
                      count=0;
                      launchSkeet(1);
                      trial+=1;
                  }
                  else if(round==2){//round2发射1号和2号飞碟
                      count=0;
                      if (trial%2==0) 
                          launchSkeet(1);
                      else 
                          launchSkeet(2);
                      trial+=1;
                  }
                  else if(round==3){//round3发射3种飞碟
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
                      if(round==3)//结束游戏
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
      ```

      * launchSkeet：发射飞碟，获取飞碟后设置发射参数

      ```c#
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
           actionManager.Fly(skeet,angle,power);
       }
      ```

      * Hit：检测射线是否与飞碟碰撞，碰撞就计分并回收飞碟

      ```c#
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
      ```

  * Views

    * UserGUI：实现GUI界面

    ```c#
    public class UserGUI : MonoBehaviour{
        private IUserAction action;
        GUIStyle text_style=new GUIStyle();
        private bool start=false;
    
        void Start (){
            action=SSDirector.getInstance().currentSceneController as IUserAction;
        }
    	
    	void OnGUI (){
            text_style.normal.textColor=new Color(0,0,0,1);
            text_style.fontSize=16;
    
            if (start){
                GUI.Label(new Rect(Screen.width-150,13,200,50),"分数:"+ action.GetScore().ToString(),text_style);
                GUI.Label(new Rect(100,13,50,50),"Round:"+action.GetRound().ToString(),text_style);
                GUI.Label(new Rect(180,13,50,50),"Trial:"+action.GetTrial().ToString(),text_style);
    
                if (action.GetRound()==3&&action.GetTrial()==10){
                    GUI.Label(new Rect(Screen.width/2-40,Screen.height/2,100,100),"游戏结束",text_style);
                    GUI.Label(new Rect(Screen.width/2-40,Screen.height/2-100,50,50),"你的分数:"+action.GetScore().ToString(),text_style);
                    if (GUI.Button(new Rect(Screen.width/2-40,Screen.height/2-200,100,50),"重新开始")){
                        action.ReStart();
                        return;
                    }
                    action.GameOver();
                }
            }
            else{
                GUI.Label(new Rect(Screen.width/2-50,100,100,100),"打飞碟",text_style);
                if (GUI.Button(new Rect(Screen.width/2-50,200,100,50),"游戏开始")){
                    start=true;
                    action.ReStart();
                }
            }
        }
       
    }
    ```



效果图：

[![BMW2TJ.png](https://s1.ax1x.com/2020/10/27/BMW2TJ.png)](https://imgchr.com/i/BMW2TJ)



[![BMfwHe.gif](https://s1.ax1x.com/2020/10/27/BMfwHe.gif)](https://imgchr.com/i/BMfwHe)