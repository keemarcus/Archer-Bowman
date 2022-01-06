using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    Text highScore;
    Text confirmTitle;
    public GameObject mainMenuCanvas;
    public GameObject optionsMenuCanvas;
    public GameObject optionsMenu;
    public GameObject info;
    public GameObject confirmMenu;
    public AudioSource musicSource;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider inputMode;
    
    // Start is called before the first frame update
    void Start()
    {
        SetUpMainMenu();
        SetVolumes();
    }
    public void playGame() {
        SceneManager.LoadScene("Main");
    }
 
    public void options() {
        // disable the main menu
        mainMenuCanvas.SetActive(false);

        // make sure the confirm menu is disabled
        confirmMenu.SetActive(false);

        // make sure the info menu is disabled
        info.SetActive(false);

        // enable the options menu
        optionsMenuCanvas.SetActive(true);
        optionsMenu.SetActive(true);
    }
 
    public void exitGame() {
        Application.Quit();
    }

    public void AskToConfirm(string message) {
        // disable the options menu
        optionsMenu.SetActive(false);

        // make sure the info menu is disabled
        info.SetActive(false);

        // enable the confirm menu with the supplied message
        confirmMenu.SetActive(true);
        // get the title object
        Text[] textObjects = FindObjectsOfType<Text>();
        foreach(Text x in textObjects){
            if(x.name == "TitleText"){confirmTitle = x;}
        }
        confirmTitle.text = message;
    }

    public void Confirm(){
        // check to see what we were confirming
        if(confirmTitle.text.Contains("High Score")){
            // reset the high score
            PlayerPrefs.SetInt("highScore", 0);
            PlayerPrefs.Save();
        }
        // go back to options menu
        options();
    }

    public void Deny(){
        // go back to options menu
        options();
    }

    public void SetUpMainMenu(){
        // make sure the main menu is active
        optionsMenuCanvas.SetActive(false);
        optionsMenu.SetActive(false);
        confirmMenu.SetActive(false);
        info.SetActive(false);
        mainMenuCanvas.SetActive(true);

        // get the score counter
        Text[] textObjects = FindObjectsOfType<Text>();
        foreach(Text x in textObjects){
            if(x.name == "HighScore"){highScore = x;}
        }
        if(int.Parse(highScore.text.Split(':')[1]) != PlayerPrefs.GetInt("highScore")){
            highScore.text = highScore.text.Replace(highScore.text.Split(':')[1]," " + PlayerPrefs.GetInt("highScore"));
        }
        
    }
    public void ShowInfo(){
        // make sure the main menu is active
        optionsMenuCanvas.SetActive(true);
        optionsMenu.SetActive(false);
        confirmMenu.SetActive(false);
        mainMenuCanvas.SetActive(false);
        info.SetActive(true);
    }
    public void UpdateMusicVolume(){
        PlayerPrefs.SetInt("musicVol", (int)musicVolume.value);
        PlayerPrefs.Save();
        SetVolumes();
    }
    public void UpdateSFXVolume(){
        PlayerPrefs.SetInt("sfxVol", (int)sfxVolume.value);
        PlayerPrefs.Save();
        SetVolumes();
    }
    public void UpdateInputMode(){
        PlayerPrefs.SetInt("inputMode", (int)inputMode.value);
        PlayerPrefs.Save();
        SetControllerMode();
    }
    void SetVolumes(){
        // update the music volume
        musicSource.volume = PlayerPrefs.GetInt("musicVol") * .025f;

        // update the volume sliders
        musicVolume.value = PlayerPrefs.GetInt("musicVol");
        sfxVolume.value = PlayerPrefs.GetInt("sfxVol");
    }
    void SetControllerMode(){
        // update the input mode slider
        inputMode.value = PlayerPrefs.GetInt("inputMode");
    }
}
