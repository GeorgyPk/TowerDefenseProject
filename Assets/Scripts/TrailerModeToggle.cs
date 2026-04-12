using UnityEngine;
using UnityEngine.InputSystem;

public class TrailerModeToggle : MonoBehaviour
{
    public GameObject gameplayCamera;
    public GameObject trailerCamera;
    public GameObject gameplayUI;

    public Key toggleKey = Key.T;

    private bool trailerMode = false;

    private void Start()
    {
        ApplyMode();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            trailerMode = !trailerMode;
            ApplyMode();
        }
    }

    private void ApplyMode()
    {
        if (gameplayCamera != null)
            gameplayCamera.SetActive(!trailerMode);

        if (trailerCamera != null)
            trailerCamera.SetActive(trailerMode);

        if (gameplayUI != null)
            gameplayUI.SetActive(!trailerMode);
    }
}