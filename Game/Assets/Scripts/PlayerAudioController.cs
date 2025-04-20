using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioSource walkAudioSource;
    private AudioSource landAudioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip burnSound;

    private PlayerController playerController;
    private bool wasGrounded;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            enabled = false;
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

        if (walkSound != null)
        {
            walkAudioSource.clip = walkSound;
            walkAudioSource.loop = true;
            walkAudioSource.playOnAwake = false;
            walkAudioSource.Stop();
            walkAudioSource.Play();
            walkAudioSource.Pause();
        }

        wasGrounded = playerController.IsGrounded;
    }

    void Update()
    {
        if (walkAudioSource == null || landAudioSource == null) return;

        float horizontalMove = playerController.HorizontalMove;
        bool isGrounded = playerController.IsGrounded;

        if (isGrounded && Mathf.Abs(horizontalMove) > 0.01f)
        {
            if (!walkAudioSource.isPlaying && walkSound != null)
            {
                walkAudioSource.Play();
            }
        }
        else if (walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
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

    public void PlayDamageSound()
    {
        if (damageSound != null)
        {
            landAudioSource.PlayOneShot(damageSound);
        }
    }

    public void PlayShootSound()
    {
        if (shootSound != null)
        {
            landAudioSource.PlayOneShot(shootSound);
        }
    }

    public void PlayBurnSound()
    {
        if (burnSound != null)
        {
            landAudioSource.PlayOneShot(burnSound);
        }
    }

    public bool IsGrounded => playerController.IsGrounded;
}