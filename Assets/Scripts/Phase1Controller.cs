using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArabicSupport;

public class Phase1Controller : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    [SerializeField] private List<TMP_Text> targetText;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private SpriteRenderer fadeInSprite; 

    private bool fadedOut = false;
    private bool fadedIn = false;
    private void Awake()
    {
        foreach(TMP_Text tMP in targetText)
        {
            tMP.text = ArabicFixer.Fix(tMP.text, false, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!fadedOut)
            {
                fadedOut = true;    
                TriggerFadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!fadedIn)
            {
                fadedIn = true;
                TriggerFadeIn();
            }
        }
    }

    public void TriggerFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void TriggerFadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;

        List<Color> spriteStartColors = new List<Color>();
        foreach (var sr in spriteRenderers)
            spriteStartColors.Add(sr.color);

        List<Color> textStartColors = new List<Color>();
        if (targetText != null)
        {
            foreach (var txt in targetText)
                textStartColors.Add(txt != null ? txt.color : Color.white);
        }

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                var sr = spriteRenderers[i];
                var startColor = spriteStartColors[i];
                sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }

            if (targetText != null)
            {
                for (int i = 0; i < targetText.Count; i++)
                {
                    var txt = targetText[i];
                    if (txt != null)
                    {
                        var startColor = textStartColors[i];
                        txt.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            var sr = spriteRenderers[i];
            var startColor = spriteStartColors[i];
            sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        }

        if (targetText != null)
        {
            for (int i = 0; i < targetText.Count; i++)
            {
                var txt = targetText[i];
                if (txt != null)
                {
                    var startColor = textStartColors[i];
                    txt.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
                }
            }
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        if (fadeInSprite == null)
            yield break;

        Color startColor = fadeInSprite.color;
        float elapsed = 0f;

        fadeInSprite.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeInSprite.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeInSprite.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }
}
