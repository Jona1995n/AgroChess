using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

struct AIProfile
{
    public int CID, EID, HID, GID;
    public string Username;
    public bool Color;
    
}
public class ProfileSet : MonoBehaviour
{
    [Header("Chat Box ")]
    public GameObject ChatBox;

    [Header("AI")]
    public string[] AIUsernames;
    
    [Tooltip("AI Profile Data (Random)")]
    AIProfile AIProfileData;



    [Header("Profile Image")]
    public Image Face;
    public Image Eyes;
    public Image Hair;
    public Image Mouth; 
    public Image Dress;

    [Header("Emote")]
    public Image Expression_Emote;
    public Image Face_Emote;
    public Image Hair_Emote;
    public Image Dress_Emote;//Emote 

    [Header("Sprites")]
    public Sprite [] FaceSprites;
    public Sprite[] EyesSprites;
    public Sprite[] HairSprites;
    public Sprite[] MouthSprites;
    public Sprite[] DressSprites;
    public Sprite[] Emotes_List;

    [Header("Username")]
    public Text UserNameField;

    [Header("Side")]
    public bool MyColor;

    Color NullColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);


    private void SetProfile(int ColorID,int EyesID,int HairID,string Username,bool Color,int Gender)
  { 
        
        //Color = true if white
        if (Color==MyColor)
        {
            UserNameField.text = Username;
            Face.sprite = FaceSprites[ColorID];
            Eyes.sprite = EyesSprites[EyesID];
            Hair.sprite = HairSprites[HairID];
            Mouth.sprite = MouthSprites[ColorID];
            if(Gender==0)Dress.sprite = DressSprites[0];
            else Dress.sprite=DressSprites[1];
        }
  }
    private void Awake()
    {
        AIProfileData.CID = Random.Range(0, FaceSprites.Length);
        AIProfileData.EID = Random.Range(0, EyesSprites.Length);
        AIProfileData.HID = Random.Range(0, HairSprites.Length);
        AIProfileData.GID = Random.Range(0,1);
        AIProfileData.Username = AIUsernames[Random.Range(0, AIUsernames.Length)];

        EndAnimation();
        SetProfile(AIProfileData.CID, AIProfileData.EID, AIProfileData.HID, AIProfileData.Username, false, AIProfileData.GID);
    }
    private void Update()
    {
        SetProfile(PlayerPrefs.GetInt("Color"), PlayerPrefs.GetInt("Eyes"), PlayerPrefs.GetInt("Hair"), PlayerPrefs.GetString("PlayerName"), true, PlayerPrefs.GetInt("Gender"));
        
    }

    public void Call_Emote(int EmoteID)
    {
        StartCoroutine(EmoteAnim(EmoteID));
    }

    private void EndAnimation()
    {
        //Making the Emotes Null
        Expression_Emote.sprite = null;
        Face_Emote.sprite = null;
        Hair_Emote.sprite = null;
        Dress_Emote.sprite = null;

        //Setting Color to 0 Aplha
        Expression_Emote.color = NullColor;
        Face_Emote.color = NullColor;
        Hair_Emote.color = NullColor;
        Dress_Emote.color = NullColor;
    }
    
    IEnumerator EmoteAnim(int EmoteID)
    {
        //Setting color back to Apha 1
        Expression_Emote.color = Color.white;
        Face_Emote.color = Color.white;
        Hair_Emote.color = Color.white;
        Dress_Emote.color = Color.white;

        ChatBox.SetActive(false);

        Expression_Emote.sprite = Emotes_List[EmoteID];
        Face_Emote.sprite = FaceSprites[PlayerPrefs.GetInt("Color")];
        Hair_Emote.sprite = HairSprites[PlayerPrefs.GetInt("Hair")];
        Dress_Emote.sprite = DressSprites[PlayerPrefs.GetInt("Gender")];
        yield return new WaitForSeconds(1f);
        EndAnimation();
    }
}
