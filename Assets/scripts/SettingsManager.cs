using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public GameObject settingsPanel;

    private float currentVolume = 1f;

    private void Start()
    {
        // Cargar volumen guardado (si existe)
        currentVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        
        if (volumeSlider != null)
        {
            volumeSlider.value = currentVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Ocultar al inicio
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void SetVolume(float value)
    {
        currentVolume = value;
        AudioListener.volume = value;        // Controla el volumen general del juego
        
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
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