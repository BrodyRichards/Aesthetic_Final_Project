using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class relocate : MonoBehaviour
{
   public Collider objCollider;
    public Camera cam;
    private Plane[] planes;
    public Color[] replacementcolor;
   // private GameObject me;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        objCollider = GetComponent<Collider>();
        // me = this.gameObject;
        //replacementcolor = new Color[((0),(125),(255))];
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void movetree()
    {
        this.gameObject.transform.position = new Vector3(Random.Range(-40, 40), 0, Random.Range(-10, 50));
        Debug.Log("swap");


        this.gameObject.GetComponent<MeshRenderer>().material.color = replacementcolor[Random.Range(0,3)];

     /*   if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            return;
        }
        else
        {
            movetree();
        }*/
    }


}
