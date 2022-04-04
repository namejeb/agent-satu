using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CutsceneDialogueLoader : MonoBehaviour
{
    [SerializeField] private TransitionScript transition;
    [SerializeField] private CutsceneDialogueSO cutsceneDialogueSo;
    [SerializeField] private Animator screenAnimator;

    [Header("For debugging: ")]
    [SerializeField] private int currCutscene;
    [SerializeField] private int currPanel;
    [SerializeField] private int currSection;
    
    
    [SerializeField] private int currDialogue;
    [SerializeField] private int currLine = -1;
    [SerializeField] private int prevLine;

    [SerializeField] private int swappedLine;

    private Line lineToSetInactive;
    
    [SerializeField] private Cutscene[] cutscenesArray;
    [SerializeField] private Dialogue[] dialoguesArray;

    [SerializeField] private bool cutsceneOrDialogue = false;

        
    [System.Serializable]
    private class Panel
    {
        public Transform panelTransform;
        public Transform[] sections;
    }


    [System.Serializable]
    struct Cutscene
    {
        public int loadId;
        public Transform cutsceneTransform;

        [Header("Cutscene scene")]
        public bool loadsBackToCutscene;

        [Header("True for Cutscene; False for Dialogue")]
        public bool cutsceneOrDialogue;
        
        [Header("Applicable to both Cutscene and Dialogue")]
        public int nextLoadId;
        
        [Header("Info for next level to load (ignore if loading back to cutscene)")]
        [SerializeField] private string levelToLoadSceneName;
        public int levelToLoadIndex;
        
        
        [Header("Panels")]
        public  Panel[] panels;
    };

   


    
    [System.Serializable]
    public class Dialogue
    {
        public int loadId;
        
        [Header("Ref:")]
        public Transform dialogueTransform;
        private Image BgImage;
        
        [Header("Cutscene scene")]
        public bool loadsBackToCutscene;

        [Header("True for Cutscene; False for Dialogue")]
        public bool cutsceneOrDialogue;
        
        [Header("Applicable to both Cutscene and Dialogue")]
        public int nextLoadId;
        
        [Header("Info for next level to load (ignore if loading back to cutscene)")]
        [SerializeField] private string levelToLoadSceneName;
        public int levelToLoadIndex;

        public Sprite[] backgrounds;
        public Line[] lines;

        private int bgCounter;
        public void Awake()
        {
            BgImage = dialogueTransform.GetComponent<Image>();
            BgImage.sprite = backgrounds[0];
        }

        public void SwitchBg()
        {
            bgCounter++;
            BgImage.sprite = backgrounds[bgCounter];
        }
    }
    
    
    [System.Serializable]
    public class Line
    {
        public GameObject line;
        public bool staysInScene = false;
        public bool switchesBg = false;
    }
    

    
    
    void Start()
    {
        cutsceneOrDialogue = cutsceneDialogueSo.loadCutsceneOrDialogue;

        
        //Disable all
        foreach (Cutscene cutscene in cutscenesArray)
        {
            cutscene.cutsceneTransform.gameObject.SetActive(false);
        }

        foreach (Dialogue dialogue in dialoguesArray)
        {
            dialogue.Awake();
            dialogue.dialogueTransform.gameObject.SetActive(false);
        }
        
        
        // True to open cutscene; false to open dialogue
        if (cutsceneOrDialogue)
        { 
            //Get index of cutscene to load
            currCutscene = -1;
            for (int i = 0; i < cutscenesArray.Length; i++)
            {
                if (cutsceneDialogueSo.loadId == cutscenesArray[i].loadId)
                {
                    currCutscene = i;
                }
            }

            if (currCutscene == -1)
            {
                print("no cutscene");
                return;
            }
            OpenFirstPanel(cutscenesArray[currCutscene]);
        }
        else
        {
            //Get index of dialogue to load
            currDialogue = -1;
            for (int i = 0; i < dialoguesArray.Length; i++)
            {
                if (cutsceneDialogueSo.loadId == dialoguesArray[i].loadId)
                {
                    currDialogue = i;
                }
            }

            if (currDialogue == -1)
            {
                print("no dialogue");
                return;
            }
        
            OpenFirstLine(dialoguesArray[currDialogue]);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown("y"))      //remove later
        {
            LoadCutScene();
        }

        
       
        if (Input.GetButtonDown("ProceedInteraction"))
        {
            if(cutsceneOrDialogue)
            {
                if (IsAllCutscenesFinished()) return;
                NextSection();
            }
            else
            {
                 NextLine();
            }
        }
    }

    public void LoadWinScene()
    {
        SceneManager.LoadScene("Win Scene");
    }
    
    
    public void LoadCutScene()       //remove later
    {
        SceneManager.LoadScene("Cutscene Scene");
    }


    //Disable all Panels except first Panel
    private void OpenFirstPanel(Cutscene thisCutscene)
    {
        thisCutscene.cutsceneTransform.gameObject.SetActive(true);
        
        for (int j = 0; j < thisCutscene.panels.Length; j++)
        {
            Panel thisPanel = thisCutscene.panels[j];
            
            //Get panel
            GameObject panelObj = thisPanel.panelTransform.gameObject;

            
            //Disable this panel's sections
            Transform[] thisPanelSections = thisPanel.sections;
            foreach (Transform section in thisPanelSections)
            {
                section.gameObject.SetActive(false);
            }
            
            //If not first panel, disable this panel
            if (j > 0)
            {
                panelObj.SetActive(false);
            }
        }
    }
    
    private void OpenFirstLine(Dialogue thisDialogue)
    {
        thisDialogue.dialogueTransform.gameObject.SetActive(true);
        thisDialogue.lines[currLine].line.SetActive(true);
    }
    
    private void NextLine()
    {
        if (!IsAllLinesFinished())
        {
            prevLine = currLine;
            currLine++;
            
            Line previousLine = dialoguesArray[currDialogue].lines[prevLine];
            if (!previousLine.staysInScene)
            {
                previousLine.line.SetActive(false);

                if (previousLine.switchesBg && swappedLine != currLine)
                {
                    screenAnimator.Play("Screen_FadeToBlack");
                    StartCoroutine(DialogueSwitchBg());

                    swappedLine = currLine;

                    prevLine--;
                    currLine--;
                    
                    return;
                }
            }
        
            
            Line thisLine = dialoguesArray[currDialogue].lines[currLine];
            thisLine.line.SetActive(true);
        }
        else
        {
            currLine = 0;
            
            if (dialoguesArray[currDialogue].loadsBackToCutscene)
            {
                cutsceneDialogueSo.loadId = dialoguesArray[currDialogue].nextLoadId;
                cutsceneDialogueSo.loadCutsceneOrDialogue = dialoguesArray[currDialogue].cutsceneOrDialogue;
                    
                transition.LoadNextLevel(SceneManager.GetActiveScene().buildIndex);

                return;
            }
            
            transition.LoadNextLevel(dialoguesArray[currDialogue].levelToLoadIndex);
            print("All lines finished");
        }
    }
    
    private bool IsAllLinesFinished()
    {
        return currLine == dialoguesArray[currDialogue].lines.Length - 1;
    }


    public void NextSection()
    {

        //Display sections and panels if amount doesnt exceed
        if (IsAllSectionsFinished())
        {
            currSection = 0;
            
            if (IsWithinPanelAmount())
            {
                ChangePanel();
            }
            //All panels finished, change to level scene
            else
            {
                //If all sections loaded, load next level
                if (IsAllCutscenesFinished())
                {
                    //do something after all cutscene finished
                    print("no more cutscene");
                }
          
                
                //If coming back to Cutscene, update SO
                if (cutscenesArray[currCutscene].loadsBackToCutscene)
                {
                    cutsceneDialogueSo.loadId = cutscenesArray[currCutscene].nextLoadId;
                    cutsceneDialogueSo.loadCutsceneOrDialogue = cutscenesArray[currCutscene].cutsceneOrDialogue;
                    
                    transition.LoadNextLevel(SceneManager.GetActiveScene().buildIndex);

                    return;
                }
                
                transition.LoadNextLevel(cutscenesArray[currCutscene].levelToLoadIndex);
                
                return;
            }
        }
        
        //Display
        DisplaySection();
        currSection++;
    }

    private bool IsAllCutscenesFinished()
    {
        if (currCutscene > cutscenesArray.Length - 1)
        {
            return true;
        }

        return false;
    }

    private void DisplaySection()
    {
        GetCurrPanel().sections[currSection].gameObject.SetActive(true);
    }

    private void ChangePanel()
    {
        currPanel++;
        GetCurrPanel().panelTransform.gameObject.SetActive(true);
    }

    private bool IsAllSectionsFinished()
    {
        if (currSection > GetCurrPanel().sections.Length - 1)
        {
            return true;
        }

        return false;
    }

    private bool IsWithinPanelAmount()
    {
        if (currPanel < cutscenesArray[currCutscene].panels.Length - 1)
        {
            return true;
        }

        return false;
    }

   
    private Panel GetCurrPanel()
    {
        return cutscenesArray[currCutscene].panels[currPanel];
    }

    private IEnumerator DialogueSwitchBg()
    {
        yield return new WaitForSeconds(.5f);
        dialoguesArray[currDialogue].SwitchBg();
    }
}