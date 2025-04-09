using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioSource walkAudioSource;
    private AudioSource landAudioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip landSound;

    private PlayerController playerController;
    private bool wasGrounded;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            enabled = false;
            return;
        }
        walkAudioSource = sources[0];
        landAudioSource = sources[1];

        wasGrounded = playerController.IsGrounded;
    }

    void Update()
    {
        if (walkAudioSource == null || landAudioSource == null) return;

        float horizontalMove = playerController.HorizontalMove;
        bool isGrounded = playerController.IsGrounded;

        if (isGrounded && Mathf.Abs(horizontalMove) > 0.1f)
        {
            if (!walkAudioSource.isPlaying || walkAudioSource.clip != walkSound)
            {
                walkAudioSource.clip = walkSound;
                walkAudioSource.loop = true;
                walkAudioSource.Play();
            }
        }
        else if (walkAudioSource.clip == walkSound)
        {
            walkAudioSource.Stop();
            walkAudioSource.loop = false;
        }
    }

    void FixedUpdate()
    {
        if (walkAudioSource == null || landAudioSource == null) return;

        bool isGrounded = playerController.IsGrounded;

        if (!wasGrounded && isGrounded)
        {
            landAudioSource.PlayOneShot(landSound);
        }
        wasGrounded = isGrounded;
    }

    public bool IsGrounded => playerController.IsGrounded;
}