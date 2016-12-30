using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEditor;
using UnityEngine;

public class ScreenTouchController : MonoBehaviour {


    public ScreenTransformGesture fingerMoveGesture;
    public TapGesture tapGesture;
    public LongPressGesture longTapGesture;
    private Vector2 dragBeginPos;
    private Vector3 dragPlayerBeginPos;
    private float dragZoneLengh = 7;
    private GameObject sphere;
    private float PanSpeed=0.01f;
    public float playerRotateYAngle;
	void Start () {
        if (GetComponent<PressGesture>() != null)
            GetComponent<PressGesture>().StateChanged += onPress;
	    fingerMoveGesture = GetComponent<ScreenTransformGesture>();
        //tapGesture = GetComponent<TapGesture>();
        sphere = GameObject.Find("Sphere");
        playerRotateYAngle = -53;
        if (playerRotateYAngle ==0)
        {
            Debug.Assert(false, "playerRotateYAngle is Null");
        }
        
        this.transform.Rotate(Vector3.up,playerRotateYAngle);
	}

    private void OnEnable()
    {
        fingerMoveGesture.Transformed += FingerMovedHandler;
        fingerMoveGesture.TransformStarted += FingerDragBeginHandler;
        fingerMoveGesture.TransformCompleted += FingerDragEndedHandler;
        //print(tapGesture);
        GetComponent<TapGesture>().Tapped += onPress1;
        longTapGesture.LongPressed += onPress2;
    }

    private void OnDisable()
    {
        fingerMoveGesture.Transformed -= FingerMovedHandler;
        fingerMoveGesture.TransformStarted -= FingerDragBeginHandler;
        fingerMoveGesture.TransformCompleted -= FingerDragEndedHandler;
        tapGesture.Tapped -= onPress1;
        longTapGesture.LongPressed -= onPress2;
        //GetComponent<PressGesture>().StateChanged -= onPress;


    }
    void onPress(object sender, GestureStateChangeEventArgs gestureStateChangeEventArgs)
    {
        //print(sender);
        //print(gestureStateChangeEventArgs);
        //print("ggggg");
    }

    void onPress1(object sender, System.EventArgs e)
    {
        //print(sender);
        //print(gestureStateChangeEventArgs);
        Vector3 scrPos = tapGesture.ScreenPosition;
        HexMapEditor.GetInstance().HandleInput(new Vector3(scrPos.x, scrPos.y, 0));

    }

    void onPress2(object sender, System.EventArgs e)
    {
        //print(sender);
        //print(gestureStateChangeEventArgs);
        //Vector3 scrPos = tapGesture.ScreenPosition;
        //HexMapEditor.GetInstance().HandleInput(new Vector3(scrPos.x, scrPos.y, 0));
        print("gogotoo");

    }
    void FingerDragBeginHandler(object sender, System.EventArgs e)
    {
        //print("begion...........");

        dragBeginPos = fingerMoveGesture.ScreenPosition;
        dragPlayerBeginPos = this.transform.localPosition;
        //HexMapEditor.GetInstance().HandleInput(new Vector3(dragBeginPos.x,dragBeginPos.y,0));

    }
    void FingerMovedHandler(object sender, System.EventArgs e)
    {
        //print("ggggggggggg");
        //print(fingerMoveGesture.MinScreenPointsDistance);
        //print(fingerMoveGesture.ScreenTransformThreshold);
        //print(fingerMoveGesture.DeltaPosition);
        //print(fingerMoveGesture.DeltaRotation);
        //print(fingerMoveGesture.ScreenPosition);
        float dragDistanceX = Screen.width/dragZoneLengh;
        float dragDistanceY = Screen.height/dragZoneLengh;
        Vector3 dragBeginWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(dragBeginPos.x, 0, dragBeginPos.y));
        Vector3 dragMovedWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(fingerMoveGesture.ScreenPosition.x, 0, fingerMoveGesture.ScreenPosition.y));
        Vector3 vec3 = dragMovedWorldPos - dragBeginWorldPos;
        //vec3 = this.transform.InverseTransformPoint(vec3);
        //this.transform.localPosition = dragPlayerBeginPos + new Vector3(vec3.x / dragDistanceX, 0, vec3.y / dragDistanceY);
        //this.transform.localPosition += new Vector3(1,0,0);
        //this.transform.Translate(new Vector3(1,0,0),Space.Self);
        //GetComponent<Rigidbody>().velocity = new Vector3();
        Vector3 vec = fingerMoveGesture.DeltaPosition;
        this.transform.localPosition += this.transform.rotation * new Vector3(-vec.x,0,-vec.y) * PanSpeed;
        //this.transform.localPosition = this.transform.rotation * new Vector3(vec3.x / dragDistanceX, 0, vec3.y / dragDistanceY);

        //player.transform.Rotate(Vector3.up, playerRotateYAngle);


    }

    void FingerDragEndedHandler(object sender, System.EventArgs e)
    {
        //print("end...........");
    }
	// Update is called once per frame
	void Update ()
	{
        //Vector2 v = new Vector2(100,0);
        //Vector3 vec3 = Camera.main.ScreenToWorldPoint(new Vector3(100, 0, 0));
        //print(vec3);
	}
}
