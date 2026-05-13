using UnityEngine;

// виконується у фазі LateUpdate ПІСЛЯ камери (ExecutionOrder 0).
// це гарантує, що ми читаємо вже оновлену позицію камери в цьому ж кадрі,
// інакше parallax міг би відставати на 1 кадр (тремтіння на швидкому русі).
[DefaultExecutionOrder(100)]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Камера, за позицією якої рухається фон")]
    [SerializeField] private Transform _camera;

    [Header("Parallax")]
    [Tooltip("Значення зсуву по X")]
    [SerializeField, Range(0f, 1f)] private float _parallaxFactorX = 1f;
    [Tooltip("Значення зсуву по Y")]
    [SerializeField, Range(0f, 1f)] private float _parallaxFactorY = 0f;

    private Vector3 _initialBackgroundPos;
    private Vector3 _initialCameraPos;

    private void Start()
    {
        // явна перевірка, щоб одразу побачити помилку
        if (_camera == null)
        {
            Debug.LogError($"[ParallaxBackground] '{name}' has no camera. Please assign the camera in the Inspector.", this);
            enabled = false;
            return;
        }

        // запам'ятовуємо стартові позиції
        _initialBackgroundPos = transform.position;
        _initialCameraPos = _camera.position;
    }

    private void LateUpdate()
    {
        // наскільки камера змістилась відносно своєї стартової позиції
        Vector3 cameraDelta = _camera.position - _initialCameraPos;

        // зміщєння фону пропорційно дельті; чим менший factor, тим менше рухається фон
        Vector3 newPos = _initialBackgroundPos;
        newPos.x += cameraDelta.x * _parallaxFactorX;
        newPos.y += cameraDelta.y * _parallaxFactorY;
        transform.position = newPos;
    }
}
