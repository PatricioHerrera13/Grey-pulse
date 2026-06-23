using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    private void Start()
    {
        // Aseguramos que el panel empiece cerrado
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Greypulse");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}