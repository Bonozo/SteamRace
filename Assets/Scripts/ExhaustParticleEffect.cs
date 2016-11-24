using UnityEngine;

public class ExhaustParticleEffect : MonoBehaviour
{
    [SerializeField]
    private float startDelay = 0.0f;

    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Start()
    {
        foreach (ParticleSystem exhaustEffect in particleSystems)
        {
            if (exhaustEffect != null)
            {
                exhaustEffect.startDelay = startDelay;
                exhaustEffect.Play();
            }
        }
    }
}