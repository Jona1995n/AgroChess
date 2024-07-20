using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using ChessEngine.Game.UI;


public class GameModeManger : MonoBehaviour
{
    public Toggle ToggleButton;
    public NetworkManager m_NetWorkManager;
    public CanvasManager CanvasManager;
    public GameTimer AIGameTimer;
    public GameTimer MultiGameTimer;
    //public GameObject GamePanel;

    [Tooltip("Bool Value Represents the toggle Interaction, False for Single")]
    bool Multiplayer=false;

    private void Awake()
    {
        PlayerPrefs.SetString("PieceColor", "White");

    }

    private void Update()
    {
        if (m_NetWorkManager.IsClient)
        {
            if (!ToggleButton.interactable)
            {
                ToggleButton.interactable = m_NetWorkManager.GetComponent<Client>().ConnectedtoServer();
            }
            
        }
        Multiplayer = ToggleButton.isOn;       
    }

    public void GameButtonPressed(int GameDuration)//Game Duration in Seconds
    {
        
        if (!Multiplayer)
        {
            //Single Player System
            CanvasManager.ActivateCanvas(CanvasManager.s_Game);
            AIGameTimer.SetTimerDuration(GameDuration);

        }
        else
        {
            //MultiPlayer Logic
        }
    }

}
