using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(1f, 0f, 0f), 90, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
