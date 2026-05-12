using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelResetController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _fallY = -10f;

    [Header("Input")]
    [SerializeField] private Key _resetKey = Key.R;

    private void Update()
    {
        // ручний ресет з клавіатури
        if (Keyboard.current != null && Keyboard.current[_resetKey].wasPressedThisFrame)
        {
            ResetLevel();
            return;
        }

        // авто-ресет при падінні нижче порогу
        if (_playerTransform != null && _playerTransform.position.y < _fallY)
        {
            ResetLevel();
        }
    }

    // публічний метод для UI Button.OnClick() і для виклику з коду
    public void ResetLevel()
    {
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex);
    }
}
