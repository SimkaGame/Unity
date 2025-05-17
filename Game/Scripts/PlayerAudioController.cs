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
    private AudioSource damageAudioSource;
    private AudioSource shootAudioSource;
    private AudioSource burnAudioSource;

    private PlayerController playerController;
    private bool wasGrounded;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        walkAudioSource = gameObject.AddComponent<AudioSource>();
        landAudioSource = gameObject.AddComponent<AudioSource>();
        damageAudioSource = gameObject.AddComponent<AudioSource>();
        shootAudioSource = gameObject.AddComponent<AudioSource>();
        burnAudioSource = gameObject.AddComponent<AudioSource>();

        ConfigureAudioSource(walkAudioSource, walkSound, true);
        ConfigureAudioSource(landAudioSource, landSound, false);
        ConfigureAudioSource(damageAudioSource, damageSound, false);
        ConfigureAudioSource(shootAudioSource, shootSound, false);
        ConfigureAudioSource(burnAudioSource, burnSound, false);

        walkAudioSource.Play();
        walkAudioSource.Pause();

        wasGrounded = playerController.IsGrounded;
    }

    private void ConfigureAudioSource(AudioSource source, AudioClip clip, bool loop)
    {
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = false;
        source.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    private void Update()
    {
        var horizontalMove = playerController.HorizontalMove;
        var isGrounded = playerController.IsGrounded;

        if (isGrounded && Mathf.Abs(horizontalMove) > 0.01f)
        {
            if (!walkAudioSource.isPlaying) walkAudioSource.Play();
        }
        else if (walkAudioSource.isPlaying)
        {
            walkAudioSource.Pause();
        }
    }

    private void FixedUpdate()
    {
        var isGrounded = playerController.IsGrounded;
        if (!wasGrounded && isGrounded)
        {
            landAudioSource.Play();
        }
        wasGrounded = isGrounded;
    }

    public void PlayDamageSound() => damageAudioSource.Play();
    public void PlayShootSound() => shootAudioSource.Play();
    public void PlayBurnSound() => burnAudioSource.Play();
}