using UnityEngine;
using System.Collections;
using Photon;

public class Network_Setup : PunBehaviour {
    //if we own this:
    //enable camera
    //enable input (player control script)
    //set kinematic to false

    void Start()
    {
        if (!photonView.isMine)
            return;

        transform.FindChild("Camera").gameObject.SetActive(true);
        GetComponent<PlayerTankControl>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
