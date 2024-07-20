using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSystem : MonoBehaviour
{
    public GameObject Board;
    GameObject[] ChessPiecesWhite;
    GameObject[] ChessPiecesBlack;
    public GameObject GamePanel;
    bool ChatVisible=false;

    private void Start()
    {
        ChessPiecesWhite = GameObject.FindGameObjectsWithTag("white");
        ChessPiecesBlack = GameObject.FindGameObjectsWithTag("black");
    }
    private void Update()
    {

        GameObject[] TempW = GameObject.FindGameObjectsWithTag("white");
        GameObject[] TempB = GameObject.FindGameObjectsWithTag("black");
        if (TempW.Length > 0)
        {
            ChessPiecesWhite = TempW;
        }
        if (TempB.Length > 0)
        {
            ChessPiecesBlack = TempB;
        }

    }
    public void ToggleChat()
    {
        if (ChatVisible)
        {
            foreach(GameObject Pawn in ChessPiecesBlack)
            {
                Pawn.SetActive(true);
            }
            foreach (GameObject Pawn in ChessPiecesWhite)
            {
                Pawn.SetActive(true);
            }
            Board.SetActive(true);
            GamePanel.SetActive(true);
            ChatVisible = false;
        }
        else
        {
            foreach (GameObject Pawn in ChessPiecesBlack)
            {
                Pawn.SetActive(false);
            }
            foreach (GameObject Pawn in ChessPiecesWhite)
            { 
                Pawn.SetActive(false);
            }
            Board.SetActive(false);
            GamePanel.SetActive(false);
            ChatVisible = true;
        }
    }
}
