using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class KameHameHaProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem chargeEffect;
    [SerializeField] Animator anim;

    [SerializeField] Rigidbody2D rb;
    private BoxCollider2D hitbox;

    [SerializeField] float prewarmTime;

    private int direction = 1; // 1 means moving right, -1 means moving left

    public float speed = 10f;              // Movement speed of the projectile

    bool hasBeenShooted;

    [SerializeField] string HitEffectName = "PlayerBulletExplosion";
    [SerializeField] string HitEffectName_WallBreak = "BreakWallEffect";
    [SerializeField] private float colliderHeight = 2f; // Altura fixa do collider
    private Vector2 _startPos;

    [SerializeField] int damage = 10;

    [SerializeField] private List<Collider2D> catchedEnemies;

    [SerializeField] private TrailRenderer trailRenderer;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        StartTrail();
        catchedEnemies.Clear();

        chargeEffect.Play();

        hitbox = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        // Ensure the collider is a trigger
        hitbox.isTrigger = true;

        Invoke("Fire", prewarmTime);
    }

    void OnDisable(){
        hasBeenShooted = false;
        rb.linearVelocity = Vector2.zero;
        StopAndResetTrail();
    }

    private void Fire()
    {
        // Determine the direction based on Y rotation
        float yRotation = transform.eulerAngles.y;

        // Set direction based on rotation
        if (Mathf.Approximately(yRotation, 180f))
        {
            direction = -1; // Move left
        }
        else
        {
            direction = 1; // Move right
        }
        
        _startPos = transform.position;
        hasBeenShooted = true;
        anim.SetTrigger("Fire");
        rb.linearVelocity = new Vector2(speed * direction, 0f);
    }

    void FixedUpdate()
    {
        if (hasBeenShooted)
        {
            // Keep moving the projectile
            rb.linearVelocity = new Vector2(speed * direction, 0f);

            Vector2 currentPos = transform.position;
            float distance = Vector2.Distance(_startPos, currentPos);
            // Define o centro do OverlapBox
            Vector2 midpoint = (_startPos + currentPos) / 2f;
            // Verifica colisões com objetos na camada "Enemy"
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(midpoint, new Vector2(distance, colliderHeight), 0f, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemy in hitEnemies)
            {
                if (!catchedEnemies.Contains(enemy))
                {
                    catchedEnemies.Add(enemy);

                    ObjectPoolManager.Instance.SpawnFromPool(HitEffectName, enemy.transform.position, Quaternion.identity);
                    PostProcessVolumeManager.Instance.TriggerFlashEffect(3, .5f);
                    EnemyLife enemyLife = enemy.GetComponent<EnemyLife>();
                    enemyLife?.TakeDamage(damage, true);
                }
            }

            // Verifica colisão com o Tilemap (apenas um)
            Collider2D wallCollider = Physics2D.OverlapBox(midpoint, new Vector2(distance, colliderHeight), 0f, LayerMask.GetMask("Wall"));
            if (wallCollider != null)
            {
                Tilemap tilemap = wallCollider.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    // Calcula os limites da área de destruição em células do Tilemap
                    Vector3 minWorldPos = midpoint - new Vector2(distance / 2, colliderHeight / 2);
                    Vector3 maxWorldPos = midpoint + new Vector2(distance / 2, colliderHeight / 2);
                    
                    Vector3Int minTilePos = tilemap.WorldToCell(minWorldPos);
                    Vector3Int maxTilePos = tilemap.WorldToCell(maxWorldPos);
                    // Itera por todas as posições de tile dentro da área
                    for (int x = minTilePos.x; x <= maxTilePos.x; x++)
                    {
                        for (int y = minTilePos.y; y <= maxTilePos.y; y++)
                        {
                            Vector3Int tilePos = new Vector3Int(x, y, 0);
                            
                            // Se existir um tile nesta posição, destrói com efeito
                            if (tilemap.GetTile(tilePos) != null)
                            {
                                Color tileColor = tilemap.color;
                                Vector3 tileWorldPos = tilemap.GetCellCenterWorld(tilePos);
                                
                                var effect = ObjectPoolManager.Instance.SpawnFromPool(
                                    HitEffectName_WallBreak, 
                                    tileWorldPos, 
                                    Quaternion.identity);
                                
                                effect.GetComponent<BreakWallEffect_ColorManager>().SetColor(tileColor);
                                tilemap.SetTile(tilePos, null);
                            }
                        }
                    }
                }
            }
        }
    }

    public void StopAndResetTrail()
    {
        // Para a emissão do TrailRenderer
        trailRenderer.emitting = false;
        // Opcional: Para "resetar" a trilha, você pode desativar e reativar o TrailRenderer
        // Isso irá limpar a trilha atual
        trailRenderer.Clear(); // Limpa a trilha atual
        // Se você quiser reativar a emissão, você pode fazer isso aqui
        // trailRenderer.emitting = true; // Descomente se quiser reiniciar a emissão imediatamente
    }
    public void StartTrail()
    {
        // Reinicia a emissão do TrailRenderer
        trailRenderer.emitting = true;
    }
    
    private void OnDrawGizmos()
    {
        // Desenha a área do OverlapBox no editor para visualização
        if (hasBeenShooted)
        {
            Vector2 currentPos = transform.position;
            float distance = Vector2.Distance(_startPos, currentPos);
            Vector2 midpoint = (_startPos + currentPos) / 2f;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(midpoint, new Vector2(distance, colliderHeight));
        }
    }
}