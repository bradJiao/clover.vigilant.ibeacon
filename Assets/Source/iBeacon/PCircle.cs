using UnityEngine;
using System.Collections;

public class PCircle : MonoBehaviour {
	
	
	
	[Range(10,100)]
	public int resolution = 10;
	public float radius = 1;
	private int currentResolution;
	private float currentRadius ;
	private ParticleSystem.Particle[] points;
	public int NCircle = 1;
	public bool is_show = false;
	
	void CreatePoints ()
	{
		currentResolution = resolution;
		currentRadius = radius;
		points = new ParticleSystem.Particle[(resolution)*NCircle];
		//float increment = 360 /resolution;
		int i = 0;
		for (int m = 0; m < resolution; m++) {
			for (int n = 1; n <= NCircle; n++) {
				float x = radius/n * Mathf.Sin(m*360/resolution);
				float z = radius/n * Mathf.Cos(m*360/resolution);
				Vector3 p = new Vector3(x,0f,z);
				points [i].position = p;
				points[i].color = Color.cyan;
				
				points [i++].size = 0.1f;
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		
		//CreatePoints ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!is_show) {
			//radius = 0.1f;
			particleSystem.SetParticles(points, 0);
			return;
				}
		if (currentResolution != resolution 
		    || currentRadius != radius
		    || points == null) {
			CreatePoints();
		}
		
		particleSystem.SetParticles(points, points.Length);
	}
	
	
}
