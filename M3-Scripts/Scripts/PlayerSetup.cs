using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;
    //public VehicleShooting vehicleShooting;
    // Start is called before the first frame update
    void Start()
    {
        this.playerCamera = transform.Find("Camera").GetComponent<Camera>();
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponentInChildren<VehicleShooting>().enabled = false;
            GetComponentInChildren<VehicleShooting>().weaponPrefab.SetActive(false);
            GetComponentInChildren<VehicleShooting>().healthBar.enabled = false;
            GetComponent<LapController>().enabled = photonView.IsMine;
            playerCamera.enabled = photonView.IsMine;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponentInChildren<VehicleShooting>().enabled = photonView.IsMine;
            GetComponentInChildren<VehicleShooting>().weaponPrefab.SetActive(true);
            GetComponentInChildren<VehicleShooting>().healthBar.enabled = true;
            GetComponent<LapController>().enabled = false;
            playerCamera.enabled = photonView.IsMine;
        }
        
    }
}
