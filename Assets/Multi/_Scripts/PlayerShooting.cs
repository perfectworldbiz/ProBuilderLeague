using UnityEngine;
using System.Collections;
using Photon;

public class PlayerShooting : PunBehaviour {

    public float range;
    float laserDuration = 1f;
    float timer;
    public float timeBetweenShots;
    public Transform firePos;
    Vector3 startPos;
    LayerMask shootMask;
    LineRenderer shootLine;
    public GameObject explosion;
    Ray shootRay = new Ray();
    RaycastHit shootHit = new RaycastHit();

    void Awake()
    {
        shootMask = LayerMask.GetMask("Target");
        shootLine = transform.GetComponentInChildren<LineRenderer>();
        shootLine.enabled = false;
        timer = timeBetweenShots;

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (shootLine.enabled && timer > laserDuration)
            shootLine.enabled = false;
    }

    public void Shoot()
    {
        if (timer < timeBetweenShots)
            return;

        timer = 0;
        startPos = firePos.position;
        shootLine.enabled = true;
        shootLine.SetPosition(0, startPos);

        shootRay.origin = startPos;
        shootRay.direction = transform.forward;
        Debug.DrawRay(startPos, transform.forward * range, Color.red, 5);
        bool hit;
        if (Physics.Raycast(shootRay, out shootHit, range, shootMask))
        {
            hit = true;
            shootLine.SetPosition(1, shootHit.point);
            Instantiate(explosion, shootHit.point, Quaternion.identity);
            MatchController.match.TargetHit(shootHit.transform.gameObject.GetComponent<PhotonView>().viewID);
        }
        else
        {
            hit = false;
            shootLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
        photonView.RPC("VisualizeShooting", PhotonTargets.Others, hit, shootHit.point);
    }
    [PunRPC]
    void VisualizeShooting(bool hit, Vector3 point)
    {
        timer = 0;
        startPos = firePos.position;
        shootLine.enabled = true;
        shootLine.SetPosition(0, startPos);
        shootRay.origin = startPos;
        shootRay.direction = transform.forward;

        if (hit)
        {
            shootLine.SetPosition(1, point);
            Instantiate(explosion, point, Quaternion.identity);
        }
        else
        {
            shootLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
}
