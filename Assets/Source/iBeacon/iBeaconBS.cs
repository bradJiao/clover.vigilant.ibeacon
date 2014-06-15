using UnityEngine;
using System.Collections;

/// <summary>
/// I beacon BS
/// 
/// </summary>
public class iBeaconBS : MonoBehaviour
{

		public bool m_isDetected;
		private int clear_display_delay = 5;
		private Beacon m_beacon;

		public void setBeacon (Beacon beacon)
		{
				this.m_beacon = beacon;

				this.m_isDetected = true;

				clear_display_delay = 5;
		}

		void OnGUI ()
		{
				if (this.m_isDetected) {

						if (clear_display_delay > 0) {
								clear_display_delay--;
						}

						if (clear_display_delay == 0) {
								this.m_isDetected = false;
						}
						

//						Debug.Log ("Beacon detected:" + this.m_beacon.UUID + "-" +
//								this.m_beacon.major + "-" + this.m_beacon.minor + 
//								"***RSSI:" + this.m_beacon.strength + ", acc: " + this.m_beacon.accuracy);

						TextMesh bsText = this.GetComponentInChildren<TextMesh> ();
						string tag = this.tag;
						//Debug.Log(bsText);
						bsText.text =  tag;
						
						PCircle circle_scrpt = this.GetComponentInChildren<PCircle> ();
						if ( circle_scrpt  != null && this.m_beacon.accuracy > 0) {
								circle_scrpt .is_show = true;
								circle_scrpt .radius = (float)this.m_beacon.accuracy;
						}
//			VectrosityCircle[] vcircle_scrpt = this.GetComponentsInChildren<VectrosityCircle>();
//			if (vcircle_scrpt.Length > 0 && vcircle_scrpt[0] != null) {
//				vcircle_scrpt[0].is_show = true;
//				vcircle_scrpt[0].radius = (float)this.m_beacon.accuracy;
//			}
//			

				} else {
						TextMesh bsText = this.GetComponentInChildren<TextMesh> ();
						//Debug.Log(bsText);
						bsText.text = "";


						PCircle[] circle_scrpt = this.GetComponentsInChildren<PCircle> ();
						if (circle_scrpt.Length > 0 && circle_scrpt [0] != null) {
								circle_scrpt [0].is_show = false;
								//circle_scrpt[0].radius = (float)this.m_beacon.accuracy;
						}

//			VectrosityCircle[] vcircle_scrpt = this.GetComponentsInChildren<VectrosityCircle>();
//			if (vcircle_scrpt.Length > 0 && vcircle_scrpt[0] != null) {
//				vcircle_scrpt[0].is_show = false;
//				//vcircle_scrpt[0].radius = (float)this.m_beacon.accuracy;
//			}
				}
		}
}
