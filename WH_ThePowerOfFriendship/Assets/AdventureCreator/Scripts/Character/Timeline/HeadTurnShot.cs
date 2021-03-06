/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2020
 *	
 *	"HeadTurnShot.cs"
 * 
 *	A PlayableAsset that keeps track of which transform to face in the HeadTurnMixer
 * 
 */

#if !ACIgnoreTimeline

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AC
{

	/**
	 * A PlayableAsset that keeps track of which transform to face in the HeadTurnMixer
	 */
	public sealed class HeadTurnShot : PlayableAsset
	{

		#region Variables

		public ExposedReference<Transform> headTurnTarget;
		
		#endregion


		#region PublicFunctions

		public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<HeadTurnPlayableBehaviour>.Create (graph);
			playable.GetBehaviour().headTurnTarget = headTurnTarget.Resolve(graph.GetResolver());
			return playable;
		}

		#endregion

	}

}

#endif