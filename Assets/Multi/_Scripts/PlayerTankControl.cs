using UnityEngine;
using System.Collections;
using Photon;

public class PlayerTankControl : PunBehaviour{

    Rigidbody rb;
    public float speed;
    public float turnSpeed;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (photonView.isMine)
            InitializePlayer();
    }

	void Update()
    {
        if (!photonView.isMine)
            return;

        //movement
        if (Input.GetAxis("Vertical") != 0)
            rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime * Input.GetAxis("Vertical"));
        if(Input.GetAxis("Horizontal") != 0)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            transform.RotateAround(transform.position, transform.InverseTransformVector(Vector3.up), horizontalMovement * turnSpeed); 
        }
        if(Input.GetKey(KeyCode.LeftShift))
            rb.AddForce(Vector3.up * 85);
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z = 0;
            transform.Rotate(rot);
        }
    }

    void InitializePlayer()
    {
        transform.FindChild("Camera").gameObject.SetActive(true);
    }
}
