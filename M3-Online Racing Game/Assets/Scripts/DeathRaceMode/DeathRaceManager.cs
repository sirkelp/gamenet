using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class DeathRaceManager : MonoBehaviourPunCallbacks
{
    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPositions;

    public static DeathRaceManager instance = null;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI eliminationText;
    public TextMeshProUGUI lastManText;

    //public int playersLeft;
    //public Dictionary<int, string> playersDictionary;
    //public List<int> playersAliveList;
    public bool localPlayerisDead = false;

    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            //playersLeft = PhotonNetwork.PlayerList.Length;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = StartingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(VehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.Euler(0, -90, 0));
            }
        }

        eliminationText.enabled = false;
        lastManText.enabled = false;

        ExitGames.Client.Photon.Hashtable localPlayerAliveState = new ExitGames.Client.Photon.Hashtable() { {Constants.PLAYER_DEAD, localPlayerisDead}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(localPlayerAliveState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuitButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        int playerCount = PhotonNetwork.PlayerList.Length;

        object isDead1;
        if (changedProps.TryGetValue(Constants.PLAYER_DEAD, out isDead1))
        {
            if ((bool)isDead1 == true) playerCount--;
        }

        if (playerCount == 1)
        {
            string alivePlayer = " ";
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isDead;
                if (p.CustomProperties.TryGetValue(Constants.PLAYER_DEAD, out isDead))
                {
                    if (!(bool)isDead)
                    {
                        alivePlayer = p.NickName;
                        lastManText.enabled = true;
                        lastManText.text = " Winner:\n" + alivePlayer;
                    }
                }
            }
        }
        
    }
}
