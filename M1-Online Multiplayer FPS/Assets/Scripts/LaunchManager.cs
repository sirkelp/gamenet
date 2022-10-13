using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterGamePanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        EnterGamePanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log(PhotonNetwork.NickName +  " Connected");
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnConnected()
    {
        //base.OnConnected();
        Debug.Log("Internet");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.LogWarning(message);
        CreateAndJoinRoom();
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            EnterGamePanel.SetActive(false);
            ConnectionStatusPanel.SetActive(true);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        Debug.Log(PhotonNetwork.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name + ". Room has now " + PhotonNetwork.CurrentRoom.PlayerCount
        + " players.");
    }
}
