using UnityEngine;
using System.Collections;
using  UnityEngine.AI;

public class TestPlayerNav : MonoBehaviour {
    
    private CharacterController controller;
    private UnityEngine.AI.NavMeshAgent navAgent;
    public LineRenderer lineRenderer;
    public float RunSpeed;
    private Vector3 MoveDrection;
    public Vector3 desPoint;
    private float deltaX;
    private float deltaZ;
    private bool isNaving;

    public enum navEnum
    {
        stopNav = 0,
        startNav
    }

	void Start () {
        controller = (CharacterController)this.GetComponent<CharacterController>();
	    navAgent = this.GetComponent<NavMeshAgent>();
	    navAgent.enabled = false;
        //desPoint = GameObject.Find("DesPostion").transform?GameObject.Find("DesPostion").transform.position:Vector3.zero;
        desPoint = Vector3.zero;
	    isNaving = false;
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        MoveDrection = Vector3.zero;
	    deltaX = 0;
	    deltaZ = 0;
        if (isNaving && navAgent.enabled == false)
        {
            print("come nav...");
            navAgent.enabled = true;
            navAgent.SetDestination(desPoint);

        }
        if (!isNaving)
        {
            navAgent.enabled = false;
        }
        if (isNaving)
        {
            this.GetComponent<Rigidbody>().angularVelocity = this.GetComponent<Rigidbody>().velocity;
            if (navAgent.remainingDistance <= 1 && Vector3.Distance(this.transform.position,desPoint) <1)
            {
                //print("ggogogo"+navAgent.remainingDistance);
                navAgent.enabled = false;
                isNaving = false;
            }
            
        }
        //if (isNaving)
        //    print("bbbb:"+(int)navAgent.remainingDistance);
        //print(string.Format("aaa:{0}", this.GetComponent<Rigidbody>().velocity));
	    if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
	    {
            return;
	    }
	    if (Input.GetAxis("Horizontal") != 0)
        {
            deltaX = Input.GetAxis("Horizontal");
            MoveDrection.z = -deltaX;
            isNaving = false;

        }
        if (Input.GetAxis("Vertical") != 0)
        {
            deltaZ = Input.GetAxis("Vertical");
            MoveDrection.x = deltaZ;
            isNaving = false;
        }

        //print(MoveDrection);
        //controller.SimpleMove(MoveDrection * (Time.deltaTime * RunSpeed));
        //this.gameObject.GetComponent<Rigidbody>().velocity = MoveDrection * (Time.deltaTime * RunSpeed);
        this.gameObject.transform.Translate(MoveDrection * (Time.deltaTime * RunSpeed));

	}

    void Update()
    {
        //print("hhhhhhhhhhh:"+navAgent.path.corners.Length);
        //Vector3[] _path = navAgent.path.corners;
        ////_path = new Vector3[2]{this.transform.position,new Vector3(0,0,0)};

        //lineRenderer.SetVertexCount(navAgent.path.corners.Length);//设置线段数
        //for (int i = 0; i < _path.Length; i++)
        //{
        //    lineRenderer.SetPosition(i, _path[i]);//设置线段顶点坐标
        //    print("****:" + _path[i]);
        //}
    }


    public void StartOrStopNavgate(int navNum)
    {
        if ((navEnum)navNum == navEnum.startNav)
        {
            isNaving = true;
        }
        else
        {
            isNaving = false;
        }

    }
}
