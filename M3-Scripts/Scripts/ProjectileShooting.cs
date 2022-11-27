using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileShooting : VehicleShooting
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float bulletForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
    }

    protected override void Fire()
    {
        base.Fire();

        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode.Impulse);
    }
}
