using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

    public static HexMapEditor thisObj;

    public static HexMapEditor GetInstance()
    {
        return thisObj;
    }
	void Awake () {
		SelectColor(0);
        thisObj = this;
	}

	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
            //HandleInput();
        }

    }

	public void HandleInput (Vector3 screenVec) 
    {

        Ray inputRay = Camera.main.ScreenPointToRay(screenVec);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			hexGrid.ColorCell(hit.point, activeColor);
		}
	}

    void HandleInput()
    {
        print(Input.mousePosition);
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            hexGrid.ColorCell(hit.point, activeColor);
        }

    }

	public void SelectColor (int index) {
		activeColor = colors[1];
	}
}