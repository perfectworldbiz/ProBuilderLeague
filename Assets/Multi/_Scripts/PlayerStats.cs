using UnityEngine;
using System.Collections;
using Photon;

public class PlayerStats : PunBehaviour {

    [HideInInspector]
    public int timesShot = 0;
    [HideInInspector]
    public int scoredHits = 0;

    public int healthMax = 3;
    int health;

    public float blinkRedTime;
    float timer;
    GameObject tank;
    MeshRenderer tankRen;
    Color matColor;
    bool colored = false;

    void Start()
    {
        tank = transform.FindChild("TankCube").gameObject;
        tankRen = tank.GetComponent<MeshRenderer>();
        matColor = tankRen.material.color;
        health = healthMax;
    }

    [PunRPC]
    void PlayerHit()
    {
        timesShot++;
        ColorTank(true);
        health--;
        if (photonView.isMine && health <= 0)
            Respawn();
        
    }
    [PunRPC]
    void ScoreHit()
    {
        scoredHits++;
        //print("I shot someone");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (colored && timer > blinkRedTime)
            ColorTank(false);
    }
    void ColorTank(bool hit)
    {
        if (hit)
        {
            tankRen.material.color = Color.red;
            colored = true;
            timer = 0;
        }
        else
        {
            tankRen.material.color = matColor;
            colored = false;
        }
    }
    void Respawn()
    {
        transform.position = MatchController.match.GetRandomSpawn();
        health = healthMax;
    }
}
