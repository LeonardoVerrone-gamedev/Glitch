using UnityEngine;
using UnityEngine.Tilemaps; // Essencial para trabalhar com Tilemaps

public class Bomb_Projectile_Script : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] string explosionVFX_tag = "NormalExplosionVFX";
    [SerializeField] string breakWallVFX_tag = "BreakWallEffect";

    [SerializeField] string poolTag = "BombProjectile";

    [Header("Components")]
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;

    [Header("Explosion Settings")]
    [SerializeField] float explosionRadius;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask wallLayer; // Nova LayerMask para os Tilemaps de parede

    [SerializeField] int damage = 10;

    [SerializeField] Vector2 direction;


    void OnCollisionEnter2D(Collision2D col)
    {
        // Mantém a lógica original para iniciar a animação
        if (col.gameObject.CompareTag("Ground")) //|| col.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("Land");
        }
    }

    /// <summary>
    /// Este método é chamado pela animação no final da explosão.
    /// </summary>
    public void StartExplosion()
    {
        // 1. Spawna o efeito visual principal da explosão
        ObjectPoolManager.Instance.SpawnFromPool(explosionVFX_tag, transform.position, Quaternion.identity);

        // 2. Detecta e aplica dano aos jogadores
        DetectPlayers();

        // 3. Detecta e destrói os tiles de parede
        DestroyTilesInRadius();

        Invoke("OnDespawn", .55f);
    }

    private void DetectPlayers()
    {
        Collider2D[] playerCollisions = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D playerCol in playerCollisions)
        {
            playerCol.GetComponent<PlayerHealth>()?.TakeDamage(damage, null);
        }
    }

    private void DestroyTilesInRadius()
    {
        // Encontra todos os colliders de parede (que devem ter o componente Tilemap)
        Collider2D[] wallColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, wallLayer);

        foreach (Collider2D wallCol in wallColliders)
        {
            Tilemap tilemap = wallCol.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                // Calcula a área de verificação em coordenadas de tile
                Vector3 minWorldPos = transform.position - new Vector3(explosionRadius, explosionRadius, 0);
                Vector3 maxWorldPos = transform.position + new Vector3(explosionRadius, explosionRadius, 0);

                Vector3Int minTilePos = tilemap.WorldToCell(minWorldPos);
                Vector3Int maxTilePos = tilemap.WorldToCell(maxWorldPos);

                // Itera por todos os tiles na caixa delimitadora da explosão
                for (int x = minTilePos.x; x <= maxTilePos.x; x++)
                {
                    for (int y = minTilePos.y; y <= maxTilePos.y; y++)
                    {
                        Vector3Int tilePos = new Vector3Int(x, y, 0);
                        Vector3 tileWorldPos = tilemap.GetCellCenterWorld(tilePos);

                        // Verifica se o centro do tile está realmente dentro do raio da explosão
                        if (Vector2.Distance(tileWorldPos, transform.position) <= explosionRadius)
                        {
                            // Se existir um tile nesta posição, destrói-o
                            if (tilemap.GetTile(tilePos) != null)
                            {
                                // Spawna o efeito de quebra
                                ObjectPoolManager.Instance.SpawnFromPool(breakWallVFX_tag, tileWorldPos, Quaternion.identity);

                                // Remove o tile do Tilemap
                                tilemap.SetTile(tilePos, null);
                            }
                        }
                    }
                }
            }
        }
    }

    // Ajuda a visualizar o raio da explosão no Editor da Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    void OnDisable()
    {
        anim.SetTrigger("Reset");
        rb.linearVelocity = Vector2.zero;
    }

    void OnEnable()
    {
        anim.SetTrigger("Reset");
        rb.linearVelocity = Vector2.zero;
    }

    public void Launch(Vector2 _dir, float _launchForce)
    {
        direction = (Vector2.up + _dir).normalized;

        if (direction.x < 0f)
        {
            anim.SetFloat("dirX", -1f);
        }
        else
        {
            anim.SetFloat("dirX", 1f);
        }

        rb.AddForce(direction * _launchForce, ForceMode2D.Impulse);
    }

    void OnDespawn()
    {
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject, poolTag);
    }
}