using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBendingController : MonoBehaviour
{
    public GameObject RockObject;
    public float PushForce = 50f;
    public Transform Camera;
    public float RaiseTime = 0.25f;

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
        _rock.transform.localPosition = new Vector3(2f, 0f, 0f);
        _rockDrawn = true;
        StartCoroutine(RaiseRock(_rock, RaiseTime));
        yield return null;
    }

    IEnumerator PushRock()
    {
        var rigidbody = _rock.GetComponent<Rigidbody>();
        rigidbody.GetComponent<Collider>().enabled = true;
        rigidbody.isKinematic = false;
        _rock.transform.SetParent(null);
        Vector3 force = Quaternion.Euler(Camera.eulerAngles.x, 0f, 0f) * new Vector3(0f, PushForce , 0f);
        rigidbody.AddRelativeForce(force);
        rigidbody.useGravity = true;
        _rock.GetComponent<Rock>().Activate();
        _rockDrawn = false;
        yield return null;
    }

    IEnumerator RaiseRock(GameObject rock, float time)
    {
        var moveVector = new Vector3(0f, 2f/time, 0f);
        while (time > 0 && _rockDrawn)
        {
            rock.transform.localPosition += moveVector * Time.deltaTime;
            time -= Time.deltaTime;
            yield return null;
        }
        if(_rockDrawn)
            rock.transform.localPosition = new Vector3(2f, 2f, 0f);
    }
}