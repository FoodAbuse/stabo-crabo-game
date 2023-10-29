using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSwapper : MonoBehaviour
{
    public Shader targetShader1;
    public Shader targetShader2;
    public Color highlightColour;
    public Color shadowColour;

    public Renderer[] activeObjectsInScene;
    //private PlayerControls controls;

    public bool Generate = false;

    // Start is called before the first frame update
    void Awake()
    {
        //controls = new PlayerControls();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Generate)
        {
            //Debug.Log("key pressed");
            GatherActiveMaterials();
            Generate = false;
        }
    }

    public void GatherActiveMaterials()
    {
        activeObjectsInScene = Object.FindObjectsOfType<Renderer>();

        foreach (Renderer activeObject in activeObjectsInScene)
        {
            //GameObject self = activeObject.gameObject;
            if (activeObject.material.shader == (targetShader1 || targetShader2))
            {
                //Debug.Log("Found Material");
                if (activeObject.materials.Length > 1)
            {
                Debug.Log("extra material found");
                int materialCount = activeObject.materials.Length;

                for (int i = 1; i < materialCount; i++)
                    {
                        //Debug.Log("updating material no. "+i);
                        activeObject.materials[i].SetColor("_HighlightColour", highlightColour);
                        activeObject.materials[i].SetColor("_ShadowColour", shadowColour);                    
                    }
            }
                
                activeObject.material.SetColor("_HighlightColour", highlightColour);
                activeObject.material.SetColor("_ShadowColour", shadowColour);
                
            }

            if (activeObject.materials.Length > 1)
            {
                Debug.Log("extra material found");
                int materialCount = activeObject.materials.Length;

                for (int i = 1; i < materialCount; i++)
                {
                    Debug.Log("updating material no. "+i);
                    activeObject.materials[i].SetColor("_HighlightColour", highlightColour);
                    activeObject.materials[i].SetColor("_ShadowColour", shadowColour);                    
                }
                
            }
        }

        activeObjectsInScene = null;
    }
}
