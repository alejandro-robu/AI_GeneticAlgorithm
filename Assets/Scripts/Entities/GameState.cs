using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Game State")]
public class GameState : ScriptableObject, ISerializationCallbackReceiver
{

    public bool IsFinished;

    public int TurnNumber;
    
    public PlayerInfo CurrentPlayer;
    public PlayerList ListOfPlayers;
    public bool LeftPlayerIsHuman;
    public bool RightPlayerIsHuman;

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
       
    }

    public void ResetState()
    {
        TurnNumber = 1;
        IsFinished = false;

        foreach (var player in ListOfPlayers.Players)
        {
            player.HP = player.InitialHP;
            player.Energy = player.InitialEnergy;
        }

        //CurrentPlayer = null;
        CurrentPlayer = ListOfPlayers.Players[0];

        LeftPlayerIsHuman = false;
        RightPlayerIsHuman = false;
    }
}
