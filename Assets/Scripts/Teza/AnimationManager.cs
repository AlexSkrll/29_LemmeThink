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
        bool IsMoving = tezaBody.GetBool("isMoving");
        float timeSinceLastMovement = tezaBody.GetFloat("timeSinceLastMovement");
        bool isAiming = tezaBody.GetBool("isAiming"); 
    

        tezaArm.SetFloat("armmoveX", moveX);
        tezaArm.SetFloat("armmoveY", moveY);
        tezaArm.SetBool("isMoving", IsMoving);
        tezaArm.SetFloat("timeSinceLastMovement", timeSinceLastMovement);
        tezaArm.SetBool("isAiming", isAiming);
        
        
    }
    
}