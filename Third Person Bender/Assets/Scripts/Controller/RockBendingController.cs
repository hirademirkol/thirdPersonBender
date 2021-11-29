using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBendingController : MonoBehaviour
{
    public GameObject RockObject;
    public GameObject WallObject;
    public float PushVelocity = 20f;
    public Transform Camera;
    public float RockRaiseTime = 0.25f;
    public float WallRaiseTime = 0.5f;
    public Vector3 rockOffsetPosition = new Vector3 (2f, 2f, 0f);

    private GameObject _rock;
    private bool _rockDrawn = false;

    public void DrawRock()
    {
        if(!_rockDrawn)
            StartCoroutine(DrawRockRoutine());
    }

    public void PushRock(Vector3 aimVector)
    {
        if(_rockDrawn)
            StartCoroutine(PushRockRoutine(aimVector));
    }

    public void RaiseWall()
    {
        StartCoroutine(RaiseWallRoutine(WallRaiseTime));
    }

    IEnumerator DrawRockRoutine()
    {
        _rock = Instantiate(RockObject, transform);
        _rock.transform.localPosition = new Vector3(rockOffsetPosition.x, 0f, rockOffsetPosition.z);
        _rockDrawn = true;
        StartCoroutine(RaiseRockRoutine(_rock, RockRaiseTime));
        yield return null;
    }

    IEnumerator PushRockRoutine(Vector3 aimVector)
    {
        var rigidbody = _rock.GetComponent<Rigidbody>();
        rigidbody.GetComponent<Collider>().enabled = true;
        rigidbody.isKinematic = false;
        _rock.transform.SetParent(null);
        Vector3 force = aimVector.normalized*PushVelocity;
        rigidbody.AddRelativeForce(Quaternion.Euler(-90f, 0f, 0f)*force, ForceMode.VelocityChange);
        rigidbody.useGravity = true;
        _rock.GetComponent<Rock>().Activate();
        _rockDrawn = false;
        yield return null;
    }

    IEnumerator RaiseRockRoutine(GameObject rock, float time)
    {
        var moveVector = new Vector3(0f, rockOffsetPosition.y/time, 0f);
        while (time > 0 && _rockDrawn)
        {
            rock.transform.localPosition += moveVector * Time.deltaTime;
            time -= Time.deltaTime;
            yield return null;
        }
        if(_rockDrawn)
            rock.transform.localPosition = rockOffsetPosition;
    }

    IEnumerator RaiseWallRoutine(float time)
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