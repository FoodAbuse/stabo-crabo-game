using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSwapper : MonoBehaviour
{
    public Shader targetShader1;
    public Shader targetShader2;
    public Color highlightColour;
    public Color shadowColour;

    public Color defaultHighlight;
    public Color defaultShadow;
    public Color desiredHighlight;
    public Color desiredShadow;

    public GameObject currentItem;
    public Renderer[] activeObjectsInScene;


    public bool Generate = false;


    
    // Start is called before the first frame update
    void Awake()
    {
        highlightColour = desiredHighlight;
        shadowColour = desiredShadow;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Generate)
        {
            GatherActiveMaterials();
            //setColor();
            Generate = false;
        }
    }

    public void GatherActiveMaterials()
    {
        activeObjectsInScene = Object.FindObjectsOfType<Renderer>();
        
        foreach (Renderer activeObject in activeObjectsInScene)
        {
            currentItem = activeObject.gameObject;
            
            //GameObject self = activeObject.gameObject;
            if (activeObject.sharedMaterial.shader == (targetShader1 || targetShader2))
            {
                //activeMaterials.Add(activeObject.sharedMaterial);

                
                int materialsInObject = activeObject.sharedMaterials.Length;
                
                //Debug.Log("Found Material");
                if (activeObject.sharedMaterials.Length == 1)
                {
                    activeObject.sharedMaterial.SetColor("_defaultHighlight", highlightColour);
                    activeObject.sharedMaterial.SetColor("_defaultShadow", shadowColour);
                }
                else
                {

                    for (int i = 0; i < materialsInObject; i++)
                    {   
                        

                            activeObject.sharedMaterials[i].SetColor("_defaultHighlight", highlightColour);
                            activeObject.sharedMaterials[i].SetColor("_defaultShadow", shadowColour);                                   
                                           
                    }            
                }
                
                
            }
            else
            {
                break;
            }
        }


        activeObjectsInScene = null;
        currentItem = null;
    }


    public void setColor()
    {
        
       Shader.SetGlobalColor("_defaultHighlight", highlightColour);
       Shader.SetGlobalColor("_defaultShadow", shadowColour);
    }

    public void OnApplicationQuit() 
    {
        highlightColour = defaultHighlight;
        shadowColour = defaultShadow;

        GatherActiveMaterials();
    }

    
}
