using UnityEngine;
using System.Collections;
using Photon;

public class MatchController : PunBehaviour {

    public static MatchController match;
    public bool debugLogs;
    public GameObject player;

    void Awake()
    {
        match = this;
    }

    void Start()
    {
        PhotonNetwork.Instantiate(player.name, Vector3.zero + new Vector3(0,1,0), Quaternion.identity, 0);
    }

    //player tells master that he hit something
    public void TargetHit(int id, string name)
    {
        photonView.RPC("Hit", PhotonTargets.MasterClient, id, name);
    }

    //MasterClient only
    [PunRPC]
    void Hit(int id, string name)
    {
        PhotonView view = PhotonView.Find(id);
        //we have the photonview of the hit target, now we affect it
        //here would be code for checking if the hit claim was legit
        if (view.transform.name == name) //in this case, the photonview is on hit object
            view.RPC("Hit", PhotonTargets.AllBuffered);
        else //in this case photonView manages a larger array of objects (is solo)
        {
            view.RPC("HitObject", PhotonTargets.AllBuffered, name);
        }
    }
}
