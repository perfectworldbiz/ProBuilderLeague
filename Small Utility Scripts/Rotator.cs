using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public Vector3 torq;
    void Start ()
    {
        GetComponent<Rigidbody>().AddTorque(torq);
	}
}
