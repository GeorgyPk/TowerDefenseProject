using UnityEngine;

public class ResetPrefsOnce : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.DeleteKey("BestEnemiesKilled");
        PlayerPrefs.Save();
        Debug.Log("BestEnemiesKilled reset.");
    }
}