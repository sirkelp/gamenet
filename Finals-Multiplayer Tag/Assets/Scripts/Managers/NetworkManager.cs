using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;
    public Text GameModeText;
   
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> PlayerListGameObjects;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }
    
    public void OnCreateRoomButtonClicked()
    {
        if (GameMode == null) return;

        ActivatePanel(CreatingRoomInfoUIPanel.name);

        string roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        string[] roomPropertiesInLobby = {"gm"}; // game mode

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { {"gm", GameMode }};

        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;
        PhotonNetwork.CreateRoom(roomName, roomOptions);

        ExitGames.Client.Photon.Hashtable playerRoleProperties = new ExitGames.Client.Photon.Hashtable() { {Constants.PLAYER_ROLE, GameMode} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerRoleProperties);
    }

    public void OnJoinRandomRoomClicked(string gameMode)
    {
        //When joining a room as a chaser, the network will look for a runner room and Vice Versa.
        if (gameMode == "cs")
        {
            GameMode = "rn";
        }
        else if (gameMode == "rn")
        {
            GameMode = "cs";
        }

        //on the other hand, Player Role property will retain the chosen role
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"gm", GameMode}};
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);

        ExitGames.Client.Photon.Hashtable playerRoleProperties = new ExitGames.Client.Photon.Hashtable() { {Constants.PLAYER_ROLE, gameMode} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerRoleProperties);
        
    }

    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {   
            PhotonNetwork.LoadLevel("TagScene");
            
        }
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created");
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        ActivatePanel(InsideRoomUIPanel.name);
        object gameModeName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
        {
            RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + 
                PhotonNetwork.CurrentRoom.MaxPlayers;

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("cs") || PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rn"))
            {
                GameModeText.text = "Multiplayer Tag";
            }
        }

        if (PlayerListGameObjects == null) PlayerListGameObjects = new Dictionary<int, GameObject>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(PlayerListPrefab);
            playerListItem.transform.SetParent(PlayerListParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            object playerRole;
            string playerRoleString = " ";
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRole))
            {
                playerRoleString = (string)playerRole;
            }

            playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(player.ActorNumber, player.NickName, playerRoleString);
            
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListItem.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool) isPlayerReady);
            }
            PlayerListGameObjects.Add(player.ActorNumber, playerListItem);
        }

        StartGameButton.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //base.OnPlayerEnteredRoom(newPlayer);
        GameObject playerListItem = Instantiate(PlayerListPrefab);
        playerListItem.transform.SetParent(PlayerListParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        object playerRole;
        string playerRoleString = " ";
        if (newPlayer.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRole))
        {
            playerRoleString = (string)playerRole;
        }

        playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName, playerRoleString);

        PlayerListGameObjects.Add(newPlayer.ActorNumber, playerListItem);

        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + 
            PhotonNetwork.CurrentRoom.MaxPlayers;

        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
        Destroy(PlayerListGameObjects[otherPlayer.ActorNumber].gameObject);
        PlayerListGameObjects.Remove(otherPlayer.ActorNumber);

        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + 
            PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        if (GameOptionsUIPanel != null) ActivatePanel(GameOptionsUIPanel.name);

        foreach (GameObject playerlistGameObject in PlayerListGameObjects.Values)
        {
            Destroy(playerlistGameObject);
        }

        PlayerListGameObjects.Clear();
        PlayerListGameObjects = null;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        if (GameMode == null) return;

        //Revert it back to normal when No Room is found
        if (GameMode == "cs") GameMode = "rn";
        else if (GameMode == "rn") GameMode = "cs";

        string roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        string[] roomPropertiesInLobby = {"gm"}; // game mode

        ExitGames.Client.Photon.Hashtable playerRoleProperties = new ExitGames.Client.Photon.Hashtable() { {Constants.PLAYER_ROLE, GameMode} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerRoleProperties);

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { {"gm", GameMode }};

        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        GameObject playerlistGameObject;
        if (PlayerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerlistGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerlistGameObject.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool) isPlayerReady);
            }
        }

        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }

    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }
    
    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    
    #endregion

    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool) isPlayerReady || PhotonNetwork.PlayerList.Length != 2)
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }

        return true;
    }
}
