using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CountDownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;

    public float timeToStartRace = 5f;
    // Start is called before the first frame update
    void Start()
    {
        if (RacingGameManager.instance != null)
        {
            timerText = RacingGameManager.instance.timeText;
        }
        else 
        {
            timerText = DeathRaceManager.instance.timeText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else 
        {
            timerText.text = " ";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().isControlEnabled = true;
        //GetComponentInChildren<VehicleShooting>().enabled = photonView.IsMine;
        this.enabled = false;
    }
}
