using UnityEngine;

public class AuthorityControllerAlpha : MonoBehaviour
{
    public float rotationAmount = 1.0F;
    public Rigidbody authorityBody;
    public Transform authorityTransform;
    public LineRenderer lrPrefab;
    private LineRenderer frontVecLine;
    public bool drawDebugVectors = true;
    private bool avoidLeft;
    private Vector3 travelDirection;

    public float walkSpeed = 8.0F;
    public float maxVel = 6.0F;
    public float playerCaptureDistance = 3.0F;
    // Start is called before the first frame update
    void Start()
    {
        if (drawDebugVectors)
            frontVecLine = Instantiate(lrPrefab);

        avoidLeft = Random.Range(0, 2) == 0;
    }

    // Update is called once per frame
    void Update()
    {
        //drawing debug frontvector
        if (drawDebugVectors)
        {
            frontVecLine.positionCount = 2;
            frontVecLine.startColor = Color.green;
            frontVecLine.endColor = Color.black;
            frontVecLine.SetPosition(0, authorityTransform.position);
            frontVecLine.SetPosition(1, authorityTransform.forward * 10 + authorityTransform.position);
        }
        travelDirection = GameManagerAlpha.instance.GetPlayerPos() - authorityTransform.position;
        travelDirection.y = 0;
        travelDirection.Normalize();
        DirectAwayFromObstacles();
        authorityBody.velocity += authorityTransform.forward * walkSpeed;

        float prevy = authorityBody.velocity.y;
        authorityBody.velocity = Vector3.ClampMagnitude(new Vector3(authorityBody.velocity.x, 0, authorityBody.velocity.z), maxVel);
        authorityBody.velocity = new Vector3(authorityBody.velocity.x, prevy, authorityBody.velocity.z);
        TestPlayerDetection();
    }

    /// <summary>
    /// Casts a ray of length playerCaputreDistance, if this ray touches the player collider, they will be captured.
    /// </summary>
    private void TestPlayerDetection()
    {
        Ray travelRay = new Ray(authorityTransform.position, authorityTransform.forward);
        RaycastHit info;
        if (Physics.Raycast(travelRay, out info, playerCaptureDistance) && info.collider == GameManagerAlpha.instance.GetPlayer().GetComponent<Collider>())
        {
            GameManagerAlpha.instance.OnPlayerCaptured();
        }
    }

    /// <summary>
    /// This function is for detecting if the villager is about to walk into an obstacle, if so, it will attempt to direct the villager around it.
    /// </summary>
    private void DirectAwayFromObstacles()
    {
        //detect if travel detection is obstructed
        //if so, try to rotate purpendicular to obstruction normal. (only in x,z axiees)
        //if travel direction is not obstructed, rotate towards travel direction.
        Ray travelRay = new Ray(authorityTransform.position, travelDirection);
        RaycastHit info;
        if (Physics.Raycast(travelRay, out info, 10.0F) && info.collider != this.GetComponent<Collider>() && info.collider != GameManagerAlpha.instance.GetPlayer().GetComponent<Collider>())
        {
            Vector3 avoidanceDir = avoidLeft ? Vector3.Normalize(new Vector3(info.normal.z, 0, -info.normal.x)) : Vector3.Normalize(new Vector3(-info.normal.z, 0, info.normal.x));
            authorityTransform.forward = Vector3.RotateTowards(authorityTransform.forward, avoidanceDir, rotationAmount * (3.14159F / 180.0F), 0.0F);
            if (drawDebugVectors)
            {
                frontVecLine.startColor = Color.red;
                frontVecLine.endColor = Color.black;
            }
        }
        else
        {

            if (drawDebugVectors)
            {
                frontVecLine.startColor = Color.blue;
                frontVecLine.endColor = Color.black;
            }

            authorityTransform.forward = Vector3.RotateTowards(authorityTransform.forward, travelDirection, rotationAmount * (3.14159F / 180.0F), 0.0F);
        }
    }
}
