using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class csTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Stopwatch watch = new Stopwatch();
		watch.Start();

		watch.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
