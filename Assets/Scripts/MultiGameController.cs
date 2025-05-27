using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MultiGameController : MonoBehaviour
{
    public GameObject PlayerAndVcamPrefab;

    public Transform StartPoint;

    public Image healthBar;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject playerAndVcam = PhotonNetwork.Instantiate(PlayerAndVcamPrefab.name, StartPoint.position, Quaternion.identity);
        if (playerAndVcam.GetComponentInChildren<PhotonView>().IsMine)
        {
            playerAndVcam.GetComponentInChildren<PlayerHealth>().healthBar = healthBar;
            playerAndVcam.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
