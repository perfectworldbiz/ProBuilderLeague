using UnityEngine;
using System.Collections;
using Photon;

public class MatchController : PunBehaviour {

    public static MatchController match;
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
    public void TargetHit(int id)
    {
        photonView.RPC("Hit", PhotonTargets.MasterClient, id);
    }

    //MasterClient only
    [PunRPC]
    void Hit(int id)
    {
        PhotonView view = PhotonView.Find(id);
        //we have the photonview of the hit target, now we affect it
        //here would be code for checking if the hit claim was legit
        view.RPC("Hit", PhotonTargets.AllBuffered);
    }
}
