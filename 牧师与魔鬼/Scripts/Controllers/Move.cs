using UnityEngine;

public class Move : MonoBehaviour{
    float speed=10;
    int mode=0;
    Vector3 destination;
    Vector3 mid;
    void Update(){
        if(mode==1){
            transform.position=Vector3.MoveTowards(transform.position,mid,speed*Time.deltaTime);
            if(transform.position==mid){
                mode=2;
            }              
        }
        else if(mode==2){
            transform.position=Vector3.MoveTowards(transform.position,destination,speed*Time.deltaTime);
            if(transform.position==destination){
                mode=0;
                Click.state=0;
            }   
        }
    }
    public void MovePosition(Vector3 position){
        destination=position;
        if(position.y==transform.position.y){
            mode=2;
            return;
        }         
        else if(position.y<transform.position.y)
            mid=new Vector3(position.x,transform.position.y,position.z);
        else
            mid=new Vector3(transform.position.x,position.y,position.z);
        mode=1;
    }
}