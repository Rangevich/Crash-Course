using System;
using UnityEngine;

public class CustomCamera : MonoBehaviour
{
    public Transform _car;
    [SerializeField] private float _followDistance = 6f; // ���������� �� ������
    [SerializeField] private float _cameraHeight = 2f;   // ������ ������ ��� �������
    [SerializeField] private float _followSpeed = 10f;   // �������� ���������� �� �������

    private Vector3 _offset;

    private void Start()
    {
        // ����������� �������� ������
        _offset = new Vector3(0, _cameraHeight, -_followDistance);
    }

    private void FixedUpdate()
    {
        if (_car)
        {
            // ������������ ������� ������� ������ � ����������� ������ Y
            Vector3 targetPosition = _car.position + _offset;
            targetPosition.y = _cameraHeight; // ��������� ������ ������

            // ������ ���������� ������ � ������� �������
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            // ���������� ������ �� ������
            transform.LookAt(_car);
        }
    }
}
