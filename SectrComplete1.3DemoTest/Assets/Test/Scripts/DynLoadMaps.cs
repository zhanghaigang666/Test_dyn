using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DynLoadMaps : MonoBehaviour {

	// Use this for initialization
    //public GameObject[] ChunkPrefabs;
    public GameObject chunksPrefabs;
    public GameObject followPlayer;
    public int MaxDepth;
    private Vector2 playerCurrentPosForMapCoordinate;
    private Vector2 lastLoadPlayerPoxForMapCoordinate = Vector2.zero;
    private Vector2[,] directionArray;
    //private GameObject[,] runTimeLoadChunksArray;
    private string prefixChunkName = "newblock_";
    private GameObject textUIObject;
    private GameObject textGoOutUIObject;
    private Hashtable runTimeChunksHashMap;
    private Hashtable newAddCacheHashMap;
    private Hashtable newRemoveCacheHashMap;
    private float perChunkSizeX = 9;
    private float perChunkSizeZ = 8;
    private int chunksCountX = 40;
    private int chunksCountZ = 40;
    private bool isDoneLoadCache;
    private static DynLoadMaps thisScriptObj;
    //private bool isDoneUnLoadCache;

    public static DynLoadMaps getInstance()
    {
        return thisScriptObj;
    }
	void Start ()
	{
        if (!followPlayer)
	        followPlayer = GameObject.Find("player");
	    thisScriptObj = this;
        runTimeChunksHashMap = new Hashtable();
        newAddCacheHashMap = new Hashtable();
        newRemoveCacheHashMap = new Hashtable();
        textUIObject = GameObject.Find("CurrentPos");
        textGoOutUIObject = GameObject.Find("CurrentTilePos");
	    playerCurrentPosForMapCoordinate = GetCurrentPlayerMapCoordinate();
	    MaxDepth = 1;
        //runTimeLoadChunksArray = new GameObject[MaxDepth*2+1,MaxDepth*2+1];

	    Vector2 centerOfdirectionArray = new Vector2(Mathf.Floor((MaxDepth*2+1)/2.0f),Mathf.Floor((MaxDepth*2+1)/2.0f));
        directionArray = new Vector2[MaxDepth*2+1,MaxDepth*2+1];
        for (int j = 0; j < MaxDepth * 2 + 1; j++)
        {
            for (int i = 0; i < MaxDepth * 2 + 1; i++)
            {
                directionArray[i,j] =new Vector2(i,j) -centerOfdirectionArray;
                //print(directionArray[i, j]);
            }
        }
	    isDoneLoadCache = true;
        //isDoneUnLoadCache = true;
	    InitLoadChunks();
        textGoOutUIObject.GetComponent<Text>().text = string.Format("Touch地格坐标:（{0},{1}）", "*", "*");
	}

    Vector2 GetCurrentPlayerMapCoordinate()
    {
        return new Vector2((int)(followPlayer.transform.position.x / perChunkSizeX), (int)-(followPlayer.transform.position.z / perChunkSizeZ));
    }
    void DynCheckChunks()
    {
        #region DynLoadChunksToCache
        for (int j = 0; j < MaxDepth * 2 + 1; j++)
        {
            for (int i = 0; i < MaxDepth * 2 + 1; i++)
            {
                Vector2 cursor = directionArray[i, j] + playerCurrentPosForMapCoordinate;
                int cursorX = (int)cursor.x;
                int cursorY = (int)cursor.y;
                if (CheckDirectionVector(cursor))
                {
                    string keyname = string.Format("{0},{1}", cursorX, cursorY);
                    if (!runTimeChunksHashMap.ContainsKey(keyname) && !newAddCacheHashMap.ContainsKey(keyname))
                    {
                        newAddCacheHashMap.Add(keyname,keyname);
                    }
                }
                //else
                //{
                //    continue;;
                //}
            }
        }
        #endregion 

        #region DynUnloadChunkFromCache
        foreach (DictionaryEntry de in runTimeChunksHashMap)
        {
            string keyname = de.Key as string;
            GameObject valueObj = de.Value as GameObject;
            string[] sArray = keyname.Split(',');
            int x = int.Parse(sArray[0]);
            int y = int.Parse(sArray[1]);
            Vector2 vec = new Vector2(x,y) - playerCurrentPosForMapCoordinate;
            float maxDistance = Mathf.Max(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
            if (maxDistance > MaxDepth && !newRemoveCacheHashMap.ContainsKey(keyname))
            {
                newRemoveCacheHashMap.Add(keyname,valueObj);
            }
        }
        #endregion

        lastLoadPlayerPoxForMapCoordinate = playerCurrentPosForMapCoordinate;
        if (true)
        {
            isDoneLoadCache = false;
            StartCoroutine(LoadNewAddCache());
        }
        //StartCoroutine(UnLoadNewRemoveCache());
        UnLoadNewRemoveCache();
    }

    IEnumerator LoadNewAddCache()
    {
        //print("at LoadNewAddCache start");
        Hashtable ht = newAddCacheHashMap.Clone() as Hashtable;
        IDictionaryEnumerator myEnumerator = ht.GetEnumerator();
        while (myEnumerator.MoveNext())
        {
            string keyname = myEnumerator.Key as string;
            string objStr = myEnumerator.Value as string;
            if (keyname != string.Empty && objStr != null && !runTimeChunksHashMap.ContainsKey(keyname))
            {
                //print("UnLoadNewRemoveCache Come");
                string[] sArray = keyname.Split(',');
                int x = int.Parse(sArray[0]);
                int y = int.Parse(sArray[1]);
                string name = string.Format(prefixChunkName + "{0},{1}", x, y);
                Transform obj = chunksPrefabs.transform.FindChild(name);
                GameObject chunkObj = GameObject.Instantiate(obj.gameObject, new Vector3(x * perChunkSizeX, 0, -y * perChunkSizeZ), Quaternion.identity);
                runTimeChunksHashMap.Add(keyname, chunkObj);
                newAddCacheHashMap.Remove(keyname);
                //yield return new WaitForFixedUpdate();
                yield return null;
                //print("LoadNewAddCache:1");
            }
        }
        isDoneLoadCache = true;
        //print("at LoadNewAddCache ended");
        //newAddCacheHashMap.Clear();
    }

    void UnLoadNewRemoveCache()
    {
        IDictionaryEnumerator myEnumerator = newRemoveCacheHashMap.GetEnumerator();
        while (myEnumerator.MoveNext())
        {
            string keyname = myEnumerator.Key as string;
            GameObject obj = myEnumerator.Value as GameObject;
            if (keyname != string.Empty && obj != null)
            {
                //DestroyImmediate(obj);
                Destroy(obj);
                runTimeChunksHashMap.Remove(keyname);
                //newRemoveCacheHashMap.Remove(keyname);
                //yield return 0;
            }
        }
        newRemoveCacheHashMap.Clear();
    }
    void InitLoadChunks()  //初始化时（或Loading时）已经把asset加载到memory，由于地表prefab共用一套材质，所以不需要额外考虑地表asset的预加载Resource.Load问题
    {
        for (int j = 0; j < MaxDepth * 2 + 1; j++)
        {
            for (int i = 0; i < MaxDepth * 2 + 1; i++)
            {
                Vector2 cursor = directionArray[i, j] + playerCurrentPosForMapCoordinate;
                int cursorX = (int)cursor.x;
                int cursorY = (int) cursor.y;
                if (CheckDirectionVector(cursor))
                {
                    string keyname = string.Format("{0},{1}", cursorX, cursorY);
                    string name = string.Format(prefixChunkName + "{0},{1}", cursorX, cursorY);
                    Transform obj = chunksPrefabs.transform.FindChild(name);
                    GameObject chunkObj = GameObject.Instantiate(obj.gameObject, new Vector3(cursorX * perChunkSizeX, 0, -cursorY * perChunkSizeZ), Quaternion.identity);
                    runTimeChunksHashMap.Add(keyname, chunkObj);
                }
            }
        }
    }

    bool CheckDirectionVector(Vector2 vec)
    {
        if (vec.x < 0 || vec.x > chunksCountX-1)
            return false;
        if (vec.y < 0 || vec.y > chunksCountZ-1)
            return false;
        return true;
    }

	void Update ()
	{
	    playerCurrentPosForMapCoordinate = GetCurrentPlayerMapCoordinate();
	    int playerX = (int) playerCurrentPosForMapCoordinate.x;
	    int playerY = (int) playerCurrentPosForMapCoordinate.y;
	    int lastLoadX = (int) lastLoadPlayerPoxForMapCoordinate.x;
	    int lastLoadY = (int) lastLoadPlayerPoxForMapCoordinate.y;
        //print("playerx_y:"+playerX+","+playerY);
        //print("lastplayerx_y:" + lastLoadX + "," + lastLoadY);

	    if (playerX!=lastLoadX || playerY!=lastLoadY)
	    {
            DynCheckChunks();
	    }
        //DynCheckChunks();

        textUIObject.GetComponent<Text>().text = string.Format("当前地块坐标:（{0},{1}）", playerX, playerY);

	}

    public void SetCurrentTapPosUI(int x,int y)
    {
        print("at tappoui");
        textGoOutUIObject.GetComponent<Text>().text = string.Format("Touch地格坐标:（{0},{1}）", x, y);

    }

}
