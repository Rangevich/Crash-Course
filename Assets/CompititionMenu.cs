using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompititionMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup MainCanvas;
    [SerializeField] private CanvasGroup CompititionCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowCompititionMenu()
    {
        MainCanvas.Disable();
        CompititionCanvas.Enable();
    }

    public void CloseCompititionMenu()
    {
        CompititionCanvas.Disable();
        MainCanvas.Enable();
    }

    public void CreateRoom()
    {
        SceneManager.LoadScene("SoloGameScene");
        StartCoroutine(Connect(true));
    }

    public void JoinRoom()
    {
        SceneManager.LoadScene("SoloGameScene");
        StartCoroutine(Connect(false));
    }
    
    IEnumerator Connect(bool server)
    {
        yield return new WaitUntil(() => NetMan.instance != null);
        if (server)
        {
            NetMan.instance.StartHost();
            Debug.Log("Created host");
        }
        else
        {
            NetMan.instance.StartClient();
            Debug.Log("Joined client");
        }
    }
}

public static class MyExtensions
{
    public static void Disable(this CanvasGroup group)
    {
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public static void Enable(this CanvasGroup group)
    {
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
}

