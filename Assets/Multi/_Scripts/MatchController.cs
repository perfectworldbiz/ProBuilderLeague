using UnityEngine;
using System.Collections;
using Photon;
using System.Collections.Generic;

public class MatchController : PunBehaviour {

    public static MatchController match;
    public bool debugLogs;
    public bool showControls;
    public GameObject playerSpawn;
    [HideInInspector]
    public GameObject localPlayer;
    public GameObject spawnParent;
    List<Vector3> spawnPoints = new List<Vector3>();

    int players = 0;

    void Awake()
    {
        match = this;
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.offlineMode = true;
            PhotonNetwork.CreateRoom("offline");
        }
    }

    void Start()
    {
        PrepareSpawnPoints();
        if (PhotonNetwork.connected)
        {
            Invoke("SpawnPlayer", 0.5f);
        }
    }
    public override void OnCreatedRoom()
    {
        if (!PhotonNetwork.connected)
        {
            SpawnPlayer();
        }
    }

    void PrepareSpawnPoints()
    {
        foreach(Transform tran in spawnParent.GetComponentsInChildren<Transform>())
        {
            spawnPoints.Add(tran.position);
        }
        spawnPoints.RemoveAt(0); //ignore the empty gameobject
    }

    void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 1, 0);
        spawnPosition += spawnPoints[players];
        GameObject p = (GameObject)PhotonNetwork.Instantiate(playerSpawn.name, spawnPosition, Quaternion.identity, 0);
        localPlayer = p;
        GetComponent<SimpleUI>().localStats = localPlayer.GetComponent<PlayerStats>();
        photonView.RPC("IncrementPlayers", PhotonTargets.AllBufferedViaServer);
    }

    //player tells master that he hit something
    public void TargetHit(int hitID, string name, int shooterID)
    {
        photonView.RPC("Hit", PhotonTargets.MasterClient, hitID, name, shooterID);
    }

    //MasterClient only
    [PunRPC]
    void Hit(int hitID, string name, int shooterID)
    {
        PhotonView view = PhotonView.Find(hitID);
        //we have the photonview of the hit target, now we affect it
        //here would be code for checking if the hit claim was legit
        Debug.Log("HitID " + hitID + " Name " + name + "Shooter " + shooterID);

        if (view.transform.tag == "Target")
        {
            if (view.transform.name == name) //in this case, the photonview is on hit object
                view.RPC("Hit", PhotonTargets.AllBuffered);
            else //in this case photonView manages a larger array of objects (is solo), like a player vehicle
            {
                view.RPC("HitObject", PhotonTargets.AllBuffered, name);
            }
        }
        else if (view.transform.tag == "Player")
        {
            print("trying to process score hit");
            view.RPC("PlayerHit", PhotonTargets.AllBuffered);
            //reward the shooter
            PhotonView.Find(shooterID).RPC("ScoreHit", PhotonTargets.AllBuffered);
        }
    }
    [PunRPC]
    void IncrementPlayers()
    {
        players++;
    }

    public Vector3 GetRandomSpawn()
    {
        Vector3 pos;
        int r = Random.Range(0, spawnPoints.Count + 1);
        pos = spawnPoints[r];

        return pos;
    }
}
