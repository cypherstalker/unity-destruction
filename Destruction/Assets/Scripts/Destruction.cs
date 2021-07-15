using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour {

    [Space(7)]
    [Header("State")]
    [Space(2)]
    [Tooltip("Whether the object is unbroken")]
    public bool together = true;
    [Tooltip("Whether the object starts broken")]
    public bool startBroken = false;

    [Space(7)]
    [Header("General Settings")]
    [Space(2)]
    [Tooltip("The higher this is, the more will break off in an impact")]
    public float breakageMultiplier = 0.3f;
    [Tooltip("How resistant the object initially is to breakage.")]
    public float strength = 0.3f;
    [Tooltip("Whether or not to use momentum rather than velocity to work out destruction")]
    public bool useMomentum = false;

    [Space(7)]
    [Header("Breaking on Collision")]
    [Space(2)]
    [Tooltip("Whether the object breaks when it collides with something")]
    public bool breakOnCollision = true;
    [Tooltip("The minimum relative velocity (or momentum if useMomentum is true) to break the object")]
    public float velocityToBreak = 1; // Velocity required to break object

    [Space(7)]
    [Header("Explode on Break")]
    [Space(2)]
    [Tooltip("Uses an explosive force on object breakage")]
    public bool breakWithExplosiveForce = false;
    [Tooltip("size of explosion")]
    public float explosiveRadius = 100;
    [Tooltip("power of blast")]
    public float explosivePower = 3f;


    [Space(7)]
    [Header("Breaking when nothing underneath")]
    [Space(2)]
    [Tooltip("Whether the object breaks when there's nothing underneath supporting it")]
    public bool breakOnNoSupports = false;
    [Tooltip("The length of the raycast used to check for supports underneath")]
    public float raycastLength = 1f;

    [Space(7)]
    [Header("Sound On Break")]
    [Space(2)]
    [Tooltip("Whether the object makes a sound when it breaks")]
    public bool soundOnBreak = false;
    [Tooltip("An array of sounds for the object to make when it breaks (A random one will be selected)")]
    public AudioClip[] clips;

    [Space(7)]
    [Header("Particles On Break")]
    [Space(2)]
    [Tooltip("Whether the object makes particles when it breaks")]
    public bool particlesOnBreak = false;

    //Private vars
    private AudioSource src;
    private ParticleSystem psys;

    private Vector3 spherePoint = Vector3.zero;
    private float sphereRadius = 0f;

    private Rigidbody rigidbody_parent;
    private Collider coll;
    private Rigidbody[] rigids;
    private MeshRenderer render;
    private MeshRenderer[] childrender;
    private Transform particlesObject;

    void Start () {
        //Get the rigidbodies
        rigids = gameObject.GetComponentsInChildren<Rigidbody>();
        childrender = gameObject.GetComponentsInChildren<MeshRenderer>();
        coll = GetComponent<Collider>();
        rigidbody_parent = GetComponent<Rigidbody>();
        render = childrender[0];

        HidePieces();

        together = !startBroken;
        SetPiecesKinematic(together);

        if (soundOnBreak)
            SetupSound();
        if (particlesOnBreak)
            SetupParticles();
    }
	
    void SetupSound() {
        //Get the audio source or create one
        src = GetComponent<AudioSource>();
        if (src == null)
            src = gameObject.AddComponent<AudioSource>();

        //Add a random audio clip to it
        src.clip = clips[Random.Range(0, clips.Length-1)];
    }

    void SetupParticles() {
        // Get the particle system or create one
        particlesObject = transform.Find("particles");
        psys = particlesObject.GetComponent<ParticleSystem>();

        //This doesn't seem to do anything b/c the gameobject is not active
        psys.Stop();
    }

    void Update () {
        /* Broken object should follow unbroken one to prevent them from
         * being in the wrong place when they switch */
        //brokenObj.transform.position = togetherObj.transform.position;

        //Make sure the right object is active
        //togetherObj.SetActive(together);
        //brokenObj.SetActive(!together);
        if (!together)
            Break();

        if (breakOnNoSupports)
            CheckForSupports();
    }

    void CheckForSupports () {
        //Check downwards for supports
        if (!Physics.Raycast(transform.position, Vector3.down, raycastLength))
            Break();
    }

    void PreBreak()
    {
        ShowPieces();
    }

    void HidePieces()
    {
        for(int i = 1;i<childrender.Length;i++)
        {
            childrender[i].enabled = false;
        }
        childrender[0].enabled = true;
    }

    void ShowPieces()
    {
        for (int i = 1; i < childrender.Length; i++)
        {
            childrender[i].enabled = true;
        }
        childrender[0].enabled = false;
    }

    void OnCollisionEnter(Collision collision) {
        if (!breakOnCollision)
            return;
        //Only break if relative velocity is high enough
        spherePoint = collision.contacts[0].point;
        if (CheckMomentumOrVelocity(collision, velocityToBreak))
        {
            if (particlesOnBreak)
                particlesObject.position = spherePoint;
            if (breakWithExplosiveForce)
                BreakWithExplosiveForce(explosivePower,explosiveRadius);
            else
                Break();
        }
        else if (CheckMomentumOrVelocity(collision, strength))
        {
            PreBreak();
            //Otherwise, if velocity is strong enough to break some bits but not others...

            //Get the impact point
            //Get the radius within which we'll break pieces
            sphereRadius = collision.relativeVelocity.magnitude / velocityToBreak * breakageMultiplier;

            //Turn on Colliders so that Physics.OverlapSphere will work
            foreach (Rigidbody rigid in rigids)
                rigid.GetComponent<Collider>().enabled = true;

            Collider[] pieces = Physics.OverlapSphere(spherePoint, sphereRadius/*, 1 << 8*/);

            //Make the broken-off pieces non-kinematic
            foreach (Collider piece in pieces)
            {
                Rigidbody rigid = piece.GetComponent<Rigidbody>();
                foreach (Rigidbody ourridgid in rigids)
                    if (rigid == ourridgid)
                        rigid.isKinematic = false;
            }

            //And turn off the Colliders of the not broken-off pieces 
            foreach (Rigidbody rigid in rigids)
                rigid.GetComponent<Collider>().enabled = !rigid.isKinematic;
        }
    }

    bool CheckMomentumOrVelocity (Collision collision, float check) {
        if (useMomentum && collision.rigidbody != null)
            return collision.relativeVelocity.magnitude * collision.rigidbody.mass > check;
        //If no rigidbody, use velocity
        return collision.relativeVelocity.magnitude > check;
    }

    public void Break () {
        PreBreak();
        SetPiecesKinematic(false);
        coll.enabled = false;
        //Play the sound
        if (soundOnBreak)
            src.Play();
        //Show particles
        if (particlesOnBreak)
            psys.Play();
    }

    void SetPiecesKinematic (bool valueIn) {
        foreach (Rigidbody rigid in rigids) {
            rigid.isKinematic = valueIn;
            rigid.GetComponent<Collider>().enabled = !valueIn;
        }
        coll.enabled = valueIn;
        rigidbody_parent.isKinematic = !valueIn;
    }

    public void BreakWithExplosiveForce(float force, float radius = 3) {
        PreBreak();
        Break();

        //Add the explosive force to each rigidbody
        foreach (Rigidbody rigid in rigids)
            rigid.AddExplosionForce(force, transform.position, radius);
    }

}
