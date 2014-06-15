using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour {

	public Transform m_Transform;
	public float m_Radius = 1; // 圆环的半径
	public float m_Theta = 0.1f; // 值越低圆环越平滑
	public Color m_Color = Color.green; // 线框颜色
	
	void Start()
	{
		if (m_Transform == null)
		{
			//throw new Exception("Transform is NULL.");
		}
	}

	void Update()
	{

		if (m_Theta < 0.0001f) m_Theta = 0.0001f;
		
	
		// 设置颜色

		
		// 绘制圆环
		Vector3 beginPoint = Vector3.zero;
		Vector3 firstPoint = Vector3.zero;
		for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
		{
			float x = m_Radius * Mathf.Cos(theta);
			float y = m_Radius * Mathf.Sin(theta);
			Vector3 endPoint = new Vector3(x, y, 0);
			if (theta == 0)
			{
				firstPoint = endPoint;
			}
			else
			{
				Gizmos.DrawLine(beginPoint, endPoint);
			}
			beginPoint = endPoint;
		}

		// Make a Vector3 array that contains points for a cube that's 1 unit in size 

		// Make a line using the above points and material, with a width of 2 pixels 
		//var line = new VectorLine("Cube" cubePoints, Color.white, lineMaterial, 2.0); 
		
		// Make this transform have the vector line object that's defined above 
		// This object is a rigidbody, so the vector object will do exactly what this object does 
		//VectorManager.ObjectSetup (gameObject, line, Visibility.Dynamic, Brightness.None); 
	}
	
	void OnDrawGizmos1()
	
	{
		if (m_Transform == null) return;
		if (m_Theta < 0.0001f) m_Theta = 0.0001f;
		
		// 设置矩阵
		Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = m_Transform.localToWorldMatrix;
		
		// 设置颜色
		Color defaultColor = Gizmos.color;
		Gizmos.color = m_Color;
		
		// 绘制圆环
		Vector3 beginPoint = Vector3.zero;
		Vector3 firstPoint = Vector3.zero;
		for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
		{
			float x = m_Radius * Mathf.Cos(theta);
			float y = m_Radius * Mathf.Sin(theta);
			Vector3 endPoint = new Vector3(x, y, 0);
			if (theta == 0)
			{
				firstPoint = endPoint;
			}
			else
			{
				Gizmos.DrawLine(beginPoint, endPoint);
			}
			beginPoint = endPoint;
		}
		
		// 绘制最后一条线段
		Gizmos.DrawLine(firstPoint, beginPoint);
		
		// 恢复默认颜色
		Gizmos.color = defaultColor;
		
		// 恢复默认矩阵
		Gizmos.matrix = defaultMatrix;
	}
}
