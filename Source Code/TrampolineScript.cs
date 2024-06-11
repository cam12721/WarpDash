using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineScript : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Play trampoline animation if trampoline has been hit by player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _animator.Play("Trampoline_Activated");
        }
    }

    // Return to idle trampoline state after activation animation ends
    private void AnimationFinishedTrigger()
    {
        _animator.Play("Trampoline_Idle");
    }
}