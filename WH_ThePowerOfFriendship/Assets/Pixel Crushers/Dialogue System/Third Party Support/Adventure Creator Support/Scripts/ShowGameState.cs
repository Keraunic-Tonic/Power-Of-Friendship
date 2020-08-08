using UnityEngine;
using AC;

namespace PixelCrushers.DialogueSystem.AdventureCreatorSupport
{

    /// <summary>
    /// This is a tiny utility script to show AC's current game state.
    // </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Adventure Creator/Show Game State")]
    public class ShowGameState : MonoBehaviour
    {

        void OnGUI()
        {
            GUILayout.Label(string.Format("GameState: {0}", KickStarter.stateHandler.gameState.ToString()));

            //---
            //--- If you're having trouble with cursors, edit AC's PlayerCursor.cs and
            //--- change the "private" to "public" in the lines "private int selectedCursor" 
            //--- and "private bool showCursor" (lines 24-25). Then uncomment the two lines
            //--- below:

            //PlayerCursor playerCursor = FindObjectOfType<PlayerCursor>();
            //GUILayout.Label(string.Format("Cursor: {0}, {1}", playerCursor.showCursor, playerCursor.selectedCursor));
        }

    }

}
