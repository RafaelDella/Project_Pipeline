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

    public enum EventCodes : byte{ //enum a specific category of data, easy to use
        NewPlayer,
        ListPlayer,
        UpdateStat,
    }

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;

    private void Start() {
        if(!PhotonNetwork.IsConnected){
            SceneManager.LoadScene(0); //Main Menu
        }else{
            NewPlayerSend(PhotonNetwork.NickName);
        }
    }

    private void Update() {
        //PlayerInfo p1 = new PlayerInfo("Rafael", 0, 8, 5);




    }

    public void OnEvent(EventData photonEvent){
        if(photonEvent.Code < 200){
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            switch(theEvent){
                case EventCodes.NewPlayer:

                    NewPlayerReceive(data);
                    
                    break;

                case EventCodes.ListPlayer:

                    ListPlayersReceive(data);

                    break;
                
                case EventCodes.UpdateStat:

                    UpdateStatReceive(data);

                    break;

            }
        }
    }

    public override void OnEnable(){
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable(){
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string username){
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;


        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayer,
            package,
            new RaiseEventOptions{ Receivers = ReceiverGroup.All},
            new SendOptions { Reliability = true}
        );

    }

    public void NewPlayerReceive(object[] dataReceived){
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);

        allPlayers.Add(player);

        ListPlayersSend();
    }

    public void ListPlayersSend(){
        object[] package = new object[allPlayers.Count];

        for(int i = 0; i < allPlayers.Count; i++){
            object[] piece = new object[4];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].death;

            package[i] = piece;
        }
    }

    public void ListPlayersReceive(object[] dataReceived){
        allPlayers.Clear();

        for(int i = 0; i < dataReceived.Length; i++){
            object[] piece = (object[])dataReceived[i];
            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
            );

            allPlayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor){
                index = i;
            }
        }
    }

    public void UpdateStatSend(int actorSending, int statToUpdate, int amountToChange){
        object[] package = new object[] {actorSending, statToUpdate, amountToChange};

         PhotonNetwork.RaiseEvent(
                (byte)EventCodes.UpdateStat,
                package,
                new RaiseEventOptions{ Receivers = ReceiverGroup.All},
                new SendOptions { Reliability = true}
            );
    }

    public void UpdateStatReceive(object[] dataReceived){
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        for(int i = 0; i < allPlayers.Count; i++){
            if(allPlayers[i].actor == actor){
                switch(statType){
                    case 0: //Kills
                        allPlayers[i].kills += amount;
                        Debug.Log("Player " + allPlayers[i].name + " : kills " + allPlayers[i].kills);
                        break;

                    case 1:
                        allPlayers[i].death += amount;
                        Debug.Log("Player " + allPlayers[i].name + " : kills " + allPlayers[i].death);
                        break;
                }
                
            }

            break;
        }
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, death;

    public PlayerInfo(string _name, int _actor, int _kilss, int _death){
        name = _name;
        actor = _actor;
        kills = _kilss;
        death = _death;
    }


}
