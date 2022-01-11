using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        if(!PhotonNetwork.IsConnected){
            SceneManager.LoadScene(0); //Main Menu
        }
    }

    public oid OnEvent(EventData photonEvent){

    }

    public override void OnEnable(){
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable(){
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
