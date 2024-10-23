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
    [SerializeField] private float _force;        // ���� ���� ��� �������� ������ � �����
    [SerializeField] private float _maxSteerAngle; // ������������ ���� �������� ����
    [SerializeField] private float _accelerationSpeed = 100f; // �������� ��������� ������
    [SerializeField] private float _decelerationSpeed = 200f; // �������� ����������
    [SerializeField] private float _brakeForce = 5000f; // ���� ����������
    [SerializeField] private float _initialBrakeForce = 20000000f; // ���� ���������� ��� �����������
    [SerializeField] private Transform _startPoint; // ����� ������
    [SerializeField] private GameObject _finishBanner; // ������ � ��������
    [SerializeField] private Button _restartButton; // ������ �����������
    [SerializeField] private Canvas _controlCanvas; // ������ � �������� ����������

    private bool _isAccelerating = false;  // ���� ��� �������� �����
    private bool _isReversing = false;     // ���� ��� �������� �����
    private bool _isSteeringRight = false; // ���� ��� �������� ������
    private bool _isSteeringLeft = false;  // ���� ��� �������� �����

    private float _currentForce = 0f; // ������� ���� ����
    private float _stuckTime = 0f;
    private float _maxStuckTime = 4f;

    // ������ ��� ������ �������� ����� � �����
    public void OnAcceleratePressed() { _isAccelerating = true; }
    public void OnAccelerateReleased() { _isAccelerating = false; }
    public void OnReversePressed() { _isReversing = true; }
    public void OnReverseReleased() { _isReversing = false; }

    // ������ ��� ������ �������� ������ � �����
    public void OnRightPressed() { _isSteeringRight = true; }
    public void OnRightReleased() { _isSteeringRight = false; }
    public void OnLeftPressed() { _isSteeringLeft = true; }
    public void OnLeftReleased() { _isSteeringLeft = false; }

    private void Start()
    {
        ResetToStart();
        _finishBanner.SetActive(false); // �������� ������ � ������
        _restartButton.onClick.AddListener(RestartGame); // ���������� ������ � ������
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
            SetBrakeTorque(_brakeForce); // ��������� ���������� ��� ���������� ����������
        }

        // ������������� �������� �������� ������ �� ��� �����
        _colliderFL.motorTorque = _currentForce;
        _colliderFR.motorTorque = _currentForce;
        _colliderBL.motorTorque = _currentForce;
        _colliderBR.motorTorque = _currentForce;

        // ������� ������ � �����
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

        // ���������� �������� � ���� ������
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep(); // ������������� ������������� ������
        }

        // ���������� ��������� ����������
        _currentForce = 0f;
        _isAccelerating = false;
        _isReversing = false;
        _isSteeringRight = false;
        _isSteeringLeft = false;

        // ���������� �������� �������� ������, ���������� � ���� �������� ����
        _colliderFL.motorTorque = 0f;
        _colliderFR.motorTorque = 0f;
        _colliderBL.motorTorque = 0f;
        _colliderBR.motorTorque = 0f;
        SetBrakeTorque(_initialBrakeForce); // ��������� ����� ������� ���������� ��� �����������
        _colliderFL.steerAngle = 0f;
        _colliderFR.steerAngle = 0f;

        // ����� ��������� �������� ������� ��������� ������� ����������
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
        else if (other.CompareTag("Finish")) // ���������, ��������� �� �����
        {
            ShowFinishBanner(); // ���������� ������ ������
        }
    }

    private void ShowFinishBanner()
    {
        _finishBanner.SetActive(true); // ���������� ������
        _controlCanvas.enabled = false; // �������� ������ � �������� ����������
    }

    private void RestartGame()
    {
        _finishBanner.SetActive(false); // �������� ������
        _controlCanvas.enabled = true; // ���������� ������ � �����������
        ResetToStart(); // ���������� ������ �� �����
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