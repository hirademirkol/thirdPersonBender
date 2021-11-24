using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBendingController : MonoBehaviour
{
    public GameObject RockObject;
    public GameObject WallObject;
    public float PushForce = 50f;
    public Transform Camera;
    public float RockRaiseTime = 0.25f;
    public float WallRaiseTime = 0.5f;

    private GameObject _rock;
    private bool _rockDrawn = false;

    void Update()
    {
        if(Input.GetButtonDown("Fire2") && !_rockDrawn){ StartCoroutine(DrawRock()); }
        if(Input.GetButtonDown("Fire1") && _rockDrawn){ StartCoroutine(PushRock()); }
        if(Input.GetButtonDown("Fire3")){ StartCoroutine(RaiseWall(WallRaiseTime)); }
    }

    IEnumerator DrawRock()
    {
        _rock = Instantiate(RockObject, transform);
        _rock.transform.localPosition = new Vector3(2f, 0f, 0f);
        _rockDrawn = true;
        StartCoroutine(RaiseRock(_rock, RockRaiseTime));
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

    IEnumerator RaiseWall(float time)
    {
        var position = transform.position + 2f * transform.forward;
        position.y = -6.15f;
        var moveVector = new Vector3(0f, 6.15f/time, 0f);
        var wall = Instantiate(WallObject, position, transform.rotation * WallObject.transform.rotation);
        while (time > 0)
        {
            wall.transform.position += moveVector * Time.deltaTime;
            time -= Time.deltaTime;
            yield return null;
        }
        position.y = 0f;
        wall.transform.position = position;
    }
}