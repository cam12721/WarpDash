using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is meant to be used for fruits that are not part of a terrain prefab.
public class CrateFruitScript : MonoBehaviour
{
    public Sprite plus1, plus5, speedArrow;
    private GameLogicScript _logic;
    private BoxCollider2D _collider;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    readonly private float _killBoundaryX = -34f;

    // Start is called before the first frame update
    void Start()
    {
        _logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogicScript>();
        _collider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += _logic.terrainMoveSpeed * Time.deltaTime * Vector3.left;
    }

    private void Update()
    {
        if (transform.position.x <= _killBoundaryX)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(ShowIndicator());
    }

    private IEnumerator ShowIndicator()
    {
        _collider.enabled = false;
        _animator.enabled = false;

        if (gameObject.CompareTag("Cherry"))
            _spriteRenderer.sprite = plus1;
        else if (gameObject.CompareTag("Kiwi"))
            _spriteRenderer.sprite = speedArrow;
        else if (gameObject.CompareTag("Goldberry"))
            _spriteRenderer.sprite = plus5;

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
