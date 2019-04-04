using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class Weapon : MonoBehaviour , IInteractable
{
    public int damage = 10;
    public int maxAmmo = 500;
    public int maxClip = 30;
    public float range = 10f;
    public float shootRate = .2f;
    public float lineDelay = .1f;
    public Transform shotOrigin;

    private int ammo = 0;
    private int clip = 0;
    private float shootTimer = 0f;
    private bool canShoot = false;

    private Rigidbody rigid;
    private BoxCollider boxCollider;
    private LineRenderer lineRenderer;
    private SphereCollider sphereCollider;

    void GetReferences()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        lineRenderer = GetComponent<LineRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Reset()
    {
        GetReferences();

        // Collect all bounds inside of children
        Renderer[] children = GetComponentsInChildren<MeshRenderer>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer rend in children)
        {
            bounds.Encapsulate(rend.bounds);
        }

        // Turn off line renderer
        lineRenderer.enabled = false;

        // Turn off rigidbody
        rigid.isKinematic = false;

        // Apply bounds to box collider
        boxCollider.center = bounds.center - transform.position;
        boxCollider.size = bounds.size;

        // Enable trigger
        sphereCollider.isTrigger = true;
        sphereCollider.center = boxCollider.center;
        sphereCollider.radius = boxCollider.size.magnitude * .5f;
    }

    void Awake()
    {
        GetReferences();

        // Turn off line renderer
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Increase shoot timer
        shootTimer += Time.deltaTime;
        //if time reaches rate
        if(shootTimer >= shootRate)
        {
            //we can shoot
            canShoot = true;
        }
    }
    public void Pickup()
    {
        // Disable rigidbody
        rigid.isKinematic = true;
        // not interactable
        sphereCollider.enabled = false;
    }
    public void Drop()
    {
        rigid.isKinematic = false;
    }
    public string GetTitle()
    {
        return "Weapon";
    }

    IEnumerator ShowLine(Ray bulletRay, float lineDelay)
    {
        //Run logic before
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, bulletRay.origin);
        lineRenderer.SetPosition(1, bulletRay.origin + bulletRay.direction * range);


        // Run logic before
        yield return new WaitForSeconds(lineDelay);

        //Disable line
        lineRenderer.enabled = false;
    }

    public virtual void Reload()
    {
        clip += ammo;
        ammo -= maxClip;
    }

    public virtual void Shoot()
    {
        //Can shoot?
        if (canShoot)
        {
            // Crete a bul.et ray from shot origin to forward
            Ray bulletRay = new Ray(shotOrigin.position, shotOrigin.forward);
            RaycastHit hit;
            //Perform raycast
            if(Physics.Raycast(bulletRay, out hit, range))
            {
                //Try geting enemy from hit
                IKillable killable = hit.collider.GetComponent<IKillable>();
                if (killable != null)
                {
                    killable.TakeDamage(damage);
                }
                Player player = hit.collider.GetComponent<Player>();
                if (player)
                {
                    //deal damage to enemy
                }

                //show line
                StartCoroutine(ShowLine(bulletRay, lineDelay));
                //reset timer
                shootTimer = 0;
                //can't shoot anymore
                shootTimer = 0;
                canShoot = false;

            }
        }
    }
}
