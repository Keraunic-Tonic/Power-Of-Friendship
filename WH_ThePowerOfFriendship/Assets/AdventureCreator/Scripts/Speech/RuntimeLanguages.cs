﻿/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2020
 *	
 *	"RuntimeLanguage.cs"
 * 
 *	This script contains all language data for the game at runtime.
 *	It transfers data from the Speech Manaager to itself when the game begins.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AC
{

	/**
	 * This script contains all language data for the game at runtime.
 	 * It transfers data from the Speech Manaager to itself when the game begins.
 	 */
	[HelpURL("https://www.adventurecreator.org/scripting-guide/class_a_c_1_1_runtime_languages.html")]
	public class RuntimeLanguages : MonoBehaviour
	{

		#region Variables

		protected Dictionary<int, SpeechLine> speechLinesDictionary = new Dictionary<int, SpeechLine>(); 
		protected List<string> languages = new List<string>();
		protected List<bool> languageIsRightToLeft = new List<bool>();
		protected List<string> languageAudioAssetBundles = new List<string>();
		protected List<string> languageLipsyncAssetBundles = new List<string>();

		protected AssetBundle currentAudioAssetBundle = null;
		protected string currentAudioAssetBundleName;
		protected AssetBundle currentLipsyncAssetBundle = null;
		protected string currentLipsyncAssetBundleName;

		protected bool isLoadingBundle;

		protected List<int> spokenOnceSpeechLineIDs = new List<int>();

		#endregion


		#region PublicFunctions

		public void OnInitPersistentEngine ()
		{
			TransferFromManager ();
		}


		/**
		 * <summary>Loads in audio and lipsync AssetBundles for a given language</summary>
		 * <param name = "language">The index number of the language to load AssetBundles for</param>
		 */
		public virtual void LoadAssetBundle (int language)
		{
			if (KickStarter.speechManager.referenceSpeechFiles == ReferenceSpeechFiles.ByDirectReference ||
				speechLinesDictionary == null ||
				speechLinesDictionary.Count == 0)
			{
				// Only reset if necessary
				speechLinesDictionary.Clear ();
				foreach (SpeechLine speechLine in KickStarter.speechManager.lines)
				{
					if (KickStarter.speechManager.IsTextTypeTranslatable (speechLine.textType))
					{
						speechLinesDictionary.Add (speechLine.lineID, new SpeechLine (speechLine, language));
					}
				}
			}

			if (KickStarter.speechManager.referenceSpeechFiles == ReferenceSpeechFiles.ByAssetBundle)
			{
				StopAllCoroutines ();
				StartCoroutine (LoadAssetBundleCoroutine (language));
			}
		}


		/**
		 * <summary>Gets the AudioClip associated with a speech line</summary>
		 * <param name = "lineID">The ID number of the speech line, as generated by the Speech Manager</param>
		 * <param name = "_speaker">The character speaking the line</param>
		 * <returns>Gets the AudioClip associated with a speech line</returns> 
		 */
		public virtual AudioClip GetSpeechAudioClip (int lineID, Char _speaker)
		{
			if (!KickStarter.speechManager.IsTextTypeTranslatable (AC_TextType.Speech))
			{
				return null;
			}

			int voiceLanguage = Options.GetVoiceLanguage ();
			string voiceLanguageName = (voiceLanguage > 0) ? Options.GetVoiceLanguageName () : string.Empty;

			switch (KickStarter.speechManager.referenceSpeechFiles)
			{
				case ReferenceSpeechFiles.ByNamingConvention:
					{
						string fullName = KickStarter.speechManager.GetAutoAssetPathAndName (lineID, _speaker, voiceLanguageName, false);
						AudioClip clipObj = Resources.Load (fullName) as AudioClip;

						if (clipObj == null && KickStarter.speechManager.fallbackAudio && voiceLanguage > 0)
						{
							fullName = KickStarter.speechManager.GetAutoAssetPathAndName (lineID, _speaker, string.Empty, false);
							clipObj = Resources.Load (fullName) as AudioClip;
						}

						if (clipObj == null && !string.IsNullOrEmpty (fullName))
						{
							ACDebug.LogWarning ("Audio file 'Resources/" + fullName + "' not found in Resources folder.");
						}
						return clipObj;
					}

				case ReferenceSpeechFiles.ByAssetBundle:
					{
						if (isLoadingBundle)
						{
							ACDebug.LogWarning ("Cannot load audio file from AssetBundle as the AssetBundle is still being loaded.");
							return null;
						}
						string fullName = KickStarter.speechManager.GetAutoAssetPathAndName (lineID, _speaker, voiceLanguageName, false);

						int indexOfLastSlash = fullName.LastIndexOf ("/") + 1;
						if (indexOfLastSlash > 0)
						{
							fullName = fullName.Substring (indexOfLastSlash);
						}

						if (currentAudioAssetBundle == null)
						{
							ACDebug.LogWarning ("Cannot load audio file '" + fullName + "' from AssetBundle as no AssetBundle is currently loaded.");
							return null;
						}

						AudioClip clipObj = currentAudioAssetBundle.LoadAsset<AudioClip> (fullName);

						if (clipObj == null && !string.IsNullOrEmpty (fullName))
						{
							ACDebug.LogWarning ("Audio file '" + fullName + "' not found in Asset Bundle '" + currentAudioAssetBundle.name + "'.");
						}
						return clipObj;
					}
					
				case ReferenceSpeechFiles.ByDirectReference:
					{
						AudioClip clipObj = GetLineCustomAudioClip (lineID, voiceLanguage);

						if (clipObj == null && KickStarter.speechManager.fallbackAudio && voiceLanguage > 0)
						{
							return GetLineCustomAudioClip (lineID, 0);
						}
						return clipObj;
					}
			}
			
			return null;
		}


		/**
		 * <summary>Gets the lipsync file associated with a speech line</summary>
		 * <param name = "lineID">The ID number of the speech line, as generated by the Speech Manager</param>
		 * <param name = "_speaker">The character speaking the line</param>
		 * <returns>Gets the lipsync file associated with a speech line</returns> 
		 */
		public virtual T GetSpeechLipsyncFile <T> (int lineID, Char _speaker) where T : Object
		{
			if (!KickStarter.speechManager.IsTextTypeTranslatable (AC_TextType.Speech))
			{
				return null;
			}

			int voiceLanguage = Options.GetVoiceLanguage ();
			string voiceLanguageName = (voiceLanguage > 0) ? Options.GetVoiceLanguageName () : string.Empty;

			switch (KickStarter.speechManager.referenceSpeechFiles)
			{
				case ReferenceSpeechFiles.ByNamingConvention:
					{
						string fullName = KickStarter.speechManager.GetAutoAssetPathAndName (lineID, _speaker, voiceLanguageName, true);

						T lipsyncFile = Resources.Load (fullName) as T;

						if (lipsyncFile == null && KickStarter.speechManager.fallbackAudio && voiceLanguage > 0)
						{
							fullName = KickStarter.speechManager.GetAutoAssetPathAndName (lineID, _speaker, string.Empty, true);
							lipsyncFile = Resources.Load (fullName) as T;
						}

						if (lipsyncFile == null)
						{
							ACDebug.LogWarning ("Lipsync file 'Resources/" + fullName + "' (" + typeof (T) + ") not found in Resources folder.");
						}
						return lipsyncFile;
					}

				case ReferenceSpeechFiles.ByAssetBundle:
					{
						string fullName = KickStarter.speechManager.GetAutoAssetPathAndName (lineID, _speaker, voiceLanguageName, true);

						if (isLoadingBundle)
						{
							ACDebug.LogWarning ("Cannot load lipsync file from AssetBundle as the AssetBundle is still being loaded.");
							return null;
						}

						int indexOfLastSlash = fullName.LastIndexOf ("/") + 1;
						if (indexOfLastSlash > 0)
						{
							fullName = fullName.Substring (indexOfLastSlash);
						}

						if (currentLipsyncAssetBundle == null)
						{
							ACDebug.LogWarning ("Cannot load lipsync file '" + fullName + "' from AssetBundle as no AssetBundle is currently loaded.");
							return null;
						}


						T lipsyncFile = currentLipsyncAssetBundle.LoadAsset<T> (fullName);

						if (lipsyncFile == null && !string.IsNullOrEmpty (fullName))
						{
							ACDebug.LogWarning ("Lipsync file '" + fullName + "' (" + typeof (T) + ") not found in Asset Bundle '" + currentLipsyncAssetBundle.name + "'.");
						}
						return lipsyncFile;
					}

				case ReferenceSpeechFiles.ByDirectReference:
					{
						UnityEngine.Object _object = KickStarter.runtimeLanguages.GetLineCustomLipsyncFile (lineID, voiceLanguage);

						if (_object == null && KickStarter.speechManager.fallbackAudio && voiceLanguage > 0)
						{
							_object = KickStarter.runtimeLanguages.GetLineCustomLipsyncFile (lineID, 0);
						}

						if (_object is T)
						{
							return (T) KickStarter.runtimeLanguages.GetLineCustomLipsyncFile (lineID, voiceLanguage);
						}
					}
					break;
			}

			return null;
		}


		/**
		 * <summary>Gets the translation of a line of text.</summary>
		 * <param name = "originalText">The line in its original language.</param>
		 * <param name = "_lineID">The translation ID number generated by SpeechManager's PopulateList() function</param>
		 * <param name = "language">The index number of the language to return the line in, where 0 = the game's original language.</param>
		 * <returns>The translation of the line, if it exists. If a translation does not exist, then the original line will be returned.</returns>
		 */
		public string GetTranslation (string originalText, int _lineID, int language)
		{
			if (language == 0 || string.IsNullOrEmpty (originalText))
			{
				return originalText;
			}
			
			if (_lineID == -1 || language <= 0)
			{
				ACDebug.Log ("Cannot find translation for '" + originalText + "' because the text has not been added to the Speech Manager.");
				return originalText;
			}
			else
			{
				SpeechLine speechLine;
				if (speechLinesDictionary.TryGetValue (_lineID, out speechLine))
				{
					if (speechLine.translationText.Count > (language-1))
					{
						return speechLine.translationText [language-1];
					}
					else
					{
						ACDebug.LogWarning ("A translation is being requested that does not exist!");
					}
				}
				else
				{
					if (KickStarter.settingsManager.showDebugLogs != ShowDebugLogs.Never)
					{
						SpeechLine originalLine = KickStarter.speechManager.GetLine (_lineID);
						if (originalLine == null)
						{
							ACDebug.LogWarning ("Cannot find translation for '" + originalText + "' because it's Line ID (" + _lineID + ") was not found in the Speech Manager.");
						}
					}
 					return originalText;
				}
			}

			return string.Empty;
		}



		/**
		 * <summary>Gets the translation of a line of text.</summary>
		 * <param name = "originalText">The line in its original language.</param>
		 * <param name = "_lineID">The translation ID number generated by SpeechManager's PopulateList() function</param>
		 * <param name = "language">The index number of the language to return the line in, where 0 = the game's original language.</param>
		 * <param name = "textType">The type of text to translatable.</param>
		 * <returns>The translation of the line, if it exists. If a translation does not exist, or the given textType is not translatable, then the original line will be returned.</returns>
		 */
		public string GetTranslation (string originalText, int _lineID, int language, AC_TextType textType)
		{
			if (KickStarter.speechManager == null || KickStarter.speechManager.IsTextTypeTranslatable (textType))
			{
				return GetTranslation (originalText, _lineID, language);
			}
			return originalText;
		}



		/**
		 * <summary>Gets a line of text, translated (if applicable) to the current language.</summary>
		 * <param name = "_lineID">The translation ID number generated by SpeechManager's PopulateList() function</param>
		 * <returns>A line of text, translated (if applicable) to the current language.</returnsy>
		 */
		public string GetCurrentLanguageText (int _lineID)
		{
			int language = Options.GetLanguage ();

			if (_lineID < 0 || language < 0)
			{
				return string.Empty;
			}
			else
			{
				SpeechLine speechLine;
				if (speechLinesDictionary.TryGetValue (_lineID, out speechLine))
				{
					if (language == 0)
					{
						return speechLine.text;
					}

					if (speechLine.translationText.Count > (language-1))
					{
						return speechLine.translationText [language-1];
					}
					else
					{
						ACDebug.LogWarning ("A translation is being requested that does not exist!");
					}
				}
				else
				{
					ACDebug.LogWarning ("Cannot find translation for line ID " + _lineID + " because it was not found in the Speech Manager.");
				}
			}

			return string.Empty;
		}


		/**
		 * <summary>Gets all translations of a line of text.</summary>
		 * <param name = "_lineID">The translation ID number generated by SpeechManager's PopulateList() function</param>
		 * <returns>All translations of the line, if they exist. If a translation does not exist, nothing will be returned.</returns>
		 */
		public string[] GetTranslations (int _lineID)
		{
			if (_lineID == -1)
			{
				return null;
			}
			else
			{
				SpeechLine speechLine;
				if (speechLinesDictionary.TryGetValue (_lineID, out speechLine))
				{
					return speechLine.translationText.ToArray ();
				}
			}
			return null;
		}


		/**
		 * <summary>Updates the translation of a given line for a given language.</summary>
		 * <param name = "lineID">The ID of the text to update, as generated by the Speech Manager</param>
		 * <param name = "languageIndex">The index number of the language to update.  Must be greater than 0, since 0 is the game's original language</param>
		 * <param name = "translationText">The updated translation text</param>
		 */
		public void UpdateRuntimeTranslation (int lineID, int languageIndex, string translationText)
		{
			if (languageIndex <= 0)
			{
				ACDebug.LogWarning ("The language index must be greater than zero.");
			}

			SpeechLine speechLine;
			if (speechLinesDictionary.TryGetValue (lineID, out speechLine))
			{
				speechLine.translationText [languageIndex-1] = translationText;
			}
		}


		/**
		 * <summary>Gets the text of an ITranslatable instance, based on the game's current language.</summary>
		 * <param name = "translatable">The ITranslatable instance.</param>
		 * <param name = "index">The index of the ITranslatable's array of translatable text</param>
		 * <returns>The translatable text.</returns>
		 */
		public string GetTranslatableText (ITranslatable translatable, int index = 0)
		{
			int language = Options.GetLanguage ();
			string originalText = translatable.GetTranslatableString (index);
			int lineID = translatable.GetTranslationID (index);

			return GetTranslation (originalText, lineID, language);
		}


		/**
		 * <summary>Imports a translation CSV file (as generated by the Speech Manager) into the game - either as a new language, or as an update to an existing one. The first column MUST be the ID number of each line, and the first row must be for the header.</summary>
		 * <param name = "textAsset">The CSV file as a text asset.</param>
		 * <param name = "languageName">The name of the language.  If a language by this name already exists in the system, the import process will update it.</param>
		 * <param name = "newTextColumn">The column number (starting from zero) that holds the new translation.  This must be greater than zero, as the first column should be occupied by the ID numbers.</param>
		 * <param name = "ignoreEmptyCells">If True, then empty cells will not be imported and the original language will be used instead</param>
		 * <param name = "isRTL">If True, the language is read right-to-left</summary>
		 */
		public void ImportRuntimeTranslation (TextAsset textAsset, string languageName, int newTextColumn, bool ignoreEmptyCells = false, bool isRTL = false)
		{
			if (textAsset != null && !string.IsNullOrEmpty (textAsset.text))
			{
				if (newTextColumn <= 0)
				{
					ACDebug.LogWarning ("Error importing language from " + textAsset.name + " - newTextColumn must be greater than zero, as the first column is reserved for ID numbers.");
					return;
				}

				if (!languages.Contains (languageName))
				{
					CreateLanguage (languageName, isRTL);
					int i = languages.Count - 1;
					ProcessTranslationFile (i, textAsset.text, newTextColumn, ignoreEmptyCells);
					ACDebug.Log ("Created new language " + languageName);
				}
				else
				{
					int i = languages.IndexOf (languageName);
					languageIsRightToLeft[i] = isRTL;
					ProcessTranslationFile (i, textAsset.text, newTextColumn, ignoreEmptyCells);
					ACDebug.Log ("Updated language " + languageName);
				}
			}
		}
	

		/**
		 * <summary>Checks if a given language reads right-to-left, Hebrew/Arabic-style</summary>
		 * <param name = "languageIndex">The index number of the language to check, where 0 is the game's original language</param>
		 * <returns>True if the given language reads right-to-left</returns>
		 */
		public bool LanguageReadsRightToLeft (int languageIndex)
		{
			if (languageIsRightToLeft != null && languageIsRightToLeft.Count > languageIndex)
			{
				return languageIsRightToLeft [languageIndex];
			}
			if (languageIsRightToLeft.Count == 0)
			{
				languageIsRightToLeft.Add (false);
			}
			return languageIsRightToLeft[0];
		}


		/**
		 * <summary>Checks if a given language reads right-to-left, Hebrew/Arabic-style</summary>
		 * <param name = "languageName">The name of the language to check, as written in the Speech Manager</param>
		 * <returns>True if the given language reads right-to-left</returns>
		 */
		public bool LanguageReadsRightToLeft (string languageName)
		{
			if (!string.IsNullOrEmpty (languageName))
			{
				if (languages.Contains (languageName))
				{
					int i = languages.IndexOf (languageName);
					return languageIsRightToLeft [i];
				}
			}

			if (languageIsRightToLeft.Count == 0)
			{
				languageIsRightToLeft.Add (false);
			}
			return languageIsRightToLeft[0];
		}


		/**
		 * <summary>Marks a speech line as having been spoken, so that it cannot be spoken again.  This will only work for speech lines that have 'Can only play once?' checked in their Speech Manager entry.</summary>
		 * <param name = "lineID">The line being spoken</param>
		 * <returns>True if the line can be spoken, False if it has already been spoken and cannot be spoken again.</returns>
		 */
		public bool MarkLineAsSpoken (int lineID)
		{
			if (lineID < 0)
			{
				return true;
			}

			if (spokenOnceSpeechLineIDs.Contains (lineID))
			{
				return false;
			}

			SpeechLine speechLine;
			if (speechLinesDictionary.TryGetValue (lineID, out speechLine))
			{
				if (speechLine.onlyPlaySpeechOnce)
				{
					spokenOnceSpeechLineIDs.Add (lineID);
				}
			}

			return true;
		}


		/**
		 * <summary>Updates a MainData class with its own variables that need saving.</summary>
		 * <param name = "mainData">The original MainData class</param>
		 * <returns>The updated MainData class</returns>
		 */
		public MainData SaveMainData (MainData mainData)
		{
			System.Text.StringBuilder spokenLinesData = new System.Text.StringBuilder ();

			for (int i=0; i<spokenOnceSpeechLineIDs.Count; i++)
			{
				spokenLinesData.Append (spokenOnceSpeechLineIDs[i].ToString ());
				spokenLinesData.Append (SaveSystem.colon);
			}

			if (spokenOnceSpeechLineIDs.Count > 0)
			{
				spokenLinesData.Remove (spokenLinesData.Length-1, 1);
			}

			mainData.spokenLinesData = spokenLinesData.ToString ();
			return mainData;
		}


		/**
		 * <summary>Updates its own variables from a MainData class.</summary>
		 * <param name = "mainData">The MainData class to load from</param>
		 */
		public void LoadMainData (MainData mainData)
		{
			spokenOnceSpeechLineIDs.Clear ();

			string spokenLinesData = mainData.spokenLinesData;
			if (!string.IsNullOrEmpty (spokenLinesData))
			{
				string[] linesArray = spokenLinesData.Split (SaveSystem.colon[0]);

				foreach (string chunk in linesArray)
				{
					int _id = -1;
					if (int.TryParse (chunk, out _id) && _id >= 0)
					{
						spokenOnceSpeechLineIDs.Add (_id);
					}
				}
			}
		}

		#endregion


		#region ProtectedFunctions
		
		protected void TransferFromManager ()
		{
			if (AdvGame.GetReferences () && AdvGame.GetReferences ().speechManager)
			{
				SpeechManager speechManager = AdvGame.GetReferences ().speechManager;
				
				languages.Clear ();
				foreach (string _language in speechManager.languages)
				{
					languages.Add (_language);
				}

				languageIsRightToLeft.Clear ();
				foreach (bool rtl in speechManager.languageIsRightToLeft)
				{
					languageIsRightToLeft.Add (rtl);
				}

				languageAudioAssetBundles.Clear ();
				foreach (string languageAudioAssetBundle in speechManager.languageAudioAssetBundles)
				{
					languageAudioAssetBundles.Add (languageAudioAssetBundle);
				}

				languageLipsyncAssetBundles.Clear ();
				foreach (string languageLipsyncAssetBundle in speechManager.languageLipsyncAssetBundles)
				{
					languageLipsyncAssetBundles.Add (languageLipsyncAssetBundle);
				}
			}
		}


		protected IEnumerator LoadAssetBundleCoroutine (int i)
		{
			isLoadingBundle = true;

			if (!KickStarter.speechManager.translateAudio)
			{
				i = 0;
			}

			if (currentAudioAssetBundleName != languageAudioAssetBundles[i] &&
				currentLipsyncAssetBundleName != languageAudioAssetBundles[i])
			{
				if (!string.IsNullOrEmpty (languageAudioAssetBundles[i]))
				{
					string bundlePath = Path.Combine (Application.streamingAssetsPath, languageAudioAssetBundles[i]);
					var bundleLoadRequest = AssetBundle.LoadFromFileAsync (bundlePath);

					yield return bundleLoadRequest;

					CurrentAudioAssetBundle = bundleLoadRequest.assetBundle;

					if (currentAudioAssetBundle == null)
					{
						ACDebug.LogWarning("Failed to load AssetBundle '" + bundlePath + "'");
					}
					else
					{
						currentAudioAssetBundleName = languageAudioAssetBundles[i];
					}
				}
				else
				{
					// None found
					CurrentAudioAssetBundle = null;
					currentAudioAssetBundleName = string.Empty;
				}
			}

			if (KickStarter.speechManager.UseFileBasedLipSyncing ())
			{
				if (currentLipsyncAssetBundleName != languageLipsyncAssetBundles[i])
				{
					if (!string.IsNullOrEmpty (languageLipsyncAssetBundles[i]))
					{
						if (currentAudioAssetBundleName == languageLipsyncAssetBundles[i])
						{
							CurrentLipsyncAssetBundle = currentAudioAssetBundle;
							currentLipsyncAssetBundleName = currentAudioAssetBundleName;
						}
						else
						{
							string bundlePath = Path.Combine (Application.streamingAssetsPath, languageLipsyncAssetBundles[i]);
							var bundleLoadRequest = AssetBundle.LoadFromFileAsync (bundlePath);
							
			        		yield return bundleLoadRequest;

							CurrentLipsyncAssetBundle = bundleLoadRequest.assetBundle;
							if (currentLipsyncAssetBundle == null)
							{
								ACDebug.LogWarning ("Failed to load AssetBundle '" + bundlePath + "'");
							}
							else
							{
								currentLipsyncAssetBundleName = languageLipsyncAssetBundles[i];
							}
						}
					}
					else
					{
						// None found
						CurrentLipsyncAssetBundle = null;
						currentLipsyncAssetBundleName = string.Empty;
					}
				}
			}

			isLoadingBundle = false;

			KickStarter.eventManager.Call_OnLoadSpeechAssetBundle (i);
		}


		protected AudioClip GetLineCustomAudioClip (int _lineID, int _language = 0)
		{
			SpeechLine speechLine;
			if (speechLinesDictionary.TryGetValue (_lineID, out speechLine))
			{
				if (KickStarter.speechManager.translateAudio && _language > 0)
				{
					if (speechLine.customTranslationAudioClips != null && speechLine.customTranslationAudioClips.Count > (_language - 1))
					{
						return speechLine.customTranslationAudioClips [_language - 1];
					}
				}
				else
				{
					return speechLine.customAudioClip;
				}
			}
			return null;
		}


		protected UnityEngine.Object GetLineCustomLipsyncFile (int _lineID, int _language = 0)
		{
			SpeechLine speechLine;
			if (speechLinesDictionary.TryGetValue (_lineID, out speechLine))
			{
				if (KickStarter.speechManager.translateAudio && _language > 0)
				{
					if (speechLine.customTranslationLipsyncFiles != null && speechLine.customTranslationLipsyncFiles.Count > (_language - 1))
					{
						return speechLine.customTranslationLipsyncFiles [_language - 1];
					}
				}
				else
				{
					return speechLine.customLipsyncFile;
				}
			}
			return null;
		}


		protected void CreateLanguage (string name, bool isRTL)
		{
			languages.Add (name);
			languageIsRightToLeft.Add (isRTL);

			foreach (SpeechLine speechManagerLine in KickStarter.speechManager.lines)
			{
				int _lineID = speechManagerLine.lineID;

				SpeechLine speechLine = null;
				if (speechLinesDictionary.TryGetValue (_lineID, out speechLine))
				{
					speechLine.translationText.Add (speechLine.text);
					continue;
				}
			}
		}
		
		
		protected void ProcessTranslationFile (int i, string csvText, int newTextColumn, bool ignoreEmptyCells)
		{
			string [,] csvOutput = CSVReader.SplitCsvGrid (csvText);
			
			int lineID = 0;
			string translationText = string.Empty;

			if (csvOutput.GetLength (0) <= newTextColumn)
			{
				ACDebug.LogWarning ("Cannot import translation file, as it does not have enough columns - searching for column index " + newTextColumn);
				return;
			}

			for (int y = 1; y < csvOutput.GetLength (1); y++)
			{
				if (csvOutput [0,y] != null && csvOutput [0,y].Length > 0)
				{
					lineID = -1;
					if (int.TryParse (csvOutput [0,y], out lineID))
					{
						translationText = csvOutput [newTextColumn, y];
						translationText = AddLineBreaks (translationText);

						if (!ignoreEmptyCells || !string.IsNullOrEmpty (translationText))
						{
							UpdateRuntimeTranslation (lineID, i, translationText);
						}
					}
					else
					{
						ACDebug.LogWarning ("Error importing translation (ID:" + csvOutput [0,y] + ") on row #" + y.ToString () + ".");
					}
				}
			}
		}


		protected string AddLineBreaks (string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				return (text.Replace ("[break]", "\n"));
			}
			return string.Empty;
		}

		#endregion


		#region GetSet

		/** The AssetBundle to retrieve audio files from */
		public AssetBundle CurrentAudioAssetBundle
		{
			get
			{
				return currentAudioAssetBundle;
			}
			set
			{
				if (currentAudioAssetBundle != null && currentAudioAssetBundle != value)
				{
					currentAudioAssetBundle.Unload (true);
				}

				currentAudioAssetBundle = value;
			}
		}


		/** The AssetBundle to retrieve lipsync files from */
		public AssetBundle CurrentLipsyncAssetBundle
		{
			get
			{
				return currentLipsyncAssetBundle;
			}
			set
			{
				if (currentLipsyncAssetBundle != null && currentLipsyncAssetBundle != value)
				{
					currentLipsyncAssetBundle.Unload (true);
				}

				currentLipsyncAssetBundle = value;
			}
		}


		/** The names of the game's languages. The first is always "Original". */
		public List<string> Languages
		{
			get
			{
				return languages;
			}
		}


		/** True if an audio or lipsync asset bundle is currently being loaded into memory */
		public bool IsLoadingBundle
		{
			get
			{
				return isLoadingBundle;
			}
		}

		#endregion

	}

}