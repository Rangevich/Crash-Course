using System;
using UnityEngine;

public class CustomCamera : MonoBehaviour
{
    public Transform _car;
    [SerializeField] private float _followDistance = 6f; // Расстояние до машины
    [SerializeField] private float _cameraHeight = 2f;   // Высота камеры над машиной
    [SerializeField] private float _followSpeed = 10f;   // Скорость следования за машиной

    private Vector3 _offset;

    private void Start()
    {
        // Настраиваем смещение камеры
        _offset = new Vector3(0, _cameraHeight, -_followDistance);
    }

    private void FixedUpdate()
    {
        if (_car)
        {
            // Рассчитываем целевую позицию камеры с сохранением высоты Y
            Vector3 targetPosition = _car.position + _offset;
            targetPosition.y = _cameraHeight; // Фиксируем высоту камеры

            // Плавно перемещаем камеру к целевой позиции
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            // Направляем камеру на машину
            transform.LookAt(_car);
        }
    }
}
