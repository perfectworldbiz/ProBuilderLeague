using UnityEngine;
using System.Collections;
using Photon;

public class TargetPractice : PunBehaviour {

    [PunRPC]
    public void Hit()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        Debug.Log("RPC went through");
    }
}
