// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
    [ActionTarget(typeof(AudioSource), "gameObject")]
    [ActionTarget(typeof(AudioClip), "oneShotClip")]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlayRandom : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

        [CompoundArray("Audio Clips", "Audio Clip", "Weight")]
        public AudioClip[] audioClips;
        [HasFloatSlider(0, 1)]
        public FsmFloat[] weights;

        [HasFloatSlider(0,1)]
        [Tooltip("Set the volume.")]
		public FsmFloat volume;
		
		//[ObjectType(typeof(AudioClip))]
		//[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		//public FsmObject oneShotClip;
		
		[Tooltip("Event to send when the AudioClip finishes playing.")]
		public FsmEvent finishedEvent;

		private AudioSource audio;
				
		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
			//oneShotClip = null;
            audioClips = new AudioClip[3];
            weights = new FsmFloat[] { 1, 1, 1 };
            finishedEvent = null;
		}

		public override void OnEnter()
		{
            if (audioClips.Length == 0) return;

            int randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);

            if (randomIndex != -1)
            {

                var go = Fsm.GetOwnerDefaultTarget(gameObject);
                if (go != null)
                {
                    // cache the AudioSource component

                    audio = go.GetComponent<AudioSource>();
                    if (audio != null)
                    {
                        var audioClip = audioClips[randomIndex] as AudioClip;

                        if (audioClip == null)
                        {
                            audio.Play();

                            if (!volume.IsNone)
                            {
                                audio.volume = volume.Value;
                            }

                            return;
                        }

                        if (!volume.IsNone)
                        {
                            audio.PlayOneShot(audioClip, volume.Value);
                        }
                        else
                        {
                            audio.PlayOneShot(audioClip);
                        }

                        return;
                    }
                }
            }

            // Finish if failed to play sound	

            Finish();
		}
		
		public override void OnUpdate ()
		{
			if (audio == null)
			{
				Finish();
			}
            if (finishedEvent == null)
            {
                Finish();
            }
			else
			{
				if (!audio.isPlaying)
				{
					Fsm.Event(finishedEvent);
					Finish();
				}
                else if (!volume.IsNone && volume.Value != audio.volume)
				{
					audio.volume = volume.Value;
				}
			}
        }
	}
}