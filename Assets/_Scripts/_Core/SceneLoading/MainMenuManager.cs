using UnityEngine;
using UnityEngine.UI;

namespace WGR.Core.Managers
{
    /* [CLASS DOCUMENTATION]
     * 
     * Button Variables: These variables change in runtime and store each main menu button reference.(Cached in runtime.)
     * 
     * [Class flow]
     * 1. When the main menu scene loads the script automatically caches each button appropriately and assigns the needed methods on the 
     *  onClick event.
     * 
     * [Must know]
     * 1. Every menu button gets automatically cached when the scene loads.
     * 2. The LOAD button gets activated or gets deactivated based on the SaveGameHandler.HasSavedBefore() value.
     */

    public class MainMenuManager : MonoBehaviour
    {
        #region BUTTON_REFERENCES
        //Main Menu buttons
        Button startButton;
        Button loadButton;
        Button quitButton;

        //Save ovewrite notification
        Button dontOverwriteSaveButton;
        Button deleteSaveButton;
        #endregion

        private void Start()
        {
            SetUpMainMenu();

            SetUpSaveDeletion();
        }

        /// <summary>
        /// Call to cache and assign the appropriate onClick events in
        /// each main menu button. (Start, Load, Quit)
        /// </summary>
        void SetUpMainMenu()
        {
            //START button
            startButton = FindStartButton();
            startButton.onClick.AddListener(() => StartNewGameButton());

            //LOAD button
            loadButton = FindLoadButton();
            if (loadButton != null)
            {
                //Make the LOAD button non-interactable if there is not a save file present.
                if (!GameManager.S.SaveDataHandler.HasSavedBefore())
                {
                    loadButton.interactable = false;
                }
                else
                {
                    loadButton.interactable = true;
                }

                loadButton.onClick.AddListener(() => LoadGame());
            }

            //QUIT Button
            quitButton = FindQuitButton();
            quitButton.onClick.AddListener(() => QuitGame());
        }

        #region MAIN_MENU_BUTTON_CACHING
        /// <summary>
        /// Call to find and return the main menu START button.
        /// </summary>
        Button FindStartButton()
        {
            Button tempButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
            return tempButton;
        }

        /// <summary>
        /// Call to find and return the main menu LOAD button.
        /// </summary>
        Button FindLoadButton()
        {
            Button tempButton = GameObject.FindGameObjectWithTag("LoadButton").GetComponent<Button>();
            return tempButton;
        }

        /// <summary>
        /// Call to find and return the main menu QUIT button.
        /// </summary>
        Button FindQuitButton()
        {
            Button tempButton = GameObject.FindGameObjectWithTag("QuitButton").GetComponent<Button>();
            return tempButton;
        }
        #endregion

        /// <summary>
        /// Call to set up the functionality of the Save Overwrite panel.
        /// <para>Caches and assigns onClick event methods for the Cancel and Continue main menu buttons.</para>
        /// </summary>
        void SetUpSaveDeletion()
        {
            //Enable the panel
            GameManager.S.UIManager.EnablePanel(UIPanel.SaveDeletion);

            //Cache the CANCEL button and assing its onClick event method.
            dontOverwriteSaveButton = FindStayInMainMenuButton();
            dontOverwriteSaveButton.onClick.AddListener(() => StayInMainMenu());

            //Cache the CONTINUE button and assing its onClick event method.
            deleteSaveButton = FindSaveDeletionButton();
            deleteSaveButton.onClick.AddListener(() => DeleteSaveAndContinue());

            //Disable the panel 
            GameManager.S.UIManager.DisablePanel(UIPanel.SaveDeletion);
        }

        #region SAVE_OVERWRITE_BUTTON_CACHING
        /// <summary>
        /// Call to find and return the main menu CANCEL button. (Save ovewrite panel)
        /// </summary>
        Button FindStayInMainMenuButton()
        {
            Button tempButton = GameObject.FindGameObjectWithTag("DontOverwrite").GetComponent<Button>();
            return tempButton;
        }
        
        /// <summary>
        /// Call to find and return the main menu CONTINUE button. (Save ovewrite panel)
        /// </summary>
        Button FindSaveDeletionButton()
        {
            Button tempButton = GameObject.FindGameObjectWithTag("DeleteSave").GetComponent<Button>();
            return tempButton;
        }
        #endregion

        /// <summary>
        /// Call to check if there is a present save data file in the user's system.
        /// <para>If there is, Display the "Save File ovewrite user notification"</para>
        /// <para>If there is not, Call DeletseSaveAndContinue() method.</para>
        /// </summary>
        void StartNewGameButton()
        {
            if (GameManager.S.SaveDataHandler.HasSavedBefore())
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSOpen);

                GameManager.S.UIManager.EnablePanel(UIPanel.SaveDeletion);
            }
            else
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSOpen);

                DeleteSaveAndContinue();
            }
        }

        /// <summary>
        /// Call to disable the MainMenuPanel and SaveDeletion panel.
        /// <para>Fire offs a fresh game sequence by calling StartNewGameScene() in the LevelManager.</para>
        /// <para>Invokes the OverwriteSaveFile() in the SaveDataHandler to delete the previous save file.</para>
        /// </summary>
        void DeleteSaveAndContinue()
        {
            GameManager.S.UIManager.DisablePanel(UIPanel.MainMenuPanel);
            GameManager.S.UIManager.DisablePanel(UIPanel.SaveDeletion);

            GameManager.S.LevelManager.StartNewGameScene();

            GameManager.S.SaveDataHandler.OverwriteSaveFile();

            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSSlideOut);
        }

        /// <summary>
        /// Called from the "CANCEL" button present in the SaveOvewrite panel
        /// to disable the SaveDeletion panel and play a SFX.
        /// </summary>
        void StayInMainMenu()
        {
            GameManager.S.UIManager.DisablePanel(UIPanel.SaveDeletion);

            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSSlideOut);
        }

        /// <summary>
        /// Called from the "CONTINUE" button present in the SaveOvewrite panel
        /// to play a SFX, disable the MainMenuPanel and call the LoadGameScene() from the LevelManager.
        /// </summary>
        void LoadGame()
        {
            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSOpen);

            GameManager.S.UIManager.DisablePanel(UIPanel.MainMenuPanel);

            GameManager.S.LevelManager.LoadGameScene();
        }

        /// <summary>
        /// Called from the "QUIT" button to play a SFX and quit the application.
        /// </summary>
        void QuitGame()
        {
            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSClose);

            Application.Quit();
        }
    }
}