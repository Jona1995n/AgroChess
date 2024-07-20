using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePage : MonoBehaviour
{
    int HairID, EyesID, ColorID;
    bool Gender;//True=Male
    string Username;

    public Image BitMojiShowcase_Face;
    public Image BitMojiShowcase_Hair;
    public Image BitMojiShowcase_Eyes;
    public Image BitMojiShowcase_Mouth;
    public Image BitMojiShowcase_Dress;
    public Image Display_Face, Display_Gender, Display_Eyes, Display_Hair;
    public Text UsernameTextField;

    public Sprite[] Eyes, Hair, Color,Mouth,Dress;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Color"))
        {
           HairID = PlayerPrefs.GetInt("Hair");
           EyesID = PlayerPrefs.GetInt("Eyes");
           ColorID = PlayerPrefs.GetInt("Color");
           Gender = (PlayerPrefs.GetInt("Gender") == 0);
           Username = "Player123"; 
           UpdateChar();
        }
        else
        {
            HairID = Random.Range(0, Hair.Length);
            EyesID = Random.Range(0, Eyes.Length); ;
            ColorID = Random.Range(0, Color.Length);
            Gender = (Random.Range(0,1)==0);
            Username = "Player123";
            UpdateChar();
        }

    }

    public void UpdateChar()
    {
        BitMojiShowcase_Eyes.sprite = Eyes[EyesID];
        BitMojiShowcase_Hair.sprite = Hair[HairID];
        BitMojiShowcase_Face.sprite = Color[ColorID];
        BitMojiShowcase_Mouth.sprite = Mouth[ColorID];
        if (Gender) BitMojiShowcase_Dress.sprite = Dress[0];
        else BitMojiShowcase_Dress.sprite = Dress[1];
        UsernameTextField.text = Username;

        PlayerPrefs.SetInt("Color", ColorID);
        PlayerPrefs.SetInt("Eyes", EyesID);
        PlayerPrefs.SetInt("Hair", HairID);
        
        PlayerPrefs.SetString("PlayerName", Username);
        if (Gender) PlayerPrefs.SetInt("Gender", 0);
        else PlayerPrefs.SetInt("Gender", 1);

        Display_Face.sprite= Color[ColorID];     
        Display_Eyes.sprite = Eyes[EyesID];
        Display_Hair.sprite = Hair[HairID];

        if (Gender) Display_Gender.sprite = Dress[0];
        else Display_Gender.sprite = Dress[1];

    }

    public void UpdateHair(bool Forward)
    {
        if (Forward)
        {
            if (HairID < Hair.Length-1)
            {
                HairID++;
            }
            else
            {
                HairID = 0;
            }

        }
        else 
        {
            if (HairID == 0)
            {
                HairID = Hair.Length-1;
            }
            else
            {
                HairID--;
            }
        }
        UpdateChar();
    }
    public void UpdateEyes(bool Forward)
    {
        if (Forward)
        {
            if (EyesID < Eyes.Length - 1)
            {
                EyesID++;
            }
            else
            {
                EyesID = 0;
            }

        }
        else
        {
            if (EyesID == 0)
            {
                EyesID = Eyes.Length - 1;
            }
            else
            {
                EyesID--;
            }
        }
        UpdateChar();
    }
    public void UpdateFace(bool Forward)
    {
        if (Forward)
        {
            if (ColorID < Color.Length - 1)
            {
                ColorID++;
            }
            else
            {
                ColorID = 0;
            }

        }
        else
        {
            if (ColorID == 0)
            {
                ColorID = Color.Length - 1;
            }
            else
            {
                ColorID--;
            }
        }
        UpdateChar();
    }
    public void UpdateGender()
    {
        Gender = !Gender;
        UpdateChar();
    }

}
