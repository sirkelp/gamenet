using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DeathRaceManager : MonoBehaviour
{
    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPositions;

    public static DeathRaceManager instance = null;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI eliminationText;

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

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = StartingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(VehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.Euler(0, -90, 0));
            }
        }

        eliminationText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
