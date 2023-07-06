using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public GameObject SpaceObject;
    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(SpaceObject.transform.position, Vector3.up, 60 * Time.deltaTime);
    }
}
