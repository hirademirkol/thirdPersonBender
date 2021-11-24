using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SonarController : MonoBehaviour
{
    public Camera MainCamera;
    public Transform Origin;
    public Shader SonarShader;
    public GameObject EnvironmentObject;
    public Material[] RockMaterials;
    public float SonarSpeed = 3f;
    public float MaxSonarDistance = 20f;

    private MeshRenderer[] _renderers;
    private bool _blinded;
    private float _distance = 0;

    void Start()
    {
        _blinded = false;
        _renderers = EnvironmentObject.GetComponentsInChildren<MeshRenderer>();
        Shader.SetGlobalFloat("_Speed",SonarSpeed);
    }

    void Update()
    {   
        if(Input.GetButtonDown("Blind"))
        {
            _blinded = !_blinded;
            StartCoroutine(ChangeMaterials());
        }
        _distance += SonarSpeed * Time.deltaTime;
        _distance %= MaxSonarDistance;
        Shader.SetGlobalFloat("_Distance",_distance);
        Shader.SetGlobalVector("_Origin", Origin.position);
    }


    void OnApplicationQuit()
    {
        if(_blinded)
        {
            foreach (Material mat in RockMaterials) { mat.shader = Shader.Find("Standard"); }
        }
    }

    IEnumerator ChangeMaterials()
    {
        if(_blinded)
        {
            foreach (MeshRenderer renderer in _renderers) { renderer.material.shader = SonarShader; }
            foreach (Material mat in RockMaterials) { mat.shader = SonarShader; }
            MainCamera.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            foreach (MeshRenderer renderer in _renderers){ renderer.material.shader = Shader.Find("Standard"); }
            foreach (Material mat in RockMaterials) { mat.shader = Shader.Find("Standard"); }
            MainCamera.clearFlags = CameraClearFlags.Skybox;
        }
        yield return null;
    }
}
