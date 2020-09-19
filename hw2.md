# 简答题

解释 游戏对象（GameObjects） 和 资源（Assets）的区别与联系。

* 区别：  
  * 游戏对象：游戏中的每个对象都是游戏对象，但其本身并不能执行任何操作。在向游戏对象提供属性后，游戏对象才能成为角色、环境或特效。
  * 资源：构造游戏对象、装饰游戏对象、配置游戏的物体和数据。
* 联系：资源可以作为属性添加到游戏对象上赋予其功能，而游戏对象可被封装为资源来复用

下载几个游戏案例，分别总结资源、对象组织的结构（指资源的目录组织结构与游戏对象树的层次结构）

* 资源结构：



编写一个代码，使用 debug 语句来验证 MonoBehaviour 基本行为或事件触发的条件
* 基本行为包括 Awake() Start() Update() FixedUpdate() LateUpdate()
* 常用事件包括 OnGUI() OnDisable() OnEnable()

查找脚本手册，了解 GameObject，Transform，Component 对象
* 分别翻译官方对三个对象的描述（Description）
* 描述下图中 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件
  * 本题目要求是把可视化图形编程界面与 Unity API 对应起来，当你在 Inspector 面板上每一个内容，应该知道对应 API。
  * 例如：table 的对象是 GameObject，第一个选择框是 activeSelf 属性。
  * 用 UML 图描述 三者的关系（请使用 UMLet 14.1.1 stand-alone版本出图）

资源预设（Prefabs）与 对象克隆 (clone)
* 预设（Prefabs）有什么好处？
* 预设与对象克隆 (clone or copy or Instantiate of Unity Object) 关系？
* 制作 table 预制，写一段代码将 table 预制资源实例化成游戏对象