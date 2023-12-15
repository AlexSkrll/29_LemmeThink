using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    public GameObject tezaBodyObject; 
    public GameObject tezaArmObject; 

    private Animator tezaBody; 
    private Animator tezaArm; 

           
    void Start()
    {
        tezaBody = tezaBodyObject.GetComponent<Animator>();
        tezaArm = tezaArmObject.GetComponent<Animator>();
    }

    void Update()
    {
        SyncAnimations();
    }

    void SyncAnimations()
    {
   
        float moveX = tezaBody.GetFloat("moveX");
        float moveY = tezaBody.GetFloat("moveY");
        bool bodyIsMoving = tezaBody.GetBool("isMoving");
        float timeSinceLastMovement = tezaBody.GetFloat("timeSinceLastMovement");
    

        tezaArm.SetFloat("armmoveX", moveX);
        tezaArm.SetFloat("armmoveY", moveY);
        tezaArm.SetBool("isMoving", bodyIsMoving);
        tezaArm.SetFloat("timeSinceLastMovement", timeSinceLastMovement);
        
    }
    
}