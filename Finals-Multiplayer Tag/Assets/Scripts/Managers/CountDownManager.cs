using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class CountDownManager : MonoBehaviourPunCallbacks
{
    private TextMeshProUGUI timerText;

    public float timeToStartGame = 5f;
    public float timeToEndGame = 60f;
    public bool stopTime = false;

    public static event Action OnGameStarted;
    public static event Action OnTimerEnded;

    void OnEnable()
    {
        PlayerCollisionHandler.OnRunnerCaught += StopTimer;
    }

    void OnDisable()
    {
        PlayerCollisionHandler.OnRunnerCaught -= StopTimer;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance != null)
        {
            timerText = GameManager.instance.timeText;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartGame > 0)
            {
                timeToStartGame -= Time.deltaTime;
                photonView.RPC("InitialCountdown", RpcTarget.AllBuffered, timeToStartGame);
            }
            else if (timeToStartGame < 0)
            {
                //Start second timer after initial countdown ends
                if (timeToEndGame > 0)
                {
                    if (stopTime) return;

                    timeToEndGame -= Time.deltaTime;
                    photonView.RPC("GameplayCountdown", RpcTarget.AllBuffered, timeToEndGame);
                }
                else if (timeToEndGame < 0)
                {
                    photonView.RPC("DisableSelf", RpcTarget.AllBuffered);
                }
            }
        }
        else 
        {
            this.enabled = false;
        }
    }

    [PunRPC]
    public void InitialCountdown(float time)
    {
        if (time > 0)
        {
            if (timerText == null) return;
            timerText.text = time.ToString("F1");
        }
        else 
        {
            OnGameStarted?.Invoke();
            timerText.text = " ";
            GetComponent<PlayerMovementAdvanced>().enabled = photonView.IsMine;
            GetComponent<WallRunningAdvanced>().enabled = photonView.IsMine;
            GetComponent<Sliding>().enabled = photonView.IsMine;
            GetComponent<Climbing>().enabled = photonView.IsMine;
        }
    }

    [PunRPC]
    public void GameplayCountdown(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else 
        {
            OnTimerEnded?.Invoke();
            timerText.text = " ";
        }
    }

    [PunRPC]
    public void DisableSelf()
    {
        this.enabled = false;
    }

    //[PunRPC]
    public void StopTimer()
    {
        stopTime = true;
    }
}
