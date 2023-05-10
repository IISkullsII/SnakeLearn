using System.Collections;
using UnityEngine;

public class TextFadeController : MonoBehaviour
{
    [SerializeField] private float duration = 2f; // In Seconds
    [SerializeField] private float timeSteps = 0.1f; // In Seconds

    private GameObject _parentGameObject;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void StartFade()
    {
        ResetFade();
        StartCoroutine(Fade());
    }

    public void ResetFade()
    {
        float offset = (_rectTransform.rect.height / 2) * 1.25f;
        _rectTransform.anchoredPosition = new Vector2(0, offset);
    }

    private IEnumerator Fade()
    {
        float offset = _rectTransform.anchoredPosition.y;
        float steps = Mathf.Floor(duration / timeSteps);
        float offsetChangeSteps = offset / steps;

        for (int i = 0; i < steps; i++)
        {
            _rectTransform.anchoredPosition -= new Vector2(0, offsetChangeSteps);
            yield return new WaitForSecondsRealtime(timeSteps);
        }

        yield return null;
    }
}
