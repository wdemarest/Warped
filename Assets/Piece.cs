using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    Collider SpaceCollider;
    public Vector3 SpacePos;
    
    // Start is called before the first frame update
    void Start()
    {
        SpaceCollider = GameObject.Find("Space").GetComponent<Collider>();
        Debug.Log(SpaceCollider);
    }

    // Update is called once per frame
    void Update()
    {
        SpacePos = FindPointOnSpace(transform.position);

        Debug.Log(SpacePos);

        transform.position = SpacePos;
    }

    Vector3 FindPointOnSpace(Vector3 location){
        return SpaceCollider.ClosestPoint(location);
    }
}
