using UnityEngine;
public class Check:MonoBehaviour{
    public FirstController sceneController;

    protected void Start(){
        sceneController=(FirstController)SSDirector.getInstance().currentSceneController;
        sceneController.check=this;
    }

    public int CheckGame(){
        int[] count1 ={0,0};
        int[] count2 ={0,0};
        int[] count3 ={0,0};
        for(int i=0;i<6;i++){
            if(sceneController.rightLand.roles[i]!=null){
                if(sceneController.rightLand.roles[i].isPriest==true) 
                    count1[0]++;
                else 
                    count1[1]++;
            }
            if(sceneController.leftLand.roles[i]!=null){
                if(sceneController.leftLand.roles[i].isPriest==true) 
                    count2[0]++;
                else 
                    count2[1]++;
            }
            if(i<2&&sceneController.boat.roles[i]!=null){
                if(sceneController.boat.roles[i].isPriest==true) 
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
        if(sceneController.boat.isRight==true){
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
