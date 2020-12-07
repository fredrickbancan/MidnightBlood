using System.Collections;
using UnityEngine;

public class NPCAnimations : MonoBehaviour
{
    private Animator animationController;
    private VillagerControllerAlpha villagerController;
    private AudioSource audioSource;

    // Start is called before the first frame update

    private void Start()
    {
        animationController = this.gameObject.GetComponentInParent<Animator>();
        villagerController = this.gameObject.GetComponentInParent<VillagerControllerAlpha>();
 //       audioSource = this.gameObject.GetComponentInParent<AudioSource>();

    }
    // Update is called once per frame
    void Update()
    {

        Debug.Log("Checking for Villager Movement" + villagerController.isWalkingFowards);
        if (villagerController.isWalkingFowards && (animationController.GetBool("Walking") == false))
        {
            Debug.Log("change to walking");
            this.gameObject.GetComponentInParent<Animator>().SetBool("Walking", true);
//            audioSource.Play();

        }
        else if (!villagerController.isWalkingFowards && (animationController.GetBool("Walking") == true))
        {
            Debug.Log("change to idle");
            this.gameObject.GetComponentInParent<Animator>().SetBool("Walking", false);
 //           audioSource.Stop();

        }
    }
}
