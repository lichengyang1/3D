# 作业

实验内容：实现粒子光环

- 实验要求
  - 参考 `http://i-remember.fr/en` 这类网站，使用粒子流编程控制制作一些效果， 如“粒子光环”
- 游戏设计要求：
  - 模仿`http://i-remember.fr/en`首页的效果制作一个粒子光环。由于该网站打不开，所以只能使用别人的截图。目标效果图如下

    ![](https://img-blog.csdnimg.cn/20191105141114569.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L1RpZmluaXR5,size_16,color_FFFFFF,t_70)

  - 由两个粒子环组成，分别往不同方向转动，此外环的一部分亮度更高并以更快的速度在旋转
  - 鼠标移动到中间的按钮上光环会迅速收缩，鼠标移开后又恢复原状


</br>

- 工程部署：
  - `ParticleHalo.cs`：控制粒子参数、碰撞检测及收缩操作

</br>

- 预制结构：

[![Dshlhd.png](https://s3.ax1x.com/2020/11/28/Dshlhd.png)](https://imgchr.com/i/Dshlhd)

    - ParticleHalo为空对象，其下有3个子空对象
    - outer为外光环，顺时针转动，有组件`Particle System`，挂载脚本`ParticleHalo.cs`
    - inner为内光环，逆时针转动，有组件`Particle System`，挂载脚本`ParticleHalo.cs`
    - sensor用于鼠标碰撞检测，有组件球形碰撞器


</br>

- 实现过程：
  - 定义结构CirclePosition，用来记录每个粒子的当前半径、角度和时间，其中时间是做游离运动需要的

    ```c#
    public class CirclePosition
    {
        public float radius = 0f, angle = 0f, time = 0f;
        public CirclePosition(float radius, float angle, float time)
        {
            this.radius = radius;   // 半径
            this.angle = angle;     // 角度
            this.time = time;       // 时间
        }
    }
    ```

  - ParticleHalo的基本变量

    ```c#
    private ParticleSystem particleSys;  // 粒子系统
    private ParticleSystem.Particle[] particleArr;  // 粒子数组
    private CirclePosition[] circle; // 极坐标数组
    public int count = 10000;       // 粒子数量
    public float size = 0.03f;      // 粒子大小
    public float minRadius = 5.0f;  // 最小半径
    public float maxRadius = 12.0f; // 最大半径
    public bool clockwise = true;   // 顺时针|逆时针
    public float speed = 2f;        // 速度
    public float pingPong = 0.02f;  // 游离范围
    ```

    相关初始化

    ```c#
    void Start ()
    {   // 初始化粒子数组
        particleArr = new ParticleSystem.Particle[count];
        circle = new CirclePosition[count];
        // 初始化粒子系统
        particleSys = this.GetComponent<ParticleSystem>();
        var main = particleSys.main;
        main.startSpeed = 0;
        main.startSize = size;
        main.loop = false;
        main.maxParticles = count;                                  // 粒子数量               
        particleSys.Emit(count);                                    // 发射粒子
        particleSys.GetParticles(particleArr);
        RandomlySpread();   // 初始化各粒子位置
    }
    ```

  - `RandomlySpread()`：将所有的粒子随机分布在圆圈轨道上

    ```c#
    void RandomlySpread()
    {
        for (int i = 0; i < count; ++i)
        {   // 随机每个粒子距离中心的半径，同时希望粒子集中在平均半径附近
            float midRadius = (maxRadius + minRadius) / 2;
            float minRate = Random.Range(1.0f, midRadius / minRadius);
            float maxRate = Random.Range(midRadius / maxRadius, 1.0f);
            float radius = Random.Range(minRadius * minRate, maxRadius * maxRate);
 
            // 随机每个粒子的角度
            float angle = Random.Range(0.0f, 360.0f);
            float theta = angle / 180 * Mathf.PI;
 
            // 随机每个粒子的游离起始时间
            float time = Random.Range(0.0f, 360.0f);
 
            circle[i] = new CirclePosition(radius, angle, time);
 
            particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));
        }
 
        particleSys.SetParticles(particleArr, particleArr.Length);
    }
    ```

  - 此时粒子还不能移动，因此`Updata()`中更新粒子的角度

    ```c#
    void Update()
    {
       for (int i = 0; i < count; i++)
        {
            if (clockwise)  // 顺时针旋转
                circle[i].angle -= 0.1f;
            else            // 逆时针旋转
                circle[i].angle += 0.1f;

            // 保证angle在0~360度
            circle[i].angle = (360.0f + circle[i].angle) % 360.0f;
            float theta = circle[i].angle / 180 * Mathf.PI;

            particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));

        }
        particleSys.SetParticles(particleArr, particleArr.Length);
    }
    ```

    虽然这样粒子能进行移动，但每个粒子角度添加的增量都是一样的，使得看起来像是一张图片在旋转，而不是每个粒子在运动。因此添加速度差分层变量，所有粒子分成了不同组，每组角度增量不一样

    ```c#
    private int tier = 10;  // 速度差分层数
    void Update()
    {
       for (int i = 0; i < count; i++)
        {
            if (clockwise)  // 顺时针旋转
                circle[i].angle -= (i % tier + 1) * (speed / circle[i].radius / tier);
            else            // 逆时针旋转
                circle[i].angle += (i % tier + 1) * (speed / circle[i].radius / tier);
 
            // 保证angle在0~360度
            circle[i].angle = (360.0f + circle[i].angle) % 360.0f;
            float theta = circle[i].angle / 180 * Mathf.PI;

            particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));

        }
        particleSys.SetParticles(particleArr, particleArr.Length);
    }
    ```

  -  为了使粒子运动的半径在一定范围内波动，形成粒子带，使用了`Mathf`类提供的方法`PingPong`。`PingPong`函数使值在范围内波动。在`Updata()`中添加如下语句

        ```c#
        // 粒子在半径方向上游离
        circle[i].time += Time.deltaTime;
        circle[i].radius += Mathf.PingPong(circle[i].time / minRadius / maxRadius, pingPong) - pingPong / 2.0f;
        ```

  - 接下来改变粒子的光效。改变透明度可以使用Gradient类。

    ```c#
    public Gradient colorGradient;
    void Start ()
    {   // 初始化粒子数组
        particleArr = new ParticleSystem.Particle[count];
        circle = new CirclePosition[count];
        // 初始化粒子系统
        particleSys = this.GetComponent<ParticleSystem>();
        var main = particleSys.main;
        main.startSpeed = 0;
        main.startSize = size;
        main.loop = false;
        main.maxParticles = count;                                  // 粒子数量               
        particleSys.Emit(count);                                    // 发射粒子
        particleSys.GetParticles(particleArr);
 
        // 初始化梯度颜色控制器
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];
        alphaKeys[0].time = 0.0f; alphaKeys[0].alpha = 1.0f;
        alphaKeys[1].time = 0.4f; alphaKeys[1].alpha = 0.4f;
        alphaKeys[2].time = 0.6f; alphaKeys[2].alpha = 1.0f;
        alphaKeys[3].time = 0.9f; alphaKeys[3].alpha = 0.4f;
        alphaKeys[4].time = 1.0f; alphaKeys[4].alpha = 0.9f;
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].time = 0.0f; colorKeys[0].color = Color.white;
        colorKeys[1].time = 1.0f; colorKeys[1].color = Color.white;
        colorGradient.SetKeys(colorKeys, alphaKeys);
 
        RandomlySpread();   // 初始化各粒子位置
    }
    void ChangeColor() {
        float colorValue;
        for (int i = 0; i < count; i++) {
            //改变颜色
            colorValue = (Time.realtimeSinceStartup - Mathf.Floor(Time.realtimeSinceStartup))/2;
            colorValue += circle[i].angle / 360;
            if (colorValue > 1) colorValue -= 1;
            particleArr[i].startColor = colorGradient.Evaluate(colorValue);
        }
    }
    // Update is called once per frame
    void Update()
    {
       for (int i = 0; i < count; i++)
        {
            if (clockwise)  // 顺时针旋转
                circle[i].angle -= (i % tier + 1) * (speed / circle[i].radius / tier);
            else            // 逆时针旋转
                circle[i].angle += (i % tier + 1) * (speed / circle[i].radius / tier);
 
            // 保证angle在0~360度
            circle[i].angle = (360.0f + circle[i].angle) % 360.0f;
            float theta = circle[i].angle / 180 * Mathf.PI;

            // 粒子在半径方向上游离
            circle[i].time += Time.deltaTime;
            circle[i].radius += Mathf.PingPong(circle[i].time / minRadius / maxRadius, pingPong) - pingPong / 2.0f;

            particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));

            particleArr[i].startColor = colorGradient.Evaluate(circle[i].angle / 360.0f);
        }
        ChangeColor();
        particleSys.SetParticles(particleArr, particleArr.Length);

    }
    ```

  - 粒子光环收缩

    ```c#
    //增加成员
    private float[] before;                                         // 收缩前粒子位置
    private float[] after;                                          // 收缩后粒子位置
    public float shrinkSpeed = 5f;                                  // 粒子缩放的速度
    private bool ischange = false;                                  // 是否收缩

    //初始化RandomlySpread
    before[i] = radius;
    after[i] = 0.7f * radius;
    if (after[i] < minRadius * 1.1f) {
        after[i] = Random.Range(Random.Range(minRadius, midRadius), (minRadius * 1.1f));
    }

    //更新Update
    if (ischange) {
        // 开始收缩
        if (circle[i].radius > after[i]) {
            circle[i].radius -= shrinkSpeed * (circle[i].radius / after[i]) * Time.deltaTime;
        }
    }
    else {
        // 开始还原
        if (circle[i].radius < before[i]) {
            circle[i].radius += shrinkSpeed * (before[i] / circle[i].radius) * Time.deltaTime;
        }
    }
    ```

  - 鼠标碰撞检测

    ```c#
    //变量更新
    public Camera camera;                                           // 主摄像机
    private Ray ray;                                                // 射线
    private RaycastHit hit;

    //Update()
    // 碰撞检测
    ray = camera.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "button") ischange = true;
    else ischange = false;
    ```

    记得在脚本挂载时设置摄像机对象

    [![Dsqbr9.png](https://s3.ax1x.com/2020/11/28/Dsqbr9.png)](https://imgchr.com/i/Dsqbr9)

</br>


效果图：

[![DsHbz4.gif](https://s3.ax1x.com/2020/11/28/DsHbz4.gif)](https://imgchr.com/i/DsHbz4)

一些问题和细节
* 在创建`Particle System`时，一开始使用的是在空对象下添加粒子系统组件，这样创建的粒子系统没有材质，使得不能进行特效方面的修改
  * 解决：直接创建Effect->Particle System
* 为了使粒子显示效果更好，这里采用了纯黑背景
  * 将光源删除
  * 新建skybox，将`Exposure`设为0

    [![Dsbgk6.png](https://s3.ax1x.com/2020/11/28/Dsbgk6.png)](https://imgchr.com/i/Dsbgk6)

  * 打开Window->Rendering->Lighting

    [![DsqiNV.png](https://s3.ax1x.com/2020/11/28/DsqiNV.png)](https://imgchr.com/i/DsqiNV)

    使用刚创建的sky

  * 这样背景就是纯黑了