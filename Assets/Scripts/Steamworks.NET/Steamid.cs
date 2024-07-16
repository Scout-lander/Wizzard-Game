using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using TMPro;

public class Steamid : MonoBehaviour
{
    public TMP_Text SteamName;
    void Start()
    {
        if(!SteamManager.Initialized) {return; }

        string name = SteamFriends.GetPersonaName();
        Debug.Log("Steam_ID " + name);   
        SteamName.text = name;
    }


}
