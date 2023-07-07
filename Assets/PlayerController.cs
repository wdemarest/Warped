using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Crawler crawler;

    public float moveSpeed = 0.5f;
    public float turnSpeed = 90f;

    float moveDistRemaining;



    // Start is called before the first frame update
    void Start()
    {
        crawler = GetComponent<Crawler>();
        moveDistRemaining = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        timeToNextMove -= Time.deltaTime;
        if(timeToNextMove <= 0){
            timeToNextMove += moveDelay;
            Move(moveSpeed);
        }
        */
        if(Input.GetKey(KeyCode.UpArrow)){
            Move(moveSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
        }
    }

    void Move(float dist){
        moveDistRemaining = dist;

        //ForLoop is a failsafe to prevent infinite loops
        for(int i = 0; i < 100; i++){
            if(moveDistRemaining > 0){
                moveDistRemaining -= crawler.Step(moveDistRemaining);
            }
        }
    }
}
