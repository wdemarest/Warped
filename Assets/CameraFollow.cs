using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject desiredPosition;
    public float cameraMoveSpeed = 30f;
    public float cameraTurnSpeed = 30f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 targetPos = desiredPosition.transform.position;
        Vector3 myPos = transform.position;
        
        transform.position = Vector3.MoveTowards(myPos  , targetPos, cameraMoveSpeed * Time.deltaTime);
        transform.LookAt(player.transform);


        /*
        transform.RotateAround(playerPos, Vector3.Cross(myPos - playerPos, targetPos - playerPos), Vector3.Angle(myPos - playerPos, targetPos - playerPos) * Time.deltaTime * cameraMoveSpeed);
        */
        //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, playerPos - myPos, cameraTurnSpeed * Time.deltaTime, 0.0f));
        
    }
}
