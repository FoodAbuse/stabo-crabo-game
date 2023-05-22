using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineScript : MonoBehaviour
{
    public Material outlineMaterial;
    public float outlineScaleFactor;
    public Color outlineColour;
    private Renderer outlineRenderer;

    public bool isOutlined = true;

    // Start is called before the first frame update
    void Start()
    {
        outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColour);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOutlined)
        {
            outlineRenderer.enabled = true;
        }
        else
            outlineRenderer.enabled = false;
    }

    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color colour)
    {
        GameObject outlineObject = Instantiate(this.gameObject, transform.position, transform.rotation, transform);
        Renderer rend = outlineObject.GetComponent<Renderer>();

        rend.material = outlineMat;
        rend.material.SetColor("_OutlineColour" , colour);
        rend.material.SetFloat("_Scale" , scaleFactor);
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        outlineObject.GetComponent<OutlineScript>().enabled = false;
        outlineObject.GetComponent<Collider>().enabled = false;
        
        return rend;
    }
}
