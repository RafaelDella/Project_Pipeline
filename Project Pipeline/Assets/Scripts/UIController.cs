using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text overheatedMsg;
    public Slider weaponTempSlider;

    public GameObject deathScreen;
    public TMP_Text deathText;

    private void Awake() {
        instance = this;
    }



}
