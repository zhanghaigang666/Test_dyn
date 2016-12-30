using UnityEngine;
using System.Collections;

public class TestCamera : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
    public Vector3 offsetPos;
    private float speedDamping = 100;
	void Start ()
	{
	    player = GameObject.Find("player");
	}

	
	// Update is called once per frame
	void Update ()
	{
        //this.transform.position = player.transform.position + offsetPos;
        //this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position + offsetPos, Time.deltaTime * speedDamping);
        //this.transform.LookAt(Vector3.Lerp(this.transform.position,player.transform.position,Time.deltaTime * speedDamping));

        //this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position + offsetPos, Time.deltaTime * speedDamping);
        //this.transform.LookAt(Vector3.Lerp(this.transform.position, player.transform.position, Time.deltaTime * speedDamping));
	}

    void FixedUpdate()
    {
        //this.transform.position = player.transform.position + offsetPos;
    }

    void LateUpdate()
    {
        //this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position + offsetPos, Time.deltaTime * speedDamping);
        this.transform.position = player.transform.position + offsetPos;
            //Vector3.Lerp(this.transform.position, player.transform.position + offsetPos, Time.deltaTime * speedDamping);
        //this.transform.LookAt(Vector3.Lerp(this.transform.position, player.transform.position, Time.deltaTime * speedDamping));

    }
}
