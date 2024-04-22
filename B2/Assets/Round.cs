using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Round : MonoBehaviour
{

    //https://www.youtube.com/watch?v=4f6Az3Sp99w
    [Header("Generic")]
    public Rigidbody rb;
    public string roundName;
    public float muzzleVelocity;
    public Vector2Int damage;

    [Header("Tracer")]
    public LineRenderer lineRenderer;
    public Material tracerMat;
    public Color tracerColor;
    [Header("Travel")]
    public float gravity;
    public Vector3 startPosition;
    public Vector3 startDir;

    public Vector3 velocity;
    public float startTime = -1;
    public float distFromOrigin;
    public float maxDist = 1000;
    public float despawnDist;
    [Header("Impact FX")]
    public GameObject concreteHit;
    public GameObject bloodHit;

    float curDropAmount;
    public List<Vector3> positions = new();

    public AnimationCurve dropCurve;

    private void Start()
    {
        //velocity.z = muzzleVelocity/50;
        startPosition = transform.position;
        startDir = transform.forward.normalized;

        lineRenderer.enabled = false;
        Invoke("Show", 0.05f);

        Material newMat = new(tracerMat);
        newMat.color = tracerColor;
        newMat.SetColor("_EmissionColor", tracerColor);
        lineRenderer.material = newMat;

        //positions.Add(startPosition);
    }
    void Show()
    {
        lineRenderer.enabled = true;
    }

    Vector3 PointOnParabola(float time)
    {
        Vector3 pos = startPosition + (muzzleVelocity * time * startDir);
        Vector3 gravityVector = Vector3.down *(gravity * time * time);
        return pos + gravityVector; 

    }
    bool RayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
    }
    private void Update()
    {
        if (startTime < 0) return;

        float currentTime = Time.time - startTime;
        Vector3 currentPoint = PointOnParabola(currentTime);



        transform.position = currentPoint;
    }
    void FixedUpdate()
    {
        distFromOrigin = (transform.position - startPosition).magnitude;
        //curDropAmount = dropCurve.Evaluate(distFromOrigin);


        if (startTime < 0) startTime = Time.time;
        float currentTime = Time.time - startTime;
        float nextTime = currentTime + Time.fixedDeltaTime;

        Vector3 currentPoint = PointOnParabola(currentTime);
        Vector3 nextPoint = PointOnParabola(nextTime);

        transform.LookAt(nextPoint);

        RaycastHit hit;

        if (RayBetweenPoints(currentPoint, nextPoint, out hit))
        {
            if (!hit.collider.CompareTag("Round"))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    GameObject g = Instantiate(concreteHit, hit.point, Quaternion.Euler(nextPoint - currentPoint));
                    g.transform.parent = null;
                }

                if (hit.collider.CompareTag("Enemy"))
                {

                    Instantiate(bloodHit, hit.point, Quaternion.Euler(nextPoint - currentPoint));
                    hit.collider.gameObject.GetComponent<Enemy>().Hit(Random.Range(damage.x, damage.y), hit.point);
                }
                if (hit.collider.CompareTag("EnemyHead"))
                {
                    hit.collider.gameObject.GetComponent<Head>().Hit();

                }
                if (hit.collider.CompareTag("Player"))
                {
                    Transform player = GameManager.GM.player.transform;
                    Vector3 hitPoint = player.position;
                    hitPoint.y += 1.75f;
                    hitPoint += 0.5f * player.forward;
                    Instantiate(bloodHit, hitPoint, Quaternion.Euler(nextPoint - currentPoint));

                    hit.collider.gameObject.GetComponent<Player>().Hit(Random.Range(damage.x, damage.y));
                }
                if (hit.collider.CompareTag("Target"))
                {
                    AudioSource g = hit.collider.gameObject.GetComponent<AudioSource>();
                    g.PlayOneShot(g.clip);
                }
                Destroy(gameObject);
                //Debug.Log(hit.collider.gameObject.name);
            }
        }

        //Vector3 tgt = new()
        //{
        //    z = distFromOrigin + muzzleVelocity,
        //    y = startPosition.y - curDropAmount
        //};
        //transform.SetLocalPositionAndRotation(tgt, transform.localRotation);



        //velocity.y = (curDropAmount - lastDropAmount);

        //velocity.z *= -(distFromOrigin / maxDist)/100;
        //Debug.Log(distFromOrigin / maxDist);

        //rb.MovePosition(velocity);
        //rb.AddRelativeForce(velocity, ForceMode.VelocityChange);
        //curPoint.y += dropAmount;

        //velocity.z -= 0.01f;
        //Vector3.ClampMagnitude(velocity, muzzleVelocity);

        //curPoint += (velocity.z * transform.forward)+(velocity.y * transform.up);
        //curPoint.y += dropAmount/1000;




        //float velocityReduction = Mathf.InverseLerp(0, maxDist, distFromOrigin);


        //velocity.z *= velocityReduction / 100;

        //velocity.z *= velocityReduction;


        //lastPoint = transform.position;

        if (distFromOrigin > despawnDist)
        {
            //Debug.Log(distFromOrigin);
            Destroy(gameObject);
        }

        positions.Add(transform.position);
        //transform.position = curPoint;
    }
    private void OnDrawGizmos()
    {
        if (Camera.current != Camera.main) return;
        Gizmos.DrawLineStrip(positions.ToArray(), false);


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().BulletWhiz();
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Destroy(gameObject);
    //}
}
