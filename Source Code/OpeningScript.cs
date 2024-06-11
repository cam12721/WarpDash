using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScript : MonoBehaviour
{
    private GameLogicScript _logic;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogicScript>();
        _animator = GetComponent<Animator>();
    }

    private void OpeningFinished()
    {
        _animator.enabled = false;

        var mode = PlayerPrefs.GetString("gameMode", "");
        if (mode == "Singleplayer")
            _logic.SpawnPlayerSinglePlay();
    }
}
