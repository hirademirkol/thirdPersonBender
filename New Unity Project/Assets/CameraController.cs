using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineCameraOffset cameraOffset;
    public float switchDuration = 1f;
    private float startOffset;
    void Start()
    {
        startOffset = cameraOffset.m_Offset.x;
    }
    void Update()
    {
        if(Input.GetButtonDown("Switch Camera"))
        {
            StartCoroutine(LerpFunction());
        }
    }

    IEnumerator LerpFunction()
    {
        float time = 0;
        startOffset = cameraOffset.m_Offset.x;
        while (time < switchDuration)
        {
            cameraOffset.m_Offset.x = Mathf.Lerp(startOffset, -startOffset, time / switchDuration);
            time += Time.deltaTime;
            yield return null;
        }
        cameraOffset.m_Offset.x = -startOffset;
    }
}
