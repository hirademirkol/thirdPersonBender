using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineCameraOffset CameraOffset;
    public float SwitchDuration = 1f;

    private float _startOffset;
    
    void Start()
    {
        _startOffset = CameraOffset.Offset.x;
    }
    void Update()
    {
        if(Input.GetButtonDown("SwitchCamera"))
        {
            StartCoroutine(LerpFunction());
        }
    }

    IEnumerator LerpFunction()
    {
        var time = 0f;
        _startOffset = CameraOffset.Offset.x;
        while (time < SwitchDuration)
        {
            CameraOffset.Offset.x = Mathf.Lerp(_startOffset, -_startOffset, time / SwitchDuration);
            time += Time.deltaTime;
            yield return null;
        }
        CameraOffset.Offset.x = -_startOffset;
    }
}
