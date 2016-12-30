using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DynLoadMaps1 : MonoBehaviour {

	// Use this for initialization
    //public GameObject[] ChunkPrefabs;
    public GameObject chunksPrefabs;
    public GameObject followPlayer;
    public int MaxDepth;
    private Vector2 playerCurrentPosForMapCoordinate;
    private Vector2 lastLoadPlayerPoxForMapCoordinate = Vector2.zero;
    private Vector2[,] directionArray;
    //private GameObject[,] runTimeLoadChunksArray;
    private string prefixChunkName = "4mblock_";
    private GameObject textUIObject;
    private Hashtable runTimeChunksHashMap;
    private Hashtable newAddCacheHashMap;
    private Hashtable newRemoveCacheHashMap;
    private float perChunkSizeX = 9;
    private float perChunkSizeZ = 8;
    private int chunksCountX = 20;
    private int chunksCountZ = 20;



	void Start ()
	{
        if (!followPlayer)
	        followPlayer = GameObject.Find("player");
        runTimeChunksHashMap = new Hashtable();
        newAddCacheHashMap = new Hashtable();
        newRemoveCacheHashMap = new Hashtable();
        textUIObject = GameObject.Find("CurrentPos");
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
	    InitLoadChunks();
        //for (int i = 0; i < Mathf.Sqrt(runTimeLoadChunksArray.Length); i++)
        //{
        //    for (int j = 0; j < Mathf.Sqrt(runTimeLoadChunksArray.Length); j++)
        //    {
        //        print(runTimeLoadChunksArray[i, j]);
        //    }
        //}

	    //directionArray = new directionArray[] {};
        //runTimeChunksHashMap.Add("北京", "帝都"); 
        //runTimeChunksHashMap.Add("gogo",new GameObject());
        //foreach (DictionaryEntry de in runTimeChunksHashMap) 
        //{
        //    print(de.Key);
        //    print(de.Value);
        //}
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
                    //string chunkName = string.Format(prefixChunkName+"{0},{1}",(int)cursor.x,(int)cursor.y);
                    //if (string.Equals(runTimeLoadChunksArray[i, j].name, chunkName))
                    //{
                    //    continue;
                    //}
                    //else
                    //{
                    //    Transform obj = chunksPrefabs.transform.FindChild(chunkName);
                    //    print(obj);
                    //    GameObject chunkObj = GameObject.Instantiate(obj.gameObject,new Vector3(cursor.x*perChunkSizeX,0,-cursor.y*perChunkSizeZ),Quaternion.identity);
                    //    DestroyImmediate(runTimeLoadChunksArray[i,j]);
                    //    runTimeLoadChunksArray[i, j] = chunkObj;
                    //}
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

        LoadNewAddCache();
        UnLoadNewRemoveCache();

    }

    void LoadNewAddCache()
    {
        IDictionaryEnumerator myEnumerator = newAddCacheHashMap.GetEnumerator();
        while (myEnumerator.MoveNext())
        {
            string keyname = myEnumerator.Key as string;
            string objStr = myEnumerator.Value as string;
            if (keyname != string.Empty && objStr != null)
            {
                string[] sArray = keyname.Split(',');
                int x = int.Parse(sArray[0]);
                int y = int.Parse(sArray[1]);
                string name = string.Format(prefixChunkName + "{0},{1}", x, y);
                Transform obj = chunksPrefabs.transform.FindChild(name);
                GameObject chunkObj = GameObject.Instantiate(obj.gameObject, new Vector3(x * perChunkSizeX, 0, -y * perChunkSizeZ), Quaternion.identity);
                runTimeChunksHashMap.Add(keyname, chunkObj);
                //newAddCacheHashMap.Remove(keyname);
            }
        }
        newAddCacheHashMap.Clear();
        //foreach (DictionaryEntry de in newAddCacheHashMap)
        //{
        //    string keyname = de.Key as string;
        //    string objStr = de.Value as string;
        //    if (keyname != string.Empty && objStr!=null)
        //    {
        //        string[] sArray = keyname.Split(',');
        //        int x = int.Parse(sArray[0]);
        //        int y = int.Parse(sArray[1]);
        //        string name = string.Format(prefixChunkName + "{0},{1}", x, y);
        //        Transform obj = chunksPrefabs.transform.FindChild(name);
        //        GameObject chunkObj = GameObject.Instantiate(obj.gameObject, new Vector3(x * perChunkSizeX, 0, -y * perChunkSizeZ), Quaternion.identity);
        //        runTimeChunksHashMap.Add(keyname, chunkObj);
        //        newAddCacheHashMap.Remove(keyname);
        //        print("55555555");
        //    }
        //}
    }

    void UnLoadNewRemoveCache()
    {
        print("########" + newRemoveCacheHashMap.Count);
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
            }
        }
        newRemoveCacheHashMap.Clear();
        //foreach (DictionaryEntry de in newRemoveCacheHashMap)
        //{
        //    print("88888888");
        //    string keyname = de.Key as string;
        //    GameObject obj = de.Value as GameObject;
        //    if (keyname != string.Empty && obj != null)
        //    {
        //        //DestroyImmediate(obj);
        //        print("999999");
        //        Destroy(obj);
        //        print("777777");
        //        runTimeChunksHashMap.Remove(keyname);
        //        newRemoveCacheHashMap.Remove(keyname);
        //        print("6666666");
        //    }
        //}
    }
    void InitLoadChunks()  //初始化时（或Loading时）已经把asset加载到memory，由于地表prefab共用一套材质，所以不需要额外考虑地表asset的预加载问题
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
                
                //if (CheckDirectionVector(cursor))
                //{
                //    string name = string.Format(prefixChunkName+"{0},{1}", (int)cursor.x, (int)cursor.y);
                //    Transform obj = chunksPrefabs.transform.FindChild(name);
                //    GameObject chunkObj = GameObject.Instantiate(obj.gameObject,new Vector3(cursor.x*perChunkSizeX,0,-cursor.y*perChunkSizeZ),Quaternion.identity);
                //    runTimeLoadChunksArray[i, j] = chunkObj;
                //    //print(cursor);
                //}
                //else
                //{
                //    GameObject obj = new GameObject();
                //    obj.name = "null";
                //    runTimeLoadChunksArray[i, j] = obj;
                //}
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

	// Update is called once per frame
	void Update ()
	{
	    playerCurrentPosForMapCoordinate = GetCurrentPlayerMapCoordinate();
	    int playerX = (int) playerCurrentPosForMapCoordinate.x;
	    int playerY = (int) playerCurrentPosForMapCoordinate.y;
	    int lastLoadX = (int) lastLoadPlayerPoxForMapCoordinate.x;
	    int lastLoadY = (int) lastLoadPlayerPoxForMapCoordinate.y;
	    DynCheckChunks();
        //if (playerX !=lastLoadX || playerY != lastLoadY)
        //{
        //    DynCheckChunks();
        //}
        textUIObject.GetComponent<Text>().text = string.Format("当前位置:（ {0},{1} ）", playerX, playerY);
        //print(runTimeChunksHashMap.Count);
        //print(playerCurrentPosForMapCoordinate.x);
        //print(playerCurrentPosForMapCoordinate.x+","+playerCurrentPosForMapCoordinate.y);
        //print(runTimeChunksHashMap["0,0"]);
	    //DynCheckChunks();
	    //print(playerCurrentPosForMapCoordinate);
	    //MaxDepth = 2;
	    //Vector2 centerOfdirectionArray = new Vector2(Mathf.Floor((MaxDepth*2+1)/2.0f),Mathf.Floor((MaxDepth*2+1)/2.0f));
	    //print(centerOfdirectionArray);

	    //print(obj);
	}
}
