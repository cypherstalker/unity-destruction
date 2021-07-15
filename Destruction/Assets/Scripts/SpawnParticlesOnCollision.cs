using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticlesOnCollision : MonoBehaviour
{
    public GameObject spawnParticleObject;
    public Material particlesMaterial;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject newParticle = Instantiate(spawnParticleObject, collision.contacts[0].point, Quaternion.Euler(Vector3.zero));
        if(particlesMaterial != null)
            newParticle.GetComponent<ParticleSystemRenderer>().material = particlesMaterial;
    }
}
