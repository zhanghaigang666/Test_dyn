using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestNavAgant : MonoBehaviour {

	// Use this for initialization
    private UnityEngine.AI.NavMeshAgent meshAgent;
    public Transform desPoint;
	void Start () {
	    meshAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        print(meshAgent.name);
        print(desPoint.position);
        print(desPoint.localPosition);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
        {
            meshAgent.enabled = true;
            meshAgent.SetDestination(desPoint.position);
            print("start nav mesh agent!!!");

        }
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "DesCube" && this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled==true)
        {
            meshAgent.enabled = false;
            print("stop nav mesh agent!!!");
        }
    }

    
}
