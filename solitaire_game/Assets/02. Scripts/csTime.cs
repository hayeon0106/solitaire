using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class csTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Stopwatch watch = new Stopwatch();
        watch.Start();

        watch.Stop();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
