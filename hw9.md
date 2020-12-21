# 作业

实验内容：血条的预制设计

- 实验要求
  - 分别使用 IMGUI 和 UGUI 实现
  - 使用 UGUI，血条是游戏对象的一个子元素，任何时候需要面对主摄像机
  - 分析两种实现的优缺点
  - 给出预制的使用方法



* 实验过程

  * IMGUI

    * IMGUI实现较为简单，直接通过脚本IMGUI.cs实现

    * 使用`GUI.HorizontalScrollbar`来模拟血条，在脚本中计算出位置然后生成即可

    * 代码如下

      ```c#
      using UnityEngine;
      
      public class IMGUI : MonoBehaviour
      {
          public float health = 0.5f;
      
          void OnGUI()
          {
              //计算血条位置
              Vector3 worldPos = new Vector3(transform.position.x, transform.position.y -0.2f, transform.position.z);
              Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
              //生成HorizontalScrollbar
              GUI.HorizontalScrollbar(new Rect(new Rect(screenPos.x - 100, screenPos.y, 200, 20)), 0.0f, health, 0.0f, 1.0f);
          }
      }
      ```

    * 效果图：

      [![r0Td8f.gif](https://s3.ax1x.com/2020/12/21/r0Td8f.gif)](https://imgchr.com/i/r0Td8f)

    

  * UGUI

    * 制作血条

      * 这里导入Standard Assets中的预制ThirdPersonCharacter作为血条挂载的对象

      * 右键预制，创建对象UI->Canvas

      * 然后右键Canvas，创建对象UI->Slider

        [![r07liq.png](https://s3.ax1x.com/2020/12/21/r07liq.png)](https://imgchr.com/i/r07liq)

      * 删掉滑动柄Handle Slide Area，然后在Fill Area->Fill->Image->Color修改血条颜色

    * 调整血条

      * 现在可以发现Canvas->RectTransform属性为灰色，不能对幕布进行大小位置上的调整，而我们需要让血条面向摄像机且随人物移动

      * 幕布的Render Mode有三种选择

        * Screen Space - Overlay：一种默认的渲染模式，将UI元素放置在场景顶部渲染的屏幕，画布会自动更改大小匹配屏幕。此模式Canvas位置大小不可改变，Canvas的起始位置就是父物体的位置，大小和设置的显示屏幕大小有关。
        * Screen Space - Camera：此模式Canvas位置大小不可改变，需要指定UI相机定，可以将Main Camera挂载到上面观察效果，画布上的内容将一直显示在相机视野里。
        * World Space： 画布行为与场景中的其他任何对象一样，UI元素将放置在其他对象的前面或后面渲染，不对画布大小和位置进行限制。

      * 这里选择World Space并为事件摄像机指定Main Camera

        [![r0HVt1.png](https://s3.ax1x.com/2020/12/21/r0HVt1.png)](https://imgchr.com/i/r0HVt1)

      * 这时幕布可以进行调整了，修改血条的位置和比例使其合适

        [![r0HoNR.png](https://s3.ax1x.com/2020/12/21/r0HoNR.png)](https://imgchr.com/i/r0HoNR)

    * 实现面向摄像头

      * 添加脚本UGUI.cs

        ```c#
        using UnityEngine;
        
        public class UGUI : MonoBehaviour {
        
        	void Update () {
        		this.transform.LookAt (Camera.main.transform.position);
        	}
        }
        ```

    * 效果图：

      [![r0b6VH.gif](https://s3.ax1x.com/2020/12/21/r0b6VH.gif)](https://imgchr.com/i/r0b6VH)



优缺点分析

* IMGUI
  * 优点
    * 操作简单，使用方便
    * 容易理解，符合传统编程习惯，更加直观
  * 缺点
    * 每次都重新生成组件，效率低
    * 没有状态，实现其他功能如运动、动画等会很麻烦
* UGUI
  * 优点
    * 拥有UI状态，修改属性或实现其他功能更容易
    * 有锚点，可以实现屏幕自适应
    * 一次性生成UI组件，效率更高
  * 缺点
    * 性能上的提升就会使得操作更加复杂，需要整体布局，对各个UI组件进行配置



预制使用方法

* IMGUI：将IMGUI.cs直接挂到人物预制上
* GUI：将UGUI.cs挂到人物预制下的Canvas上