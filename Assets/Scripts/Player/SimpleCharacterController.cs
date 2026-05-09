using Synty.AnimationBaseLocomotion.Samples.InputSystem;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int _isJumpingAnimHash = Animator.StringToHash("IsJumping");
    private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int _currentGaitHash = Animator.StringToHash("CurrentGait");
    private readonly int _strafeDirectionXHash = Animator.StringToHash("StrafeDirectionX");
    private readonly int _strafeDirectionZHash = Animator.StringToHash("StrafeDirectionZ");
    private readonly int _isStrafingHash = Animator.StringToHash("IsStrafing");
    private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int _isStoppedHash = Animator.StringToHash("IsStopped");
    private readonly int _movementInputHeldHash = Animator.StringToHash("MovementInputHeld");

    [Header("Components")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private InputReaderExtension _inputReaderExtension;
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _modelTransform;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _acceleration = 25f;
    [SerializeField] private float _decelerationStop = 30f;
    [SerializeField] private float _decelerationTurn = 60f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _jumpForceRepeated = 8f;
    [SerializeField] private float _gravityMultiplier = 2f;
    [SerializeField] private float _lowJumpMultiplier = 4f;
    [SerializeField] private int _maxJumps = 2;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundedOffset = -0.14f;

    private Vector3 _velocity;
    private bool _isGrounded = true;
    private float _speed2D;
    private float _currentSpeed;
    private float _lastMoveSign;
    private Vector3 _moveDirection;
    private int _currentGait;
    private float _strafeDirectionX = 0f;
    private float _strafeDirectionZ = 1f;
    private bool _isWalking = false;
    private bool _isStopped = true;
    private bool _movementInputHeld = false;
    private int _jumpsRemaining;

    private void Start()
    {
        _inputReader.onJumpPerformed += OnJump;
    }

    private void Update()
    {
        GroundedCheck();
        CalculateMovement();
        CheckIfStopped();
        // FaceMoveDirection();
        ApplyGravity();
        Move();
        UpdateAnimator();
    }

    private void CalculateMovement()
    {
        // визначаємо напрямок руху
        _moveDirection = new Vector3(_inputReader._moveComposite.x, 0f, 0f);
        // визначаємо чи рухаємося
        _movementInputHeld = _moveDirection.magnitude > 0.01f;

        // кешуємо знак руху
        float inputSign = _movementInputHeld ? Mathf.Sign(_moveDirection.x) : 0f;
        // визначаємо чи змінюємо напрямок
        bool isChangingDirection = _movementInputHeld
            && _currentSpeed > 0.01f
            && _lastMoveSign != 0f
            && inputSign != _lastMoveSign;

        float targetSpeed;
        float rateOfAcceleration;
        
        // якщо змінюємо напрямок, то застосовуємо уповільнення для розвороту
        if (isChangingDirection)
        {
            targetSpeed = 0f;
            rateOfAcceleration = _decelerationTurn;
        }
        // якщо рухаємося без зміни напрямку, то застосовуємо прискорення
        else if (_movementInputHeld)
        {
            targetSpeed = _moveSpeed;
            rateOfAcceleration = _acceleration;
            _lastMoveSign = inputSign;
        }
        // якщо звершуємо рух, то застосовуємо уповільнення для зупинки
        else
        {
            targetSpeed = 0f;
            rateOfAcceleration = _decelerationStop;
        }

        // основний розрахунок швидкості з урахуванням прискорення(уповільнення)
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rateOfAcceleration * Time.deltaTime);

        // якщо змінюємо напрямок, то застосовуємо уповільнення для розвороту
        if (isChangingDirection && _currentSpeed < 0.01f)
        {
            _lastMoveSign = inputSign;
        }

        // визначаємо швидкість руху, з урахуванням знаку напрямку
        _velocity.x = _lastMoveSign * _currentSpeed;
        _velocity.z = 0f;

        // округляємо швидкість до 3 знаків після коми, для виключення похибки округлення
        _speed2D = Mathf.Round(_currentSpeed * 1000f) / 1000f;

        // визначаємо тип руху (idle, walk, run)
        CalculateGait();
    }

    private void CalculateGait()
    {
        if (_speed2D < 0.01f)
        {
            _currentGait = 0; // Idle
        }
        else if (_speed2D < _moveSpeed * 0.5f)
        {
            _currentGait = 1; // Walk
        }
        else
        {
            _currentGait = 2; // Run
        }
    }

    private void CheckIfStopped()
    {
        _isStopped = _moveDirection.magnitude == 0 && _speed2D < 0.5f;
        _isWalking = !_isStopped && _isGrounded;
    }

    private void FaceMoveDirection()
    {
        if (_modelTransform == null)
            return;

        if (_moveDirection.magnitude > 0.01f)
        {
            Vector3 faceDirection = new Vector3(_velocity.x, 0f, 0f);
            if (faceDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(faceDirection);
                _modelTransform.rotation = Quaternion.Slerp(_modelTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void OnJump()
    {
        // якщо немає доступних стрибків — ігноруємо
        if (_jumpsRemaining <= 0)
            return;

        // визначаємо силу стрибка: перший стрибок — повна сила, наступні — зменшена
        float actualJumpForce = _jumpsRemaining == _maxJumps ? _jumpForce : _jumpForceRepeated;

        // виконуємо стрибок
        _velocity.y = actualJumpForce;
        _animator.SetBool(_isJumpingAnimHash, true);

        // зменшуємо лічильник доступних стрибків
        _jumpsRemaining--;
    }

    private void ApplyGravity()
    {
        // якщо персонаж підіймається, але кнопку стрибка вже відпустили — посилена гравітація (короткий стрибок)
        if (_velocity.y > 0f && !_inputReaderExtension.isJumpHeld)
        {
            _velocity.y += Physics.gravity.y * _lowJumpMultiplier * Time.deltaTime;
        }
        // інакше — стандартна гравітація (повний стрибок або падіння)
        else if (_velocity.y > Physics.gravity.y)
        {
            _velocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
        }

        // скидаємо анімацію стрибка коли персонаж починає падати
        if (_velocity.y <= 0f)
        {
            _animator.SetBool(_isJumpingAnimHash, false);
        }
    }

    private void Move()
    {
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(
            _controller.transform.position.x,
            _controller.transform.position.y - _groundedOffset,
            _controller.transform.position.z
        );
        _isGrounded = Physics.CheckSphere(spherePosition, _controller.radius, _groundLayerMask, QueryTriggerInteraction.Ignore);

        // скидаємо лічильник стрибків коли персонаж на землі і не підіймається
        if (_isGrounded && _velocity.y <= 0f)
        {
            _jumpsRemaining = _maxJumps;
        }
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat(_moveSpeedHash, _speed2D);
        _animator.SetInteger(_currentGaitHash, _currentGait);
        _animator.SetBool(_isGroundedHash, _isGrounded);
        _animator.SetFloat(_strafeDirectionXHash, _strafeDirectionX);
        _animator.SetFloat(_strafeDirectionZHash, _strafeDirectionZ);
        _animator.SetFloat(_isStrafingHash, 0f);
        _animator.SetBool(_isWalkingHash, _isWalking);
        _animator.SetBool(_isStoppedHash, _isStopped);
        _animator.SetBool(_movementInputHeldHash, _movementInputHeld);
    }

    private void OnDestroy()
    {
        _inputReader.onJumpPerformed -= OnJump;
    }
}
