using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;

public class ClientControl : PunBehaviour {

    public float mouseSensitivity = 100f;
    public float ClampAngle = 80f;
    public bool invertedVertical = true;
    public LayerMask cubeMask;
    public ParticleSystem sys;
    float rotX = 0f;
    float rotY = 0f;
    GameController gamecon;

    //shooting
    float fireRate = 1f;
    float lastFire = 0;
    Text fireRateText;
    int shotDamage = 200;
    bool UI_open = false;


    void Start()
    {
        transform.name = "Client";
        if (!photonView.isMine)
           return;
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        
        gamecon = GameObject.Find("GameController").GetComponent<GameController>();
        gamecon.ShowClientUI();
        fireRateText = GameObject.Find("FireRateText").GetComponent<Text>();
        Cursor.visible = false;
    }
    void Update()
    {
        if (!photonView.isMine)
            return;
        if (Input.GetKeyDown(KeyCode.Escape)) gamecon.ShowESCMenuClient();
        if (UI_open)
            return;
        //Mouse look movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        RotateClient(mouseX, mouseY);


        //Rate of fire logic
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            fireRate -= Input.GetAxis("Mouse ScrollWheel")/(1/fireRate); //the variable is inversely proportional to the displayed information
            fireRate = Mathf.Clamp(fireRate, 0.001f, 1f);
        }
        fireRateText.text = "Szybkostrzelność = " + System.Math.Round(1 / fireRate, 1) + " strz/s";

        //Firing input and logic
        if (Input.GetButton("Fire1") && Time.time > lastFire) {

            lastFire = Time.time + fireRate;
            FocusedFire();
        }
        if (Input.GetButton("Fire2") && Time.time > lastFire) {
            lastFire = Time.time + fireRate;
            SpreadFire();
        }  
    }

    private void RotateClient(float mouseX, float mouseY)
    {
        if (Input.GetKey(KeyCode.LeftShift)) return;
        rotX += mouseY * mouseSensitivity * Time.deltaTime * (invertedVertical ? -1 : 1);
        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
    }

    void FocusedFire()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        BulletEmission(fwd);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, fwd), out hit, 200, cubeMask))
        {
            if(hit.collider.tag == "Cube")
            {
                hit.collider.SendMessage("DamageCube", shotDamage, SendMessageOptions.DontRequireReceiver);
                print("Cube hit");
                
            }
        }
        Debug.DrawRay(transform.position, fwd*200, Color.yellow, 2);
    }

    void SpreadFire()
    {
        int spreadFactor = Random.Range(0, 30);
        Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        pos += (Vector3)Random.insideUnitCircle * spreadFactor;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        BulletEmission(ray.direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, cubeMask))
        {
            if (hit.collider.tag == "Cube")
            {
                hit.collider.SendMessage("DamageCube", shotDamage, SendMessageOptions.DontRequireReceiver);
                print("Cube hit");

            }
        }
        Debug.DrawRay(transform.position, ray.direction * 200, Color.yellow, 2);
    }
    
    void BulletEmission(Vector3 dir)
    {
        Vector3 localDir = transform.position + dir*20;
        photonView.RPC("PunEmission", PhotonTargets.All, localDir);
    }
    [PunRPC]
    void PunEmission(Vector3 dir)
    {
        sys.transform.LookAt(dir);
        sys.Play();
    }
    public void ShowUI()
    {
        Cursor.visible = true;
        UI_open = true;

    }
    public void HideUI()
    {
        UI_open = false;
        Cursor.visible = false;
    }
}
