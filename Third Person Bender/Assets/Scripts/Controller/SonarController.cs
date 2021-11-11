using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SonarController : MonoBehaviour
{
    public Transform Origin;
    public Material sonarMaterial;
    public GameObject environmentObject;
    private MeshRenderer[] renderers;
    private Material[] savedMaterials;

    private bool blinded;
    
    void Start()
    {
        blinded = false;
        renderers = environmentObject.GetComponentsInChildren<MeshRenderer>();
        savedMaterials = new Material[renderers.GetLength(0)];
        int i=0;
        foreach(MeshRenderer renderer in renderers){ savedMaterials[i++] = renderer.material;};
    }
    void Update()
    {   if(Input.GetButtonDown("Blind"))
        {
            blinded = !blinded;
            StartCoroutine(ChangeMaterials());
        }
        sonarMaterial.SetVector("_Origin",new Vector4(Origin.position.x, Origin.position.y, Origin.position.z));
    }

    IEnumerator ChangeMaterials()
    {
        if(blinded)
        {
            foreach (MeshRenderer renderer in renderers) { renderer.material = sonarMaterial; }
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            int i = 0;
            foreach (MeshRenderer renderer in renderers)
            { 
                renderer.material = savedMaterials[i++];
            }
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }
        yield return null;
    }
}
