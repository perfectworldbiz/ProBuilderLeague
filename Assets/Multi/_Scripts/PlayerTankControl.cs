using UnityEngine;
using System.Collections;
using Photon;

public class PlayerTankControl : PunBehaviour{

    Rigidbody rb;
    public float speed;
    public float turnSpeed;
    PlayerShooting shoot;
    bool showControls = true;
    public Vector2 WidthAndHeight;
    GameObject turret;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shoot = GetComponent<PlayerShooting>();
        turret = GetComponent<PlayerShooting>().turret;
        PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        //input other
        if (Input.GetButton("Fire1"))
            shoot.Shoot();
        if (Input.GetKey(KeyCode.O))
            showControls = !showControls;
    }
    void FixedUpdate()
    {
        if (!photonView.isMine)
            return;

        //movement input
        if (Input.GetAxis("Vertical") != 0)
            //rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime * Input.GetAxis("Vertical"));
            rb.AddForce(transform.forward * speed * Input.GetAxis("Vertical"), ForceMode.Force);
        if (Input.GetAxis("Horizontal") != 0)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            Quaternion turretRot = turret.transform.rotation;
            transform.RotateAround(transform.position, transform.InverseTransformVector(Vector3.up), horizontalMovement * turnSpeed);
            turret.transform.rotation = turretRot;
        }


        if (Input.GetKey(KeyCode.LeftShift))
            rb.AddForce(Vector3.up * 20, ForceMode.Acceleration);
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
    }
    void OnGUI()
    {
        if (MatchController.match.debugLogs)
        {
            GUILayout.Label("FPS: " + System.Math.Round(FPS_Counter.fpsCounter.fps, 1));
            /*if (Camera.current != null)
                GUILayout.Label("CamPos: " + Camera.current.transform.position);*/
            GUILayout.Label("CamTarget: " + shoot.turretTarget);

            //Network Information
            NetworkingPeer npeer = PhotonNetwork.networkingPeer;
            GUILayout.Label("Network Information");
            if (PhotonNetwork.isMasterClient)
                GUILayout.Label("Master");
            else
                GUILayout.Label("Client");
            GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
            GUILayout.Label("BytesIn: " + npeer.BytesIn + " AvgSizePerPacketInc: " + npeer.TrafficStatsIncoming.TotalPacketBytes/npeer.TrafficStatsIncoming.TotalPacketCount);
            GUILayout.Label("BytesOut: " + npeer.BytesOut + " AvgSizePerPacketOut: " + npeer.TrafficStatsOutgoing.TotalPacketBytes/npeer.TrafficStatsOutgoing.TotalPacketCount);
            GUILayout.Label("TrafficStatsIncoming: " + npeer.TrafficStatsIncoming);
            GUILayout.Label("TrafficStatsOutgoing: " + npeer.TrafficStatsOutgoing);
            Debug.Log(" BytesCurrentDispatch: " + npeer.ByteCountCurrentDispatch);
            Debug.Log(" BytesLastOperation: " + npeer.ByteCountLastOperation);

        }
        if (MatchController.match.showControls && this.showControls)
        {
            Rect content = new Rect((Screen.width - this.WidthAndHeight.x), 0, this.WidthAndHeight.x, this.WidthAndHeight.y);
            GUI.Box(content, "Controls");
            GUILayout.BeginArea(content);
            GUILayout.Space(20);
            GUILayout.Label("WASD to move");
            GUILayout.Label("SHIFT to fly");
            GUILayout.Label("Left Mouse to shoot");
            GUILayout.Label("R to flip upside down");
            GUILayout.Label("SPACE to reset position");
            GUILayout.Label("Lft Alt to Free Mouse");
            GUILayout.Label("ESC for menu");
            GUILayout.EndArea();
        }
    }
}
