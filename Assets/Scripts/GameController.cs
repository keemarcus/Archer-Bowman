using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float timeRemaining = 30;
    int timePassed;
    public GameObject target;
    public Player player;
    Text timer;
    Text scoreCounter;
    Slider timeSlider;
    Vector2 topLeftBound;
    Vector2 bottomRightBound;
    int lastTimeSpawned;
    public GameObject inGameUI;
    public GameObject endGameUI;
    public GameObject pauseMenuUI;
    bool paused = false;
    bool inEndGame = false;
    public AudioSource musicSource;
    public AudioClip gameOver;
    public AudioClip highScore;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider inputMode;
    int spawnModifier;
    
    // Start is called before the first frame update
    void Start()
    {
        // make sure that the in game ui is active
        endGameUI.SetActive(false);
        inGameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        inEndGame = false;
        paused = false;

        // get settings from player prefs
        SetVolumes();
        SetControllerMode();
        

        // set initial spawn modifier
        spawnModifier = 3;
        timePassed = 0;

        // set bounds for spawning targets
        topLeftBound = new Vector2(-25,10);
        bottomRightBound = new Vector2(17,-15);

        // get the score counter
        Text[] textObjects = FindObjectsOfType<Text>();
        foreach(Text x in textObjects){
            if(x.name == "Time"){timer = x;}
            else if(x.name == "Score"){scoreCounter = x;}
        }

        // get the time slider
        timeSlider = FindObjectOfType<Slider>();

        // spawn the first target
        SpawnTarget(0);

        // make sure the player is in the right state
        player.currentState = Player.PlayerState.Default;

        // make sure game isn't paused
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused){
            if(Input.GetButtonDown("Pause")){
                Pause();
            }
            
            // update the timer
            string [] timerElements = timer.text.Split(':');
        
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timePassed = (int)(Time.timeSinceLevelLoad);

                if(timePassed > lastTimeSpawned && timePassed % 5 == 0){
                    SpawnTarget(timePassed);
                }
            }
            else
            {
                if(!inEndGame){EndGame();}
            }
            timeSlider.value = timeRemaining;
        } else{
            if(Input.GetButtonDown("Pause")){
                Resume();
            }
        }
    }

    void SpawnTarget(int timeSpawned){
        lastTimeSpawned = timeSpawned;
        SpawnTarget();
        UpdateSpawnRate();
    }
    public void SpawnTarget(){
        // generate random spawn point for new target
        Vector2 spawnPoint = new Vector2(Random.Range(topLeftBound.x, bottomRightBound.x),Random.Range(bottomRightBound.y, topLeftBound.y));

        // insatantiate the new target at that point
        GameObject newTarget = Instantiate(target, (Vector3)spawnPoint, Quaternion.identity);

        // set the type of the new target randomly
        int rand1 = Random.Range(1,7);
        int rand2 = Random.Range(1,7);
        int randTotal = rand1 + rand2;
        if(randTotal <= 2 + spawnModifier){
            newTarget.GetComponent<Target>().UpdateType(1);
        } else if(randTotal >= 12 - spawnModifier){
            newTarget.GetComponent<Target>().UpdateType(2);
        }
    }

    void UpdateSpawnRate(){
        if(timePassed % 20 == 0 && spawnModifier > 0){
            spawnModifier -= 1;
        } 
    }
    void EndGame(){
        // disable player movement
        player.currentState = Player.PlayerState.Paused;

        // stop the music
        AudioSource musicPlayer = FindObjectOfType<Camera>().GetComponentInChildren<AudioSource>();
        musicPlayer.Stop();
        
        // get the score
        string [] scoreElements = scoreCounter.text.Split(':');
        int score = int.Parse(scoreElements[1]);
        string endGameText;
        // compare to current high score
        if(score >  GetHighScore()){
            // display the score
            endGameText = "New High Score! - " + score;

            // update the high score to the new value
            SaveHighScore(score);

            // play the highscore sound effect
            AudioSource.PlayClipAtPoint(highScore, musicSource.transform.position, PlayerPrefs.GetInt("sfxVol") * .1f);
        }
        else{
            // display the score
            endGameText = "Score - " + score;

            // play the game over sound effect
            AudioSource.PlayClipAtPoint(gameOver, musicSource.transform.position, PlayerPrefs.GetInt("sfxVol") * .1f);
        }
        // switch to the endgame ui
        inGameUI.SetActive(false);
        endGameUI.SetActive(true);
        endGameUI.GetComponentInChildren<Text>().text = endGameText;

        inEndGame = true;
    }
    void Pause(){
        // pause the timer
        paused = true;
        Time.timeScale = 0;

        // disable player controls
        player.currentState = Player.PlayerState.Paused;

        // enable the pause ui elements
        pauseMenuUI.SetActive(true);
    }
    public void Resume(){
        // disable the pause ui elements
        pauseMenuUI.SetActive(false);
        
        // reenable player controls
        player.currentState = Player.PlayerState.Default;

        // resume the timer
        paused = false;
        Time.timeScale = 1;
    }
    public void Retry(){
        SceneManager.LoadScene("Main");
    }
    public void Exit(){
        SceneManager.LoadScene("Menu");
    }
    void SaveHighScore(int score){
        PlayerPrefs.SetInt("highScore", score);
        PlayerPrefs.Save();
    }

    int GetHighScore(){
        return PlayerPrefs.GetInt("highScore");
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
        musicSource.volume = PlayerPrefs.GetInt("musicVol") * .05f;

        // update the sfx volume for any existing arrows
        Arrow [] arrows = FindObjectsOfType<Arrow>();
        foreach(Arrow x in arrows){
            x.volumeLevel = PlayerPrefs.GetInt("sfxVol");
        }

        // update the volume sliders
        musicVolume.value = PlayerPrefs.GetInt("musicVol");
        sfxVolume.value = PlayerPrefs.GetInt("sfxVol");
    }

    void SetControllerMode(){
        // update the controllermode on player
        if( PlayerPrefs.GetInt("inputMode") == 1){
            player.controllerMode = true;
        }
        else{
            player.controllerMode = false;
        }

        // update the slider
        inputMode.value = PlayerPrefs.GetInt("inputMode");
    }

}
