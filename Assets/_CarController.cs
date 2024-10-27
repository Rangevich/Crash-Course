using System;
using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CarController : NetworkBehaviour
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
    [SerializeField] private Material[] playersMaterials;
    [SerializeField] private MeshRenderer meshRenderer;

    public bool _isAccelerating = false;  // Флаг для движения вперёд
    public bool _isReversing = false;     // Флаг для движения назад
    public bool _isSteeringRight = false; // Флаг для поворота вправо
    public bool _isSteeringLeft = false;  // Флаг для поворота влево

    private float _currentForce = 0f; // Текущая сила тяги
    private float _stuckTime = 0f;
    private float _maxStuckTime = 4f;

    private int playerIndex = -1;

    private void Start()
    {
        var array = GameObject.FindGameObjectsWithTag("Player"); 
        playerIndex = array.Count() - 1;
        meshRenderer.sharedMaterial = playersMaterials[playerIndex];
        if (isOwned)
        {
            ResetToStart();
            Camera.main.GetComponent<CustomCamera>()._car = transform;
            NetMan.instance.OnRestartGame += ResetToStart;
            NetMan.instance.player = this;
        }
    }

    private void FixedUpdate()
    {
        if (isOwned)
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
        transform.position = NetMan.instance.StartPoints[playerIndex].position;
        transform.rotation = NetMan.instance.StartPoints[playerIndex].rotation;

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
        if (isOwned)
        {
            if (other.CompareTag("LoseZone"))
            {
                ResetToStart();
            }
            else if (other.CompareTag("Finish")) // Проверяем, пересекли ли финиш
            {
                NetMan.instance.ShowFinishBanner(); // Показываем баннер победы
            }
        }
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