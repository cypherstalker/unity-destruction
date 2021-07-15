using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour {

    public GameObject ball;
    public float forceMultiplier;
    public UnityEngine.UI.Text text;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    void Start () {
        camera = GetComponent<Camera>();
    }
	
    void Update () {
        text.text = "Throw Power: "+forceMultiplier;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            forceMultiplier += forceMultiplier/100;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            forceMultiplier -= forceMultiplier/100;

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            GameObject newBall = Instantiate(ball, ray.origin, Quaternion.Euler(Vector3.zero));
            newBall.GetComponent<Rigidbody>().AddForce(ray.direction * forceMultiplier);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000000f))
                hit.collider.gameObject.GetComponent<Destruction>()?.BreakWithExplosiveForce(500f);
        }

    }
}
