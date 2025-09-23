using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class TetrisManager : MonoBehaviour
{
    [Header("Tetris Settings")]
    public GameObject[] tetrisPiecePrefab;
    public List<int> colunas;
    public List<int> rotações;
    public Transform parentObject;
    public Transform spawnPosition;
    public LineManager lineManager;
    public float waitTime = 0.5f;

    [Header("Player Detection")]
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 detectionAreaSize = new Vector3(10f, 5f, 10f);
    [SerializeField] private Vector3 detectionAreaOffset = Vector3.zero;
    [SerializeField] private Color gizmoColor = new Color(0, 1, 0, 0.3f);
    [SerializeField] private bool alwaysShowGizmo = false;

    private Coroutine pieceCreationCoroutine;
    private bool isActive = false;
    private int currentPieceIndex = 0; // Índice da peça atual

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(TetrisSystemRoutine());
    }

    private IEnumerator TetrisSystemRoutine()
    {
        while (true)
        {
            bool playerInside = IsPlayerInBounds();
            
            if (playerInside && !isActive)
            {
                isActive = true;
                pieceCreationCoroutine = StartCoroutine(CreateTetrisPieces());
            }
            else if (!playerInside && isActive)
            {
                isActive = false;
                if (pieceCreationCoroutine != null)
                {
                    StopCoroutine(pieceCreationCoroutine);
                    pieceCreationCoroutine = null;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CreateTetrisPieces()
    {
        while (true) // Loop infinito para criar peças continuamente
        {
            if (!isActive) yield break;

            // Verifica se o índice atual é maior que o número de colunas
            if (currentPieceIndex >= colunas.Count)
            {
                currentPieceIndex = 0; // Reinicia o índice para 0
            }

            lineManager.VerificarLinhasCompletas();

            var newPiece = Instantiate(tetrisPiecePrefab[currentPieceIndex], spawnPosition.position, Quaternion.identity);
            var fallingPiece = newPiece.GetComponent<FallingTetrisPiece>();
            fallingPiece.coluna = colunas[currentPieceIndex];
            fallingPiece.rotacao = rotações[currentPieceIndex];
            fallingPiece.parentTransform = parentObject;

            yield return new WaitUntil(() => fallingPiece.caiu);

            lineManager.VerificarLinhasCompletas();
            yield return new WaitForSeconds(waitTime);

            currentPieceIndex++; // Avança para a próxima peça
        }
    }

    private bool IsPlayerInBounds() 
    {
        if (player == null) return false;
        
        Vector3 centerPosition = transform.position + transform.TransformDirection(detectionAreaOffset);
        Vector3 playerPos = centerPosition - player.transform.position;
        
        playerPos = Quaternion.Inverse(transform.rotation) * playerPos;

        return Mathf.Abs(playerPos.x) < detectionAreaSize.x/2 && 
               Mathf.Abs(playerPos.y) < detectionAreaSize.y/2 &&
               Mathf.Abs(playerPos.z) < detectionAreaSize.z/2;
    }

    private void OnDrawGizmos()
    {
        if (alwaysShowGizmo)
        {
            DrawDetectionArea();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!alwaysShowGizmo)
        {
            DrawDetectionArea();
        }
    }

    private void DrawDetectionArea()
    {
        Gizmos.color = gizmoColor;
        Vector3 centerPosition = transform.position + transform.TransformDirection(detectionAreaOffset);
        
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(centerPosition, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.DrawWireCube(Vector3.zero, detectionAreaSize);
        Gizmos.DrawCube(Vector3.zero, detectionAreaSize * 0.99f);
    }
}
