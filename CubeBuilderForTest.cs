using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class CubeBuilderForTest : PunBehaviour
{

    public bool TestAsServer = false;
    public float cubeSize = 1;
    float currentLayer;
    public GameObject cube;
    public GameObject player;
    public GameObject testServer;
    public GameObject centerHook;
    Vector3 offset;
    Vector3 rowRight;
    Vector3 rowDown;
    Vector3 up;
    public

    void Start()
    {
        offset = cube.transform.localScale;

        rowRight = new Vector3(offset.x, 0, 0);
        rowDown = new Vector3(0, 0, offset.z);
        up = new Vector3(0, offset.y, 0);
        BuildCube();
        SpawnPlayer();
        currentLayer = cubeSize;
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
    }
    public void ResetCamera()
    {
        if (GameObject.Find("CubeCenter(Clone)") == null)
        {
            Instantiate(centerHook, GetCubeCenter(), Quaternion.identity);
        }
        else
        {
            GameObject.Find("CubeCenter(Clone)").transform.position = Vector3.zero + new Vector3(currentLayer / 2, currentLayer / 2, currentLayer / 2);
        }
        MouseOrbitImproved server = GameObject.Find("Server").GetComponent<MouseOrbitImproved>();
        server.target = GameObject.Find("CubeCenter(Clone)").transform;
        server.distanceMin = (currentLayer * Mathf.Sqrt(3)) / 2 + 1;

    }
    public void RebuildCube()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            cube.GetComponent<MeshRenderer>().enabled = true;
            cube.GetComponent<BoxCollider>().enabled = true;
        }
    }
    public void AddLayer()
    {
        
        RebuildCube();
        if(currentLayer < cubeSize)
        {
            string name = "Layer" + (currentLayer+1);
            GameObject.Find(name).GetComponent<CubeLayer>().ActivateLayer();
        }
        else
        {
            GameObject layer = new GameObject();
            layer.transform.SetParent(transform);
            layer.transform.name = "Layer" + (currentLayer+1);
            layer.AddComponent<CubeLayer>();
            CubeLayer layerS = layer.GetComponent<CubeLayer>();
            layerS.cube = this.cube;
            layerS.BuildLayer(currentLayer+1);
            cubeSize++;
        }
        currentLayer++;
        print("Layer is " + currentLayer);
        ResetCamera();
    }
    public void RemoveLayer()
    {
        RebuildCube();
        string name = "Layer" + currentLayer;
        GameObject.Find(name).GetComponent<CubeLayer>().DeactivateLayer();
        currentLayer--;
        print("Layer is " + currentLayer);
        ResetCamera();
    }
    void SpawnPlayer()
    {
        if (!TestAsServer)
            Instantiate(player, GetCubeCenter() + new Vector3(0, 0, -20), Quaternion.identity);
        else
            Instantiate(testServer, GetCubeCenter(), Quaternion.identity);
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
    /*void BuildCube()
    {
        Vector3 startOffset = Vector3.zero + rowDown / 2 + rowRight / 2;
        Bounds bounds = new Bounds(GetCubeCenter(), new Vector3(cubeSize, cubeSize, cubeSize));

        Vector3 tempCurrent = startOffset + up / 2;
        Vector3 tempRow = tempCurrent;
        Vector3 tempFloor = tempCurrent;
        tempCurrent -= rowRight;
        print("Starting build, temo floor is " + tempFloor);
        while (bounds.Contains(tempFloor))
        {
            print("Contains temp floor");
            while (bounds.Contains(tempRow))
            {
                print("Contains temp row");
                while (bounds.Contains(tempCurrent + rowRight))
                {
                    print("Rowing right");
                    tempCurrent += rowRight;
                    SpawnCube(tempCurrent);
                }
                // we arrived at rightmost
                tempRow += rowDown;
                tempCurrent = tempRow;
                tempCurrent -= rowRight;
            }
            // we finished a floor
            tempFloor += up;
            tempCurrent = tempFloor;
            tempRow = tempFloor;
            tempCurrent -= rowRight;
        }
    }*/
}

