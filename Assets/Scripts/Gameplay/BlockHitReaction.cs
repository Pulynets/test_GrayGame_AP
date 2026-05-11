using UnityEngine;

public class BlockHitReaction : MonoBehaviour
{
    [Header("Bounce")]
    [SerializeField] private bool _isBouncy = false;
    [SerializeField] private float _bounceDistance = 0.15f;
    [SerializeField] private float _bounceDuration = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioClip _hitSound;

    private Vector3 _originalPosition;
    private float _bounceTimer;

    private void Start()
    {
        _originalPosition = transform.localPosition;
        // вимикаємо Update одразу після створення
        enabled = false;
    }

    private void Update()
    {
        _bounceTimer += Time.deltaTime;
        float progress = _bounceTimer / _bounceDuration;

        // анімація завершена — повертаємо блок на місце і вимикаємо Update
        if (progress >= 1f)
        {
            transform.localPosition = _originalPosition;
            enabled = false;
            return;
        }

        // анімація по синусоїді: вгору і назад
        float offset = Mathf.Sin(progress * Mathf.PI) * _bounceDistance;
        transform.localPosition = _originalPosition + Vector3.up * offset;
    }

    public void OnHitFromBelow()
    {
        // ігноруємо повторні удари поки анімація в процесі
        if (enabled)
            return;

        // звук відтворюється завжди (якщо призначений)
        if (_hitSound != null)
        {
            AudioSource.PlayClipAtPoint(_hitSound, transform.position);
        }

        // відскок тільки якщо блок позначений як bouncy
        if (_isBouncy)
        {
            _bounceTimer = 0f;
            enabled = true;
        }
    }
}
