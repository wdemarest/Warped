using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Crawler crawler;
    
    public float timeToNextMove = 0f;
    public float moveDelay = 1f;

    public float moveSpeed = 0.5f;

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
    }

    void Move(float dist){
        Debug.Log("=============STARING MOVE=============");
        //Debug.Log("Move Called " + dist);
        moveDistRemaining = dist;

        //ForLoop is a failsafe to prevent infinite loops
        for(int i = 0; i < 100; i++){
            if(moveDistRemaining > 0){
                //Debug.Log("Move Remaining: " + moveDistRemaining);
                moveDistRemaining -= crawler.Step(moveDistRemaining);
            }
        }
    }
}
