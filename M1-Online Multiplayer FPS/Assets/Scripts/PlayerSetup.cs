using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    public TextMeshProUGUI playerNameTxt;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            playerCamera.GetComponent<Camera>().enabled = true;
        }
        else 
        {
            transform.GetComponent<MovementController>().enabled = false;
            playerCamera.GetComponent<Camera>().enabled = false;
        }
        playerNameTxt.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
