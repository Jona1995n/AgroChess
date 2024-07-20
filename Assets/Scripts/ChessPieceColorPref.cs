using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceColorPref : MonoBehaviour
{
    public void ToggleChatPanel()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf); // Toggle the active state of the panel
    }

    private void Awake()
    {
        if (PlayerPrefs.GetString("PieceColor") == "")
        {
            PlayerPrefs.SetString("PieceColor", "White");
        }
    }

    public void SetPreference(string Pref)
    {
        PlayerPrefs.SetString("PieceColor", Pref);
        this.gameObject.SetActive(false);
    }
}
