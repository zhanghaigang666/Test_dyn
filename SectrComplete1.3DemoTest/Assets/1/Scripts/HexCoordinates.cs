using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions.Comparers;

[System.Serializable]
public struct HexCoordinates
{
    
    //public GameObject cube;
	[SerializeField]
	private int x, z;

	public int X {
		get {
			return x;
		}
	}

	public int Z {
		get {
			return z;
		}
	}

	public int Y {
		get {
			return -X - Z;
		}
	}

	public HexCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

	public static HexCoordinates FromOffsetCoordinates (int x, int z) {
        //return new HexCoordinates(x - z / 2, z);
        return new HexCoordinates(x, z);
	}

	public static HexCoordinates FromPosition (Vector3 position) {
		float x = position.x / (HexMetrics.innerRadius * 2f);  //
		float y = -x;

        //float offset = position.z / (HexMetrics.outerRadius * 1.5f * 2); //1.5f * 2
        //float offset =  HexMetrics.outerRadius * 1.5f * 2; //1.5f * 2，
        //float offset =  HexMetrics.outerRadius * 1.5f * 1; //1.5f * 2，
        //float offset =  HexMetrics.outerRadius * 1.5f * 0; //1.5f * 2
        //float offset =  1/(HexMetrics.outerRadius * 1.5f * 2); //1.5f * 2，
        float offset =  2/(HexMetrics.outerRadius * 1.5f * 2); //1.5f * 2，
        x -= offset;
        y -= offset;

		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

        //if (iX + iY + iZ != 0) {
        //    float dX = Mathf.Abs(x - iX);
        //    float dY = Mathf.Abs(y - iY);
        //    float dZ = Mathf.Abs(-x -y - iZ);

        //    if (dX > dY && dX > dZ) {
        //        iX = -iY - iZ;
        //    }
        //    else if (dZ > dY) {
        //        iZ = -iX - iY;
        //    }
        //}

        //return new HexCoordinates(iX, iZ);
	    int doz = (int)(position.z/(HexMetrics.outerRadius*1.5f));
        doz = Mathf.RoundToInt(position.z / (HexMetrics.outerRadius * 1.5f));

        return new HexCoordinates(iX,doz );
	}
    static float distance2(float x1, float y1, float x2, float y2)
    {
        return ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    }
    public static HexCoordinates FromPosition2(Vector3 position)
    {
        float x = position.x;
        float y = -position.z;
        Vector2[] points = new Vector2[3];
        
        float dist = 0;
        float g_MinDistance2 = HexMetrics.minDistance2;
        float mindist= g_MinDistance2 * 1000; 
        int i, index=0;
        float g_unitx = HexMetrics.innerRadius*2f;
        float g_unity = HexMetrics.outerRadius*1.5f;
        
        int cx = (int)(x/g_unitx);
        int cy = (int)(y/g_unity);

        if (y < 0)
        {
            cy -= 1;
        }
        if (x <0)
        {
            cx -= 1;
        }
    
        points[0].x = g_unitx * cx;
        points[1].x = g_unitx * (cx+0.5f);
        points[2].x = g_unitx * (cx+1f);
        
        if(cy % 2 == 0)
        {
            points[0].y = points[2].y = g_unity * cy;
            points[1].y = g_unity * (cy+1);
        }    
        else
        {
            points[0].y = points[2].y = g_unity * (cy+1);
            points[1].y = g_unity * cy;
        }
    
        for(i=0;i<3;i++)
        {
            dist = distance2((float)x,(float)y, (float)points[i].x, (float)points[i].y);
            if(dist < g_MinDistance2)
            {
                index = i;
                break;
            }
        
            if(dist < mindist)
            {
                mindist = dist;
                index = i;
            }
        }
        int iz = (int)(points[index].y / g_unity);
        //int ix = (int)(points[index].x / g_unitx - iz * 0.5f + iz / 2);
        float fx = points[index].x/g_unitx - iz*0.5f + iz/2;
        int ix = Mathf.RoundToInt(fx);
        //HexGrid hex = HexGrid.getInstanse();
        //hex.setCubePos(new Vector2(points[index].x, points[index].y));
        //hex.hexc = new Vector2(points[index].x, points[index].y);
        return new HexCoordinates(ix, iz);
    }

    //public static Vector3 getCubePos()
    //{
    //}

	public override string ToString () {
		return "(" +
			X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}

    public string ToStringOnSeparateLinesNoY()
    {
        return "x = " + X.ToString() + "\n" + "\n" + "y = " +Z.ToString();
    }
}