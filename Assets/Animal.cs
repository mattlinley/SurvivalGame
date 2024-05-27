using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip rabbitHitAndScream;
    [SerializeField] AudioClip rabbitHitAndDie;

    [SerializeField] GameObject bloodPuddle;

    private Animator animator;
    public bool isDead = false;

    enum AnimalType
    {
        Rabbit,
        Lion,
        Snake
    }

    [SerializeField] AnimalType thisAnimalType;
    [SerializeField] ParticleSystem bloodSplashParticles;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage) 
    {
        if (!isDead)
        {
            currentHealth -= damage;
            bloodSplashParticles.Play();


            if (currentHealth <= 0)
            {
                PlayDyingSound();
                animator.SetTrigger("die");
                GetComponent<AI_Movement>().enabled = false;
                bloodPuddle.SetActive(true);
                isDead = true;
            }
            else
            {
                PlayHitSound();
            } 
        }
    }

    private void PlayDyingSound()
    {
        switch (thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndDie);
                break;
            default:
                break;
        }
        
    }

    private void PlayHitSound()
    {
       switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndScream);
                break;
            default:
                break;
            }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
