using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip burnSound;

    private AudioSource walkAudioSource;
    private AudioSource landAudioSource;
    private PlayerController playerController;
    private bool wasGrounded;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        AudioSource[] sources = GetComponents<AudioSource>();
        walkAudioSource = sources[0];
        landAudioSource = sources[1];

        walkAudioSource.clip = walkSound;
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;
        walkAudioSource.Play();
        walkAudioSource.Pause();

        wasGrounded = playerController.IsGrounded;
    }

    private void Update()
    {
        float horizontalMove = playerController.HorizontalMove;
        bool isGrounded = playerController.IsGrounded;

        if (isGrounded && Mathf.Abs(horizontalMove) > 0.01f)
        {
            if (!walkAudioSource.isPlaying) walkAudioSource.Play();
        }
        else if (walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        bool isGrounded = playerController.IsGrounded;

        if (!wasGrounded && isGrounded) landAudioSource.PlayOneShot(landSound);
        wasGrounded = isGrounded;
    }

    public void PlayDamageSound() => landAudioSource.PlayOneShot(damageSound);
    public void PlayShootSound() => landAudioSource.PlayOneShot(shootSound);
    public void PlayBurnSound() => landAudioSource.PlayOneShot(burnSound);
    public bool IsGrounded => playerController.IsGrounded;
}