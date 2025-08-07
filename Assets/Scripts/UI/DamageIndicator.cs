using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed;

    private Coroutine coroutine;


    void Start()
    {
        CharacterManager.Instance.Player.condition.onTakeDamage += Flash;   // onTakeDamage 에 Flash를 추가
    }

    public void Flash() // 코루틴으로 서서히 사라지게
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        image.enabled = true;
        image.color = new Color(1f, 100f / 255f, 100f / 255f);
        coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;        // 시작알파값 = 0.3f (투명도)
        float a = startAlpha;           // 여기서 알파값 = 투명도

        while(a > 0)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;            // 알파값 서서히 사라지게
            image.color = new Color(1f, 100f / 255f, 100f / 255f, a);
            yield return null;
        }

        image.enabled = false;
    }
}
