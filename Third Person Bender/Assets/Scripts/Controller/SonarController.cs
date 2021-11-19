using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarController : MonoBehaviour
{
    public Camera MainCamera;
    public Transform Origin;
    public Material SonarMaterial;
    public Shader RockSonarShader;
    public GameObject EnvironmentObject;
    public Material[] RockMaterials;

    private MeshRenderer[] _renderers;
    private Material[] _savedMaterials;
    private bool _blinded;
    
    void Start()
    {
        _blinded = false;
        _renderers = EnvironmentObject.GetComponentsInChildren<MeshRenderer>();
        _savedMaterials = new Material[_renderers.GetLength(0)];
        int i = 0;
        foreach(MeshRenderer renderer in _renderers){ _savedMaterials[i++] = renderer.material; };
    }

    void Update()
    {   
        if(Input.GetButtonDown("Blind"))
        {
            _blinded = !_blinded;
            StartCoroutine(ChangeMaterials());
        }
        SonarMaterial.SetVector("_Origin",new Vector4(Origin.position.x, Origin.position.y, Origin.position.z));
    }

    IEnumerator ChangeMaterials()
    {
        if(_blinded)
        {
            foreach (MeshRenderer renderer in _renderers) { renderer.material = SonarMaterial; }
            MainCamera.clearFlags = CameraClearFlags.SolidColor;
            foreach (Material mat in RockMaterials) { mat.shader = RockSonarShader; }
        }
        else
        {
            int i = 0;
            foreach (MeshRenderer renderer in _renderers){ renderer.material = _savedMaterials[i++]; }
            MainCamera.clearFlags = CameraClearFlags.Skybox;
            foreach (Material mat in RockMaterials) { mat.shader = Shader.Find("Standard"); }
        }
        yield return null;
    }
}
