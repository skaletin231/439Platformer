using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public TMP_InputField LobbyNameInputField;

    public TMP_Text WarningText;
    // Start is called before the first frame update
    void Start()
    {
        if (!LobbyNameInputField)
        {
            Debug.LogError("LobbyNameInputField Not Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

    public void OnClickCreate()
    {
        if (!String.IsNullOrWhiteSpace(LobbyNameInputField.text))
        {
            PhotonNetwork.CreateRoom(LobbyNameInputField.text.Trim());
        }
        else
        {
            WarningText.text = "Lobby name can not be empty";
        }
    }

    public void OnClickJoin()
    {
        if (!String.IsNullOrWhiteSpace(LobbyNameInputField.text))
        {
            PhotonNetwork.JoinRoom(LobbyNameInputField.text.Trim());
        }
        else
        {
            WarningText.text = "Lobby name can not be empty";
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.SendRate = 14;
        PhotonNetwork.SerializationRate = 10;
        base.OnJoinedRoom();
        Debug.Log("SendRate" + PhotonNetwork.SendRate); 
        Debug.Log("SerializationRate" + PhotonNetwork.SerializationRate); 
        PhotonNetwork.LoadLevel("MLevel1");
    }

    public void clearWarning()
    {
        WarningText.text = "";
    }
}
