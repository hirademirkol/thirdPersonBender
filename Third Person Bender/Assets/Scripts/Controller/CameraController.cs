using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineCameraOffset CameraOffset;
    public float SwitchDuration = 1f;

    private float _startOffset;
    
    void Start()
    {
        _startOffset = CameraOffset.m_Offset.x;
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
        _startOffset = CameraOffset.m_Offset.x;
        while (time < SwitchDuration)
        {
            CameraOffset.m_Offset.x = Mathf.Lerp(_startOffset, -_startOffset, time / SwitchDuration);
            time += Time.deltaTime;
            yield return null;
        }
        CameraOffset.m_Offset.x = -_startOffset;
    }
}
