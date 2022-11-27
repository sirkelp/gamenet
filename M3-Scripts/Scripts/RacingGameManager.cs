using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RacingGameManager : MonoBehaviour
{
    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPositions;
    public TextMeshProUGUI[] finisherTexts;

    public static RacingGameManager instance = null;

    public TextMeshProUGUI timeText;

    public List<GameObject> lapTriggers = new List<GameObject>();
    
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

        foreach (TextMeshProUGUI text in finisherTexts)
        {
            text.enabled = false;
        }
    }
}
