using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public Transform parentTransform; // Referência ao objeto pai que contém as peças
    public float lineHeight = 1f; // Altura de cada linha
    private List<int> linhasCompletas = new List<int>(); // Lista para armazenar linhas completas

    [SerializeField] LayerMask peçasLayerMask;

    public void VerificarLinhasCompletas()
    {
        // Limpa a lista de linhas completas
        linhasCompletas.Clear();

        // Verifica cada linha
        for (int y = 0; y < 20; y++) // Supondo que o jogo tenha 20 linhas
        {
            bool linhaCompleta = true;

            // Verifica se todas as colunas na linha y estão ocupadas
            for (int x = -5; x <= 5; x++) // Supondo que você tenha 10 colunas (de -5 a 5)
            {
                // Calcula a posição local
                Vector3 posicaoLocal = new Vector3(x, y * lineHeight, 0);
                // Converte a posição local para global
                Vector3 posicaoGlobal = parentTransform.TransformPoint(posicaoLocal);

                Collider2D[] colliders = Physics2D.OverlapBoxAll(posicaoGlobal, new Vector2(.5f, .5f), peçasLayerMask);

                if (colliders.Length == 0)
                {
                    linhaCompleta = false;
                    break;
                }
            }

            if (linhaCompleta)
            {
                linhasCompletas.Add(y);
            }
        }

        // Remove as linhas completas
        if (linhasCompletas.Count > 0)
        {
            RemoverLinhasCompletas();
        }
    }

    private void RemoverLinhasCompletas()
    {
        foreach (int linha in linhasCompletas)
        {
            for (int x = -5; x <= 5; x++) // Ajuste para verificar todas as colunas
            {
                // Calcula a posição local
                Vector3 posicaoLocal = new Vector3(x, linha * lineHeight, 0);
                // Converte a posição local para global
                Vector3 posicaoGlobal = parentTransform.TransformPoint(posicaoLocal);
                
                Collider2D[] colliders = Physics2D.OverlapBoxAll(posicaoGlobal, new Vector2(1, 1), 0);

                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Tetris"))
                    {
                        collider.gameObject.GetComponent<SingleBlockScript>().Destroy(); // Remove a peça
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Desenha Gizmos para as linhas que estão sendo verificadas
        Gizmos.color = Color.red; // Cor dos Gizmos

        for (int y = 0; y < 20; y++) // Supondo que o jogo tenha 20 linhas
        {
            for (int x = -5; x <= 5; x++) // Supondo que você tenha 10 colunas (de -5 a 5)
            {
                // Calcula a posição local
                Vector3 posicaoLocal = new Vector3(x, y * lineHeight, 0);
                // Converte a posição local para global
                Vector3 posicaoGlobal = parentTransform.TransformPoint(posicaoLocal);
                
                Gizmos.DrawWireCube(posicaoGlobal, new Vector3(1, 1, 0)); // Desenha um cubo wireframe
            }
        }
    }
}