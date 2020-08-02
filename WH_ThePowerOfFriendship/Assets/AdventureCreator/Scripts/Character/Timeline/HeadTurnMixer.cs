/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2020
 *	
 *	"HeadTurnMixer.cs"
 * 
 *	A PlayableBehaviour that allows for a character to turn their head using IK in Timeline.
 * 
 */

#if !ACIgnoreTimeline

using UnityEngine;
using UnityEngine.Playables;

namespace AC
{

	/**
	 * A PlayableBehaviour that allows for a character to turn their head using IK in Timeline.
	 */
	internal sealed class HeadTurnMixer : PlayableBehaviour
	{

		#region Variables

		private Char _character;

		#endregion


		#region PublicFunctions

		public override void OnGraphStop (Playable playable)
		{
			if (_character != null)
			{
				_character.ReleaseTimelineHeadTurnOverride ();
			}
		}


		public override void ProcessFrame (Playable playable, FrameData info, object playerData)
		{
			base.ProcessFrame (playable, info, playerData);

			if (_character == null)
			{
				GameObject characterObject = playerData as GameObject;
				if (characterObject != null)
				{
					_character = characterObject.GetComponent<AC.Char>();
				}
			}

			if (_character == null)
			{
				return;
			}

			int activeInputs = 0;
			ClipInfo clipA = new ClipInfo ();
			ClipInfo clipB = new ClipInfo ();
			Transform headTurnTarget = null;

			for (int i=0; i<playable.GetInputCount (); ++i)
			{
				float weight = playable.GetInputWeight (i);
				ScriptPlayable <HeadTurnPlayableBehaviour> clip = (ScriptPlayable <HeadTurnPlayableBehaviour>) playable.GetInput (i);

				HeadTurnPlayableBehaviour shot = clip.GetBehaviour ();
				if (shot != null && 
					shot.IsValid &&
					playable.GetPlayState() == PlayState.Playing &&
					weight > 0.0001f)
				{
					clipA = clipB;

					clipB.weight = weight;
					clipB.localTime = clip.GetTime ();
					clipB.duration = clip.GetDuration ();
					clipB.headTurnTarget = shot.headTurnTarget;

					if (++activeInputs == 2)
					{
						break;
					}
				}
			}

			headTurnTarget = (clipB.headTurnTarget != null) ? clipB.headTurnTarget : clipA.headTurnTarget;
			float _weight = clipB.weight;

			_character.SetTimelineHeadTurnOverride (headTurnTarget, _weight);
		}

		#endregion


		#region PrivateStructs

		private struct ClipInfo
		{

			public Transform headTurnTarget;
			public float weight;
			public double localTime;
			public double duration;

		}

		#endregion

	}

}

#endif