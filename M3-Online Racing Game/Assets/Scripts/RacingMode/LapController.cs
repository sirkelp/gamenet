using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

    public enum RaiseEventCode
    {
        WhoFinishedEventCode = 0
    }

    private int finishOrder = 0;


    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            
            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            TextMeshProUGUI orderUiText = RacingGameManager.instance.finisherTexts[finishOrder - 1];
            orderUiText.enabled = true;

            if (viewId == photonView.ViewID)
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nickNameOfFinishedPlayer + "(You)";
                orderUiText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else 
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nickNameOfFinishedPlayer;
            }         
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in RacingGameManager.instance.lapTriggers)
        {
            lapTriggers.Add(go);
        }
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (lapTriggers.Contains(col.gameObject))
        {
            Debug.Log("Collided");
            int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);

            lapTriggers[indexOfTrigger].SetActive(false);
        }

        if (col.gameObject.tag == "FinishTrigger")
        {
            Debug.Log("Finish");
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().playerCamera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        finishOrder++;
        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        object[] data = new object[] { nickName, finishOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions {Reliability = false};
        PhotonNetwork.RaiseEvent((byte) RaiseEventCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);

    }
}
