using UnityEngine;

namespace DigitalRuby.PyroParticles
{
    /// <summary>
    /// Provides an easy wrapper to looping audio sources with nice transitions for volume when starting and stopping
    /// </summary>
    public class LoopingAudioSource
    {
        public AudioSource AudioSource { get; private set; }
        public float TargetVolume { get; private set; }

        private float startMultiplier;
        private float stopMultiplier;
        private float currentMultiplier;

        public LoopingAudioSource(MonoBehaviour script, AudioSource audioSource, float startMultiplier, float stopMultiplier)
        {
            AudioSource = audioSource;
            if (audioSource != null)
            {
                AudioSource.loop = true;
                AudioSource.volume = 0.0f;
                AudioSource.Stop();
            }

            TargetVolume = 1.0f;
            this.startMultiplier = currentMultiplier = startMultiplier;
            this.stopMultiplier = stopMultiplier;
        }

        public void Play()
        {
            Play(TargetVolume);
        }

        public void Play(float targetVolume)
        {
            if (AudioSource != null && !AudioSource.isPlaying)
            {
                AudioSource.volume = 0.0f;
                AudioSource.Play();
                currentMultiplier = startMultiplier;
            }
            TargetVolume = targetVolume;
        }

        public void Stop()
        {
            if (AudioSource != null && AudioSource.isPlaying)
            {
                TargetVolume = 0.0f;
                currentMultiplier = stopMultiplier;
            }
        }

        public void Update()
        {
            if (AudioSource != null && AudioSource.isPlaying &&
                (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime / currentMultiplier)) == 0.0f)
            {
                AudioSource.Stop();
            }
        }
    }

    /// <summary>
    /// Script for objects such as wall of fire that never expire unless manually stopped
    /// </summary>
    public class FireConstantBaseScript : FireBaseScript
    {
        [HideInInspector]
        public LoopingAudioSource LoopingAudioSource;

        [Header("Fire Settings")]
        public ParticleSystem fireParticles;  // assign fire particle system in Inspector
        public float shrinkAmount = 0.1f;     // how much to shrink each spray hit
        public float minSize = 0.2f;          // minimum fire size before extinguishing

        protected override void Awake()
        {
            base.Awake();
            // constant effect, so set the duration really high and add an infinite looping sound
            LoopingAudioSource = new LoopingAudioSource(this, AudioSource, StartTime, StopTime);
            Duration = 999999999;
        }

        protected override void Update()
        {
            base.Update();
            LoopingAudioSource.Update();
        }

        protected override void Start()
        {
            base.Start();
            LoopingAudioSource.Play();
        }

        public override void Stop()
        {
            LoopingAudioSource.Stop();
            base.Stop();
        }

        /// <summary>
        /// Detects collision with spray particles and shrinks fire
        /// </summary>
        private void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Spray")) // make sure your spray GameObject has tag "Spray"
            {
                if (fireParticles != null)
                {
                    Vector3 currentScale = fireParticles.transform.localScale;
                    float newScale = Mathf.Max(minSize, currentScale.x - shrinkAmount);

                    fireParticles.transform.localScale = new Vector3(newScale, newScale, newScale);

                    if (newScale <= minSize)
                    {
                        Stop(); // fire extinguished
                    }
                }
            }
        }
    }
}
