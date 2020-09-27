using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public Transform Sun;
    public Transform Mercury;
    public Transform Venus;
    public Transform Earth;
    public Transform Mars;
    public Transform Jupiter;
    public Transform Saturn;
    public Transform Uranus;
    public Transform Neptune;
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        //自转
        Sun.Rotate(Vector3.up * Time.deltaTime*2);
        Earth.Rotate(Vector3.up * Time.deltaTime * 20);
        Mercury.Rotate(Vector3.up * Time.deltaTime * 20);
        Venus.Rotate(Vector3.up * Time.deltaTime * 20);
        Mars.Rotate(Vector3.up * Time.deltaTime * 40);
        Jupiter.Rotate(Vector3.up * Time.deltaTime * 60);
        Saturn.Rotate(Vector3.up * Time.deltaTime * 100);
        Uranus.Rotate(Vector3.up * Time.deltaTime * 150);
        Neptune.Rotate(Vector3.up * Time.deltaTime * 150);
        //公转
        Mercury.RotateAround(Sun.transform.position, new Vector3(1,1, 0), 10 * Time.deltaTime);
        Venus.RotateAround(Sun.transform.position, new Vector3(2,3, 0), 30 * Time.deltaTime);
        Earth.RotateAround(Sun.transform.position, new Vector3(5, -10, 0), -30 * Time.deltaTime);
        Mars.RotateAround(Sun.transform.position, new Vector3(3, 10, 0), 24 * Time.deltaTime);
        Jupiter.RotateAround(Sun.transform.position, new Vector3(3, -10, 0), -15 * Time.deltaTime);
        Saturn.RotateAround(Sun.transform.position, new Vector3(7, -10, 0), -10 * Time.deltaTime);
        Uranus.RotateAround(Sun.transform.position, new Vector3(3, 10, 0), 5 * Time.deltaTime);
        Neptune.RotateAround(Sun.transform.position, new Vector3(5, -10, 0), -10 * Time.deltaTime);
    }
}
