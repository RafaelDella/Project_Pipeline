using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    private void Awake() {
        instance = this;
    }

    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject menuButtons;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    private void Start() {
        CloseMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network...";

        PhotonNetwork.ConnectUsingSettings();
    }

    void CloseMenus(){
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
    }

    public override void OnConnectedToMaster(){
        
        PhotonNetwork.JoinLobby();

        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby(){
        
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void OpenRoomCreate(){
        CloseMenus();
        createRoomScreen.SetActive(true);
    }

    public void CreateRoom(){
        if(!string.isNullOrEmpty(roomNameInput.text)){
            RoomOptions options = new RoomOptions();
            options.maxPlayer = 8;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true);
        }
    }

}
