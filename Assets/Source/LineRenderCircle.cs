using UnityEngine;
using System.Collections;

public class LineRenderCircle : MonoBehaviour {

	public float m_Radius = 1; // 圆环的半径
	public float m_Theta = 0.01f; // 值越低圆环越平滑
	public Color m_Color = Color.green; // 线框颜色

	LineRenderer m_line;
	// Use this for initialization
	void Start () {
		m_line = this.gameObject.AddComponent<LineRenderer> ();
		m_line.SetWidth (5, 5);
		m_line.renderer.enabled = true;

		m_line.SetColors (m_Color, m_Color);
	}
	
	// Update is called once per frame
	public
	void Update () {

		int count = (int)(2 * Mathf.PI / m_Theta)  + 3;

		m_line.SetVertexCount (count);
		float theta = 0;
		for (int i = 0; i<count; i++)
		{
			float x = m_Radius * Mathf.Cos(theta);
			float z = m_Radius * Mathf.Sin(theta);
			Vector3 endPoint = new Vector3(x, 0, z);

			m_line.SetPosition(i++,endPoint);

			theta += m_Theta;
		}
	}
}
