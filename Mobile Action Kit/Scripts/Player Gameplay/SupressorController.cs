using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class SupressorController : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script modify 'Player Weapon Script Properties' when the supressor is enabled on this weapon.";

        [Tooltip("Reference to the PlayerWeapon script to modify its shooting features.")]
        public PlayerWeapon PlayerWeaponScript;

        [Tooltip("Toggle to use a muzzle mesh instead of muzzle flash particles.")]
        public bool UseMuzzleMesh = false;

        [Tooltip("The GameObject representing the muzzle flash mesh to be displayed during firing.")]
        public GameObject MuzzleMesh;

        [Tooltip("The particle system used for the muzzle flash effect when not using a muzzle mesh.")]
        public ParticleSystem MuzzleFlashParticles;

        [Tooltip("Time (in seconds) after which the muzzle mesh will be deactivated automatically.")]
        public float TimeToDeactivateMuzzleMesh = 0.2f;

        [Tooltip("Audio clip to play when firing with suppressed sound.")]
        public AudioClip SuppressedFireSound;

        [Tooltip("SphereCollider representing the area where sound alerts enemies. Adjusts dynamically based on RadiusValue.")]
        public SphereCollider AlertingSoundRadius;

        [Tooltip("Radius of the sound alert effect caused by firing. Larger values alert enemies farther away.")]
        public float RadiusValue = 5f;

        private void OnEnable()
        {
            StartCoroutine(Coro());
        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(0.01f); // waiting so OnEnable function in the Player weapon script can call first 
            PlayerWeaponScript.ShootingFeatures.UseMuzzleFlashMesh = UseMuzzleMesh;

            if (UseMuzzleMesh == true)
            {
                if (MuzzleMesh != null)
                {
                    PlayerWeaponScript.ShootingFeatures.MuzzleFlashMesh = MuzzleMesh;

                }
                else
                {
                    PlayerWeaponScript.ShootingFeatures.MuzzleFlashMesh = null;
                }
            }
            else
            {
                if (MuzzleFlashParticles != null)
                {
                    if (PlayerWeaponScript.ShootingFeatures.MuzzleFlashParticleFX != null)
                    {
                        PlayerWeaponScript.ShootingFeatures.MuzzleFlashParticleFX.gameObject.SetActive(false);
                    }
                    MuzzleFlashParticles.gameObject.SetActive(true);
                    PlayerWeaponScript.ShootingFeatures.MuzzleFlashParticleFX = MuzzleFlashParticles;

                }
                else
                {
                    PlayerWeaponScript.ShootingFeatures.MuzzleFlashParticleFX = null;
                }
            }

            if (SuppressedFireSound != null)
            {
                PlayerWeaponScript.WeaponSounds.FireAudioClip = SuppressedFireSound;
            }

            PlayerWeaponScript.ShootingFeatures.MuzzleFlashMeshActiveTime = TimeToDeactivateMuzzleMesh;
            AlertingSoundRadius.radius = RadiusValue;
        }
    }
}