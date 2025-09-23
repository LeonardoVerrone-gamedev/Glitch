using Unity.VisualScripting;
using UnityEngine;

public class PlayerMaterialManager : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    [SerializeField] private SpriteRenderer spriteRenderer;

    //INDEX 0 = unlit defeault
    //INDEX 1 = glitch shader
    //INDEX 2 = Outline only shader;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    public void SetMaterial(int index)
    {
        spriteRenderer.material = materials[index];
    }
}
