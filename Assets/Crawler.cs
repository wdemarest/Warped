using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour
{
    //Space Collider and Space Mesh
    public GameObject SpaceObject;

    public GameObject Marker;
    public GameObject Marker2;
    public GameObject Marker3;
    
    Mesh SM;
    public Vector3[] myTri;

    //=== TEMP VARIABLES===

    Vector3 intersection;

    //======================

    float raycastBackupDist = 0.1f;

    public Vector3 gizEdgePointA;
    public Vector3 gizEdgePointB;
    public Vector3 gizPointBehind;
    public Vector3 gizPointAhead;

    public Vector3 gizFrom;
    public Vector3 gizTo;

    public Vector3 fromUnaltered;
    public Vector3 toUnaltered;

    public Vector3 fromNormalized;
    public Vector3 toNormalized;
    
    public Vector3[] vertices;
    public int[] showTriangles;

    public Vector3[] segmentIntersectTestParams;


    
    // Start is called before the first frame update
    void Start()
    {
        SM = SpaceObject.GetComponent<MeshCollider>().sharedMesh;
        vertices = SM.vertices;
        for(int i = 0; i < vertices.Length; i++){
            vertices[i] = SpaceObject.transform.TransformPoint(vertices[i]);
        }

        showTriangles = SM.triangles;

        InitPosAndRotation();
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
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * -1);

            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(gizEdgePointA, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(gizEdgePointB, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(gizPointBehind, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gizPointAhead, 0.1f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(gizFrom, 0.1f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(gizTo, 0.1f);

            float cubeSize = 0.09f;
            Gizmos.color = Color.white;
            Gizmos.DrawCube(myTri[0], new Vector3(cubeSize, cubeSize, cubeSize));
            Gizmos.DrawCube(myTri[1], new Vector3(cubeSize, cubeSize, cubeSize));
            Gizmos.DrawCube(myTri[2], new Vector3(cubeSize, cubeSize, cubeSize));
            
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
                Instantiate(Marker, transform.position, Quaternion.identity);

                return distMoved;
            }
        }

        transform.position = targetPos;

        Debug.Log("Moved Full Remaining: " + moveDistRemaining);
        CheckOnSurfaceHACKED();

        Instantiate(Marker, transform.position, Quaternion.identity);

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
            throw new System.Exception("No point ahead");
            //return;
        }

        Vector3 pointAhead = (Vector3)pointAheadNullable;

        myTri = GetTriFromIndex(triIndex);
        Debug.Log("--------------------------------------------------------- Assigned myTri using index: " + triIndex);

        gizEdgePointA = edgePointA;
        gizEdgePointB = edgePointB;
        gizPointBehind = pointBehind;
        gizPointAhead = pointAhead;

        Vector3 axisOfEdge = edgePointB - edgePointA;

        //Align points with me
        Vector3 adjustedPointAhead = pointAhead + Vector3.Project(transform.position - pointAhead, axisOfEdge);
        Vector3 forwardFromMe = transform.position + transform.forward;
        Vector3 adjustedForwardFromMe = forwardFromMe + Vector3.Project(transform.position - forwardFromMe, axisOfEdge);

        gizFrom = adjustedForwardFromMe;
        gizTo = adjustedPointAhead;
        Vector3 from = adjustedForwardFromMe - transform.position;
        Vector3 to = adjustedPointAhead - transform.position;
        
        fromUnaltered = from;
        toUnaltered = to;
        fromNormalized = from.normalized;
        toNormalized = to.normalized;

        Debug.Log("From: " + from.normalized);
        Debug.Log("To: " + to.normalized);

        float angleToTurn = Vector3.Angle(from.normalized, to.normalized);

        

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

    void InitPosAndRotation(){
        RaycastHit hit;
        Vector3 raycastDirection = -transform.up;

        if(!Physics.Raycast(transform.position + (-raycastDirection * raycastBackupDist), raycastDirection, out hit)){
            throw new System.Exception("No tri");
        }

        //Debug.Log("hit.triIndex: " + hit.triangleIndex);
        myTri = GetTriFromIndex(hit.triangleIndex);

        Vector3 initialUp = hit.normal;

        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, initialUp, Mathf.Infinity, 0.0f));

        transform.Rotate(90, 0, 0);
    }
    
    Vector3[] GetTriFromIndex(int triIndex){
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

    public void segmentIntersectTest(float moveDistRemaining){
        Vector3[] myParams = segmentIntersectTestParams;

        Instantiate(Marker2, myParams[0], Quaternion.identity);
        Instantiate(Marker2, myParams[1], Quaternion.identity);

        if(LineSegmentIntersection(out Vector3 intersection, transform.position, transform.position + transform.forward * moveDistRemaining, myParams[0], myParams[1])){
            Debug.Log("Intersection: " + intersection);
            Instantiate(Marker3, intersection, Quaternion.identity);
        }
    }

    public static bool LineSegmentIntersection(out Vector3 intersection, Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2){
        //Debug.Log("a1: " + a1);
        //Debug.Log("a2: " + a2);
        //Debug.Log("b1: " + b1);
        //Debug.Log("b2: " + b2);
        //gizB1 = b1;
        //gizB2 = b2;


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