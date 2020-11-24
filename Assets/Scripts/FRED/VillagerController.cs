using UnityEngine;

public class VillagerController : MonoBehaviour
{
    public Rigidbody villagerBody;
    public Transform villagerTransform;
    public LineRenderer lr;
    public enum VillagerState
    {
        WANDERING,
        FLEEING
    };
    public VillagerState state = VillagerState.WANDERING;
    public float walkSpeed;
    public float maxVel;
    /// <summary>
    /// Chance for the villager to walk fowards each update while wandering (out of 100)
    /// </summary>
    public float walkChance = 1.0F;

    /// <summary>
    /// Chance for the villaget to stop walking fowards each update while wandering (out of 100, only if the villager is walking fowards.)
    /// </summary>
    public float walkStopChance = 25.0F;

    /// <summary>
    /// Chance for the villager to rotate while wandering (out of 100, only if villager is not walking fowards)
    /// </summary>
    public float rotateChance = 1.0F;

    /// <summary>
    /// Chance for the villager to change rotation while wandering (out of 100, only if villager is rotating)
    /// </summary>
    public float rotateChangeChance = 1.0F;

    /// <summary>
    /// Chance for the villager to stop rotating (out of 100, only if rotating already)
    /// </summary>
    public float rotateStopChance = 25.0F;

    /// <summary>
    /// How much the villager rotates when randomly rotating
    /// </summary>
    public float rotationAmount = 15.0F;

    public bool walking { get => isWalkingFowards; }
    private bool isWalkingFowards;

    public bool rotating { get => isRotating; }
    private bool isRotating = false;

    public bool drawDebugVectors = true;
    /// <summary>
    /// true if villager is rotating right, else rotating left.
    /// </summary>
    private bool rotRight;

    /// <summary>
    /// Direction that the villager is currently traveling in. Changes each time the villager randomly rotates, and when they are fleeing the player.
    /// </summary>
    private Vector3 travelDirection;

    // Start is called before the first frame update
    void Start()
    {
        rotRight = Random.Range(0, 2) == 0;//50% chance
        isWalkingFowards = Random.Range(0, 2) == 0;//50% chance
        travelDirection = villagerTransform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //drawing debug frontvector
        if(drawDebugVectors)
        {
            lr.positionCount = 2;
            lr.startColor = Color.green;
            lr.endColor = Color.black;
            lr.SetPosition(0, villagerTransform.position);
            lr.SetPosition(1, villagerTransform.forward * 10 + villagerTransform.position);
        }
        handleState();
    }

    private void handleState()
    {
        switch(state)
        {
            case VillagerState.WANDERING:
                {
                    doStateWandering();
                }
                break;
            case VillagerState.FLEEING:
                {

                }
                break;
        }
    }

    private void doStateWandering()
    {
        if (!isWalkingFowards)
            isWalkingFowards = Random.Range(0.0F, 100.0F) <= walkChance;

        if (isWalkingFowards)
        {
            directAwayFromObstacles();

            villagerBody.velocity += villagerTransform.forward * walkSpeed;

            if (Random.Range(0.0F, 100.0F) <= walkStopChance)
            {
                isWalkingFowards = false;
            }
        }
        else
        {
            if(!isRotating && Random.Range(0.0F,100.0F) <= rotateChangeChance)
            {
                rotRight = !rotRight;
            }
            isRotating = Random.Range(0.0F, 100.0F) <= rotateChance;
            if(isRotating)
            {
                Quaternion rot = villagerTransform.rotation;
                Vector3 euler = villagerTransform.rotation.eulerAngles;
                euler.y += rotRight ? rotationAmount : -rotationAmount;
                rot.eulerAngles = euler;
                villagerTransform.rotation = rot;
                travelDirection = villagerTransform.forward;
            }
        }



        float prevy = villagerBody.velocity.y;
        villagerBody.velocity = Vector3.ClampMagnitude(new Vector3(villagerBody.velocity.x, 0, villagerBody.velocity.z), maxVel);
        villagerBody.velocity = new Vector3(villagerBody.velocity.x, prevy, villagerBody.velocity.z);

    }

    /// <summary>
    /// This function is for detecting if the villager is about to walk into an obstacle, if so, it will attempt to direct the villager around it.
    /// </summary>
    private void directAwayFromObstacles()
    {
        //detect if travel detection is obstructed
        //if so, try to rotate purpendicular to obstruction normal. (only in x,z axiees)
        //if travel direction is not obstructed, rotate towards travel direction.
        Ray travelRay = new Ray(villagerTransform.position, travelDirection);
        RaycastHit info;
        if (Physics.Raycast(travelRay, out info, 10.0F) && info.collider != this.GetComponent<Collider>())
        {
            Vector3 avoidanceDir = Vector3.Normalize(new Vector3(info.normal.z,0, -info.normal.x));
            villagerTransform.forward = Vector3.RotateTowards(villagerTransform.forward, avoidanceDir, rotationAmount * (3.14159F / 180.0F), 0.0F);
            if (drawDebugVectors)
            {
                lr.startColor = Color.red;
                lr.endColor = Color.black;
            }
        }
        else
        {

            if (drawDebugVectors)
            {
                lr.startColor = Color.blue;
                lr.endColor = Color.black;
            }

            villagerTransform.forward = Vector3.RotateTowards(villagerTransform.forward, travelDirection, rotationAmount * (3.14159F / 180.0F), 0.0F);
        }
    }
}