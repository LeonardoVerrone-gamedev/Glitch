using UnityEngine;

public class WindowPlatformScript : MonoBehaviour
{
    [SerializeField]Animator anim;
    BoxCollider2D boxCol;
    [SerializeField] float timeToReactivate = 5f;

    void Awake(){
        anim = GetComponent<Animator>();
        boxCol =  GetComponent<BoxCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D coll){
        if (coll.gameObject.CompareTag("Player"))
        {
            anim.SetTrigger("Close");
        }
    }

    public void SetUnactine()
    {
        boxCol.enabled = false;
        Invoke("SetActive", timeToReactivate);
    }

    public void SetActive(){
        anim.SetTrigger("Default");
        boxCol.enabled = true;
    }
}
