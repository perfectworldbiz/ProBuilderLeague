using UnityEngine;
using System.Collections;
using Photon;

public class PlayerTankControl : PunBehaviour{

    Rigidbody rb;
    public float speed;
    public float turnSpeed;
    PlayerShooting shoot;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shoot = GetComponent<PlayerShooting>();
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        //movement
        if (Input.GetAxis("Vertical") != 0)
            rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime * Input.GetAxis("Vertical"));
        if (Input.GetAxis("Horizontal") != 0)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            transform.RotateAround(transform.position, transform.InverseTransformVector(Vector3.up), horizontalMovement * turnSpeed);
        }
        if (Input.GetKey(KeyCode.LeftShift))
            rb.AddForce(Vector3.up * 85);
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        if (Input.GetKey(KeyCode.R))
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.x = 0;
            transform.rotation = Quaternion.identity;
        }
        //input
        if (Input.GetButton("Fire1"))
            shoot.Shoot();

    }
    void OnGUI()
    {
        if (!MatchController.match.debugLogs)
            return;
        GUILayout.Label("Ping is " + PhotonNetwork.GetPing() + " and whole trip is " + PhotonNetwork.GetPing() * 2);
        GUILayout.Label("WASD to move");
        GUILayout.Label("SHIFT to fly");
        GUILayout.Label("Left Mouse/LeftControl to shoot");
        GUILayout.Label("R to flip upside down");
        GUILayout.Label("SPACE to reset position");

    }
}
