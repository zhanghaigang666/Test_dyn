using UnityEngine;

public static class HexMetrics {

    //public const float outerRadius = 10f;
    public const float outerRadius = 1f / 2f; //6.298723e-07

    //public const float innerRadius = outerRadius * 0.866025404f;
    public const float innerRadius = 0.8660352f/2;

    public const float minDistance2 = outerRadius * outerRadius*0.75f;

	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};
}