using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.UI;

public class CubeBuilder : PunBehaviour
{
    public bool TestAsServer = false;
    public float cubeSize = 10;
    float currentLayer;
    public GameObject cube;
    public GameObject player;
    public GameObject testServer;
    public GameObject centerHook;
    public Text cubeSizeText;
    Vector3 offset;
    Vector3 rowRight;
    Vector3 rowDown;
    Vector3 up;
    public

    void Start()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        offset = cube.transform.localScale;
        rowRight = new Vector3(offset.x, 0, 0);
        rowDown = new Vector3(0, 0, offset.z);
        up = new Vector3(0, offset.y, 0);
        BuildCube();
        currentLayer = cubeSize;
        SpawnPlayer();
    }

    void BuildCube()
    {
        for (int i = 1; i <= cubeSize; i++)
        {
            GameObject layer = new GameObject();
            layer.transform.SetParent(transform);
            layer.transform.name = "Layer" + i;
            layer.AddComponent<CubeLayer>();
            CubeLayer layerS = layer.GetComponent<CubeLayer>();
            layerS.cube = this.cube;
            layerS.BuildLayer(i);
        }
        Invoke("ResetCamera", 0.5f);
    }
    public void ResetCamera()
    {
        if (GameObject.Find("CubeCenter(Clone)") == null)
        {
            PhotonNetwork.InstantiateSceneObject(centerHook.name, GetCubeCenter(), Quaternion.identity, 0, null);
        }
        else
        {
            GameObject.Find("CubeCenter(Clone)").transform.position = Vector3.zero + new Vector3(currentLayer / 2, currentLayer / 2, currentLayer / 2);
        }
        MouseOrbitImproved server = GameObject.Find("Server").GetComponent<MouseOrbitImproved>();
        server.target = GameObject.Find("CubeCenter(Clone)").transform;
        server.distanceMin = (currentLayer * Mathf.Sqrt(3)) / 2 + 1;
        cubeSizeText.text = "Wielkość kostki: " + currentLayer;
        GameObject client = GameObject.Find("ClientTEST(Clone)");
        if(client != null)
            client.GetComponent<Network_Setup>().NewPosition(Vector3.zero + new Vector3(currentLayer / 2, currentLayer / 2, currentLayer / 2) + new Vector3(0, 0, -currentLayer/2 -20));

    }
    public void RebuildCube()
    {
        photonView.RPC("Rebuild", PhotonTargets.All);
    }
    [PunRPC]
    void Rebuild()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            cube.GetComponent<MeshRenderer>().enabled = true;
            cube.GetComponent<BoxCollider>().enabled = true;
            cube.GetComponent<CubeScript>().health = 10;
            cube.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);
        }
    }
    public void AddLayer()
    {
        if (currentLayer == 35)
            return;
        RebuildCube();
        if (currentLayer < cubeSize)
        {
            string name = "Layer" + (currentLayer + 1);
            GameObject.Find(name).GetComponent<CubeLayer>().ActivateLayer();
        }
        else
        {
            GameObject layer = new GameObject();
            layer.transform.SetParent(transform);
            layer.transform.name = "Layer" + (currentLayer + 1);
            layer.AddComponent<CubeLayer>();
            CubeLayer layerS = layer.GetComponent<CubeLayer>();
            layerS.cube = this.cube;
            layerS.BuildLayer(currentLayer + 1);
            cubeSize++;
        }
        currentLayer++;
        print("Layer is " + currentLayer);
        ResetCamera();
    }
    public void RemoveLayer()
    {
        if (currentLayer == 1)
            return;
        RebuildCube();
        string name = "Layer" + currentLayer;
        GameObject.Find(name).GetComponent<CubeLayer>().DeactivateLayer();
        currentLayer--;
        print("Layer is " + currentLayer);
        ResetCamera();
    }
    void SpawnPlayer()
    {
        photonView.RPC("SpawnPlayerController", PhotonTargets.All);
    }
    [PunRPC]
    void SpawnPlayerController()
    {
        if (PhotonNetwork.isMasterClient) { PhotonNetwork.Instantiate(testServer.name, GetCubeCenter() + new Vector3 (0,0,-30), Quaternion.identity, 0); }
        else { PhotonNetwork.Instantiate(player.name, GetCubeCenter() + new Vector3(0, 0, -20), Quaternion.identity, 0); }
    }

    void SpawnCube(Vector3 loc)
    {
        GameObject c = (GameObject)Instantiate(cube, loc, Quaternion.identity);
        c.transform.SetParent(transform);
    }

    Vector3 GetCubeCenter()
    {
        Vector3 center = Vector3.zero;
        center += new Vector3(cubeSize / 2, cubeSize / 2, cubeSize / 2);

        return center;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetCubeCenter(), new Vector3(cubeSize, cubeSize, cubeSize));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero + rowDown / 2 + rowRight / 2 + up / 2, offset);
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this object: send the others our data
            stream.SendNext(currentLayer);
        }
        else
        {
            // Network player, receive data
            this.currentLayer = (float)stream.ReceiveNext();
        }
    }
}
