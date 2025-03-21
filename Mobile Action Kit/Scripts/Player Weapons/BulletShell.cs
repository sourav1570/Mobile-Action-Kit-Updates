using System.Collections;
using UnityEngine;

namespace MobileActionKit
{
    public class BulletShell : MonoBehaviour
    {
        public float TimeToDeactiveShell = 2f;

        public AudioSource BulletShellSound;
        public AudioClip[] BulletShellSoundClip;

        public float MinimumPitchSound;
        public float MaximumPitchSound;
        public float MinimumVolume;
        public float MaximumVolume;

        private void OnEnable()
        {
            StartCoroutine(TimeToDeactive());
        }
        IEnumerator TimeToDeactive()
        {
            yield return new WaitForSeconds(TimeToDeactiveShell);
            gameObject.SetActive(false);
        }
        void OnCollisionEnter(Collision c)
        {
            if (c.transform.root.tag != "Player")
            {
                if (c.transform.gameObject.GetComponent<CollisionSounds>() == null)
                {
                    PlayDefaultSounds();
                }
            }
        }
        public void PlayDefaultSounds()
        {
            if (BulletShellSound != null)
            {
                BulletShellSound.clip = BulletShellSoundClip[Random.Range(0, BulletShellSoundClip.Length)];
                if (!BulletShellSound.isPlaying)
                {
                    BulletShellSound.pitch = Random.Range(MinimumPitchSound, MaximumPitchSound);
                    BulletShellSound.volume = Random.Range(MinimumVolume, MaximumVolume);
                    BulletShellSound.Play();
                }
            }
        }
    }
}
