using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Rock : MonoBehaviour
{   
    public GameObject BrokenObject;

    private bool _activated = false;

    void OnCollisionEnter(Collision collision)
    {   
        if(_activated)
        {
            Instantiate(BrokenObject, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void Activate()
    {
        _activated = true;
    }
}
