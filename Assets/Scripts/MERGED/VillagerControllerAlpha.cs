using UnityEngine;

public class VillagerControllerAlpha : MonoBehaviour
{
    public Rigidbody villagerBody;
    public Transform villagerTransform;
    public LineRenderer lrPrefab;
    private LineRenderer frontVecLine;
    private LineRenderer fovLeftLine;
    private LineRenderer fovRightLine;
    public enum VillagerState
    {
        WANDERING,
        FLEEING
    };
    public VillagerState state = VillagerState.WANDERING;
    public float walkSpeed = 2.5f;
    public float maxVel = 5.0f;

    public float fleeingMaxVelModifier = 3.0F;

    public float fovAngle = 60.0F;

    public float viewDistance = 10.0F;

    /// <summary>
    /// The distance the villager must be from the player and out of line of sight to despawn and spawn an authority.
    /// </summary>
    public float spawnAuthorityDistance = 100.0F;

    /// <summary>
    /// Chance for the villager to walk fowards each update while wandering (out of 100)
    /// </summary>
    public float walkChance = 0.35F;

    /// <summary>
    /// Chance for the villaget to stop walking fowards each update while wandering (out of 100, only if the villager is walking fowards.)
    /// </summary>
    public float walkStopChance = 0.075F;

    /// <summary>
    /// Chance for the villager to rotate while wandering (out of 100, only if villager is not walking fowards)
    /// </summary>
    public float rotateChance = 25.0F;

    /// <summary>
    /// Chance for the villager to change rotation while wandering (out of 100, only if villager is rotating)
    /// </summary>
    public float rotateChangeChance = 0.25F;

    /// <summary>
    /// Chance for the villager to stop rotating (out of 100, only if rotating already)
    /// </summary>
    public float rotateStopChance = 0.15F;

    /// <summary>
    /// How much the villager rotates when randomly rotating
    /// </summary>
    public float rotationAmount = 1.0F;

    public bool walking { get => isWalkingFowards; }
    public bool isWalkingFowards;

    public bool rotating { get => isRotating; }
    private bool isRotating = false;

    /// <summary>
    /// if this bool is true, this villager will traverse around objects left side, otherwise will go around objects right side
    /// </summary>
    private bool avoidLeft;

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
        avoidLeft = Random.Range(0, 2) == 0;//50% chance
        travelDirection = villagerTransform.forward;

        if (drawDebugVectors)
        {
            frontVecLine = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
            fovLeftLine = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
            fovRightLine = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {

        if (drawDebugVectors)
        {
            Destroy(frontVecLine);
            Destroy(fovLeftLine);
            Destroy(fovRightLine);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManagerAlpha.instance.paused)
        {
            //drawing debug frontvector
            if (drawDebugVectors)
            {
                frontVecLine.positionCount = 2;
                frontVecLine.startColor = Color.green;
                frontVecLine.endColor = Color.black;
                frontVecLine.SetPosition(0, villagerTransform.position + villagerTransform.GetComponent<BoxCollider>().center);
                frontVecLine.SetPosition(1, villagerTransform.forward * 10 + villagerTransform.position + villagerTransform.GetComponent<BoxCollider>().center);
                float halfAngle = fovAngle * 0.5F;
                float villagerAngle = villagerTransform.eulerAngles.y;
                float leftAngle = villagerAngle - halfAngle;
                float rightAngle = villagerAngle + halfAngle;
                Vector3 fovLeftEdge = new Vector3(Mathf.Sin(leftAngle * Mathf.Deg2Rad), 0, Mathf.Cos(leftAngle * Mathf.Deg2Rad));
                Vector3 fovRightEdge = new Vector3(Mathf.Sin(rightAngle * Mathf.Deg2Rad), 0, Mathf.Cos(rightAngle * Mathf.Deg2Rad));
                fovLeftEdge.Normalize();
                fovRightEdge.Normalize();
                fovLeftLine.positionCount = 2;
                fovLeftLine.startColor = Color.cyan;
                fovLeftLine.endColor = Color.white;
                fovLeftLine.SetPosition(0, villagerTransform.position + villagerTransform.GetComponent<BoxCollider>().center);
                fovLeftLine.SetPosition(1, fovLeftEdge * viewDistance + villagerTransform.position + villagerTransform.GetComponent<BoxCollider>().center);
                fovRightLine.positionCount = 2;
                fovRightLine.startColor = Color.cyan;
                fovRightLine.endColor = Color.white;
                fovRightLine.SetPosition(0, villagerTransform.position + villagerTransform.GetComponent<BoxCollider>().center);
                fovRightLine.SetPosition(1, fovRightEdge * viewDistance + villagerTransform.position + villagerTransform.GetComponent<BoxCollider>().center);
            }


            handleState();

            float prevy = villagerBody.velocity.y;
            villagerBody.velocity = Vector3.ClampMagnitude(new Vector3(villagerBody.velocity.x, 0, villagerBody.velocity.z), state == VillagerState.FLEEING ? maxVel * fleeingMaxVelModifier : maxVel);
            villagerBody.velocity = new Vector3(villagerBody.velocity.x, prevy, villagerBody.velocity.z);
        }
    }

    private void handleState()
    {
        switch(state)
        {
            case VillagerState.WANDERING:
                {
                    DoStateWandering();
                    CheckToRunFromPlayer();
                }
                break;
            case VillagerState.FLEEING:
                {
                    DoStateFleeing();
                }
                break;
        }
    }

    private void DoStateWandering()
    {

        if (!isWalkingFowards)
            isWalkingFowards = Random.Range(0.0F, 100.0F) <= walkChance;

        if (isWalkingFowards)
        {
            DirectAwayFromObstacles();

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
    }

    private void CheckToRunFromPlayer()
    {
        if (!GameManagerAlpha.instance.IsPlayerBloody())
        {
            // return;
        }

        if (Vector3.Distance(GameManagerAlpha.instance.GetPlayerPos(), villagerTransform.position) >= viewDistance)
        {
            return;
        }
        Vector3 playerToVillager = villagerTransform.position - GameManagerAlpha.instance.GetPlayerPos();
        playerToVillager.y = 0;
        playerToVillager.Normalize();

        float halfAngle = fovAngle * 0.5F;
        float villagerAngle = villagerTransform.eulerAngles.y;
        float leftAngle = villagerAngle - halfAngle;
        float rightAngle = villagerAngle + halfAngle;
        Vector3 fovLeftEdge = new Vector3(Mathf.Sin(leftAngle * Mathf.Deg2Rad), 0, Mathf.Cos(leftAngle * Mathf.Deg2Rad));
        Vector3 fovRightEdge = new Vector3(Mathf.Sin(rightAngle * Mathf.Deg2Rad), 0, Mathf.Cos(rightAngle * Mathf.Deg2Rad));
        Vector3 fovLeftEdgeNormal = new Vector3(fovLeftEdge.z, 0, -fovLeftEdge.x);
        Vector3 fovRightEdgeNormal = new Vector3(-fovRightEdge.z, 0, fovRightEdge.x);
        if (Vector3.Dot(playerToVillager, fovLeftEdgeNormal) < 0 && Vector3.Dot(playerToVillager, fovRightEdgeNormal) < 0)
        {
            //SET STATE TO FLEEING, PLAYER IS DETECTED.
            state = VillagerState.FLEEING;
            GameManagerAlpha.instance.OnVillagerAlerted();
            if (drawDebugVectors)
            {
                fovLeftLine.startColor = Color.red;
                fovRightLine.startColor = Color.red;
            }
        }
    }


    private void DoStateFleeing()
    {
        if(CheckToSpawnAuthority())
        {
            GameManagerAlpha.instance.OnVillagerEscape(villagerTransform.position);
            Destroy(gameObject);//spawned an authority and this villager has escaped. Despawning villager.
            if(drawDebugVectors)
            {
                Destroy(frontVecLine);
                Destroy(fovLeftLine);
                Destroy(fovRightLine);
            }
            return;
        }

        if (drawDebugVectors)
        {
            fovLeftLine.startColor = Color.red;
            fovRightLine.startColor = Color.red;
        }

        isWalkingFowards = true;
        isRotating = false;
        Vector3 playerToVillager = villagerTransform.position - GameManagerAlpha.instance.GetPlayerPos();
        playerToVillager.y = 0;
        travelDirection = playerToVillager.normalized;
        DirectAwayFromObstacles();
        villagerBody.velocity += villagerTransform.forward * walkSpeed;
    }

    /// <summary>
    /// returns true if this villager is far away enough from player and does not have line of sight with player. 
    /// </summary>
    private bool CheckToSpawnAuthority()
    {
        float dist = 0;
        if((dist = Vector3.Distance(GameManagerAlpha.instance.GetPlayerPos(), villagerTransform.position)) >= spawnAuthorityDistance)
        {
            Ray lineOfSight = new Ray(villagerTransform.position, GameManagerAlpha.instance.GetPlayerPos() - villagerTransform.position);
            RaycastHit info;
            if (Physics.Raycast(lineOfSight, out info, dist))
            {
                if(info.collider != this.GetComponent<Collider>() && info.collider != GameManagerAlpha.instance.GetPlayer().GetComponent<Collider>())
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// This function is for detecting if the villager is about to walk into an obstacle, if so, it will attempt to direct the villager around it.
    /// </summary>
    private void DirectAwayFromObstacles()
    {
        //detect if travel detection is obstructed
        //if so, try to rotate purpendicular to obstruction normal. (only in x,z axiees)
        //if travel direction is not obstructed, rotate towards travel direction.
        Ray travelRay = new Ray(villagerTransform.position, travelDirection);
        RaycastHit info;
        if (Physics.Raycast(travelRay, out info, 10.0F) && info.collider != this.GetComponent<Collider>())
        {
            Vector3 avoidanceDir = avoidLeft ? Vector3.Normalize(new Vector3(info.normal.z,0, -info.normal.x)) : Vector3.Normalize(new Vector3(-info.normal.z, 0, info.normal.x));
            villagerTransform.forward = Vector3.RotateTowards(villagerTransform.forward, avoidanceDir, rotationAmount * (3.14159F / 180.0F), 0.0F);
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

            villagerTransform.forward = Vector3.RotateTowards(villagerTransform.forward, travelDirection, rotationAmount * (3.14159F / 180.0F), 0.0F);
        }
    }
}