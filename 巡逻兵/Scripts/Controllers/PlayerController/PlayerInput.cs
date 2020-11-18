using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour{
    public string keyUp="w";
    public string keyDown="s";
    public string keyLeft="a";
    public string keyRight="d";
    public string keyRun="left shift";
    public string keyJUp="up";
    public string keyJDown="down";
    public string keyJLeft="left";
    public string keyJRight="right";
    public float Dup;
    public float Dright;
    public float Dmag;//方向
    public Vector3 Dvec;//速度
    public float Jup;
    public float Jright;
    public bool run;
    private float targetDup;
    private float targetDright;
    private float velocityDup;
    private float velocityDright;

    void Start(){}

    void Update(){
        Jup=(Input.GetKey(keyJUp))?1.0f:0-(Input.GetKey(keyJDown)?1.0f:0);
        Jright=(Input.GetKey(keyJRight))?1.0f:0-(Input.GetKey(keyJLeft)?1.0f:0);

        targetDup=(Input.GetKey(keyUp)?1.0f:0)-(Input.GetKey(keyDown)?1.0f:0);
        targetDright=(Input.GetKey(keyRight)?1.0f:0)-(Input.GetKey(keyLeft)?1.0f:0);

        //平滑变动
        Dup=Mathf.SmoothDamp(Dup,targetDup,ref velocityDup,0.1f);
        Dright=Mathf.SmoothDamp(Dright,targetDright,ref velocityDright,0.1f);
        
        Vector2 tempDAxis=SquareToCircle(new Vector2(Dup,Dright));
        float Dup2=tempDAxis.x;
        float Dright2=tempDAxis.y;

        Dmag=Mathf.Sqrt((Dup2*Dup2)+(Dright2*Dright2));
        Dvec=Dright*transform.right+Dup*transform.forward;
        run=Input.GetKey(keyRun);
    }

    /*矩形坐标转圆坐标*/
    private Vector2 SquareToCircle(Vector2 input){
        Vector2 output=Vector2.zero;
        output.x=input.x*Mathf.Sqrt(1-(input.y*input.y)/2.0f);
        output.y=input.y*Mathf.Sqrt(1-(input.x*input.x)/2.0f);
        return output;
    }
}
