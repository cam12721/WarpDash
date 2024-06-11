using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicScript : MonoBehaviour
{
    public uint playerScore;
    public TextMeshProUGUI playerScoreText, finalScoreText;
    public bool scoreEnabled = true;
    public float terrainMoveSpeed = 5.5f;
    public float terrainTopSpeed = 8f;
    public float terrainSpeedIncrease = 0.25f;
    public bool infiniteTerrain = false;
    public GameObject gameOverScreen;
    public GameObject[] characterPrefabs;
    private float _originalTerrainMoveSpeed;
    public GameObject gameCanvas;
    public GameObject scoreCanvas;
    public Camera mainCamera;

    // Used for multiplayer
    private string gamerTag;
    private int playerNum;

    private void Awake()
    {
        playerNum = PhotonNetwork.playerList.Length - 1;
        gameCanvas.SetActive(true);
    }

    private void Start()
    {
        mainCamera.orthographicSize = 19f;
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);

        _originalTerrainMoveSpeed = terrainMoveSpeed;
        terrainMoveSpeed = 0;
    }

    public void SpawnPlayerMultiPlay()
    {
        if (playerNum < 4)
        {
            PhotonNetwork.Instantiate(characterPrefabs[playerNum].name, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity, 0);
            gameCanvas.SetActive(false);
        }
    }

    public void SpawnPlayerSinglePlay()
    {
        var index = PlayerPrefs.GetInt("selectedOption", 0);
        Instantiate(characterPrefabs[index], transform.position, Quaternion.identity);
        gameCanvas.SetActive(false);
    }

    public void MoveTerrain()
    {
        terrainMoveSpeed = _originalTerrainMoveSpeed;
    }

    public void AddScore(uint scoreIncrement)
    {
        var index = PlayerPrefs.GetString("gameMode", "");
        if (index == "Singleplayer")
        {
            if (scoreEnabled)
            {
                playerScore += scoreIncrement;
                playerScoreText.text = "Score: " + playerScore.ToString();
            }
        }
    }

    // If Continue button pressed, return to Title screen, for now
    public void Continue()
    {
        Debug.Log(gamerTag + " has left " + PhotonNetwork.room.Name);
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
        PhotonNetwork.Disconnect();
    }

    // Enable game over screen
    public void GameOver(string name = "")
    {
        gamerTag = name;
        if (scoreEnabled)
        {
            scoreEnabled = false;
            playerScoreText.enabled = false;
            finalScoreText.text = playerScore.ToString();
            gameOverScreen.SetActive(true);
        }
    }
}
