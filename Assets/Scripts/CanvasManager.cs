using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject s_Game;
    public GameObject GameMode;
    public GameObject Tournament;
    //public GameObject Store;
    public GameObject Profile;
    public GameObject Statistics;
    public GameObject Leaderboard;
    public GameObject ChatPanel;
    public GameObject GameOverUI;
    public GameObject ChatBtn;
    private GameModeManger GameModeManager;

    // Start is called before the first frame update
    private void Awake()
    {
        GameModeManager = GetComponent<GameModeManger>();
        ActivateCanvas(MainMenu);
    }

    public void ActivateCanvas(GameObject CanvasToActivate)
    {
      MainMenu.SetActive(false);
        s_Game.SetActive(false);
      GameMode.SetActive(false);
      Tournament.SetActive(false);
      //Store.SetActive(false);
      Profile.SetActive(false);
      Statistics.SetActive(false);
      Leaderboard.SetActive(false);
      GameOverUI.SetActive(false);

        GameObject[] WChessPieces = GameObject.FindGameObjectsWithTag("white");
        foreach(GameObject Piece in WChessPieces)
        {
            Destroy(Piece);
        }
        GameObject[] BChessPieces = GameObject.FindGameObjectsWithTag("black");
        foreach (GameObject Piece in BChessPieces)
        {
            Destroy(Piece);
        }
        GameObject Board = GameObject.FindGameObjectWithTag("Board");
        if (Board)
        {
            Board.SetActive(false);
        }
        CanvasToActivate.SetActive(true);
        }

    public void ToggleChatPanel()
    {
        ChatPanel.SetActive(!ChatPanel.activeSelf); // Toggle the active state of the panel
    }

    public void ToggleHome()
    {
        if (GameModeManager.AIGameTimer.GameInProgress)
        {
            ChatBtn.SetActive(true);
        } 
	    else
        {
           
        }
    }
}
