using DG.Tweening;
using UnityEngine;

public class SizeAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float targetScale = 1.2f;
    [SerializeField]
    [Range(0f,2f)] private float startCountdown = 0f;
    [SerializeField]
    [Range(0f, 4f)] private float tweenTimer = 1f;
    private Tween sizeTween;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SizeAnimationLoop",startCountdown);
    }

    private void SizeAnimationLoop()
    {
        sizeTween = transform.DOScale(targetScale, tweenTimer).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        sizeTween?.Kill();
    }
}
