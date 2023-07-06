using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour
{
    //Space Collider and Space Mesh
    public GameObject SpaceObject;

    public GameObject Marker;
    public GameObject Marker2;
    
    Mesh SM;
    public Vector3[] myTri;

    //=== TEMP VARIABLES===

    Vector3 intersection;

    //======================

    float raycastBackupDist = 0.01f;
    public float moveSpeed = 0.5f;

    Vector3 gizEdgePointA;
    Vector3 gizEdgePointB;
    Vector3 gizPointBehind;
    Vector3 gizPointAhead;


    
    // Start is called before the first frame update
    void Start()
    {
        SM = SpaceObject.GetComponent<MeshCollider>().sharedMesh;
        SetInitialMyTriHACKED();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void OnDrawGizmos(){
        if(Application.isPlaying){
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gizEdgePointA, gizEdgePointB);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * moveSpeed);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * -1);

            /*
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(gizEdgePointA, 0.01f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(gizEdgePointB, 0.01f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(gizPointBehind, 0.01f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gizPointAhead, 0.01f);
            */
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(myTri[0], 0.01f);
            Gizmos.DrawSphere(myTri[1], 0.01f);
            Gizmos.DrawSphere(myTri[2], 0.01f);

        }

        if(intersection != Vector3.zero){
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(intersection, 0.01f);
        }
    }

    

    public float Step(float moveDistRemaining){
        Debug.Log("<<<MOVING NEW STEP>>>");
        Vector3 targetPos = transform.position + transform.forward * moveDistRemaining;

        for(int i = 0; i < 3; i++){
            Vector3 edgePointA = myTri[i];
            Vector3 edgePointB = myTri[(i + 1) % 3];
            

            if(LineSegmentIntersection(out Vector3 intersection, edgePointA, edgePointB, transform.position, transform.position + transform.forward * moveDistRemaining) && intersection != transform.position){
                //Debug.Log("Intersection: " + intersection);
                //Debug.Log("My Position: " + transform.position);
                
                float distMoved = Vector3.Distance(transform.position, intersection);

                transform.position = intersection;

                Vector3 pointBehind = myTri[(i + 2) % 3];
                
                TurnOverEdge(edgePointA, edgePointB, pointBehind);

                //Debug.Log("Stopped Short After: " + distMoved);
                return distMoved;
            }
        }

        transform.position = targetPos;

        Debug.Log("Moved Full Remaining: " + moveDistRemaining);
        CheckOnSurfaceHACKED();
        return moveDistRemaining;
    }

    //Old versions of TurnOverEdge
    /*
    //V1
    void TurnOverEdge(Vector3 edgePointA, Vector3 edgePointB, Vector3 pointBehind){
        //Debug.Log("TurnOverEdge Called");
        Vector3? pointAheadNullable = FindPointAhead(edgePointA, edgePointB, pointBehind);

        if(pointAheadNullable == null){
            Debug.Log("No point ahead");
            return;
        }

        Vector3 pointAhead = (Vector3)pointAheadNullable;

        gizEdgePointA = edgePointA;
        gizEdgePointB = edgePointB;
        gizPointBehind = pointBehind;
        gizPointAhead = pointAhead;

        //Align points
        Vector3 adjustedPointAhead = pointAhead + Vector3.Project(edgePointB - pointAhead, edgePointB - edgePointA);
        Vector3 adjustedPointBehind = pointBehind + Vector3.Project(edgePointB - pointBehind, edgePointB - edgePointA);

        //Debug.Log("AngleFrom: " + (adjustedPointAhead - edgePointB));
        //Debug.Log("AngleTo: " + (adjustedPointBehind - edgePointB));

        float angleToTurn = 180 - Vector3.Angle(adjustedPointBehind - edgePointB, adjustedPointAhead - edgePointB);

        
        Debug.Log("CROSS PRODUCT: " + Vector3.Cross(edgePointA - adjustedPointBehind, edgePointB - adjustedPointBehind));
        Debug.Log("Up: " + transform.up);

        //If edgePointA is on the left
        if(Vector3.Angle(Vector3.Cross(edgePointA - adjustedPointBehind, edgePointB - adjustedPointBehind), transform.up) > 90){


            
            angleToTurn = -angleToTurn;
        }

        Debug.Log("------------> AngleToTurn: " + angleToTurn);
        if(angleToTurn == 0f){Debug.Log("-------------------------------------------0 Angle");}

        //Instantiate(Marker, edgePointA, Quaternion.identity);
        //Instantiate(Marker2, edgePointB, Quaternion.identity);

        

        //Debug.Log("pointAhead: " + pointAhead);

        //Debug.Log("Axis: " + (edgePointB - edgePointA));

        //Debug.Log("Prev Forward: " + transform.forward);
        transform.Rotate(edgePointB - edgePointA, angleToTurn, Space.World);
        

        //Debug.Log("New forward: " + transform.forward);
    }
    
    //V2
    void TurnOverEdge(Vector3 edgePointA, Vector3 edgePointB, Vector3 pointBehind){
        //Debug.Log("TurnOverEdge Called");
        Vector3? pointAheadNullable = FindPointAhead(edgePointA, edgePointB, pointBehind);

        if(pointAheadNullable == null){
            Debug.Log("No point ahead");
            return;
        }

        Vector3 pointAhead = (Vector3)pointAheadNullable;

        gizEdgePointA = edgePointA;
        gizEdgePointB = edgePointB;
        gizPointBehind = pointBehind;
        gizPointAhead = pointAhead;

        

        //Align points
        Vector3 adjustedPointAhead = pointAhead + Vector3.Project(transform.position - pointAhead, edgePointB - edgePointA);
        Vector3 adjustedPointBehind = pointBehind + Vector3.Project(transform.position - pointBehind, edgePointB - edgePointA);

        //Debug.Log("AngleFrom: " + (adjustedPointAhead - edgePointB));
        //Debug.Log("AngleTo: " + (adjustedPointBehind - edgePointB));

        Vector3 edgePointOnMyLeft;
        //If edgePointA is on my left
        if(Vector3.Angle(Vector3.Cross(transform.forward, edgePointA - adjustedPointBehind), transform.up) < 90){
            edgePointOnMyLeft = edgePointA;
            Debug.Log("EdgePointA is on my left");
        }else{
            Debug.Log("EdgePointB is on my left");
            edgePointOnMyLeft = edgePointB;
        }

        float myYaw = Vector3.Angle(edgePointOnMyLeft - transform.position, transform.forward)-90;

        transform.Rotate(transform.up, -myYaw, Space.World);

        float angleToTurn = Vector3.Angle(transform.forward, adjustedPointAhead - transform.position);
        
        Debug.Log("Angle to turn: " + angleToTurn);

        //Turn over edge
        transform.Rotate(edgePointB - edgePointA, angleToTurn, Space.World);

        transform.Rotate(transform.up, myYaw, Space.World);
    }
    */
    
    void TurnOverEdge(Vector3 edgePointA, Vector3 edgePointB, Vector3 pointBehind){
        //Debug.Log("TurnOverEdge Called");
        Vector3? pointAheadNullable = FindPointAhead(edgePointA, edgePointB, pointBehind, out int triIndex);

        if(pointAheadNullable == null){
            Debug.Log("No point ahead");
            return;
        }

        myTri = GetTriFromIndex(triIndex);
        Debug.Log("--------------------------------------------------------- Assigned myTri using index: " + triIndex);

        Vector3 pointAhead = (Vector3)pointAheadNullable;

        gizEdgePointA = edgePointA;
        gizEdgePointB = edgePointB;
        gizPointBehind = pointBehind;
        gizPointAhead = pointAhead;

        Vector3 axisOfEdge = edgePointB - edgePointA;

        //Align points with me
        Vector3 adjustedPointAhead = pointAhead + Vector3.Project(transform.position - pointAhead, axisOfEdge);
        Vector3 forwardFromMe = transform.position + transform.forward;
        Vector3 adjustedForwardFromMe = forwardFromMe + Vector3.Project(transform.position - forwardFromMe, axisOfEdge);

        Vector3 from = adjustedForwardFromMe - transform.position;
        Vector3 to = adjustedPointAhead - transform.position;

        float angleToTurn = Vector3.Angle(from, to);

        //Check if we're turning upwards
        if(Vector3.Angle(Vector3.Cross(from, to), transform.right) > 90){
            angleToTurn = -angleToTurn;
        }

        Debug.Log("Angle to turn: " + angleToTurn);

        //Turn over edge
        transform.Rotate(axisOfEdge, angleToTurn, Space.World);
    }


    Vector3? FindPointAhead(Vector3 edgePointA, Vector3 edgePointB, Vector3 pointBehind, out int triIndex){
        int[] triangles = SM.triangles;
        Vector3[] vertices = SM.vertices;

        for (triIndex = 0; triIndex < triangles.Length/3; triIndex ++){
            
            for(int j = 0; j < 3; j++){
                Vector3 triPointA = vertices[triangles[triIndex * 3 + j]];
                Vector3 triPointB = vertices[triangles[triIndex * 3 + (j + 1) % 3]];
                Vector3 triPointC = vertices[triangles[triIndex * 3 + (j + 2) % 3]];

                //Check that new tri shares edge points, but does not share pointBehind
                if(triPointA == edgePointA && triPointB == edgePointB || triPointA == edgePointB && triPointB == edgePointA){
                    //Debug.Log("Found edge");
                    if(triPointC != pointBehind){
                        //Debug.Log("Found point ahead");
                        return triPointC;
                    }
                }
            }
        }
        return null;
    }
    
    void CheckOnSurfaceHACKED(){
        RaycastHit hit;
        Vector3 raycastDirection = -transform.up;

        //HACK: Add 0.01f so ray doesn't land on the edge I'm on.
        if(!Physics.Raycast(transform.position + (-raycastDirection * raycastBackupDist) + transform.forward * 0.01f, raycastDirection, out hit, raycastBackupDist + 1f)){
            throw new System.Exception("No tri");
        }

        //Debug.Log("hit.triIndex: " + hit.triangleIndex);
    }

    void SetInitialMyTriHACKED(){
        RaycastHit hit;
        Vector3 raycastDirection = -transform.up;

        //HACK: Add 0.01f so ray doesn't land on the edge I'm on.
        if(!Physics.Raycast(transform.position + (-raycastDirection * raycastBackupDist) + transform.forward * 0.01f, raycastDirection, out hit)){
            throw new System.Exception("No tri");
        }

        //Debug.Log("hit.triIndex: " + hit.triangleIndex);
        myTri = GetTriFromIndex(hit.triangleIndex);
    }
    
    Vector3[] GetTriFromIndex(int triIndex){
        Vector3[] vertices = SM.vertices;
        int[] triangles = SM.triangles;

        /*
        Debug.Log(triIndex);
        Debug.Log(triIndex * 3 + 0);
        int tri = triangles[triIndex * 3 + 0];

        Debug.Log(tri);
        */

        Vector3 p0 = vertices[triangles[triIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[triIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[triIndex * 3 + 2]];

        return new Vector3[] {p0, p1, p2};
    }

/*
    float AreaOfTri(Vector3 p0, Vector3 p1, Vector3 p2){

        // Calculate the differences between points
        Vector3 diff1 = new Vector3(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z);
        Vector3 diff2 = new Vector3(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);

        // Calculate the cross product
        Vector3 crossProduct = Vector3.Cross(diff1, diff2);

        // Calculate the magnitude of the cross product
        float magnitude = crossProduct.magnitude;

        // Calculate the area and return it
        return 0.5f * magnitude;
    }

    bool PointInsideTri(Vector3 a, Vector3 b, Vector3 c, Vector3 p){
        // Calculate the areas of the sub-triangles formed by p and the triangle vertices
        float mainArea = AreaOfTri(a, b, c);
        float area1 = AreaOfTri(a, b, p);
        float area2 = AreaOfTri(b, c, p);
        float area3 = AreaOfTri(c, a, p);
        
        // Check if the sum of the sub-triangle areas is equal to the main area
        return Mathf.Approximately(mainArea, area1 + area2 + area3);
    }
*/

    public static bool LineSegmentIntersection(out Vector3 intersection, Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2){
        Debug.Log("a1: " + a1);
        Debug.Log("a2: " + a2);
        Debug.Log("b1: " + b1);
        Debug.Log("b2: " + b2);


        intersection = Vector3.zero;
        Vector3 aDiff = a2-a1;
        Vector3 bDiff = b2-b1;
        if (LineLineIntersection(out intersection, a1, aDiff, b1, bDiff))
        {
            float aSqrMagnitude = aDiff.sqrMagnitude;
            float bSqrMagnitude = bDiff.sqrMagnitude;

            if ((intersection - a1).sqrMagnitude <= aSqrMagnitude  
                && (intersection - a2).sqrMagnitude <= aSqrMagnitude  
                && (intersection - b1).sqrMagnitude <= bSqrMagnitude 
                && (intersection - b2).sqrMagnitude <= bSqrMagnitude)
            {
                return true;
            }
        }
        //Debug.Log("Intersection: " + intersection);
        return false;
    }
    
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2); 

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if( Mathf.Abs(planarFactor) < 0.0001f )//&& crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            /*if(intersection ==  linePoint1 && !acceptSameAsLinePoint1){
                Debug.Log("FOUND CURRENT POSITION");
                return false;
            }*/
            
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

}