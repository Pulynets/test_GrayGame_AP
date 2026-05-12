using UnityEngine;

public class SimpleCharacterCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _facingSource;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private Vector3 _lookAtOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Look Ahead")]
    [SerializeField] private float _lookAheadDistance = 3f;
    [SerializeField] private float _lookAheadSmoothSpeed = 2f;

    [Header("Bounds")]
    [SerializeField] private float _minX = 0f;
    [SerializeField] private float _maxX = 10f;
    [SerializeField] private float _minY = 0f;
    [SerializeField] private float _maxY = 10f;

    private float _currentFacingX;


    void LateUpdate()
    {
        if (_target == null)
            return;

        // визначаємо джерело напрямку (модель гравця); якщо не задане або помилково вказана сама камера — fallback на _target
        Transform facingSource = (_facingSource != null && _facingSource != transform) ? _facingSource : _target;

        // цільовий знак напрямку по X (від -1 до 1) на основі forward вектора моделі
        float targetFacingX = Mathf.Clamp(facingSource.forward.x, -1f, 1f);

        // плавно інтерполюємо поточний напрямок до цільового — щоб камера не "смикалась" при різкій зміні / тапанні клавіш
        _currentFacingX = Mathf.Lerp(_currentFacingX, targetFacingX, _lookAheadSmoothSpeed * Time.deltaTime);

        // зсув камери у напрямку погляду персонажа; однаковий зсув для позиції і LookAt дає 2/3 попереду і 1/3 позаду без зміни кута огляду
        Vector3 lookAheadShift = new Vector3(_currentFacingX * _lookAheadDistance, 0f, 0f);

        // цільова позиція камери = персонаж + базовий offset + look-ahead зсув
        Vector3 targetPosition = _target.position + _offset + lookAheadShift;

        // обмежуємо позицію камери у межах рівня по обох осях
        targetPosition.x = Mathf.Clamp(targetPosition.x, _minX, _maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, _minY, _maxY);

        // плавно рухаємо камеру до цільової позиції
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // точка погляду = цільова позиція камери + сталий зсув; завдяки цьому ракурс не змінюється навіть коли камера clamp-нулася на межі рівня
        transform.LookAt(targetPosition + (_lookAtOffset - _offset));
    }

    // малюємо прямокутник меж камери в Scene View для зручної візуального налаштування
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 bl = new Vector3(_minX, _minY, 0f);
        Vector3 br = new Vector3(_maxX, _minY, 0f);
        Vector3 tr = new Vector3(_maxX, _maxY, 0f);
        Vector3 tl = new Vector3(_minX, _maxY, 0f);
        Gizmos.DrawLine(bl, br);
        Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(tl, bl);
    }
}
