using UnityEngine;

public class BreakWallEffect_ColorManager : MonoBehaviour
{
    [SerializeField]ParticleSystem destroços;

    public void SetColor(Color _color){
        destroços.startColor = _color;
    }
}
