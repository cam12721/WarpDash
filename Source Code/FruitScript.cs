using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitScript : MonoBehaviour
{
    public Sprite plus1, plus5;
    private BoxCollider2D _collider;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(ShowScoreIndicator());
    }

    private IEnumerator ShowScoreIndicator()
    {
        _collider.enabled = false;
        _animator.enabled = false;

        if (gameObject.CompareTag("Cherry"))
            _spriteRenderer.sprite = plus1;
        else if (gameObject.CompareTag("Goldberry"))
            _spriteRenderer.sprite = plus5;

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
