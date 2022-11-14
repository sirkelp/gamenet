using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public GameObject playerPrefab;

    public Transform spawnPointsParent;
    public List<Transform> spawnPoints;

    public Canvas killfeedCanvas;
    public Image killfeedImagePrefab;

    public GameObject killfeedPanel;
    public List<Image> killfeedRecords;

    public static event Action<string> ExecuteEndGame;

    public override void OnEnable()
    {
        base.OnEnable();
        ShootingScript.OnTenthKill += EndGame;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        ShootingScript.OnTenthKill -= EndGame;
    }

    void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // int randomPointX = Random.Range(-10, 10);
            // int randomPointZ = Random.Range(-10, 10);

            if (spawnPoints == null) 
            {
                Debug.LogWarning("SpawnPoints List is Empty");
                return;
            }

            foreach (Transform child in spawnPointsParent)
            {
                spawnPoints.Add(child);
            }

            killfeedPanel = killfeedCanvas.transform.GetChild(0).gameObject;

            Transform randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
            PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPoint.position, Quaternion.identity);
        }
    }

    void EndGame(string name)
    {
        Debug.Log("End Game");
        ExecuteEndGame?.Invoke(name);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
