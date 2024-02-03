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
        // TODO: Move all of thiss in Teza Controller and delete script!
        float moveX = tezaBody.GetFloat("moveX");
        float moveY = tezaBody.GetFloat("moveY");
        bool IsMoving = tezaBody.GetBool("isMoving");
        float timeSinceLastMovement = tezaBody.GetFloat("timeSinceLastMovement");
        bool isAiming = tezaBody.GetBool("isAiming");


        if (moveX != 0 && moveY != 0)
        {
            moveX = 0;
        }
        tezaArm.SetFloat("armmoveX", moveX);
        tezaArm.SetFloat("armmoveY", moveY);



        tezaArm.SetBool("isMoving", IsMoving);
        tezaArm.SetFloat("timeSinceLastMovement", timeSinceLastMovement);
        tezaArm.SetBool("isAiming", isAiming);


    }

}