using UnityEngine;
using System.Collections;
using Photon;

public class TargetPracticeSolo : PunBehaviour {

    [PunRPC]
    public void HitObject(string name)
    {
        transform.FindChild(name).GetComponent<MeshRenderer>().material.color = Color.red;
        Debug.Log("RPC went through solo photonView");
    }
}
