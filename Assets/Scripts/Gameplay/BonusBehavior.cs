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
    [SerializeField] private Ease _riseEase = Ease.OutQuad;
    [SerializeField] private Ease _fallEase = Ease.InQuad;

    [Header("Rotation")]
    [SerializeField] private Vector3 _rotation = new Vector3(0f, 360f, 0f);
    [SerializeField] private Ease _rotationEase = Ease.Linear;

    [Header("Scale")]
    [SerializeField] private float _scaleStart = 0f;
    [SerializeField] private float _scaleMAX = 1f;
    [SerializeField] private float _scaleEnd = 0f;
    [SerializeField] private Ease _riseScaleEase = Ease.OutQuad;
    [SerializeField] private Ease _fallScaleEase = Ease.InQuad;

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

        // задаємо початковий scale одразу
        transform.localScale = Vector3.one * _scaleStart;

        Sequence seq = DOTween.Sequence();
        // фаза підйому: позиція + scale паралельно
        seq.Append(transform.DOMoveY(startY + _riseHeight, _riseDuration).SetEase(_riseEase));
        seq.Join(transform.DOScale(_scaleMAX, _riseDuration).SetEase(_riseScaleEase));
        // фаза падіння: позиція + scale паралельно
        seq.Append(transform.DOMoveY(startY + _riseHeight - _fallBack, _fallDuration).SetEase(_fallEase));
        seq.Join(transform.DOScale(_scaleEnd, _fallDuration).SetEase(_fallScaleEase));
        // обертання — паралельно з усією анімацією (вставка в момент 0 на тривалість rise+fall)
        seq.Insert(0f, transform.DORotate(_rotation, _riseDuration + _fallDuration, RotateMode.LocalAxisAdd).SetEase(_rotationEase));

        seq.OnComplete(() => Destroy(gameObject));
        // прив'язуємо Sequence до gameObject: при знищенні об'єкту (наприклад, через рестарт сцени)
        // DOTween автоматично вбиває Sequence ДО того, як вона викличе callback на null target/MonoBehaviour
        seq.SetLink(gameObject);
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
