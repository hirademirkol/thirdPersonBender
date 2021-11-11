using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBending : MonoBehaviour
{
    public GameObject rockObject;
    public float pushForce;
    public Transform camera;

    private GameObject rock;
    private bool rockDrawn = false;
    void Update()
    {
        if(Input.GetButtonDown("Fire2") && !rockDrawn){ StartCoroutine(DrawRock()); }
        if(Input.GetButtonDown("Fire1") && rockDrawn){ StartCoroutine(PushRock()); }
    }

    IEnumerator DrawRock()
    {
        rock = GameObject.Instantiate(rockObject, this.transform, false);
        rockDrawn = true;
        yield return null;
    }

    IEnumerator PushRock()
    {
        var rigidbody = rock.GetComponent<Rigidbody>();
        rock.transform.SetParent(null);
        var force = Quaternion.Euler(camera.eulerAngles.x, 0f, 0f) * new Vector3(0f, pushForce , 0f);
        rigidbody.AddRelativeForce(force);
        rigidbody.useGravity = true;
        rockDrawn = false;
        yield return null;
    }
}
