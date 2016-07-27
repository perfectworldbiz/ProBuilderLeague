using UnityEngine;
using System.Collections;
using Photon;

public class CubeScript : PunBehaviour {

    [HideInInspector]
    public int health = 10;
    public GameObject explosion;
    public LayerMask cubeMask;
    int leftDamage;

    public void DamageCube(int dmg) //done by client
    {
        photonView.RPC("PunDamage", PhotonTargets.MasterClient, dmg);
    }
    public void PartialDamageCube(int dmg)
    {
        //subtract health
        //transmit  1/6 damage in 6 directions
        //check health
        float invokeTime = 0.2f;
        if (dmg != 200)
        {
            invokeTime = 0.05f;
        }

        if (dmg > health)
        {
            dmg -= health;
            health = 0;
            leftDamage = dmg;
            Invoke("TransmitDamage", invokeTime);
        }
        else health -= dmg;
        Invoke("CheckHealth", invokeTime);
    }
    [PunRPC]
    void PunDamage(int dmg)
    {
        //subtract health
        //transmit  1/6 damage in 6 directions
        //check health
        float invokeTime = 0.2f;
        if (dmg != 200)
        {
            invokeTime = 0.05f;
        }

        if (dmg > health)
        {
            dmg -= health;
            health = 0;
            leftDamage = dmg;
            Invoke("TransmitDamage", invokeTime);
        }
        else health -= dmg;
        Invoke("CheckHealth", invokeTime);
    }
    void TransmitDamage()
    {
        int partialDamage = leftDamage / 6;
        GetComponent<BoxCollider>().enabled = false;
        //calc
        float lenght = 1.05f;
        Vector3[] directions = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        foreach (Vector3 dir in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, dir), out hit, lenght, cubeMask))
            {
                if (hit.collider.tag == "Cube")
                {
                    hit.collider.SendMessage("PartialDamageCube", partialDamage, SendMessageOptions.DontRequireReceiver);
                    print("Cube hit");
                }
            }
        }
        GetComponent<BoxCollider>().enabled = true;
    }

    void CheckHealth()
    {
        if(health <= 0)
        {
            photonView.RPC("HideCube", PhotonTargets.All, true);
        }
        else //ChangeColor
        {
            photonView.RPC("HideCube", PhotonTargets.All, false);
        }
    }
    [PunRPC]
    void HideCube(bool hide)
    {
        if (hide)
        {
            //spawn some sparks
            Instantiate(explosion, transform.position, Quaternion.identity);
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }
        else //ChangeColor
        {
            float c = 0.5f;
            GetComponent<MeshRenderer>().material.color = new Color(c, c, c, 1);
        }
    }
    public void Activate(bool state)
    {
        photonView.RPC("PunActivation", PhotonTargets.All, state);
    }
    [PunRPC]
    void PunActivation(bool state)
    {
        gameObject.SetActive(state);
        if (state)
            health = 10;
    }
}
