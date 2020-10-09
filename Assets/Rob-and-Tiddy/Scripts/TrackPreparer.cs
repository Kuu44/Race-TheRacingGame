using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPreparer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dummyVertexObject;
    Mesh trackDirectorMesh;
    Vector3[] vertices;
    List<Vector3> trackWayPoints;

    public GameObject tempWaypoints;
    /*struct meshTriangle{
        public int i1, i2, i3;
        public meshTriangle(int ii1, int ii2, int ii3){
            i1 = ii1;
            i2 = ii2;
            i3 = ii3;
        }

    }*/

    void Start()
    {
        trackDirectorMesh = SceneObjects.current.trackDirector.GetComponent<MeshFilter>().mesh;
        vertices = trackDirectorMesh.vertices;
       /* int[] triangles = trackDirectorMesh.triangles;
        bool[] vertexInWaypoint = new bool[vertices.Length];
        List<meshTriangle> meshTriangles = new List<meshTriangle>();

        for(int i = 0; i < triangles.Length; i = i + 3){
            meshTriangles.Add(new meshTriangle(triangles[i], triangles[i+1], triangles[i+2]));
        }
*/
        for(int i = 0; i < vertices.Length; i++){
            vertices[i] = SceneObjects.current.trackDirector.transform.TransformPoint(vertices[i]);
                GameObject temp = Instantiate(dummyVertexObject, vertices[i], Quaternion.identity);
                temp.name = i.ToString() + " tempwaypoint";
        }
/*
        for(int i = 0; i < vertices.Length; i++){
            for(int ii = 0; ii < meshTriangles.Count; ii++){
                if(meshTriangles[ii].i1 == i){

                }else if(meshTriangles[ii].i2 == i){
                    
                }else if(meshTriangles[ii].i3 == i){
                    
                }else{

                }
            }
        }*/



    }


/*
    float xzDistance(Vector3 a, Vector3 b){
        return (new Vector3(a.x, 0, a.z) - new Vector3(b.x, 0, b.z)).magnitude;
    }*/
    // Update is called once per frame
    void Update()
    {
        

       if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 2000)){
                if(hit.collider.tag == "Track"){
                    MeshRenderer temp = hit.transform.GetComponent<MeshRenderer>();
                    temp.material.color = Color.red;

                    hit.transform.SetParent(tempWaypoints.transform);
                }
            }
        }

        
    }
}
