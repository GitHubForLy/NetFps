using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game.Player;

namespace Game.AI
{

    [RequireComponent(typeof(Health))]
    public class AIController : MonoBehaviour
    {
        private Health aiHealth;

        private Animator animator;


        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            aiHealth = GetComponent<Health>();

            //aiHealth.OnDied += AiHealth_OnDied;
            //aiHealth.OnRevive += AiHealth_OnRevive;
        }

        private void AiHealth_OnRevive()
        {
            EnableInput();
        }

        private void AiHealth_OnDied(Behaviour damSender, Behaviour sender)
        {
            DisableInput();
        }




        private void DisableInput()
        {
            GetComponent<AIAnimation>().enabled = false;
            GetComponentInChildren<AIListen>().enabled = false;
            GetComponent<AINavigntion>().enabled = false;
            GetComponent<AIShoot>().enabled = false;
            GetComponentInChildren<NavMeshAgent>().enabled = false;
            GetComponentInChildren<Light>().enabled = false;
            GetComponentInChildren<LineRenderer>().enabled = false;
            GetComponentInChildren<CapsuleCollider>().enabled = false;

            animator.SetBool(AnimatorParametars.Died, true);
            animator.SetBool(AnimatorParametars.SightEnemy, false);
        }

        private void EnableInput()
        {
            GetComponent<AIAnimation>().enabled = true;
            GetComponentInChildren<AIListen>().enabled = true;
            GetComponent<AINavigntion>().enabled = true;
            GetComponent<AIShoot>().enabled = true;
            GetComponentInChildren<NavMeshAgent>().enabled = true;
            GetComponentInChildren<Light>().enabled = true;
            GetComponentInChildren<LineRenderer>().enabled = true;
            GetComponentInChildren<CapsuleCollider>().enabled = true;

            animator.SetBool(AnimatorParametars.Died, false);
            //animator.SetBool(AnimatorParametars.SightEnemy, true);
        }
    }

}
