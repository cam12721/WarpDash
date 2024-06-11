using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MultiPlayCharacterMovement : Photon.MonoBehaviour, IPunObservable
{
    private GameLogicScript _logic;
    private int score = 0;

    readonly private float _killLimitY = -19f;
    readonly private float _killLimitX = -34f;

    public float moveSpeed = 10f;               // Adjusts the movement speed of the player. This should be about 4 units higher than the terrain movement speed
    public float dashForce = 50f;               // Adjusts the dash force of the player
    public float jumpForce = 16f;               // Adjusts the jump force of the player
    public float trampolineJumpForce = 30f;     // Adjusts the jump force of the player after hitting a trampoline
    public float speedBoost = 10f;              // Increases the movement speed and dash force of the player after they hit a kiwi
    public float speedBoostDuration = 5f;       // Duration of speed boost
    private float _moveDirectionX = 0f;          // Determines the direction player is moving along x-axis (-1 = left, 0 = idle, 1 = right)
    private bool _jump = false;                 // Flag for when "Jump" button is hit
    private bool _pushBackPlayer = true;        // Determines if player is affected by terrain movement
    private bool _trampolineActivated = false;  // Determines if player has activated a trampoline
    private bool _speedBoostActive = false;
    public LayerMask jumpGround;

    public float dashingTime = 0.1f;            // Adjusts duration of player dash
    public float dashCooldown = 0.5f;           // Adjusts the cooldown time for player dash
    private bool _canDash = true;
    private bool _isDashing;
    private bool _dash = false;                 // Flag for when "Dash" button is hit

    public bool playerIsAlive = true;
    private bool _playerSpawned = false;

    public TrailRenderer trailRenderer;
    private Rigidbody2D _rigidBod;
    private BoxCollider2D _collide;
    private Animator _animator;
    private SpriteRenderer _sprite;
    public GameObject ProjectilePrefab;
    public Transform projectileLauncherL;
    public Transform projectileLauncherR;

    private enum MovementState { idle, running, jumping, falling }
    private enum ActiveProjectileLauncher { left, right };
    private ActiveProjectileLauncher _launcher;

    // Used for multiplayer
    private PhotonView _photonView;
    private string gamerTag;
    private Vector2 _networkPosition;
    private float _lerpPosition = 5.0f;
    public bool teleportIfFarFromNetworkPosition;
    public float distanceFromNetworkPosition;


    public float MoveDirectionX { get => _moveDirectionX; set => _moveDirectionX = value; }
    public bool Jump { get => _jump; set => _jump = value; }

    private void Awake()
    {
        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 10;
        PhotonNetwork.automaticallySyncScene = true;
    }

    // All values in this function are shared with the server and other players
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // This player is writing data; other players are reading data
        if (stream.isWriting)
        {
            // Send this player's position to server
            stream.SendNext(_rigidBod.position);
            stream.SendNext(_rigidBod.velocity);
        }
        else if (stream.isReading)
        {
            // Change this player's position if it is changed by the server 
            _networkPosition = (Vector2)stream.ReceiveNext();
            _rigidBod.velocity = (Vector2)stream.ReceiveNext();

            // Compensate for lag
            float lag = Mathf.Abs((float)(PhotonNetwork.time - info.timestamp));
            _rigidBod.position += _rigidBod.velocity * lag;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogicScript>();
        _rigidBod = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _collide = GetComponent<BoxCollider2D>();
        _launcher = ActiveProjectileLauncher.right;

        if (_photonView.isMine)
        {
            gamerTag = PhotonNetwork.playerName;
            Debug.Log(gamerTag + " has joined " + PhotonNetwork.room.Name);
        }

        //freeze the rotation of the Rigidbody2D component
        _rigidBod.freezeRotation = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if this instance of the character is the player's
        if (_photonView.isMine)
        {
            if (_playerSpawned)
            {
                // If the player reaches either x or y boundary, kill them
                if (transform.position.y <= _killLimitY || transform.position.x <= _killLimitX)
                {
                    playerIsAlive = false;
                    _rigidBod.constraints = RigidbodyConstraints2D.FreezePositionY;
                    _rigidBod.constraints = RigidbodyConstraints2D.FreezePositionX;
                    _logic.GameOver(gamerTag);
                    //_animator.Play("Player_Destroyed");
                    _photonView.RPC("PlayDestroyAnimation", PhotonTargets.AllBuffered);
                }

                if (playerIsAlive)
                {
                    // Prevent player from jumping or running while dashing
                    if (_isDashing)
                        return;

                    // Get the horizontal input (left/right arrow keys, A/D keys, or joystick)
                    _moveDirectionX = Input.GetAxisRaw("Horizontal");

                    if (Input.GetButtonDown("Jump") && IsGrounded())
                    {
                        _jump = true;
                    }

                    if (_moveDirectionX != 0)
                    {
                        // If player is moving, allow them to dash by pressing left shift
                        if (Input.GetButtonDown("Dash") && _canDash)
                            _dash = true;
                    }

                    // Fire projectile
                    if (Input.GetButtonDown("Fire1"))
                    {
                        switch (_launcher)
                        {
                            case ActiveProjectileLauncher.left:
                                Instantiate(ProjectilePrefab, projectileLauncherL.position, projectileLauncherL.rotation);
                                break;
                            case ActiveProjectileLauncher.right:
                                Instantiate(ProjectilePrefab, projectileLauncherR.position, projectileLauncherR.rotation);
                                break;
                        }
                    }

                    UpdateAnimationState();
                }
            }
        }
    }

    //FixedUpdate is called before Update, at the same rate based on Delta Time. Use FixedUpdate for physics-based events.
    private void FixedUpdate()
    {
        if (photonView.isMine)
        {
            // Moves the character at speed of [moveSpeed]
            if (!_isDashing)
                _rigidBod.velocity = new Vector2(_moveDirectionX * moveSpeed, _rigidBod.velocity.y);

            if (_jump)
            {
                _rigidBod.velocity = new Vector2(_rigidBod.velocity.x, jumpForce);
                _jump = false;
            }

            // If player has activated trampoline, make them jump really high
            if (_trampolineActivated)
            {
                _rigidBod.velocity = new Vector2(_rigidBod.velocity.x, trampolineJumpForce);
                _trampolineActivated = false;
            }

            if (_moveDirectionX != 0)
            {
                if (_dash)
                {
                    StartCoroutine(Dash());
                    _dash = false;
                }
            }
            else if (_pushBackPlayer)
            {
                // If player is not moving according to input, push them to the left with the terrain
                _rigidBod.velocity = new Vector2(_logic.terrainMoveSpeed * -1, _rigidBod.velocity.y);
            }
        }
        else
        {
            // Try to sync movement of the other player's character with the character in our scene
            _rigidBod.position = Vector2.Lerp(_rigidBod.position, _networkPosition, _lerpPosition * Time.fixedDeltaTime);
            
            if (Vector2.Distance(_rigidBod.position, _networkPosition) > distanceFromNetworkPosition && teleportIfFarFromNetworkPosition)
            {
                _rigidBod.position = _networkPosition;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(_collide.bounds.center, _collide.bounds.size, 0f, Vector2.down, .1f, jumpGround);
    }

    // Method for player dash ability; IEnumerator method runs asynchronously
    [PunRPC]
    private IEnumerator Dash()
    {
        _pushBackPlayer = false;
        _canDash = false;
        _isDashing = true;
        float originalGrav = _rigidBod.gravityScale;                                // Store player's original gravity
        _rigidBod.gravityScale = 0f;                                                // Make it so the rigidbod is unaffected by gravity
        _rigidBod.velocity = new Vector2(_moveDirectionX * dashForce, 0f);           // Player dashes in direction they are currently moving
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);                               // Player dashes for [dashingTime] seconds

        trailRenderer.emitting = false;
        _rigidBod.gravityScale = originalGrav;                                      // Restore player's original gravity
        _isDashing = false;
        _pushBackPlayer = true;
        yield return new WaitForSeconds(dashCooldown);                              // Player can dash again after [dashCooldown] seconds

        _canDash = true;
    }

    // Play player spawn animation
    private void SpawnAnimationFinished()
    {
        _playerSpawned = true;
        Debug.Log("Player spawned");
    }

    [PunRPC]
    private void PlayDestroyAnimation()
    {
        _animator.Play("Player_Destroyed");
    }

    // Play player death animation
    private void DestroyAnimationFinished()
    {
        Debug.Log("Player deleted");
        _photonView.RPC("DestroyPlayer", PhotonTargets.AllBuffered);
    }

    // Function to update the animation state; state: idle = 0, running = 1, jumping = 2, falling = 3
    private void UpdateAnimationState()
    {
        MovementState state;

        if (_moveDirectionX > 0f)
        {
            state = MovementState.running;
            _launcher = ActiveProjectileLauncher.right;
            _photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }
        else if (_moveDirectionX < 0f)
        {
            state = MovementState.running;
            _launcher = ActiveProjectileLauncher.left;
            _photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }
        else
        {
            state = MovementState.idle;
        }

        if (_rigidBod.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (_rigidBod.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        _animator.SetInteger("state", (int)state);
    }

    // Player collision logic
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            _logic.AddScore(1);
            score++;
            Debug.Log("Score: " + score);
        }
        else if (collision.gameObject.CompareTag("Kiwi"))
        {
            if (!_speedBoostActive)
                StartCoroutine(SpeedBoost());
        }
        else if (collision.gameObject.CompareTag("Goldberry"))
        {
            _logic.AddScore(5);
            score += 5;
            Debug.Log("Score: " + score);
        }

        if (collision.gameObject.CompareTag("Trampoline"))
        {
            _trampolineActivated = true;
        }
    }

    [PunRPC]
    // Method for speed boost powerup earned from kiwis
    private IEnumerator SpeedBoost()
    {
        _speedBoostActive = true;
        _sprite.color = new Color(1f, 0.7010124f, 0f, 1f);
        float originalMoveSpeed = moveSpeed;
        float originalDashForce = dashForce;
        float originalJumpForce = jumpForce;
        moveSpeed += speedBoost;
        dashForce += speedBoost;
        jumpForce += speedBoost;
        yield return new WaitForSeconds(speedBoostDuration);

        moveSpeed = originalMoveSpeed;
        dashForce = originalDashForce;
        jumpForce = originalJumpForce;
        _sprite.color = Color.white;
        _speedBoostActive = false;
    }

    [PunRPC]
    private void FlipTrue()
    {
        _sprite.flipX = true;
    }

    [PunRPC]
    private void FlipFalse()
    {
        _sprite.flipX = false;
    }

    [PunRPC]
    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }
}
