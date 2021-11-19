using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBendingController : MonoBehaviour
{
    public GameObject RockObject;
    public float PushForce = 50f;
    public Transform Camera;

    private GameObject _rock;
    private bool _rockDrawn = false;

    void Update()
    {
        if(Input.GetButtonDown("Fire2") && !_rockDrawn){ StartCoroutine(DrawRock()); }
        if(Input.GetButtonDown("Fire1") && _rockDrawn){ StartCoroutine(PushRock()); }
    }

    IEnumerator DrawRock()
    {
        _rock = GameObject.Instantiate(RockObject, transform);
        _rock.transform.localPosition =  new Vector3(2.5f, 1f, 0f);
        _rockDrawn = true;
        yield return null;
    }

    IEnumerator PushRock()
    {
        var rigidbody = _rock.GetComponent<Rigidbody>();
        _rock.transform.SetParent(null);
        Vector3 force = Quaternion.Euler(Camera.eulerAngles.x, 0f, 0f) * new Vector3(0f, PushForce , 0f);
        rigidbody.AddRelativeForce(force);
        rigidbody.useGravity = true;
        _rock.GetComponent<Rock>().Activate();
        _rockDrawn = false;
        yield return null;
    }
}
