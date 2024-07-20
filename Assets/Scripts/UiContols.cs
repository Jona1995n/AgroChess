using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiContols : MonoBehaviour
{    
    GameObject[] blackEmoji = new GameObject[3];
    GameObject[] whiteEmoji = new GameObject[3];

    Sprite[] hair = new Sprite[3];
    Sprite[] face = new Sprite[3];
    Sprite[] eyes = new Sprite[3];




    private void Start()
    {
       
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Game");
    }


}
