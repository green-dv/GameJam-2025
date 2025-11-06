using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> ops = new List<string>();
        int currentResolution = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string op = resolutions[i].width + " x " + resolutions[i].height;
            ops.Add(op);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }
        resolutionDropdown.AddOptions(ops);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution (int resolutionI)
    {
        Resolution resolution = resolutions[resolutionI];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
