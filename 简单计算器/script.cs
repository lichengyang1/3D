using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script : MonoBehaviour {
    public static string str1;//第一个操作数
    public static string str2;//第二个操作数
    public static string Operator;//操作符
    public string result="0";//显示结果
    float temp=0;//计算结果
    public GUIStyle style=new GUIStyle();//用于改变样式
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
    }
}