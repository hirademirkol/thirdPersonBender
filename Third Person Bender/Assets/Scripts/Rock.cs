using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{   
    void onCollisionEnter(Collision collision)
    {   
        Destroy(this.gameObject);
        Debug.Log("Collision!");
    }
}
