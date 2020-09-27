# 简答题

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
