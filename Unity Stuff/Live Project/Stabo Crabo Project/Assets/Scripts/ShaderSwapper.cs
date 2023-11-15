using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

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


    [Header("Level Presets")]
    [Space(10)]

    [SerializeField]
    private Color highlightLevel1;
    [SerializeField]
    private Color shadowLevel1;
    [Space(5)]

    [SerializeField]
    private Color highlightLevel2;
    [SerializeField]
    private Color shadowLevel2;
    [Space(5)]

    [SerializeField]
    private Color highlightLevel3;
    [SerializeField]
    private Color shadowLevel3;
    [Space(5)]


    public bool Generate = false;
    private string globalShadow = ("_GlobalShadow");
    private string globalHighlight = ("_GlobalHighlight");

    
    void Awake()
    {
        //AssignSceneColour();
    }

    void Start()
    {   
        AssignSceneColour(SceneManager.GetActiveScene().name);

        SetGlobalColor(globalHighlight, defaultHighlight);
        SetGlobalColor(globalShadow, defaultShadow);
        //GatherActiveMaterials();
    }
    public void AssignSceneColour(string sceneName)
    {

        string o = sceneName;

        if (o == ("MainMenu") || o == ("Level1"))
        {
            highlightColour = highlightLevel1;
            shadowColour = shadowLevel1;
            return;
        }
        if (o == ("Level2"))
        {
            Debug.Log("Shaders set to Lv2 Lighting");
            highlightColour = highlightLevel2;
            shadowColour = shadowLevel2;
            return;
        }
        if (o == ("Level3"))
        {
            Debug.Log("Shaders set to Lv3 Lighting");
            highlightColour = highlightLevel3;
            shadowColour = shadowLevel3;
            return;
        }

        else
        {
            Debug.Log("Error: Scene ref not recognized. Ref = " +o);
            highlightColour = defaultHighlight;
            shadowColour = defaultShadow;
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


    public void setColour()
    {
        
       Shader.SetGlobalColor(globalHighlight, highlightColour);
       Shader.SetGlobalColor(globalShadow, shadowColour);
    }

    public void OnApplicationQuit() 
    {
        highlightColour = defaultHighlight;
        shadowColour = defaultShadow;
        
        setColour();
    }

    public static void SetGlobalColor(string globalName, Color globalColour)
    {
        Shader.SetGlobalColor(globalName, globalColour);
    }

    
}
