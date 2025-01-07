using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpin : MonoBehaviour
{
    private float spinSpeed = 40;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0, Space.World);
    }
}
