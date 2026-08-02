using UnityEngine;
using System.Collections.Generic;
public class VFXManager : MonoBehaviour
{

    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>(); 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayDashParticle()
    {
        particleSystems[0].Play();
    }
}
