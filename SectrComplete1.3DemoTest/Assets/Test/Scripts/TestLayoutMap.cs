using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestLayoutMap : MonoBehaviour {

	// Use this for initialization
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;
    private int perBlockx = 9;
    private int perBlockz = 8;
    //private int perBlockx = 10;
    //private int perBlockz = 10;
    public GameObject portalPrefab;
    public GameObject portalsSetsObj;
    public GameObject MapsObjects;
    private string preMark = "new";
    public float speed = 10;


	void Start ()
	{
        return;
        //GameObject prefab_cp1 = Instantiate(prefab1, new Vector3(0, 0, 0), Quaternion.identity);
        //GameObject prefab_cp2 = Instantiate(prefab2, new Vector3(0, 0, perBlockz), Quaternion.identity);
        ////GameObject prefab_cp3 = Instantiate(prefab2, new Vector3(10, 0, 0), Quaternion.identity);
        ////GameObject prefab_cp4 = Instantiate(prefab2, new Vector3(10, 0, 0), Quaternion.identity);
        //Vector3 vec1 = prefab_cp1.GetComponent<MeshFilter>().mesh.bounds.size;
        //Vector3 vec2 = prefab_cp1.GetComponent<BoxCollider>().bounds.size;
        //print("vec size is:" + vec1.ToString());
        //print("vec collider size is :" + vec2);

        //portalPrefab = GameObject.Find("protalPrefabs");
        //GameObject par = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
	    int mapCount = 40;
        for (int j = 0; j < mapCount; j++)
        {
            for (int i = 0; i < mapCount; i++)
            {
                print("vec:"+i+","+j);
                int ii = i%2;
                int jj = j%2;
                GameObject thisObj=null;
                switch (ii)
                {
                    case 0:
                    {
                        if (jj == 0)
                        {
                            //(0,0)
                            thisObj = prefab1;
                        }
                        if (jj == 1)
                        {
                            //(0,1)
                            thisObj = prefab2;
                        }
                        break;
                    }
                    case 1:
                    {
                        if (jj ==0)
                        {
                            //(1,0)
                            thisObj = prefab3;

                        }
                        if (jj ==1)
                        {
                            //(1,1)
                            thisObj = prefab4;
                        }
                        break;
                    }
                }
                print(string.Format("({0},{1})", i, j));
                GameObject prefab = Instantiate(thisObj, new Vector3(perBlockx * i, 0, -perBlockz * j), Quaternion.identity);
                prefab.name = string.Format(preMark+"block_{0},{1}", i, j);
                //prefab.AddComponent<SECTR_Sector>();
                prefab.transform.parent = MapsObjects.transform;
            }
        }
#if false
        for (int j = 0; j < mapCount; j++)
        {
            for (int i = 0; i < mapCount; i++)
            {
                int ii = i % 2;
                int jj = j % 2;
                Vector2 downLeft = new Vector2(i - 1, j + 1);
                Vector2 down = new Vector2(i, j + 1);
                Vector2 downRight = new Vector2(i + 1, j + 1);
                Vector2 right = new Vector2(i + 1, j);
                Vector2[] directionVec2 = new Vector2[4] { downLeft, down, downRight, right };
                for (int k = 0; k < directionVec2.Length; k++)
                {
                    if (directionVec2[k].x < 0 || directionVec2[k].x >= mapCount)
                        continue;
                    if (directionVec2[k].y < 0 || directionVec2[k].y >= mapCount)
                        continue;
                    GameObject aPortal = Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
                    GameObject currentObj = GameObject.Find(string.Format(preMark+"block_{0},{1}", i, j));
                    GameObject cursorObj = GameObject.Find(string.Format(preMark+"block_{0},{1}", directionVec2[k].x, directionVec2[k].y));
                    aPortal.GetComponent<SECTR_Portal>().FrontSector = currentObj.GetComponent<SECTR_Sector>();
                    aPortal.GetComponent<SECTR_Portal>().BackSector = cursorObj.GetComponent<SECTR_Sector>();
                    aPortal.name = string.Format(preMark+"portal_({0},{1})_to_({2},{3})", i, j, directionVec2[k].x, directionVec2[k].y);
                    aPortal.transform.parent = portalsSetsObj.transform;
                }

            }
        }
#endif

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            print(touchDeltaPosition);
            this.gameObject.transform.Translate(touchDeltaPosition * speed);
            //transform.Translate(touchDeltaPosition.x * speed, touchDeltaPosition.y * speed, 0);
        }  
	}
}
