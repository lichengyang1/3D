using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour{
    public GameObject model;
    public PlayerInput input;
    public float walkSpeed=1.5f;
    public float runSpeed=2.7f;

    [SerializeField]
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 move;

    void Awake(){
        input=GetComponent<PlayerInput>();
        anim=model.GetComponent<Animator>();
        rigid=GetComponent<Rigidbody>();
    }

    void Update(){
        float targetRunSpeed=input.run?2.0f:1.0f;
        anim.SetFloat("forward",input.Dmag*Mathf.Lerp(anim.GetFloat("forward"),targetRunSpeed,0.3f));
        
        if(input.Dmag>0.01f){
            Vector3 targetForward=Vector3.Slerp(model.transform.forward, input.Dvec, 0.2f);
            model.transform.forward=targetForward;
        }
        move=input.Dmag*model.transform.forward*walkSpeed*(input.run?runSpeed:1.0f);
    }

    private void FixedUpdate(){
        rigid.velocity=new Vector3(move.x,rigid.velocity.y,move.z) ;
    }
}
