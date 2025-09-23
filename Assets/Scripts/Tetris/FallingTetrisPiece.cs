using System.Collections;
using UnityEngine;

public class FallingTetrisPiece : MonoBehaviour
{
    public float coluna; // Coluna onde a peça deve cair (1 a 10)
    public int rotacao; // Rotação da peça (0 a 3)
    public float velocidadeDeQueda = 2f; // Velocidade de queda da peça
    public Transform parentTransform; // Referência ao objeto pai

    public ParticleSystem dust;
    private ScratchAndStretch squash;

    public bool caiu = false;
    DamageArea damage;
    CompositeCollider2D composite_collider;

    private void Start()
    {
        composite_collider = GetComponent<CompositeCollider2D>();
        damage = GetComponent<DamageArea>();
        damage.canCauseDamage = true;
        dust = GetComponent<ParticleSystem>();
        squash = GetComponent<ScratchAndStretch>();
        // Definir a posição inicial da peça em relação ao objeto pai
        Vector3 posicaoInicial = parentTransform.position + new Vector3(coluna - 3f, 25f, 0); // Ajuste a posição conforme necessário
        transform.position = posicaoInicial;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Ground") && !caiu){
            caiu = true;
            squash.PlayStretchAnimation("Landing");
            dust.Play();

            damage.canCauseDamage = false;

            if(composite_collider != null){
                composite_collider.enabled = false;
            }


            // Transformar a peça em um chão normal
            gameObject.layer = LayerMask.NameToLayer("Ground"); // Mude para a camada de chão
            gameObject.tag = "Ground"; // Define a tag como "Ground"

            //Destroy(GetComponent<DamageArea>()); // Remove o script de dano

            foreach(Transform child in transform){
                child.gameObject.GetComponent<SingleBlockScript>().DestroySpawnedMark();
            }
        }
    }
}