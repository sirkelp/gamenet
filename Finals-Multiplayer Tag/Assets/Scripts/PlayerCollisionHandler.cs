using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerCollisionHandler : MonoBehaviourPunCallbacks
{
    public static event Action OnRunnerCaught;

    public UiManager uiManager;
    public string buffName = " ";

    public enum RaiseEventCode
    {
        RunnerCaught = 0,
        TimerEnded = 1,
        PlayerAcquiredBuff = 2,
        GameStarted = 3
    }

    protected void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        CountDownManager.OnGameStarted += GameStarted; 
        CountDownManager.OnTimerEnded += TimerEnded;
    }

    protected void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        CountDownManager.OnGameStarted -= GameStarted; 
        CountDownManager.OnTimerEnded -= TimerEnded;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCode.RunnerCaught)
        {
            OnRunnerCaught?.Invoke();
            string winnerName = (string)photonEvent.CustomData;  
            GameManager.instance.DisplayWinnerName(winnerName);
            GameManager.instance.ExitGame();
        }
        else if (photonEvent.Code == (byte)RaiseEventCode.TimerEnded)
        {
            string winnerName = (string)photonEvent.CustomData; 
            GameManager.instance.DisplayWinnerName(winnerName);
            GameManager.instance.ExitGame();
        }
        else if (photonEvent.Code == (byte)RaiseEventCode.PlayerAcquiredBuff)
        {
            object[] data = (object[])photonEvent.CustomData;
            string winnerName = (string)data[0];
            string grantedBuffName = (string)data[1];

            GameManager.instance.BuffTaken(winnerName, grantedBuffName);
        }
        else if (photonEvent.Code == (byte)RaiseEventCode.GameStarted)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool playerIsRunner = (bool)data[0];
            int viewId = (int)data[1];

            GameManager.instance.actionText.enabled = true;
            GameManager.instance.actionText.GetComponent<TextMeshProUGUI>().color = Color.red;

            if (!playerIsRunner)
            {
                if (viewId == photonView.ViewID)
                {
                    GameManager.instance.actionText.text = " Run! ";
                }
                else 
                {
                    GameManager.instance.actionText.text = " Chase! ";
                }           
            }
            else 
            {
                if (viewId == photonView.ViewID)
                {
                    GameManager.instance.actionText.text = " Chase! ";
                }
                else 
                {
                    GameManager.instance.actionText.text = " Run! ";
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Buff")
        {
            photonView.RPC("GrantBuff", RpcTarget.AllBuffered, other.GetComponent<Buff>().buffCode);
        }
        else if (other.tag == "Runner")
        {
            EndGame();
        }
    }

    [PunRPC]
    void GrantBuff(int code)
    {   
        if (code == 1)
        {
            buffName = "Super Speed";
            GetComponent<PlayerMovementAdvanced>().walkSpeed = 12f;
            if (uiManager != null) uiManager.buffIcon.GetComponent<Image>().enabled = true;

            BroadcastBuff();
        }
    }

    void BroadcastBuff()
    {
        string playerName = photonView.Owner.NickName;
        string buffName = this.buffName;
        object[] data = new object[] { playerName, buffName };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions {Reliability = false};
        PhotonNetwork.RaiseEvent((byte) RaiseEventCode.PlayerAcquiredBuff, data, raiseEventOptions, sendOptions);
    }

    void TimerEnded()
    {
        Player player = photonView.Owner;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerRoleHolder;
            if (p.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRoleHolder))
            {
                if ((string)playerRoleHolder == "rn")
                {
                    player = p;
                }
            }
        }

        object data = player.NickName;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions {Reliability = false};
        PhotonNetwork.RaiseEvent((byte) RaiseEventCode.TimerEnded, data, raiseEventOptions, sendOptions);
    }

    void EndGame()
    {
        object data = photonView.Owner.NickName;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions {Reliability = false};
        PhotonNetwork.RaiseEvent((byte) RaiseEventCode.RunnerCaught, data, raiseEventOptions, sendOptions);
    }

    void GameStarted()
    {
        bool isRunner = false;
        Player player = photonView.Owner;
        int viewId = photonView.ViewID;

        object playerRoleHolder;
        if (player.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRoleHolder))
        {
            if ((string)playerRoleHolder == "cs")
            {
                isRunner = false;
            }
            else
            {
                isRunner = true;
            }
        }

        object[] data = new object[] { isRunner, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions {Reliability = false};
        PhotonNetwork.RaiseEvent((byte) RaiseEventCode.GameStarted, data, raiseEventOptions, sendOptions);
    }
}
