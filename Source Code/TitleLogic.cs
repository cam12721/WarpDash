using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleLogic : MonoBehaviour
{
    public string LevelName;
    
    public void LoadLevel()
    {
        if (LevelName == "CharacterSelection")
            PlayerPrefs.SetString("gameMode", "Singleplayer");
        else if (LevelName == "LobbyMenu")
            PlayerPrefs.SetString("gameMode", "Multiplayer");

        SceneManager.LoadScene(LevelName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
