using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBlockScript : MonoBehaviour
{
    private ScratchAndStretch squash;
    ParticleSystem dust;
    //private SpiteRenderer spiteRenderer;

    [SerializeField] GameObject predictionMarkTetris;

    GameObject spawnedMark;

    private LayerMask groundLayer;

    // Start is called before the first frame update

    void Start(){
        predictionMarkTetris = Resources.Load<GameObject>("TetrisFallMarker");
        groundLayer = LayerMask.GetMask("Ground");

        UpdatePredictionMark();

        squash = transform.parent.GetComponent<ScratchAndStretch>();
        dust = transform.parent.GetComponent<FallingTetrisPiece>().dust;

        //spiteRenderer = GetComponent<SpiteRenderer>();
    }

    void UpdatePredictionMark(){
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y - 3f);
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, 30f, groundLayer);
        if(hit.collider != null){
            Vector3 pos = new Vector3(hit.point.x, (hit.point.y + 0.5f), 0f);
            spawnedMark = Instantiate(predictionMarkTetris, pos, Quaternion.identity);
        }
    }

    public void DestroySpawnedMark(){
       Destroy(spawnedMark);
    }

    public void Destroy()
    {
        StartCoroutine(_Destroy());
    }

    IEnumerator _Destroy(){
        dust.Play();
        squash.PlayStretchAnimation("BlockDestroy");
        yield return new WaitForSeconds(.5f);
        Destroy(this.gameObject);
    }

}
