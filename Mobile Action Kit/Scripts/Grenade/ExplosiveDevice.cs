using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ExplosiveDevice : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script contains parameters  of the explosion.";

        [Tooltip("Drag and drop the Explosive gameObject from the hierarchy into this field to deactive when it will explode.")]
        public GameObject ExplosiveMesh;

        [Tooltip("Drag and drop Explosive`s child VFX explosion prefab into this field.")]
        public ParticleSystem ExplosionVfx;

        [Tooltip("Drag and drop explosion sound from the project tab.")]
        public AudioSource ExplosionSound;

        [Tooltip("Specify delay before explosion.")]
        public float DelayExplosion = 3f;

        float Timer;

        [HideInInspector]
        public bool CanStartExplosion = false;
        float DestroyTimer;

        [HideInInspector]
        public bool CanDestroy = false;

        bool pool = false;
        bool CanActivateRadius = false;

        [HideInInspector]
        public bool Explode = false;

        [Tooltip("Drag and drop the component called 'Kill radius' from the hierarchy into this field.")]
        public ExplosiveDamage KillRadius;
        [Tooltip("Drag and drop the component called 'Injury radius')from the hierarchy into this field.")]
        public ExplosiveDamage InjuryRadius;

        [Tooltip("Drag and drop the gameobject component called 'Explosive Notificator' with the script called 'Explosive Notificator' from the Explosive hierarchy into this field.")]
        public ExplosiveNotificator ExplosiveNotificator;

        [Tooltip("If enabled than Ai agent may or may not react to the incoming Explosive depending on the situation and in case the Explosive alert behaviour is activated on that Ai agent or not." +
            "If enabled then 'ExplosiveNotificator'  will be activated only after Explosive is landed on the surface.")]
        public bool ExplosiveNotificatorOnGroundOnly = true;

        [Tooltip("Minimum time to wait before activating ExplosiveNotificator radius.")]
        public float MinTimeToActivateExplosiveNotificator = 1f;
        [Tooltip("Maximum time to wait before activating ExplosiveNotificator radius.")]
        public float MaxTimeToActivateExplosiveNotificator = 3f;

        [Tooltip("Drag and drop game objet named 'ExplosiveSoundAlertRadius' with Sound Manager Script attached to it  from the Explosive hierarchy into this field.")]
        public AlertingSoundActivator ExplosiveSoundAlertRadius;

        [Tooltip("If checked than the explosion timer will start from the launch of the Explosive, but if unchecked than the timer will start only upon landing of the Explosive.")]
        public bool CanExplodeInAir = false;

        bool DeactiveOnce = false;
        float AiAvoidanceActivationTimer;

        [HideInInspector]
        public bool IsCollided = false;

        [HideInInspector]
        public List<Transform> EnemiesGotHitByDamage = new List<Transform>();

        [HideInInspector]
        public List<Transform> TargetsToApplyForce = new List<Transform>();

        private void OnEnable()
        {
            AiAvoidanceActivationTimer = Random.Range(MinTimeToActivateExplosiveNotificator, MaxTimeToActivateExplosiveNotificator);
            Explode = false;
            CanActivateRadius = false;
            DestroyTimer = ExplosionVfx.main.duration;
            DeactiveOnce = false;

            
            KillRadius.gameObject.SetActive(false);
            InjuryRadius.gameObject.SetActive(false);

            if(ExplosiveNotificator != null)
            {
                ExplosiveNotificator.gameObject.SetActive(false);
            }


            //for (int x = 0; x < ObjectsToDeactivateOnExplode.Length; x++)
            //{
            //    ObjectsToDeactivateOnExplode[x].SetActive(true);
            //}
            if (ExplosiveNotificatorOnGroundOnly == false)
            {
                StartCoroutine(Coro());
            }
            TargetsToApplyForce.Clear();
        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(AiAvoidanceActivationTimer);
            if (ExplosiveNotificator != null)
            {
                ExplosiveNotificator.gameObject.SetActive(true);
            }
        }
        private void Update()
        {
            Timer += Time.deltaTime;
            //Debug.Log(Timer);
            if (Timer > DelayExplosion)
            {
                Explode = true;

                if (DeactiveOnce == false)
                {
                    GrenadeExplosionStarts();
                    DeactiveOnce = true;
                }

            }
        }
        //private void OnTriggerEnter(Collider col)
        //{
        //    if (col.gameObject.transform.root.tag == "AI")
        //    {
        //        if (col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>() != null)
        //        {             
        //            col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>().BotMovingAway = true;
        //        }
        //    }
        //}
        private void OnCollisionEnter(Collision col)
        {
            IsCollided = true;
            // before this was uncommented 
            //if (GetComponent<GrenadePhysics>() == null) 
            //{
            //    CanStartExplosion = true;
            //    GrenadeExplosionStarts();
            //}


            // Added this code so that the same grenades explosion scripts can be useful in the case of Jet air support .
            // when the jet drop the bombs we make sure that it does not take other bombs collision and do not get explode in air and to avoid this we add this check here.
            if (col.gameObject.GetComponent<ExplosiveDevice>() == null)
            {
                CanStartExplosion = true;
                GrenadeExplosionStarts();
            }
        }
        public void GrenadeExplosionStarts()
        {
            if (CanExplodeInAir == false && CanStartExplosion == true)
            {
                ExplodeFunctionality();
            }
            else if (CanExplodeInAir == true)
            {
                ExplodeFunctionality();
            }
        }
        public void ExplodeFunctionality()
        {
            if (ExplosiveNotificatorOnGroundOnly == true)
            {
                if (ExplosiveNotificator != null)
                {
                    ExplosiveNotificator.gameObject.SetActive(true);
                }
            }
            if (Explode == true)
            {
                if(CanActivateRadius == false)
                {
                    ExplosiveSoundAlertRadius.ActivateNoiseHandler(transform);
                    StartCoroutine(ActivateDamageRadius());
                    CanActivateRadius = true;
                }

                //for (int x = 0; x < ObjectsToDeactivateOnExplode.Length; x++)
                //{
                //    ObjectsToDeactivateOnExplode[x].SetActive(false);
                //}


            }
        }
        IEnumerator ActivateDamageRadius()
        {
            KillRadius.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            InjuryRadius.gameObject.SetActive(true);
            ExplosionSound.gameObject.SetActive(true);
            ExplosionSound.Play();
            if (CanDestroy == true)
            {
                ExplosionVfx.gameObject.SetActive(true);
                ExplosiveMesh.SetActive(false);
                Destroy(transform.root.gameObject, DestroyTimer);
            }
            else
            {
                if (pool == false)
                {
                    ExplosionVfx.gameObject.SetActive(true);
                    ExplosiveMesh.SetActive(false);
                    StartCoroutine(Pooling());
                    pool = true;
                }
            }
        }
        //private void OnTriggerStay(Collider col)
        //{
        //    if (CanStartExplosion == true && CanExplodeInAir == false)
        //    {   
        //        if (Explode == true)
        //        {
        //            if (CanPlaySounds == false)
        //            {
        //                //if (col.gameObject.transform.root.tag == "Player")
        //                //{
        //                //    col.gameObject.transform.root.GetComponent<Health>().PlayerHealthbar.Curvalue -= DamageToTarget;
        //                //}
        //                //if (col.gameObject.transform.root.tag == "AI")
        //                //{


        //                //    col.gameObject.transform.root.SendMessage("Takedamage", DamageToTarget);
        //                //}

        //                ExplosionSound.Play();
        //                CanPlaySounds = true;
        //            }


        //            if(CanDestroy == true)
        //            {
        //                Explosion.gameObject.SetActive(true);
        //                GrenadeMesh.SetActive(false);
        //                Destroy(transform.root.gameObject, DestroyTimer);
        //            }
        //            else
        //            {
        //                if(pool == false)
        //                {
        //                    Explosion.gameObject.SetActive(true);
        //                    GrenadeMesh.SetActive(false);
        //                    StartCoroutine(Pooling());
        //                    pool = true;
        //                }

        //            }

        //        }


        //    }
        //    //if (col.gameObject.transform.root.tag == "AI")
        //    //{
        //    //    if (col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>() != null)
        //    //    {
        //    //        col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>().BotMovingAway = true;
        //    //    }
        //    //}
        //}
        //private void OnTriggerExit(Collider col)
        //{
        //    if (col.gameObject.transform.root.tag == "AI")
        //    {
        //        if (col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>() != null)
        //        {
        //            StartCoroutine(col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>().RunTimer());
        //        }
        //    }
        //}
        IEnumerator Pooling()
        {
            yield return new WaitForSeconds(DestroyTimer);
            ExplosionVfx.gameObject.SetActive(false);
            ExplosiveMesh.SetActive(true);
            CanStartExplosion = false;
            Timer = 0;
            gameObject.transform.root.gameObject.SetActive(false);
            pool = false;
            gameObject.SetActive(false);
            KillRadius.gameObject.SetActive(false);
            InjuryRadius.gameObject.SetActive(false);
            if (ExplosiveNotificator != null)
            {
                ExplosiveNotificator.AiInMyList.Clear();
                ExplosiveNotificator.gameObject.SetActive(false);
            }
            Explode = false;
            DeactiveOnce = false;
            IsCollided = false;

            if (GetComponent<Rigidbody>() != null)
            {
                DestroyImmediate(GetComponent<Rigidbody>());
            }
        }
    }
}