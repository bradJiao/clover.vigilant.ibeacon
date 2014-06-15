using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class iBeaconReceiverExample : MonoBehaviour
{
		private Vector2 scrolldistance;
		private List<Beacon> mybeacons = new List<Beacon> ();
//		private bool scanning = true;
		// Use this for initialization
		void Start ()
		{
				iBeaconReceiver.Init ();
				if (iBeaconReceiver.is_device_init_ok) {
						iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
						iBeaconReceiver.BeaconArrivedEvent += OnBeaconArrived;
						iBeaconReceiver.BeaconOutOfRangeEvent += OnBeaconOutOfRange;
						Debug.Log ("Listening for beacons");
				} else {
						Debug.Log ("init device fail");
				}
		}
	
		void OnDestroy ()
		{
				iBeaconReceiver.BeaconRangeChangedEvent -= OnBeaconRangeChanged;
				iBeaconReceiver.BeaconArrivedEvent -= OnBeaconArrived;
				iBeaconReceiver.BeaconOutOfRangeEvent -= OnBeaconOutOfRange;
				iBeaconReceiver.Stop ();
		}

		private void OnBeaconArrived (Beacon beacon)
		{
				Debug.Log ("Beacon arrived: " + beacon.ToString ());
		}

		private void OnBeaconOutOfRange (Beacon beacon)
		{
				Debug.Log ("Beacon out of range: " + beacon.ToString ());
		}

		private void OnBeaconRangeChanged (List<Beacon> beacons)
		{
		
				mybeacons = beacons;
		
				connectToUnityEntity ();
		
				positioning ();
		}

		private void connectToUnityEntity ()
		{
				for (int i =0; i<mybeacons.Count; i++) {
						var b = mybeacons [i];
						string bs_tag = "bs-" + b.major + "-" + b.minor;
						try {
								b.BSObject = GameObject.FindGameObjectWithTag (bs_tag);
								if (b.BSObject != null) {
									b.bsScript = b.BSObject.GetComponent<iBeaconBS> ();
									mybeacons [i] = b;
								}

						} catch (System.Exception ex) {
				
						}
			
				}
		}
	#if UNITY_IOS	
	[DllImport ("__Internal")]
	private static extern bool calcPos(double[] data, int archorNum, out double outx,out double outy);
	#endif
		private void positioning ()
		{
		#if !UNITY_EDITOR
		#if UNITY_IOS
				Positioning.Point point = null;


				int archornum = 0;
				double[] data = new double[mybeacons.Count * 3];
				double outx = 0;
				double outy = 0;

				int i = 0;
		
				foreach (var item in mybeacons) {
						if (item.BSObject == null && item.accuracy <= 0) {
								continue;
						}
						data [i * 3] = item.BSObject.transform.position.x;
						data [i * 3 + 1] = item.BSObject.transform.position.z;
						data [i * 3 + 2] = (double)item.accuracy;
						i++;
				}
				archornum = i;
		
				if (archornum >= 3) {
						bool ret = calcPos (data, archornum, out outx, out outy);
						if (ret) {
								point = new Positioning.Point ();
								point.x = outx;
								point.y = outy;
						}

				}
#endif
#else

		
		Positioning.Node targetNode = new Positioning.Node (@"target");
				foreach (var item in mybeacons) {
						if (item.BSObject == null) {
								continue;
						}
						Positioning.AnchorNode beaconNode = 
				new Positioning.AnchorNode (item.IDString (),
				                           item.BSObject.transform.position.x,
				                           item.BSObject.transform.position.z,
				                           item.strength,
				                           (double)item.accuracy);
						targetNode.Anchors.Add (beaconNode);

				}
				//Positioning.Point point = Positioning.ExtendedTrilateration.CalculatePosition (targetNode, null, null, false);
				Positioning.Point point = Positioning.MinMaxExtended.CalculatePosition (targetNode, null, null, false);
				//Positioning.Point point = Positioning.MinMax.CalculatePosition (targetNode, null, null, false);
//				//Positioning.Point point = Positioning.ClusterTrilateration.CalculatePosition (targetNode, null, null, false);

#endif

//
//
				if (point != null) {
						var location = new Vector3 ((float)point.x, 0f, (float)point.y);
					
						//Debug.Log ("target position :(" + point.x + "," + point.y + ")");
						showTargetAt (location, true);
						//return location;
				} else {
						var location = new Vector3 (0f, 0f, 0f);
						showTargetAt (location, false);
						//return location;
				}

		}

		private void showTargetAt (Vector3 location, bool show)
		{
				var target = GameObject.FindGameObjectWithTag ("target");
				if (target != null) {
						target.transform.position = location;
						if (show) {
								target.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
						} else {
								target.transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);
						}

				}
		}

		private void hideTarget ()
		{
		}


		//int count = 1000;

		void Update ()
		{
				if (!iBeaconReceiver.is_device_init_ok) {
						return;
				}
				//for test
//				if (count > 0) {
//						count --;
//						var test_beacon = new Beacon ();
//						test_beacon.UUID = "11111-1111-11111";
//						test_beacon.major = 1;
//						test_beacon.minor = 1;
//						mybeacons = new List<Beacon> (){test_beacon};
//				} else {
//						mybeacons = new List<Beacon> ();
//				}
				//for test end

				foreach (var b in mybeacons) {
						//string bs_tag = "bs-" + b.major + "-" + b.minor;
						if (b.bsScript != null) {
								b.bsScript.setBeacon (b);
						}
										
								
						
				}
		}
	
//	void OnGUI() {
//		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
//#if UNITY_ANDROID
//		labelStyle.fontSize = 40;
//#elif UNITY_IOS
//		labelStyle.fontSize = 25;
//#endif
//		float currenty = 10;
//		float labelHeight = labelStyle.CalcHeight(new GUIContent("IBeacons"), Screen.width-20);
//		GUI.Label(new Rect(currenty,10,Screen.width-20,labelHeight),"IBeacons");
//		
//		currenty += labelHeight;
//		scrolldistance = GUI.BeginScrollView(new Rect(10,currenty,Screen.width -20, Screen.height - currenty - 10),scrolldistance,new Rect(0,0,Screen.width - 20,mybeacons.Count*100));
//		GUILayout.BeginVertical("box",GUILayout.Width(Screen.width-20),GUILayout.Height(50));
//		foreach (Beacon b in mybeacons) {
//			GUILayout.Label("UUID: "+b.UUID);
//			GUILayout.Label("Major: "+b.major);
//			GUILayout.Label("Minor: "+b.minor);
//			GUILayout.Label("Range: "+b.range.ToString());
//		}
//		GUILayout.EndVertical();
//		GUI.EndScrollView();
//	}
}
