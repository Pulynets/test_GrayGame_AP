using UnityEngine;
using DG.Tweening;

public class BonusBehavior : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float _riseDuration = 1f;
    [SerializeField] private float _fallDuration = 1.5f;

    [Header("Trajectory")]
    [SerializeField] private float _riseHeight = 1.5f;
    [SerializeField] private float _fallBack = 1f;

    [Header("Easing")]
    [SerializeField] private Ease _riseEase = Ease.OutQuad;
    [SerializeField] private Ease _fallEase = Ease.InQuad;

    [Header("Effects")]
    [SerializeField] private GameObject _prefabVFX;
    [SerializeField, Min(0f)] private float _delayVFX = 0f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField, Min(0f)] private float _delaySFX = 0f;

    private GameObject _spawnedVfx;

    private void Start()
    {
        PlayBonusAnimation();

        // VFX/SFX незалежні від анімації
        Invoke(nameof(PlaySfx), _delaySFX);
        Invoke(nameof(SpawnVfx), _delayVFX);
    }

    private void LateUpdate()
    {
        // оновлюємо у LateUpdate щоб VFX слідував за бонусом
        if (_spawnedVfx != null)
        {
            _spawnedVfx.transform.position = transform.position;
        }
    }

    private void PlayBonusAnimation()
    {
        float startY = transform.position.y;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(startY + _riseHeight, _riseDuration).SetEase(_riseEase));
        seq.Append(transform.DOMoveY(startY + _riseHeight - _fallBack, _fallDuration).SetEase(_fallEase));
        seq.OnComplete(() => Destroy(gameObject));
    }

    private void PlaySfx()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }

    private void SpawnVfx()
    {
        if (_prefabVFX == null)
        {
            return;
        }

        // копіюємо лише позицію батьківського об'єкта для VFX
        _spawnedVfx = Instantiate(_prefabVFX, transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        // вбиваємо всі активні твіни на цьому Transform, щоб не залишити callback на знищеному об'єкті
        transform.DOKill();
    }
}
