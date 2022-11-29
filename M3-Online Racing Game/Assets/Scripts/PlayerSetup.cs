using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;
    public TextMeshProUGUI playerName;
    public GameObject playerUiPrefab;

    //public VehicleShooting vehicleShooting;
    // Start is called before the first frame update
    void Start()
    {
        //this.playerCamera = transform.Find("Camera").GetComponent<Camera>();
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponentInChildren<VehicleShooting>().enabled = false;
            GetComponentInChildren<VehicleShooting>().weaponPrefab.SetActive(false);
            GetComponentInChildren<VehicleShooting>().healthBarParent.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<LapController>().enabled = photonView.IsMine;
            playerCamera.enabled = photonView.IsMine;

            if (photonView.IsMine)
            {
                GameObject uiPrefab = Instantiate(playerUiPrefab);
                uiPrefab.GetComponent<UiManager>().onScreenHealthbarParent.SetActive(false);
                uiPrefab.GetComponent<UiManager>().crosshair.SetActive(false);
                GameObject uiMainCanvas = uiPrefab.transform.Find("Main UI Canvas").gameObject;
                uiMainCanvas.transform.Find("Quit Button").GetComponent<Button>().onClick.AddListener( () => DeathRaceManager.instance.OnQuitButtonClicked());
            }
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponentInChildren<VehicleShooting>().enabled = photonView.IsMine;
            GetComponentInChildren<VehicleShooting>().weaponPrefab.SetActive(true);
            
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<LapController>().enabled = false;
            playerCamera.enabled = photonView.IsMine;
            playerName.text = photonView.Owner.NickName;

            if (photonView.IsMine)
            {
                GameObject uiPrefab = Instantiate(playerUiPrefab);
                uiPrefab.GetComponent<UiManager>().onScreenHealthbarParent.SetActive(true);
                uiPrefab.GetComponent<UiManager>().crosshair.SetActive(true);
                GetComponentInChildren<VehicleShooting>().healthBarParent.SetActive(false);
                GameObject uiMainCanvas = uiPrefab.transform.Find("Main UI Canvas").gameObject;
                uiMainCanvas.transform.Find("Quit Button").GetComponent<Button>().onClick.AddListener( () => DeathRaceManager.instance.OnQuitButtonClicked());
            
                GetComponentInChildren<VehicleShooting>().uiManager = uiPrefab.GetComponent<UiManager>();
            }
            else
            {
                GetComponentInChildren<VehicleShooting>().healthBarParent.SetActive(true);
            }
        }
        
    }
}
