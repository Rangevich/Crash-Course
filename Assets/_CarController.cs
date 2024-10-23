using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;
    [SerializeField] private float _force;        // Сила тяги для движения вперед и назад
    [SerializeField] private float _maxSteerAngle; // Максимальный угол поворота колёс
    [SerializeField] private float _accelerationSpeed = 100f; // Скорость ускорения машины
    [SerializeField] private float _decelerationSpeed = 200f; // Скорость торможения
    [SerializeField] private float _brakeForce = 5000f; // Сила торможения
    [SerializeField] private float _initialBrakeForce = 20000000f; // Сила торможения при возрождении
    [SerializeField] private Transform _startPoint; // Точка старта
    [SerializeField] private GameObject _finishBanner; // Панель с баннером
    [SerializeField] private Button _restartButton; // Кнопка перезапуска
    [SerializeField] private Canvas _controlCanvas; // Канвас с кнопками управления

    private bool _isAccelerating = false;  // Флаг для движения вперёд
    private bool _isReversing = false;     // Флаг для движения назад
    private bool _isSteeringRight = false; // Флаг для поворота вправо
    private bool _isSteeringLeft = false;  // Флаг для поворота влево

    private float _currentForce = 0f; // Текущая сила тяги
    private float _stuckTime = 0f;
    private float _maxStuckTime = 4f;

    // Методы для кнопок движения вперёд и назад
    public void OnAcceleratePressed() { _isAccelerating = true; }
    public void OnAccelerateReleased() { _isAccelerating = false; }
    public void OnReversePressed() { _isReversing = true; }
    public void OnReverseReleased() { _isReversing = false; }

    // Методы для кнопок поворота вправо и влево
    public void OnRightPressed() { _isSteeringRight = true; }
    public void OnRightReleased() { _isSteeringRight = false; }
    public void OnLeftPressed() { _isSteeringLeft = true; }
    public void OnLeftReleased() { _isSteeringLeft = false; }

    private void Start()
    {
        ResetToStart();
        _finishBanner.SetActive(false); // Скрываем баннер в начале
        _restartButton.onClick.AddListener(RestartGame); // Подключаем кнопку к методу
    }

    private void FixedUpdate()
    {
        if (_isAccelerating)
        {
            _currentForce = Mathf.Clamp(_currentForce + _accelerationSpeed * Time.deltaTime, 0, _force);
            SetBrakeTorque(0f);
        }
        else if (_isReversing)
        {
            _currentForce = Mathf.Clamp(_currentForce - _accelerationSpeed * Time.deltaTime, -_force, 0);
            SetBrakeTorque(0f);
        }
        else
        {
            _currentForce = 0f;
            SetBrakeTorque(_brakeForce); // Применяем торможение при отсутствии управления
        }

        // Устанавливаем моторный крутящий момент на все колёса
        _colliderFL.motorTorque = _currentForce;
        _colliderFR.motorTorque = _currentForce;
        _colliderBL.motorTorque = _currentForce;
        _colliderBR.motorTorque = _currentForce;

        // Поворот вправо и влево
        if (_isSteeringRight)
        {
            _colliderFL.steerAngle = _maxSteerAngle;
            _colliderFR.steerAngle = _maxSteerAngle;
        }
        else if (_isSteeringLeft)
        {
            _colliderFL.steerAngle = -_maxSteerAngle;
            _colliderFR.steerAngle = -_maxSteerAngle;
        }
        else
        {
            _colliderFL.steerAngle = 0f;
            _colliderFR.steerAngle = 0f;
        }

        CheckForStuck();
    }

    private void SetBrakeTorque(float brakeTorque)
    {
        _colliderFL.brakeTorque = brakeTorque;
        _colliderFR.brakeTorque = brakeTorque;
        _colliderBL.brakeTorque = brakeTorque;
        _colliderBR.brakeTorque = brakeTorque;
    }

    private void ResetToStart()
    {
        transform.position = _startPoint.position;
        transform.rotation = _startPoint.rotation;

        // Сбрасываем скорость и силы машины
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep(); // Принудительно останавливаем физику
        }

        // Сбрасываем параметры управления
        _currentForce = 0f;
        _isAccelerating = false;
        _isReversing = false;
        _isSteeringRight = false;
        _isSteeringLeft = false;

        // Сбрасываем моторный крутящий момент, торможение и угол поворота колёс
        _colliderFL.motorTorque = 0f;
        _colliderFR.motorTorque = 0f;
        _colliderBL.motorTorque = 0f;
        _colliderBR.motorTorque = 0f;
        SetBrakeTorque(_initialBrakeForce); // Применяем очень сильное торможение при возрождении
        _colliderFL.steerAngle = 0f;
        _colliderFR.steerAngle = 0f;

        // Через небольшую задержку снимаем начальное сильное торможение
        StartCoroutine(RemoveInitialBrakeForce());
    }

    private IEnumerator RemoveInitialBrakeForce()
    {
        yield return new WaitForSeconds(0.1f);
        SetBrakeTorque(_brakeForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LoseZone"))
        {
            ResetToStart();
        }
        else if (other.CompareTag("Finish")) // Проверяем, пересекли ли финиш
        {
            ShowFinishBanner(); // Показываем баннер победы
        }
    }

    private void ShowFinishBanner()
    {
        _finishBanner.SetActive(true); // Показываем баннер
        _controlCanvas.enabled = false; // Скрываем канвас с кнопками управления
    }

    private void RestartGame()
    {
        _finishBanner.SetActive(false); // Скрываем баннер
        _controlCanvas.enabled = true; // Показываем канвас с управлением
        ResetToStart(); // Перемещаем машину на старт
    }

    private void CheckForStuck()
    {
        if (Mathf.Abs(transform.rotation.eulerAngles.z) > 1f)
        {
            _stuckTime += Time.deltaTime;
            if (_stuckTime >= _maxStuckTime)
            {
                ResetToStart();
                _stuckTime = 0f;
            }
        }
        else
        {
            _stuckTime = 0f;
        }
    }
}