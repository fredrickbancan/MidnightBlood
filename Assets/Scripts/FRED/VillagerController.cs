using UnityEngine;

public class VillagerController : MonoBehaviour
{
    public Rigidbody villagerBody;
    public Transform villagerTransform;
    public float walkSpeed;
    public float maxVel;
    /// <summary>
    /// Chance for the villager to walk fowards each update (out of 100)
    /// </summary>
    public float walkChance = 1.0F;

    /// <summary>
    /// Chance for the villaget to stop walking fowards each update (out of 100, only if the villager is walking fowards.)
    /// </summary>
    public float walkStopChance = 25.0F;
    public bool isWalkingFowards = false;
    public bool isRotatingLeft = false;
    public bool isRotatingRight = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!isWalkingFowards)
        isWalkingFowards = Random.Range(0.0F, 100.0F) <= walkChance;
        if (isWalkingFowards)
        {
            villagerBody.velocity += villagerTransform.forward * walkSpeed;
            if(Random.Range(0.0F, 100.0F) <= walkStopChance)
            {
                isWalkingFowards = false;
            }
        }



        float prevy = villagerBody.velocity.y;
        villagerBody.velocity = Vector3.ClampMagnitude(new Vector3(villagerBody.velocity.x, 0, villagerBody.velocity.z), maxVel);
        villagerBody.velocity = new Vector3(villagerBody.velocity.x, prevy, villagerBody.velocity.z);
    }
}