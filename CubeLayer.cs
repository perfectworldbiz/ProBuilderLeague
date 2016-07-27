using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class CubeLayer : PunBehaviour
{
    public GameObject cube;
    List<GameObject> layerCubes = new List<GameObject>();
    Vector3 offset;
    Vector3 rowRight;
    Vector3 rowDown;
    Vector3 up;

    public void ActivateLayer()
    {
        ChangeState(true);
    }
    public void DeactivateLayer()
    {
        ChangeState(false);
    }
    void ChangeState(bool state)
    {
        foreach (GameObject c in layerCubes)
        {
            c.GetComponent<CubeScript>().Activate(state);
        }
    }
    public void BuildLayer(float size)
    {
        offset = cube.transform.localScale;
        rowRight = new Vector3(offset.x, 0, 0);
        rowDown = new Vector3(0, 0, offset.z);
        up = new Vector3(0, offset.y, 0);

        if (size == 1)
        {
            Vector3 start = Vector3.zero + rowDown / 2 + rowRight / 2 + up / 2;
            GameObject cstart = (GameObject)PhotonNetwork.InstantiateSceneObject(cube.name, start, Quaternion.identity, 0, null);
            cstart.transform.SetParent(transform);
            layerCubes.Add(cstart);
        }
        else {

            Vector3 start = Vector3.zero + rowDown / 2 + rowRight / 2 + up / 2 + rowRight * (size - 1);

            //offset for first time, go down and spawn size amount of times, then go minus right and spawn size amount of times. Repeat this for <size number of floors. Last floor is roof.
            Vector3 tempCurrent = start - rowDown;
            for (int floors = 0; floors < size; floors++)
            {
                for (int r = 0; r < size; r++)
                {
                    tempCurrent += rowDown;
                    GameObject c = (GameObject)PhotonNetwork.InstantiateSceneObject(cube.name, tempCurrent, Quaternion.identity, 0, null);
                    c.transform.SetParent(transform);
                    layerCubes.Add(c);
                }
                //tempCurrent += rowRight;
                for (int r = 0; r < size-1; r++)
                {
                    tempCurrent -= rowRight;
                    GameObject c = (GameObject)PhotonNetwork.InstantiateSceneObject(cube.name, tempCurrent, Quaternion.identity, 0, null);
                    c.transform.SetParent(transform);
                    layerCubes.Add(c);
                }
                tempCurrent = start - rowDown + up * (floors + 1);
            }
            //here goes roof
            start = Vector3.zero + rowDown / 2 + rowRight / 2 + up / 2 + rowRight + up * (size - 1);
            tempCurrent = start - rowDown - rowRight;
            for(int rows = 0; rows < size-1; rows++)
            {
                for(int columns = 0; columns < size-1; columns++)
                {
                    tempCurrent += rowDown;
                    GameObject c = (GameObject)PhotonNetwork.InstantiateSceneObject(cube.name, tempCurrent, Quaternion.identity, 0, null);
                    c.transform.SetParent(transform);
                    layerCubes.Add(c);
                }
                tempCurrent = start - rowDown + rowRight*rows;
            }
        }
    }
}
