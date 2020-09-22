# 简答题

## 解释 游戏对象（GameObjects） 和 资源（Assets）的区别与联系。

* **区别：**  
  * 游戏对象：游戏中的每个对象都是游戏对象，但其本身并不能执行任何操作。在向游戏对象提供属性后，游戏对象才能成为角色、环境或特效。
  * 资源：构造游戏对象、装饰游戏对象、配置游戏的物体和数据。
* **联系：** 资源可以作为属性添加到游戏对象上赋予其功能，而游戏对象可被封装为资源来复用

</br>

## 下载几个游戏案例，分别总结资源、对象组织的结构（指资源的目录组织结构与游戏对象树的层次结构）

* **资源结构：** 在网上下载一个天天酷跑游戏的demo源码，目录结构如下，包含了动画、图片、预设、场景以及脚本

    ![w5O8OA.png](https://s1.ax1x.com/2020/09/19/w5O8OA.png)

</br>

* **对象组织的目录结构：** 包含摄像头、玩家、背景、画布等对象

    ![w5OJeI.png](https://s1.ax1x.com/2020/09/19/w5OJeI.png)

</br>

## 编写一个代码，使用 debug 语句来验证 MonoBehaviour 基本行为或事件触发的条件

* 基本行为包括 Awake() Start() Update() FixedUpdate() LateUpdate()
* 常用事件包括 OnGUI() OnDisable() OnEnable()

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
    }

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }

    private void LateUpdate()
    {
        Debug.Log("LateUpdate");
    }

    private void OnGUI()
    {
        Debug.Log("OnGUI");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }
}
```

</br>

运行结果：

[![w5X89U.png](https://s1.ax1x.com/2020/09/19/w5X89U.png)](https://imgchr.com/i/w5X89U)

</br>

## 查找脚本手册，了解 GameObject，Transform，Component 对象

* **分别翻译官方对三个对象的描述（Description）**
  * **GameObject：** Unity场景中所有实体的基类
  * **Transform：** 对象的位置，旋转和比例
  * **Component：** 所有与游戏对象关联的事物的基类
* **描述下图中 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件**
  * **本题目要求是把可视化图形编程界面与 Unity API 对应起来，当你在 Inspector 面板上每一个内容，应该知道对应 API。**
    * table 的对象是 GameObject
    * 第一个选择框是activeSelf：定义对象的名称，static等属性
    * 第二个选择框是Transform：定义对象的位置、旋转角度、大小
    * 第三个选择框是Box Collider：调整坐标系的位置、大小
    * 第四个选择框是Component：赋予对象行为
  * **用 UML 图描述 三者的关系（请使用 UMLet 14.1.1 stand-alone版本出图）**
  
    [![w5xygU.png](https://s1.ax1x.com/2020/09/19/w5xygU.png)](https://imgchr.com/i/w5xygU)

</br>

## 资源预设（Prefabs）与 对象克隆 (clone)

* **预设（Prefabs）有什么好处？**

  * 预设主要用于资源重用，当需要重复用到某一个对象时，可以通过预设生成一个模板，然后直接从资源里加载，方便了游戏设计过程。

* **预设与对象克隆 (clone or copy or Instantiate of Unity Object) 关系？**
  * 预设的修改对后续产生的对象都会产生影响，而克隆产生的对象是相互独立的，修改某个克隆体不对其他克隆体产生影响

* **制作 table 预制，写一段代码将 table 预制资源实例化成游戏对象**

  * 代码：

    ```c#
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class init : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Object temp = Resources.Load("table");
            GameObject cube = Instantiate(temp) as GameObject;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
    ```

  * 运行截图：

    ![wIpo80.png](https://s1.ax1x.com/2020/09/19/wIpo80.png)

</br></br></br>

# 简单计算器

* **实验内容**：使用IMGUI实现一个简单计算器，该计算器只能同时进行2个操作数的计算，支持4种基本算术运算、清空和退格功能

* **工程部署**：计算器实现难度不高，可直接在OnGUI中实现，故只有一个脚本文件script,直接挂载到空对象上运行即可

* **实现思路**：

  * 通过OnGUI函数来生成UI元素，该函数将在每帧执行，并绘制到屏幕上。在整个过程中，所有对象都是即时生成的，没有其他持久性游戏对象。

  * 对于一个计算器而言，UI元素包括按钮和显示框。对应到IMGUI中的控件为button和label。此外还使用了box控件来界定计算器的边界。

  * 通过if语句和控件声明语句的组合来实现按钮点击的逻辑，例如if(GUI.Button(new Rect(...),"1"))。当游戏运行并单击 Button时，此if语句返回true并执行if代码块中的所有代码。由于每帧都会调用 OnGUI() 代码，因此无需显式创建和销毁 GUI 控件。完成所有控件的逻辑即完成了计算器的实现。

* **实现过程**：
  * 声明公共变量：
    * str1,str2:两个操作数
    * operator:操作符
    * temp:计算结果
    * result:显示结果
    * style:改变样式
  ```c#
      public static string str1;//第一个操作数
      public static string str2;//第二个操作数
      public static string Operator;//操作符
      public string result="0";//显示结果
      float temp=0;//计算结果
      public GUIStyle style=new GUIStyle();//用于改变样式
  ```

  * 控件：
    * box:划分边界，用来装下整个计算器
    * label：显示结果，显示文本为result
    * button：输入操作数和操作符
      * 当输入数字时，将其附加到str1尾部并更新result
      * 当输入+-*/时，更新operator并将str1赋给str2然后清空str1
      * 当输入CE时，全部变量重置
      * 当输入←时，若str1为空，则result显示0，否则退格
      * 当输入=时，进行计算并更新result

  ```c#
      void OnGUI(){

          int m=Screen.width/2;
          GUI.Box(new Rect(m-200,50,400,600),"简单计算器");
          GUI.Label(new Rect(m-190, 75, 400, 50),result);   
          //输入操作数
          if(GUI.Button(new Rect(m,300,100,100),"1")){
              str1+="1";
              result=str1;
          }

          if(GUI.Button(new Rect(m-100,300,100,100),"2")){
              str1+="2";
              result=str1;
          }

          if(GUI.Button(new Rect(m-200,300,100,100),"3")){
              str1+="3";
              result=str1;
          }

          if(GUI.Button(new Rect(m,200,100,100),"4")){
              str1+="4";
              result=str1;
          }

          if(GUI.Button(new Rect(m-100,200,100,100),"5")){
              str1+="5";
              result=str1;
          }

          if(GUI.Button(new Rect(m-200,200,100,100),"6")){
              str1+="6";
              result=str1;
          }

          if(GUI.Button(new Rect(m,100,100,100),"7")){
              str1+="7";
              result=str1;
          }

          if(GUI.Button(new Rect(m-100,100,100,100),"8")){
              str1+="8";
              result=str1;
          }

          if(GUI.Button(new Rect(m-200,100,100,100),"9")){
              str1+="9";
              result=str1;
          }

          if(GUI.Button(new Rect(m-100,400,100,100),"0")){
              str1+="0";
              result=str1;
          }

          //输入操作符
          if(GUI.Button(new Rect(m+100,100,100,100),"+")){
              Operator="+";
              if(str1!=""){
                  str2=str1;
              }
              str1="";
              result=str2;
          }

          if(GUI.Button(new Rect(m+100,200,100,100),"-")){
              Operator="-";
              if(str1!=""){
                  str2=str1;
              }        
              str1="";
              result=str2;
          }        

          if(GUI.Button(new Rect(m+100,300,100,100),"*")){
              Operator="*";
              if(str1!=""){
                  str2=str1;
              }
              str1="";
              result=str2;
          }  

          if(GUI.Button(new Rect(m+100,400,100,100),"/")){
              Operator="/";
              if(str1!=""){
                  str2=str1;
              }
              str1="";
              result=str2;
          }

          if(GUI.Button(new Rect(m,400,100,100),"←")){
              if(str1==""){
                  result="0";
              }
              else{
                  str1=str1.Substring(0,str1.Length-1);
                  result=str1;
              }         
          }

          if(GUI.Button(new Rect(m-200,400,100,100),"CE")){
              str1="";
              str1="";
              temp=0;
              result="0";
          }

          if(GUI.Button(new Rect(m-200,500,400,50),"=")){
              if(Operator=="+"){
                  temp=float.Parse(str2)+float.Parse(str1);
              }
              else if(Operator=="-"){
                  temp=float.Parse(str2)-float.Parse(str1);
              }
              else if(Operator=="*"){
                  temp=float.Parse(str2)*float.Parse(str1);
              }
              else if(Operator=="/"){
                  temp=float.Parse(str2)/float.Parse(str1);
              }    
              str1="";
              str2=temp.ToString();         
              result=temp.ToString();
          }     
  ```

* **遇到的问题**
  * 项目中遇到最大的问题就是样式问题
    * 默认样式的字体太小，很难看清，因此需要调大字体，而对于样式的其他方面并不需要修改。因此最初代码如下

    ```c#
    GUIStyle style=new GUIStyle();
    style.fonSize=20;
    ```

    * 但new GUIStyle()中所有样式参数为空，我只设定了字体参数，还需要根据需求设置hover等参数。于是代码修改如下

    ```c#
    GUIStyle style=new GUIStyle();
    style.normal.textColor=Color.white;
    style.hover.textColor=Color.red;
    style.fonSize=20;
    ```

    * 应用之后发现，只有hover修改无效且控制台无错误信息。通过查阅资料和改为onHover等方式都没有找到原因，至今仍未解决
  * 解决方法
    * 方法一：在默认样式的基础上进行修改，代码如下
    ```c#
      GUIStyle s=GUI.skin.button;//获得button的默认样式
      s.fontSize=20;
    ```
    * 方法二：声明公共变量，在编辑界面设置hover等样式
    ```c#
      public GUIStyle style=new GUIStyle();//用于改变样式
    ```

* 代码： <https://github.com/lichengyang1/3D/tree/master/%E7%AE%80%E5%8D%95%E8%AE%A1%E7%AE%97%E5%99%A8>

* 成果展示：

![wLd57j.gif](https://s1.ax1x.com/2020/09/22/wLd57j.gif)