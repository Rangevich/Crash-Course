using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NetMan : NetworkManager
{
    public Transform[] StartPoints; // Точки старта
    [NonSerialized] public static int currentStartPointIndex = -1;
    
    [SerializeField] private GameObject _finishBanner; // Панель с баннером
    [SerializeField] private Button _restartButton; // Кнопка перезапуска
    [SerializeField] private Canvas _controlCanvas; // Канвас с кнопками управления
    public static NetMan instance;
    public CarController player;

    public Action OnRestartGame;
    
    // Методы для кнопок движения вперёд и назад
    public void OnAcceleratePressed() { player._isAccelerating = true; }
    public void OnAccelerateReleased() { player._isAccelerating = false; }
    public void OnReversePressed() { player._isReversing = true; }
    public void OnReverseReleased() { player._isReversing = false; }

    // Методы для кнопок поворота вправо и влево
    public void OnRightPressed() { player._isSteeringRight = true; }
    public void OnRightReleased() { player._isSteeringRight = false; }
    public void OnLeftPressed() { player._isSteeringLeft = true; }
    public void OnLeftReleased() { player._isSteeringLeft = false; }
    private void Start()
    {
        instance = this;
        _finishBanner.SetActive(false); // Скрываем баннер в начале
        _restartButton.onClick.AddListener(RestartGame); // Подключаем кнопку к методу
    }
    
    public void ShowFinishBanner()
    {
        _finishBanner.SetActive(true); // Показываем баннер
        _controlCanvas.enabled = false; // Скрываем канвас с кнопками управления
    }

    private void RestartGame()
    {
        _finishBanner.SetActive(false); // Скрываем баннер
        _controlCanvas.enabled = true; // Показываем канвас с управлением
        OnRestartGame?.Invoke();
    }
}