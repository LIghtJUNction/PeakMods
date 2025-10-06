using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveScan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Space ) ) {
            ScanFeature.ExecuteScan( transform );
        }
        // mark player's walked ground as road: when player moves more than threshold, register position
        float moveThreshold = 0.5f;
        // store last recorded position in a component field (use static backing via gameObject name)
        if (!this.gameObject.TryGetComponent<LastPosTracker>(out var tracker)) tracker = this.gameObject.AddComponent<LastPosTracker>();
        if (Vector3.Distance(tracker.lastPos, transform.position) > moveThreshold) {
            tracker.lastPos = transform.position;
            ScanFeature.MarkRoadAt(transform.position);
        }
    }
}

public class LastPosTracker : MonoBehaviour { public Vector3 lastPos = Vector3.zero; }
