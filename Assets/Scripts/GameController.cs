using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float timeRemaining = 30;
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
    public Slider musicVolume;
    public Slider sfxVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        // make sure that the in game ui is active
        endGameUI.SetActive(false);
        inGameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        inEndGame = false;
        paused = false;
        SetVolumes();

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
        SpawnTarget((int)timeRemaining);

        // make sure the player is in the right state
        player.currentState = Player.PlayerState.Default;
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused){
            if(Input.GetKeyDown(KeyCode.Escape)){
                Pause();
            }
            
            // update the timer
            string [] timerElements = timer.text.Split(':');
            int newTime = (int)timeRemaining;
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if(newTime != lastTimeSpawned && newTime % 5 == 0){
                    SpawnTarget(newTime);
                }
            }
            else
            {
                if(!inEndGame){EndGame();}
            }
            timeSlider.value = timeRemaining;
        } else{
            if(Input.GetKeyDown(KeyCode.Escape)){
                Resume();
            }
        }
    }

    void SpawnTarget(int timeSpawned){
        lastTimeSpawned = timeSpawned;
        SpawnTarget();
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
        if(randTotal < 5){
            newTarget.GetComponent<Target>().UpdateType(1);
        } else if(randTotal > 10){
            newTarget.GetComponent<Target>().UpdateType(2);
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
            // update the high score to the new value
            endGameText = "New High Score! - " + score;
            SaveHighScore(score);
        }
        else{endGameText = "Score - " + score;}
        // switch to the endgame ui
        inGameUI.SetActive(false);
        endGameUI.SetActive(true);
        endGameUI.GetComponentInChildren<Text>().text = endGameText;

        inEndGame = true;
    }
    void Pause(){
        // pause the timer
        paused = true;

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
    void SetVolumes(){
        // update the music volume
        musicSource.volume = PlayerPrefs.GetInt("musicVol") * .1f;

        // update the sfx volume for any existing arrows
        Arrow [] arrows = FindObjectsOfType<Arrow>();
        foreach(Arrow x in arrows){
            x.volumeLevel = PlayerPrefs.GetInt("sfxVol");
        }

        // update the volume sliders
        musicVolume.value = PlayerPrefs.GetInt("musicVol");
        sfxVolume.value = PlayerPrefs.GetInt("sfxVol");
    }

}
