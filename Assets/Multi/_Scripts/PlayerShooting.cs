using UnityEngine;
using System.Collections;
using Photon;

public class PlayerShooting : PunBehaviour {

    public float range;
    float laserDuration = 1f;
    float timer;
    public float timeBetweenShots;
    public Transform firePos;
    public GameObject turret;
    public GameObject pcamera;
    GameObject barell;

    public float turretRotateSpeed;
    public float mouseSensitivity = 100f;
    public float ClampAngleTop;
    public float ClampAngleBot;
    public bool invertedVertical = true;
    float rotX = 0f;
    float rotY = 0f;
    bool freeCursor = false;

    public Vector3 turretTarget;

    Vector3 startPos;
    public LayerMask lookMask;
    LayerMask shootMask;
    LineRenderer shootLine;
    public GameObject explosion;
    Ray shootRay = new Ray();
    RaycastHit shootHit = new RaycastHit();

    void Awake()
    {
        shootMask = LayerMask.GetMask("Target", "Player");
        shootLine = transform.GetComponentInChildren<LineRenderer>();
        shootLine.enabled = false;
        timer = timeBetweenShots;

        Vector3 rot = turret.transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        Cursor.lockState = CursorLockMode.Locked;

        barell = turret.transform.FindChild("Barell").gameObject;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (shootLine.enabled && timer > laserDuration)
            shootLine.enabled = false;

        if (!photonView.isMine)
            return;
        //Mouse look movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        RotateClient(mouseX, mouseY);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            freeCursor = !freeCursor;
            Cursor.lockState = freeCursor? CursorLockMode.None : CursorLockMode.Locked;
        }     
    }
    void LateUpdate()
    {
        //check if crosshair has a target for turret
        if (photonView.isMine)
        {
            if (Camera.current != null)
            {
                Ray ray = new Ray(Camera.current.transform.position, Camera.current.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, range, lookMask))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                    turretTarget = hit.point;
                }
                else
                {
                    turretTarget = ray.direction * range + turret.transform.position;
                    Debug.DrawRay(ray.origin, ray.direction * range, Color.yellow);
                }
            } 
        }
        RotateTurret();
    }
    void RotateTurret()
    {
        Vector3 direction;
        Quaternion toRotation = turret.transform.rotation;
        //turret has a precise target
        direction = turretTarget - turret.transform.position;
        toRotation = Quaternion.LookRotation(direction);
        turret.transform.rotation = Quaternion.Lerp(turret.transform.rotation, toRotation, turretRotateSpeed * Time.deltaTime);
    }
    [PunRPC]
    void ShareTarget(Vector3 sharedTarget)
    {
        turretTarget = sharedTarget;
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
        shootRay.direction = barell.transform.up;
        Debug.DrawRay(startPos, barell.transform.up * range, Color.red, 5);
        bool hit;
        if (Physics.Raycast(shootRay, out shootHit, range, shootMask))
        {
            hit = true;
            shootLine.SetPosition(1, shootHit.point);
            Instantiate(explosion, shootHit.point, Quaternion.identity);
            MatchController.match.TargetHit(shootHit.transform.gameObject.GetComponentInParent<PhotonView>().viewID, shootHit.transform.name, photonView.viewID);
        }
        else if (Physics.Raycast(shootRay, out shootHit, range, lookMask))
        {
            hit = true;
            shootLine.SetPosition(1, shootHit.point);
            //Instantiate(explosion, shootHit.point, Quaternion.identity);
            //MatchController.match.TargetHit(shootHit.transform.gameObject.GetComponentInParent<PhotonView>().viewID, shootHit.transform.name);
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
        shootRay.direction = barell.transform.up;

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
    private void RotateClient(float mouseX, float mouseY)
    {
        if (freeCursor) return;
        rotX += mouseY * mouseSensitivity * Time.deltaTime * (invertedVertical ? -1 : 1);
        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, ClampAngleBot, ClampAngleTop);

        pcamera.transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(turretTarget, 2);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(turretTarget);
        }
        else
        {
            turretTarget= (Vector3)stream.ReceiveNext();
        }
    }
    /*stare na wszelki wypadek
     * void RotateTurret(Vector3 target, bool hit)
    {
        Vector3 direction;
        Quaternion toRoration = turret.transform.rotation;
        if (hit) //turret has a precise target
        {
            direction = target-turret.transform.position;
            toRoration = Quaternion.LookRotation(direction);
        }
        else //turret just follows camera y
        {
            Vector3 angles = turret.transform.rotation.eulerAngles;
            angles.y = pcamera.transform.rotation.eulerAngles.y;
            toRoration.eulerAngles = angles;
        }
        turret.transform.rotation = Quaternion.Lerp(turret.transform.rotation, toRoration, turretRotateSpeed * Time.deltaTime);*/
}
