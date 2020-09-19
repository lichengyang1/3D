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
