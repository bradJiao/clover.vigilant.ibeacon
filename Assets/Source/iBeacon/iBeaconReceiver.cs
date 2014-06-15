using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum BeaconRange
{
		UNKNOWN,
		FAR,
		NEAR,
		IMMEDIATE
}

public struct Beacon
{
	
		public string UUID;
		public int major;
		public int minor;
		public BeaconRange range;
		public int strength;
		public double accuracy;

	public GameObject BSObject;
	public iBeaconBS bsScript;
	
		public Beacon (string _uuid, int _major, int _minor, int _range, int _strength, double _accuracy)
		{
				UUID = _uuid;
				major = _major;
				minor = _minor;
				range = (BeaconRange)_range;
				strength = _strength;
				accuracy = _accuracy;

				///
				maxSmoothLength = 5;
				rssiHistory = null;
				accHistory = null;
		BSObject = null;
		bsScript = null;
		}
	
		public override string ToString ()
		{
				return "UUID: " + this.UUID + "\nMajor: " + this.major + "\nMinor: " + this.minor + "\nRange: " + this.range.ToString ();
		}

		public string IDString ()
		{
			return "UUID: " + this.UUID + "-Major: " + this.major + "-Minor: " + this.minor ;
		}
		///smooth rssi
	
		public List<int> rssiHistory ;//for smooth rssi
		public int maxSmoothLength; //smooth avg times

	///smooth rssi
	
	public List<double> accHistory ;//for smooth acc


		/// <summary>
		/// Sets the new RSSI sample,and get smoothed rssi value.
		/// </summary>
		/// <returns>the smoothed rssi value</returns>
		/// <param name="rssi">Rssi.</param>
		public int setNewRSSISample (int rssi)
		{
				if (rssiHistory == null) {
						rssiHistory = new List<int> ();
				}
				if (rssiHistory.Count >= 10) {
						rssiHistory.RemoveAt (0);
				}
				rssiHistory.Add (rssi);

				int sum = 0;
				foreach (var item in rssiHistory) {
						sum += item;
				}
				return sum / rssiHistory.Count;
		}

	public double setNewAccSample (double acc)
	{
		if (accHistory == null) {
			accHistory = new List<double> ();
		}
		if (accHistory.Count >= 10) {
			accHistory.RemoveAt (0);
		}
		accHistory.Add (acc);
		
		double sum = 0;
		foreach (var item in accHistory) {
			sum += item;
		}
		return sum / accHistory.Count;
	}
	
}

public class iBeaconReceiver : MonoBehaviour
{
		public delegate void BeaconRangeChanged (List<Beacon> beacons);

		public static event BeaconRangeChanged BeaconRangeChangedEvent;
		public delegate void BeaconArrived (Beacon beacon);

		public static event BeaconArrived BeaconArrivedEvent;
		public delegate void BeaconOutOfRange (Beacon beacon);

		public static event BeaconOutOfRange BeaconOutOfRangeEvent;

		public string uuid;
		public string region;
	
#if UNITY_ANDROID
	private static AndroidJavaObject plugin;
#endif
	
		private static iBeaconReceiver m_instance;
		public static bool is_device_init_ok = false;
	
		// assign variables to statics
		void Awake ()
		{
				m_instance = this;
		}
	
	#if UNITY_IOS	
	[DllImport ("__Internal")]
	private static extern bool InitReceiver(string uuid, string regionIdentifier, bool shouldLog);

	[DllImport ("__Internal")]
	private static extern void StopIOSScan();
	#endif
		private List<Beacon> m_beacons;
	
		void Start ()
		{
				m_beacons = new List<Beacon> ();
		}
	
		public static bool Init ()
		{
//		is_device_init_ok = true;
//		return true;//for test
				#if !UNITY_EDITOR
				#if UNITY_IOS
		is_device_init_ok = InitReceiver(m_instance.uuid,m_instance.region,true);
		return is_device_init_ok;
				#elif UNITY_ANDROID
		GetPlugin().Call("Init",true);
		is_device_init_ok = true;
		return true;
				#endif
				#endif
				is_device_init_ok = true;
				return true;
		}
	
		public static void Stop ()
		{
#if !UNITY_EDITOR
#if UNITY_IOS
		Stop();
#elif UNITY_ANDROID
		GetPlugin().Call("Stop");
#endif
#endif
		}
	
		public static void Scan ()
		{
#if !UNITY_EDITOR
#if UNITY_IOS
		is_device_init_ok = InitReceiver(m_instance.uuid,m_instance.region,true);
#elif UNITY_ANDROID
		GetPlugin().Call("Scan");
#endif
#endif		
		}
	
#if UNITY_ANDROID
	public static AndroidJavaObject GetPlugin() {
		if (plugin == null) {
			plugin = new AndroidJavaObject("com.kaasa.ibeacon.BeaconService");
		}
		return plugin;
	}
#endif
	
		public void RangeBeacons (string beacons)
		{
				if (!string.IsNullOrEmpty (beacons)) {
						string beaconsClean = beacons.Remove (beacons.Length - 1); // Get rid of last ';'
						string[] beaconsArr = beaconsClean.Split (';');
						List<string> uuids = new List<string> ();
						foreach (string beacon in beaconsArr) {
								string[] beaconArr = beacon.Split (',');
								string uuid = beaconArr [0];
								uuids.Add (uuid);
								int major = int.Parse (beaconArr [1]);
								int minor = int.Parse (beaconArr [2]);
								int range = int.Parse (beaconArr [3]);
								int strenght = int.Parse (beaconArr [4]);
								double accuracy = double.Parse (beaconArr [5]);
								Beacon bTmp = new Beacon (uuid, major, minor, range, strenght, accuracy);
								int listident = 0;
								bool removeme = false;
								foreach (Beacon b in m_beacons) {
										if (b.UUID.Equals (uuid) && b.major == major && b.minor == minor) {
												listident = m_beacons.IndexOf (b);
												removeme = true;
												// only this case need to smooth rssi //by jlk 2014-6-8
												bTmp.rssiHistory = b.rssiHistory;
												bTmp.accHistory = b.accHistory;
										}
								}
								bTmp.strength = bTmp.setNewRSSISample (bTmp.strength);
								bTmp.accuracy = bTmp.setNewAccSample(bTmp.accuracy);

								if (removeme) { // Beacon is already in list, remove it for now and add it again later
								
										m_beacons.RemoveAt (listident);	
								} else { // this is a new beacon, fire new Beacon Event
										if (BeaconArrivedEvent != null)
												BeaconArrivedEvent (bTmp);
								}

								m_beacons.Add (bTmp);
						}
						List<Beacon> deleted_beacon = new List<Beacon> ();
						for (int i = 0; i < m_beacons.Count; i++) {
								if (!uuids.Contains (m_beacons [i].UUID)) { //beacon uuid is not in the list of ranged beacons, delete beacon and fire beacon out of range event 
										if (BeaconOutOfRangeEvent != null) {
												BeaconOutOfRangeEvent (m_beacons [i]);
										}
										//m_beacons.RemoveAt (i);////should not remove while Traversal
										deleted_beacon.Add (m_beacons [i]);
								}
						}

						foreach (var b in deleted_beacon) {
								m_beacons.Remove (b);
						}
						if (BeaconRangeChangedEvent != null) {
								BeaconRangeChangedEvent (m_beacons);
						}
				} else {
						for (int i = 0; i < m_beacons.Count; i++) {
				
								if (BeaconOutOfRangeEvent != null)
										BeaconOutOfRangeEvent (m_beacons [i]);
								//m_beacons.RemoveAt (i); //clear it like this way has bug:after remover the count reduce,but the i already increaced,so some will not be check
						}
						m_beacons.Clear ();
				
						if (BeaconRangeChangedEvent != null) {
								BeaconRangeChangedEvent (m_beacons);
						}
				}
		}
}
