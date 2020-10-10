# 作业

## 1、基本操作演练【建议做】

* 下载 Fantasy Skybox FREE， 构建自己的游戏场景
  * 天空盒
    * 导入Fantasy Skybox后，创建skybox,在6个面中放入合适的贴图

        ![061RER.png](https://s1.ax1x.com/2020/10/10/061RER.png)

    * 然后在Camera中添加部件Skybox，然后拖入，这样该摄像头就使用天空盒

        [![061z28.png](https://s1.ax1x.com/2020/10/10/061z28.png)](https://imgchr.com/i/061z28)

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
