using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockParts : MonoBehaviour
{   
    public float DisappearTime = 3f;

    void Update()
    {
        if(DisappearTime<0)
            Destroy(this.gameObject);
        DisappearTime-=Time.deltaTime;
    }
}
