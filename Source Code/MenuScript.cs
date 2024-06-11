using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public string versionName = "0.1";
    public GameObject usernamePanel;
    public GameObject lobbyPanel;
    public TMP_InputField usernameInput;
    public TMP_InputField createGameInput;
    public TMP_InputField joinGameInput;
    public GameObject confirmButton;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);
    }

    private void Start()
    {
        // Set default username
        usernameInput.text = "WarpDasher" + Random.Range(10, 999);

        usernamePanel.SetActive(true);
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void OnUsernameChange()
    {
        if (usernameInput.text.Length >= 2)
            confirmButton.SetActive(true);
        else
            confirmButton.SetActive(false);
    }

    public void SetUsername()
    {
        usernamePanel.SetActive(false);
        PhotonNetwork.playerName = usernameInput.text;
        Debug.Log("Player added: " + PhotonNetwork.playerName);
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(createGameInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(joinGameInput.text, roomOptions, TypedLobby.Default);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiPlayer");
    }
}
