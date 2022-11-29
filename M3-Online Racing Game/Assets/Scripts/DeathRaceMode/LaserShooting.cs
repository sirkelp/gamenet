using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserShooting : VehicleShooting
{
    public Transform firePoint1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        Debug.DrawRay(firePoint1.position,firePoint1.TransformDirection(Vector3.forward) * 20, Color.red);
    }

    protected override void Fire()
    {
        base.Fire();

        RaycastHit hit;
        //Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        
        //if (Physics.Raycast(ray, out hit, 200))
        if (Physics.Raycast(firePoint1.position, firePoint1.TransformDirection(Vector3.forward), out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            
            //photonView.RPC("CreateHitEffect", RpcTarget.All, hit.point);

            if (hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
            else 
            {
                Debug.Log("Else block");
            }
        }
    }
}
