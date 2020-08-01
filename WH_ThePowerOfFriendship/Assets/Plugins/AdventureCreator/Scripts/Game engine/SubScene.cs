/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2020
 *	
 *	"SubScene.cs"
 * 
 *	This component keeps track of data regarding any scene added to the active scene during gameplay.
 *	It is generated automatically by the sub-scene's MultiSceneChecker, and should not be added manually.
 * 
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace AC
{

	/**
	 *	This component keeps track of data regarding any scene added to the active scene during gameplay.
	 *	It is generated automatically by the sub-scene's MultiSceneChecker, and should not be added manually.
	 */
	public class SubScene : MonoBehaviour
	{

		#region Variables

		protected int sceneIndex;

		protected LocalVariables localVariables;
		protected SceneSettings sceneSettings;

		protected KickStarter kickStarter;
		protected MainCamera mainCamera;

		#endregion


		#region PublicFunctions

		/**
		 * <summary>Syncs the component with the correct scene.</summary>
		 * <param name = "_multiSceneChecker">The MultiSceneChecker component in the scene for which this component is to sync with.</param>
		 */
		public void Initialise (MultiSceneChecker _multiSceneChecker)
		{
			Scene scene = _multiSceneChecker.gameObject.scene;
			gameObject.name = "SubScene " + scene.buildIndex;

			kickStarter = _multiSceneChecker.GetComponent <KickStarter>();

			sceneIndex = scene.buildIndex;

			localVariables = _multiSceneChecker.GetComponent <LocalVariables>();
			sceneSettings = _multiSceneChecker.GetComponent <SceneSettings>();

			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (gameObject, scene);

			kickStarter = _multiSceneChecker.GetComponent<KickStarter> ();
			_multiSceneChecker.gameObject.SetActive (false);

			mainCamera = UnityVersionHandler.GetOwnSceneInstance <MainCamera> (gameObject);
			if (mainCamera != null)
			{
				mainCamera.gameObject.SetActive (false);
			}

			Player ownPlayer = UnityVersionHandler.GetOwnSceneInstance <Player> (gameObject);
			if (ownPlayer != null)
			{
				ownPlayer.gameObject.SetActive (false);
			}
			if (sceneSettings.OverridesCameraPerspective ())
			{
				ACDebug.LogError ("The added scene (" + scene.name + ", " + scene.buildIndex + ") overrides the default camera perspective - this feature should not be used in conjunction with multiple-open scenes.", gameObject);
			}
			KickStarter.sceneChanger.RegisterSubScene (this);
		}


		/**
		 * Prepares the sub-scene to become the new active scene, due to the active scene being removed.  The gameobject will be destroyed afterwards.
		 */
		public void MakeMain ()
		{
			if (mainCamera)
			{
				mainCamera.gameObject.SetActive (true);
				if (KickStarter.settingsManager.blackOutWhenInitialising)
				{
					mainCamera.ForceOverlayForFrames (4);
				}
			}
			if (kickStarter)
			{
				kickStarter.gameObject.SetActive (true);
				KickStarter.SetGameEngine (kickStarter.gameObject);
			}

			KickStarter.sceneChanger.SubScenes.Remove (this);
			Destroy (gameObject);
		}

		#endregion


		#region GetSet

		/**
		 * Gets the build index for the scene that this component represents.
		 */
		public int SceneIndex
		{
			get
			{
				return sceneIndex;
			}
		}


		/**
		 * Gets the LocalVariables for the scene that this component represents.
		 */
		public LocalVariables LocalVariables
		{
			get
			{
				return localVariables;
			}
		}


		/**
		 * Gets the SceneSettings for the scene that this component represents.
		 */
		public SceneSettings SceneSettings
		{
			get
			{
				return sceneSettings;
			}
		}

		#endregion

	}

}