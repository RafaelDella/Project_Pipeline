using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    private void Awake() {
        instance = this;
    }

    public GameObject playerPrefab;
    private GameObject player;

    private void Start() {
        if(PhotonNetwork.IsConnected){
            SpawnPlayer();
        }
    }

    public void SpawnPlayer(){
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
    
}
