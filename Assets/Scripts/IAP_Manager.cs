using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAP_Manager : MonoBehaviour
{
    public GameObject ThunderGO;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (PlayerPrefs.GetInt("Thunder") == 1)
        {
            ThunderGO.GetComponent<Button>().interactable = false;
        }
    }

    public void PurchaseThunder()
    {
        if (PlayerPrefs.GetInt("Thunder") == 0)
        {
            PlayerPrefs.SetInt("Thunder", 1);
            ThunderGO.GetComponent<Button>().interactable = false;
        }
        
    }
}
