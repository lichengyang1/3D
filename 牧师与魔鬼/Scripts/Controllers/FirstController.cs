using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour,ISceneController,IUserAction{
    public Land rightLand;          
    public Land leftLand;
    public River river;
    public Boat boat;
    public Role[] roles;
    private UserGUI GUI;

    void Awake(){
        SSDirector director=SSDirector.getInstance();
        director.currentSceneController=this;
        GUI=gameObject.AddComponent<UserGUI>() as UserGUI;
        LoadResources();
    }

    public int GetEmptyPositionOnLand(int type){
        Land t=(type==1)?rightLand:leftLand;
        for(int i=0;i<6;i++){
            if(t.roles[i]==null)
                return i;
        }
        return -1;
    }
    public int GetEmptyPositionInBoat(int type){
        Vector3[] tmp=(type==1)?boat.rightPos:boat.leftPos;
        for(int i=0;i<2;i++){
            if(boat.roles[i]==null)
                return i;
        }
        return -1;
    }
    public int EmptyFullBoat(){
        if(boat.roles[0]==null&&boat.roles[1]==null)
            return 0;
        else if(boat.roles[0]!=null&&boat.roles[1]!=null)
            return 2;
        else
            return 1;      
    }
    public void LoadResources(){
        river=new River();
        rightLand=new Land("left",1);
        leftLand=new Land("right",0);
        boat=new Boat();
        roles=new Role[6];
        for(int i=0;i<3;i++){
            Role role=new Role(rightLand.positions[GetEmptyPositionOnLand(1)],true);
            role.role.name=("priest" + i);
            rightLand.roles[GetEmptyPositionOnLand(1)]=role;
            roles[i]=role;
        }
        for(int i=0;i<3;i++){
            Role role=new Role(rightLand.positions[GetEmptyPositionOnLand(1)],false);
            role.role.name=("devil" + i);
            rightLand.roles[GetEmptyPositionOnLand(1)]=role;
            roles[i+3]=role;
        }
    }

    public void MoveBoat(){
        bool t=boat.isRight;
    //   Debug.Log(EmptyFullBoat());
        if(EmptyFullBoat()==0||GUI.sign!=0){
            Click.state=0;//移动失败后复原state
            return;
        }           
        if(boat.isRight==false) 
            boat.move.MovePosition(new Vector3(3.5f,0.5f,0));
        else 
            boat.move.MovePosition(new Vector3(-3.5f,0.5f,0));
        boat.isRight=(t==true?false:true);
        GUI.sign=Check();
    }
    public void deleteRoleInBoat(string name){
        for(int i=0;i<boat.roles.Length;i++){
            if(boat.roles[i]!=null&&boat.roles[i].role.name==name){
                boat.roles[i]=null;
            }
        }      
    }
    public void deleteRoleOnLand(string name,int type){
        Land t=(type==1)?rightLand:leftLand;
        for(int i=0;i<6;i++){
            if(t.roles[i]!=null&&t.roles[i].role.name==name){
                t.roles[i]=null;
            }
        }      
    }
    public void MoveRole(Role role){
        if(GUI.sign!=0) 
            return;
        if(role.isInBoat){
            if(boat.isRight==true){
                deleteRoleInBoat(role.role.name);
                role.move.MovePosition(rightLand.positions[GetEmptyPositionOnLand(1)]);
                role.role.transform.parent=null;
                role.isInBoat=false;
                role.isRight=true;
                rightLand.roles[GetEmptyPositionOnLand(1)]=role;
            }
            else{
                deleteRoleInBoat(role.role.name);
                role.move.MovePosition(leftLand.positions[GetEmptyPositionOnLand(0)]);
                role.role.transform.parent=null;
                role.isInBoat=false;
                role.isRight=false;
                leftLand.roles[GetEmptyPositionOnLand(0)]=role;
            }       
        }
        else{
            if(role.isRight==true){
                if(EmptyFullBoat()==2||rightLand.isRight!= boat.isRight){
                    Click.state=0; //移动失败后复原state
                    return;
                } 
                deleteRoleOnLand(role.role.name,1);
                role.move.MovePosition(boat.rightPos[GetEmptyPositionInBoat(1)]);
                role.role.transform.parent=boat.boat.transform;
                role.isInBoat=true;
                boat.roles[GetEmptyPositionInBoat(1)]=role;
            }
            else{
                if(EmptyFullBoat()==2||leftLand.isRight!=boat.isRight){
                    Click.state=0; //移动失败后复原state
                    return;
                } 
                deleteRoleOnLand(role.role.name,0);
                role.move.MovePosition(boat.leftPos[GetEmptyPositionInBoat(0)]);
                role.role.transform.parent=boat.boat.transform;
                role.isInBoat=true;
                boat.roles[GetEmptyPositionInBoat(0)]=role;
            }
        }
        GUI.sign=Check();
    }

    public void Restart(){
        GUI.sign=0;
        Click.state=0;
        SceneManager.LoadScene("Untitled");
    }

    public int Check(){
        int[] count1 ={0,0};
        int[] count2 ={0,0};
        int[] count3 ={0,0};
        for(int i=0;i<6;i++){
            if(rightLand.roles[i]!=null ){
                if(rightLand.roles[i].isPriest==true) 
                    count1[0]++;
                else 
                    count1[1]++;
            }
            if(leftLand.roles[i]!=null){
                if(leftLand.roles[i].isPriest==true) 
                    count2[0]++;
                else 
                    count2[1]++;
            }
            if(i<2&&boat.roles[i]!=null){
                if(boat.roles[i].isPriest==true) 
                    count3[0]++;
                else 
                    count3[1]++;
            }
        }
        int leftPriests=count1[0];
        int leftDevils=count1[1];
        int rightPriests=count2[0];
        int rightDevils=count2[1];
        if(rightPriests+rightDevils==6) 
            return 2;
        if(boat.isRight==true){
            leftPriests+=count3[0];
            leftDevils+=count3[1];
        }
        else{
            rightPriests+=count3[0];
            rightDevils+=count3[1];
        }
        if((rightPriests>0&&rightPriests<rightDevils)||(leftPriests>0&&leftPriests<leftDevils)){
            return 1;
        }
        return 0;
    }
}