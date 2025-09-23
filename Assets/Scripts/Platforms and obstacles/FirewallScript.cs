using UnityEngine;
using System.Collections; // Necessário para Corrotinas

public class FirewallScript : MonoBehaviour
{

    [SerializeField] enum Mode { timed, looped, moving, rotating };
    [Tooltip("Modos de operação que podem ser combinados. Ex: 'timed' e 'moving' para um lança-chamas que liga/desliga e se move.")]
    [SerializeField] Mode[] modes;

    [SerializeField] ParticleSystem[] fires;
    [SerializeField] ParticleSystem[] smokes;

    [Header("Timed Settings")]
    [Tooltip("Tempo em segundos que o fogo permanece ativo.")]
    [SerializeField] float duration = 2f;
    [Tooltip("Tempo em segundos que o fogo permanece inativo entre as ativações.")]
    [SerializeField] float interval = 3f;
    [Tooltip("Tempo de delay para iniciar")]
    [SerializeField] float startDelay = 0f;

    [Header("Rotate Settings")]
    [Tooltip("Velocidade da rotação em graus por segundo.")]
    [SerializeField] float rotateSpeed = 50f;
    [Tooltip("Ângulo máximo de rotação a partir da origem (em graus).")]
    [SerializeField] float maxRotateAngle = 45f;

    [Header("Move Settings")]
    [Tooltip("Velocidade de movimento do objeto.")]
    [SerializeField] float moveSpeed = 3f;
    [Tooltip("Pontos (Transforms) pelos quais o objeto se moverá em sequência.")]
    [SerializeField] Transform[] movePoints;

    // Variáveis de controle interno
    private int currentMovePointIndex = 0;
    private int rotateDirection = 1;
    private bool isMoving = false;
    private bool isRotating = false;

    void Start()
    {
        // Itera sobre todos os modos selecionados no Inspector
        foreach (Mode mode in modes)
        {
            switch (mode)
            {
                case Mode.looped:
                    StartLoopedFirewall();
                    break;

                case Mode.timed:
                    // Inicia a rotina de ligar e desligar
                    StartCoroutine(TimedFirewallCoroutine());
                    break;

                case Mode.moving:
                    // Ativa a flag de movimento se houver pontos definidos
                    if (movePoints != null && movePoints.Length > 0)
                    {
                        isMoving = true;
                    }
                    else
                    {
                        Debug.LogWarning("Modo 'moving' selecionado, mas nenhum 'movePoints' foi atribuído.", this);
                    }
                    break;

                case Mode.rotating:
                    // Ativa a flag de rotação
                    isRotating = true;
                    break;
            }
        }
    }

    void OnEnable()
    {
        Start();
    }

    void Update()
    {
        // Executa a lógica de frame a frame para os modos ativos
        if (isMoving)
        {
            HandleMovement();
        }
        if (isRotating)
        {
            HandleRotation();
        }
    }

    /// <summary>
    /// Ativa as partículas em modo de loop contínuo. 🔥
    /// </summary>
    public void StartLoopedFirewall()
    {
        foreach (ParticleSystem fire in fires)
        {
            var main = fire.main;
            main.loop = true;
            fire.Play();
        }
        foreach (ParticleSystem smoke in smokes)
        {
            var main = smoke.main;
            main.loop = true;
            smoke.Play();
        }
    }

    /// <summary>
    /// Corrotina que gerencia o ciclo de ligar/desligar o fogo. ⏲️
    /// </summary>
    private IEnumerator TimedFirewallCoroutine()
    {
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // Define a duração das partículas para corresponder à variável 'duration'
        foreach (ParticleSystem fire in fires)
        {
            var main = fire.main;
            main.duration = this.duration;
        }
        foreach (ParticleSystem smoke in smokes)
        {
            var main = smoke.main;
            main.duration = this.duration;
        }

        // Loop infinito para o ciclo de ativação e desativação
        while (true)
        {
            // Ativa todas as partículas
            foreach (ParticleSystem fire in fires) { fire.Play(); }
            foreach (ParticleSystem smoke in smokes) { smoke.Play(); }

            // Espera a duração do fogo
            yield return new WaitForSeconds(duration);

            // Para todas as partículas (permite que as existentes desapareçam naturalmente)
            foreach (ParticleSystem fire in fires) { fire.Stop(); }
            foreach (ParticleSystem smoke in smokes) { smoke.Stop(); }

            // Espera o intervalo antes de recomeçar o ciclo
            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Controla o movimento do objeto entre os waypoints. ↔️
    /// </summary>
    private void HandleMovement()
    {
        // Define o ponto de destino
        Transform targetPoint = movePoints[currentMovePointIndex];
        
        // Move o objeto em direção ao ponto de destino
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Verifica se o objeto chegou ao destino
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Avança para o próximo ponto na lista
            currentMovePointIndex++;
            // Se chegar ao último ponto, retorna ao primeiro
            if (currentMovePointIndex >= movePoints.Length)
            {
                currentMovePointIndex = 0;
            }
        }
    }

    /// <summary>
    /// Controla a rotação do objeto para frente e para trás. 🔄
    /// </summary>
    private void HandleRotation()
    {
        // Calcula o ângulo de rotação atual
        float angle = transform.localEulerAngles.z;
        // Converte para um intervalo de -180 a 180 para facilitar a verificação
        angle = (angle > 180) ? angle - 360 : angle;

        // Inverte a direção se atingir os limites
        if (angle >= maxRotateAngle)
        {
            rotateDirection = -1;
        }
        else if (angle <= -maxRotateAngle)
        {
            rotateDirection = 1;
        }

        // Aplica a rotação
        transform.Rotate(0, 0, rotateSpeed * rotateDirection * Time.deltaTime);
    }
}