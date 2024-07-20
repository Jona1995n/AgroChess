using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThunder : MonoBehaviour
{
    public GameObject thunder;

    private void Start()
    {
        Destroy(thunder, .5f);
    }
}
