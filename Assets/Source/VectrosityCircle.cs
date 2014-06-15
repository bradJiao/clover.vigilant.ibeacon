using UnityEngine;
using System.Collections;
using Vectrosity;

public class VectrosityCircle : MonoBehaviour
{

		public float dotSize = 2.0f;
		public int numberOfDots = 100;
		public int numberOfRings = 1;
		public Color dotColor = Color.cyan;
		public float radius = 2;
		public bool is_show = false;

		private float currentRadius;
		private bool currentShowState;

		VectorPoints dots;

		// Use this for initialization
		void Start ()
		{
	
		}

		void CreateandShowPoints ()
		{
				currentRadius = radius;
				currentShowState = is_show;

				var totalDots = numberOfDots * numberOfRings;
				var dotPoints = new Vector3[totalDots];
				var dotColors = new Color[totalDots];
		
				//		var reduceAmount = 1.0 - .75/totalDots;
				for (int i = 0; i < totalDots; i++) {
						dotColors [i] = dotColor;
				}
				dots = new VectorPoints ("Dots", dotPoints, dotColors, null, dotSize);
				//var x = this.transform.position.x;
				//var y = this.transform.position.y;
				for (int i = 0; i < numberOfRings; i++) {
						dots.MakeCircle (this.transform.position, radius / (i + 1), numberOfDots, numberOfDots * i);
				}
				dots.Draw ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (is_show) {
						if (currentRadius != radius) {
								CreateandShowPoints ();


						} else {
								var totalDots = 0;
								var dotPoints = new Vector3[totalDots];
								var dotColors = new Color[totalDots];
								dots = new VectorPoints ("Dots", dotPoints, dotColors, null, dotSize);
								dots.Draw ();
						}
	

				}
		}
}
