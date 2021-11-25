using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Rock : MonoBehaviour
{   
    public GameObject BrokenObject;
    public int Damage;
    public string CharacterTag = "Character";

    private bool _activated = false;

    void OnCollisionEnter(Collision collision)
    {   
        if(_activated)
        {   
            if(collision.gameObject.tag == CharacterTag)
            {
                var character = collision.gameObject.GetComponentInParent<Character>();
                character.ApplyDamage(Damage);
            }
            
            Instantiate(BrokenObject, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void Activate()
    {
        _activated = true;
    }
}
