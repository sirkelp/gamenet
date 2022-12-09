using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] PlayerColors;
    public Transform ChaserPosition;
    public Transform RunnerPosition;

    public static GameManager instance = null;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI buffGrantedText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI actionText;
    public string winnerName;
    public GameObject buffObject;

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
            int playerSelectionNumberCode = 0;
            Vector3 instantiatePosition = Vector3.zero;

            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                playerSelectionNumberCode = (int)playerSelectionNumber;   
            }

            object playerRole;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRole))
            {
                if ((string)playerRole == "cs")
                {
                    instantiatePosition = ChaserPosition.position;
                }
                else if ((string)playerRole == "rn")
                {
                    instantiatePosition = RunnerPosition.position;
                }
            }

            GameObject player = PhotonNetwork.Instantiate(PlayerColors[playerSelectionNumberCode].name, instantiatePosition, Quaternion.identity);
        }

        buffGrantedText.enabled = false;
        winnerText.enabled = false;
        actionText.enabled = false;
    }

    public void ExitGame()
    {
        StartCoroutine(ExitTimer());
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
        Destroy(gameObject);
    }

    public void DisplayWinnerName(string name)
    {
        Debug.Log(name);
        winnerText.enabled = true;
        winnerText.text = name + " wins";
    }

    public void BuffTaken(string playerName, string buffName)
    {
        if (buffObject == null) return;

        buffObject.SetActive(false);

        buffGrantedText.enabled = true;
        buffGrantedText.text = playerName + " has acquired:\n " + buffName;
    }

    IEnumerator ExitTimer()
    {
        yield return new WaitForSeconds(4f);
        buffGrantedText.enabled = false;
        actionText.enabled = false;
        winnerText.text = "Exiting Lobby...";

        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PhotonNetwork.IsConnectedAndReady) PhotonNetwork.LeaveRoom();
    }
}
