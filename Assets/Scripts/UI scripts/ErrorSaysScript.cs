using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ErrorSaysScript : MonoBehaviour
{
    [SerializeField] GameObject bubble;
    [SerializeField] TextMeshProUGUI ErrorText;

    [SerializeField] float charDelay = 0.05f;

    [SerializeField] float afterTime = 3f;

    public void ErrorSaysCall(string text){
        StopAllCoroutines();
        ErrorText.text = "";
        StartCoroutine(ErrorSaysCoroutine(text));
    }

    public void ResetAndClose(){
        ErrorText.text = "";
        bubble.SetActive(false);
    }

    private IEnumerator ErrorSaysCoroutine(string text){
        bubble.SetActive(true);
        string completeText = text;

        ErrorText.text = "";
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(charDelay);

        for(int i = 0; i < completeText.Length; i++){
            ErrorText.text = completeText.Substring(0, i + 1);

            yield return delay;
        }
        ErrorText.text = completeText;

        yield return new WaitForSecondsRealtime(afterTime);
        ResetAndClose();
    }
}
