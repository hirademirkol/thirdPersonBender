using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SonarController : MonoBehaviour
{
    public Camera MainCamera;
    public Transform Origin;
    public Shader SonarShader;
    public Shader SonarTerrainShader;
    //public GameObject EnvironmentObject;
    public Material TerrainMaterial;
    public Material[] RockMaterials;
    public Material[] CharacterMaterials;
    public float SonarSpeed = 3f;
    public float MaxSonarDistance = 20f;

    private MeshRenderer[] _renderers;
    private bool _blinded;
    private float _distance;

    void Start()
    {
        _blinded = false;
        //_renderers = EnvironmentObject.GetComponentsInChildren<MeshRenderer>();
        Shader.SetGlobalFloat("_Speed",SonarSpeed);
    }

    void OnApplicationQuit()
    {
        if(_blinded)
        {
            TerrainMaterial.shader = Shader.Find("Nature/Terrain/Standard");
            foreach (Material mat in RockMaterials) { mat.shader = Shader.Find("Standard"); }
            foreach (Material mat in CharacterMaterials) { mat.shader = Shader.Find("Standard"); }
        }
    }

    public void Blind()
    {
            _blinded = !_blinded;
            StartCoroutine(ChangeMaterials());
            if (_blinded)
                StartCoroutine(Blimp());
    }

    IEnumerator ChangeMaterials()
    {
        if(_blinded)
        {
            //foreach (MeshRenderer renderer in _renderers) { renderer.material.shader = SonarShader; }
            TerrainMaterial.shader = SonarTerrainShader;
            foreach (Material mat in RockMaterials) { mat.shader = SonarShader; }
            foreach (Material mat in CharacterMaterials) { mat.shader = SonarShader; }
            MainCamera.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            //foreach (MeshRenderer renderer in _renderers){ renderer.material.shader = Shader.Find("Standard"); }
            TerrainMaterial.shader = Shader.Find("Nature/Terrain/Standard");
            foreach (Material mat in RockMaterials) { mat.shader = Shader.Find("Standard"); }
            foreach (Material mat in CharacterMaterials) { mat.shader = Shader.Find("Standard"); }
            MainCamera.clearFlags = CameraClearFlags.Skybox;
        }
        yield return null;
    }

    IEnumerator Blimp()
    {
        _distance = 0.5f;
        while(_blinded)
        {
            _distance += SonarSpeed * Time.deltaTime;
            _distance %= MaxSonarDistance;
            if(_distance < 0.5f)
                _distance = 0.5f;
            
            Shader.SetGlobalFloat("_Distance", _distance);
            Shader.SetGlobalVector("_Origin", Origin.position);
            yield return null;
        }
    }
}
