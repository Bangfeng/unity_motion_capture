using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlCube : MonoBehaviour {
    private GameObject obj1;
    private GameObject obj2;
    // Use this for initialization
    void Start ()
    {
        obj1 = GameObject.Find("Cylinder");
        //obj1 = GameObject.FindWithTag("Cylinder");
        obj2 = GameObject.Find("Cube");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {

            transform.Rotate(0, 5, 0);
            //obj1.transform.Rotate(0, 5, 3);
            obj2.transform.Rotate(0, 5, 3);
            //mvnActor.Rotate(new Vector3(0, 5, 0));
            //Quaternion orientation;

        }
        if (Input.GetKeyDown(KeyCode.A))
        {

            
            transform.Find("Cylinder").Rotate(0, 5, 0);
            //obj1.transform.Rotate(0, 5, 3);

            //mvnActor.Rotate(new Vector3(0, 5, 0));
            //Quaternion orientation;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {


            transform.Find("Cylinder/Capsule").Rotate(0, 5, 9);
            //obj1.transform.Rotate(0, 5, 3);

            //mvnActor.Rotate(new Vector3(0, 5, 0));
            //Quaternion orientation;

        }
    }
}
