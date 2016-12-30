using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public bool isCorouteShowMpas;
    public GameObject cube;
    public GameObject cube1;
	public int width = 6;
	public int height = 6;

	public Color defaultColor = Color.white;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;
    public GameObject[] hexCellMeshPrefab;


	HexCell[] cells;

	Canvas gridCanvas;
	HexMesh hexMesh;
    public static  HexGrid grid;
    public Vector2 hexc;
    private GameObject hexMaps;
    private GameObject hexCells;
    private int[] tempStartCoroute;
    private int currentTapType ;
    public static HexGrid getInstanse()
    {
        return grid;
    }
	void Awake ()
	{
        grid = this;
	    isCorouteShowMpas = true;
	    currentTapType = 1;
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();
	    hexMaps = GameObject.Find("Hex Maps");
        hexCells = GameObject.Find("Hex Cells");
		cells = new HexCell[height * width];
        tempStartCoroute = new int[3];

        //for (int z = 0, i = 0; z < height; z++) {
        //    for (int x = 0; x < width; x++) {
        //        //CreateCellByPrefab(x, z, i++);
        //        tempStartCoroute[0] = x;
        //        tempStartCoroute[1] = z;
        //        tempStartCoroute[2] = i++;

        //        StartCoroutine(CreateCellByPrefab());
        //        //CreateCellByPrefab(x, z, i++);
        //    }
        //}
	    if (isCorouteShowMpas)
	    {
            StartCoroutine(CorouteShowMap());
	    }
	    else
	    {
	        ShowMap();
	    }
	    

	}

    IEnumerator CorouteShowMap()
    {
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                //CreateCellByPrefab(x, z, i++);
                tempStartCoroute[0] = x;
                tempStartCoroute[1] = z;
                tempStartCoroute[2] = i++;
                //StartCoroutine(CreateCellByPrefab());
                CreateCellByPrefab();
                //CreateCellByPrefab(x, z, i++);
            }
            yield return null;
        }
    }

    IEnumerator GoOutMaps()
    {
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                string name = string.Format("meshObject_{0},{1}", x, z);
                string name1 = string.Format("cellObject_{0},{1}", x, z);
                string name2 = string.Format("label_{0},{1}",x,z);
                Transform removeObj = hexMaps.transform.FindChild(name);
                Transform removeCells = hexCells.transform.FindChild(name1);
                Transform removeCanvas = gridCanvas.transform.FindChild(name2);

                if (removeObj)
                {
                    Destroy(removeObj.gameObject);
                }
                if (removeCells)
                {
                    Destroy(removeCells.gameObject);
                }
                if (removeCanvas)
                {
                    Destroy(removeCanvas.gameObject);
                }

            }
            yield return null;
        }
    }
    GameObject GetWhichPrefab(int[] prefabs)
    {
        int ii = prefabs[0] % 2;
        int jj = prefabs[1] % 2;
        GameObject thisObj = null;
        switch (ii)
        {
            case 0:
                {
                    if (jj == 0)
                    {
                        //(0,0)
                        thisObj = hexCellMeshPrefab[0];
                    }
                    if (jj == 1)
                    {
                        //(0,1)
                        thisObj = hexCellMeshPrefab[1];
                    }
                    break;
                }
            case 1:
                {
                    if (jj == 0)
                    {
                        //(1,0)
                        thisObj = hexCellMeshPrefab[2];

                    }
                    if (jj == 1)
                    {
                        //(1,1)
                        thisObj = hexCellMeshPrefab[3];
                    }
                    break;
                }
        }
        return thisObj;
    }
    void ShowMap()
    {
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                //CreateCellByPrefab(x, z, i++);
                tempStartCoroute[0] = x;
                tempStartCoroute[1] = z;
                tempStartCoroute[2] = i++;
                //StartCoroutine(CreateCellByPrefab());
                CreateCellByPrefab();
                //CreateCellByPrefab(x, z, i++);
            }
        }
    }

    public void OnTapEvent(int tapType)
    {
        print(tapType);
        if (tapType==0 && currentTapType ==1)
        {
            currentTapType = 0;
            StartCoroutine(GoOutMaps());
            print("go out done");
        }
        else if (tapType == 1 && currentTapType ==0)
        {
            currentTapType = 1;
            StartCoroutine(CorouteShowMap());
            print("go on done");
        }
    }


	void Start () {
        //hexMesh.Triangulate(cells);
        //GameObject g = GameObject.Find("Hex_Cell2");
        //print(g.GetComponent<BoxCollider>().size.z);
	}

    void Update()
    {
        //print(Time.time);
        //if (Time.time >=5)
        //{
        //    StartCoroutine(GoOutMaps());
        //}
    }
	public void ColorCell (Vector3 position, Color color) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition2(position);
        print("("+coordinates.X+","+coordinates.Z+")");
        if (coordinates.X <0 || coordinates.X >=width)
	    {
            print("x is error");
	        return;
	    }
        if (coordinates.Z < 0 || coordinates.Z >=height)
	    {
	        print("z is error");
            return;
	    }
        //int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		int index = coordinates.X + coordinates.Z * width;
		HexCell cell = cells[index];
		cell.color = color;
        //hexMesh.Triangulate(cells);
	}

    void CreateCell(int x, int z, int i) 
	{

		Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        //position.x = (x +z/2 - z/2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = -z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;
	    GameObject go = Instantiate<GameObject>(cube);
	    go.transform.localPosition = position;

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
	}

    //void CreateCellByPrefab(int x, int z, int i)
    //IEnumerator CreateCellByPrefab()
    void CreateCellByPrefab()

    {
        int x = tempStartCoroute[0];
        int z = tempStartCoroute[1];
        int i = tempStartCoroute[2];
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        //position.x = (x +z/2 - z/2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = -z * (HexMetrics.outerRadius * 1.5f);
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //int count = hexCellMeshPrefab.Length;
        GameObject modlePrefab = GetWhichPrefab(new int[] {x, z});
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        cell.name = string.Format("cellObject_{0},{1}", x, z);
        cell.transform.SetParent(hexCells.transform,false);
        cell.meshObject = Instantiate<GameObject>(modlePrefab);
        cell.meshObject.transform.localPosition = position;
        cell.meshObject.transform.SetParent(hexMaps.transform,false);
        cell.meshObject.name = string.Format("meshObject_{0},{1}", x, z);

        //GameObject go = Instantiate<GameObject>(cube);
        //go.transform.localPosition = position;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.gameObject.name = string.Format("label_{0},{1}",x,z);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLinesNoY();

    }
    public void setCubePos(Vector2 v)
    {
        hexc = v;
        if (hexc!=null)
        {
            var g = Instantiate(cube1, new Vector3(hexc.x, 0, -hexc.y), Quaternion.identity);
            g.transform.SetParent(transform,false);

            //g.transform.localPosition = new Vector3(hexc.x, 0, hexc.y);
        }
    }
}