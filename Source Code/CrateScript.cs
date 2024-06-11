using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    public GameObject crateCherry, crateGoldberry, crateKiwi;
    private Animator _animator;
    private int chosenNum = 0;        // Determines whether a cherry or a goldberry is spawned.

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        chosenNum = Random.Range(0, 6);         // chosenNum is a number between 0 and 5
    }

    // When a crate is broken, it will spawn a fruit. The spawned fruit depends on the value of chosenNum.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            _animator.Play("Crate_Destroyed");

            if (chosenNum == 5)
                Instantiate(crateGoldberry, transform.position, Quaternion.identity);
            else if (chosenNum == 4 || chosenNum == 3)
                Instantiate(crateKiwi, transform.position, Quaternion.identity);
            else
                Instantiate(crateCherry, transform.position, Quaternion.identity);
        }
    }

    // Destroy crate after destroy animation ends
    private void AnimationFinishedTrigger()
    {
        Destroy(gameObject);
    }
}
