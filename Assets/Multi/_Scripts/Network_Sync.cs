using UnityEngine;
using System.Collections;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class Network_Sync : PunBehaviour {

    Vector3 latestCorrectPos;
    Vector3 onUpdatePos;
    Quaternion latestCorrectRot;
    Quaternion onUpdateRot;
    float fraction;
    Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        this.latestCorrectPos = rb.position;
        this.onUpdatePos = rb.position;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = rb.position;
            Quaternion rot = rb.rotation;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else
        {
            // Receive latest state information
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            this.latestCorrectPos = pos;                 // save this to move towards it in FixedUpdate()
            this.onUpdatePos = rb.position;  // we interpolate from here to latestCorrectPos
            this.latestCorrectRot = rot;                 // save this to move towards it in FixedUpdate()
            this.onUpdateRot = rb.rotation;  // we interpolate from here to latestCorrectPos
            this.fraction = 0;                           // reset the fraction we already moved. see Update()
        }
    }

    public void FixedUpdate()
    {
        if (this.photonView.isMine)
        {
            return;     // if this object is under our control, we don't need to apply received position-updates 
        }

        // We get 10 updates per sec. Sometimes a few less or one or two more, depending on variation of lag.
        // Due to that we want to reach the correct position in a little over 100ms. We get a new update then.
        // This way, we can usually avoid a stop of our interpolated cube movement.
        //
        // Lerp() gets a fraction value between 0 and 1. This is how far we went from A to B.
        //
        // So in 100 ms, we want to move from our previous position to the latest known. 
        // Our fraction variable should reach 1 in 100ms, so we should multiply deltaTime by 10.
        // We want it to take a bit longer, so we multiply with 9 instead!

        this.fraction = this.fraction + Time.deltaTime * 9;
        rb.position = Vector3.Lerp(this.onUpdatePos, this.latestCorrectPos, this.fraction); // set our pos between A and B
        rb.rotation = Quaternion.Lerp(onUpdateRot, latestCorrectRot, fraction);
    }
}
