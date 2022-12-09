using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;
    public TextMeshProUGUI playerName;
    public GameObject localUiHolder;
    public GameObject playerUiPrefab;
    public GameObject playerBuff;
    public string playerRole;

    void Start()
    {
        object playerRoleHolder;
        if (photonView.Owner.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRoleHolder))
        {
            playerRole = (string)playerRoleHolder;
            if (playerRole == "rn")
            {
                gameObject.tag = "Runner";
                GetComponent<PlayerMovementAdvanced>().enabled = photonView.IsMine;
                GetComponent<WallRunningAdvanced>().enabled = photonView.IsMine;
                GetComponent<Sliding>().enabled = photonView.IsMine;
                GetComponent<Climbing>().enabled = photonView.IsMine;
            }
            else if (playerRole == "cs")
            {
                gameObject.tag = "Chaser";
                GetComponent<PlayerMovementAdvanced>().enabled = false;
                GetComponent<WallRunningAdvanced>().enabled = false;
                GetComponent<Sliding>().enabled = false;
                GetComponent<Climbing>().enabled = false;
            }

            if (photonView.IsMine)
            {
                localUiHolder.SetActive(false);
                GameObject uiPrefab = Instantiate(playerUiPrefab);
                GetComponent<PlayerCollisionHandler>().uiManager = uiPrefab.GetComponent<UiManager>();
            }
            else 
            {
                localUiHolder.SetActive(true);
            }
        }
        
        playerName.text = photonView.Owner.NickName;
        playerCamera.enabled = photonView.IsMine;
        gameObject.transform.GetChild(2).transform.GetChild(0).GetComponent<PlayerCam>().enabled = photonView.IsMine;
        
    }
}
