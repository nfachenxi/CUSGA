using System.Collections;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    private static readonly int FadeIn = Animator.StringToHash("FadeIn");
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");
    
    public GameObject gameStatement;
    public GameObject gameTips;

    public Animator statementAnim;
    public Animator tipsAnim;
    
    void Start()
    {
        statementAnim = gameStatement.GetComponent<Animator>();
        tipsAnim = gameTips.GetComponent<Animator>();

        StartCoroutine(StartAnim());
    }

    private IEnumerator StartAnim()
    {
        statementAnim.SetTrigger(FadeIn);
        yield return new WaitForSeconds(3f);
        statementAnim.SetTrigger(FadeOut);
        yield return new WaitForSeconds(2f);
        tipsAnim.SetTrigger(FadeIn);
        yield return new WaitForSeconds(5f);
        tipsAnim.SetTrigger(FadeOut);
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
    }
}
