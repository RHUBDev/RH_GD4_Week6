using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Don't destroy music object
        DontDestroyOnLoad(gameObject);
    }
}
