using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Player 
{
    public Image panel;
    public Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor 
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour {

    // public Text[] buttonList; 
    public List<GameObject> listGrid = new List<GameObject>(); 
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;
    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject startInfo;    
    public GameObject cameraMove;
    public GameObject followCamera;
    private AIScript aiScript;

    public void setAIController(AIScript ai){
        aiScript = ai;
    }

    private string playerSide = "X";
    private string computerSide = "O";
    private string currentSide;
    public int moveCount;
    private bool isGameOver = false;

    public int maxMoveCount;
    public bool playerMove;
    private int value;
    private int lastMovePos;
    private int gridValue; // NxN
    private double defaultLength = 256;
    private int IsBoardCreated = 0; // 0 = not create , 1 = created , for setting boundary at the beginning
    public int[,] boardValue;

    public GameObject prefabGrid;
    public GameObject prefabBoard;
    public GameController gameController;
    public GameObject canvas;
    public InputField inputField; 
    
    private Vector2 screenBound;
    
    private float screenHeight = Screen.height;
    private float screenWidth  = Screen.width;
    void Awake()
    {
        gameOverPanel.SetActive(false);
        moveCount = 0;
        restartButton.SetActive(false);
        playerMove = true;
        cameraMove.GetComponent<CameraScript>().SetGameControllerReference(this);  
        gameController.GetComponent<AIScript>().setGameController(this);
        
        

        var followCameraRectTransform = followCamera.transform as RectTransform;
        followCameraRectTransform.sizeDelta = new Vector2 (screenWidth,screenHeight/4);   
        var canvasRectTransform = canvas.transform as RectTransform;
        canvasRectTransform.sizeDelta = new Vector2 (screenWidth,screenHeight);   
        
    }   

    public void ArrayUpdate(int a){
        int row = a/gridValue;
        int column = a%gridValue;
        if(a<gridValue*gridValue){
            if(currentSide == "X") boardValue[row,column] = -1;
            else boardValue[row,column] = 1;
        }
    }

    public void Update()
    {


        if (playerMove == false && !isGameOver)
        {
            value = GetNextComputerMove();
            Debug.Log("value = " +value);
            ArrayUpdate(value);
            if (listGrid[value].GetComponentInParent<Button>().interactable == true)
            {
                listGrid[value].GetComponentInParent<GridSpace>().buttonText.text = GetComputerSide();                
                listGrid[value].GetComponentInParent<Button>().interactable = false;
                EndTurn(value);
                lastMovePos = value;
            }
        }
        
        
        
    }

    public float ScreenHeight(){
        return Screen.height;
    }

    public int lastMove(){
        return lastMovePos;
    }

    private int GetNextComputerMove()
    {
        int sideValue;
        if (playerSide == "X") sideValue = 1;
        else sideValue = -1;
        return aiScript.GameState(boardValue,sideValue);
        
    }
        
    void SetGameControllerReferenceOnButtons ()
    {
        for(int i = 0; i < listGrid.Count; i++){
            listGrid[i].GetComponent<GridSpace>().SetGameControllerReference(this);            
        }      
    }

    public void SetStartingSide(string startingSide)
    {
        playerSide = startingSide;        
        if(playerSide == "X")
        {
            computerSide = "O";
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            computerSide = "X";
            SetPlayerColors(playerO, playerX);
        }
        StartGame();
    }

    void StartGame ()
    {
        // SetBoardInteractable(true);
        SetPlayerButtons (false);
        startInfo.SetActive(false);
    }

    public string GetPlayerSide ()
    {
        return playerSide;
    }
    public string GetComputerSide()
    {
        return computerSide;
    }

    public void EndTurn(int pos)
    {
        moveCount++;
        // Debug.Log(pos);
        if (IsGameWonBy(playerSide,pos))
        {
            isGameOver = true;
            GameOver(playerSide);
        }
        else if (IsGameWonBy(computerSide,pos))
        {
            isGameOver = true;
            GameOver(computerSide);
        }
        else if (moveCount >= gridValue*gridValue-1)
        {
            isGameOver = true;
            GameOver("draw");
        }
        // else
        
            ChangeSides();
        
    }

    public bool getGameOverState(){
        return isGameOver;
    }                                 

    public string getCurrentSide(){
        return currentSide;
    }

    private bool IsGameWonBy(string side,int pos)
    {
        currentSide = listGrid[pos].GetComponentInParent<GridSpace>().buttonText.text;
        int count = getMax(
            checkLine(pos,"left")       +checkLine(pos,"right"),
            checkLine(pos,"top")        +checkLine(pos,"bot"),
            checkLine(pos,"top-left")   +checkLine(pos,"bot-right"),
            checkLine(pos,"top-right")  +checkLine(pos,"bot-left")            
            );
        if(count >5 ){
            Debug.Log(currentSide + " winnnnnnnnn");
            return true;
        }else
         return false;

    }
    int checkLine(int pos,string nextpos){
        int count = 0;
        if(pos >=0 && pos < gridValue*gridValue){
            if(listGrid[pos].GetComponentInParent<GridSpace>().buttonText.text == currentSide ){
                count++;  
                if(nextpos == "top") return count + checkLine(pos-gridValue,nextpos);
                if(nextpos == "bot") return count + checkLine(pos+gridValue,nextpos); 

                if(nextpos == "top-left" ) return count + checkLine(pos-gridValue-1,nextpos);
                if(nextpos == "bot-right") return count + checkLine(pos+gridValue+1,nextpos);

                if(nextpos == "top-right") return count + checkLine(pos-gridValue+1,nextpos);
                if(nextpos == "bot-left" ) return count + checkLine(pos+gridValue-1,nextpos);

                if(nextpos == "left" && pos%gridValue != 0) return count + checkLine(pos-1,nextpos);
                if(nextpos == "right" && pos%gridValue != gridValue-1) return count + checkLine(pos+1,nextpos);
            }
        }
         return count;       
    }
    
    int getMax(int a, int b, int c, int d){ // return max of a,b,c,d
        int max = a;
        if(b>max) max = b;
        if(c>max) max = c;
        if(d>max) max = d;
        return max;
    }


   void ChangeSides ()
   {
    playerMove = (playerMove == true) ? false : true;
    if(playerMove == true){
        SetPlayerColors(playerX, playerO);
    }
    else
    {
        SetPlayerColors(playerO, playerX);
    }
   }

   void SetPlayerColors (Player newPlayer, Player oldPlayer)
   {
       newPlayer.panel.color = activePlayerColor.panelColor;
       newPlayer.text.color = activePlayerColor.textColor;
       oldPlayer.panel.color = inactivePlayerColor.panelColor;
       oldPlayer.text.color = inactivePlayerColor.textColor;
   }

    void GameOver(string winningPlayer)
    {
        
        SetBoardInteractable(false);
        if (winningPlayer == "draw")
        {
            SetGameOverText("Draw");
            SetPlayerColorsInactive();
        }
        else
        {
            SetGameOverText(winningPlayer + " wins");
        }
        restartButton.SetActive(true);
    }


   void SetGameOverText (string value)
   {
        //gameOverPanel.transform.position = new Vector2(gridValue * getBoardLength(gridValue) / 2, gridValue * getBoardLength(gridValue)/ 2);
       gameOverPanel.SetActive(true);
       gameOverText.text = value;
   }

   public void RestartGame ()
   {
        moveCount = 0;
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        SetPlayerButtons (true);
        SetPlayerColorsInactive();
        startInfo.SetActive(true);
        playerMove = true;

   }

   void SetBoardInteractable (bool toggle)
   {
       //Debug.Log("gameover ");
       for(int i = 0; i < listGrid.Count; i++){
            listGrid[i].GetComponentInParent<Button>().interactable = toggle;
       }
   }

   void SetPlayerButtons (bool toggle)
   {
       playerX.button.interactable = toggle;
       playerO.button.interactable = toggle;  
   }

   void SetPlayerColorsInactive ()
   {
       playerX.panel.color = inactivePlayerColor.panelColor;
       playerX.text.color = inactivePlayerColor.textColor;
       playerO.panel.color = inactivePlayerColor.panelColor;
       playerO.text.color = inactivePlayerColor.textColor;
   }


    public void CreateMap(){        
        gridValue = int.Parse(inputField.text);
        if(gridValue <3) gridValue = 3;
        boardValue = new int[gridValue, gridValue];
        for(int i=0;i < gridValue; i++)
        {
            for(int j =0; j < gridValue; j++)
            {
                boardValue[i, j] = 0;
            }
        }
        //Debug.Log("board.length = " + boardValue.LongLength);
        RestartGame();
        maxMoveCount = gridValue*gridValue;
        int order =GenerateBoard(gridValue);
        followCamera.transform.SetSiblingIndex(order+1);
        aiScript.setGridValue(gridValue);
        IsBoardCreated = 1;
        isGameOver = false;
    }

    public int getActualLength(int n){
        int length;
        double multi = 0;
        for(int i = 1; i<=n ;i++){
            multi = multi + (double) 1/i;
        }
        length = (int) (defaultLength * (double) multi/n);
        if (length < 100){
            length = 100;
        }
        return   length;
    }

    private int getBoardLength(int n){
        return  getActualLength(n);
        
    }
    public float boundaries(){        
        if(IsBoardCreated == 1) return getActualLength(int.Parse(inputField.text))* int.Parse(inputField.text);
        return 500;
    }


    public void GenerateGrid(int n,GameObject board ){
        
        ClearAllGrid();        
        int gridLength = getActualLength(n);
        for(int i =0; i <n;i++){
            for(int j=0; j<n ;j++){
                GameObject newobj = (GameObject) Instantiate (prefabGrid) as GameObject;
                listGrid.Add(newobj);                
                newobj.transform.position = new Vector2(0 + j*gridLength,screenHeight/4 - i*gridLength);    
                var theBarRectTransform = newobj.transform as RectTransform;
                theBarRectTransform.sizeDelta = new Vector2 (gridLength/1.2f,gridLength/1.2f);   
                newobj.transform.SetParent(board.transform);                 
                }
        }
        SetGameControllerReferenceOnButtons();
    } 

    public int GenerateBoard(int n){
        ClearBoard();
        int boardLength = getBoardLength(n);
        GameObject newobj = (GameObject) Instantiate (prefabBoard) as GameObject;
        newobj.transform.position = new Vector2(0 + boardLength*(n-1)/2 , screenHeight/4 - boardLength*(n-1)/2  );           
        var theBarRectTransform = newobj.transform as RectTransform;
        theBarRectTransform.sizeDelta = new Vector2 (boardLength*n,boardLength*n);   
        newobj.transform.SetParent(canvas.transform); 
        GenerateGrid(n,newobj);
        newobj.transform.position = new Vector2(0,0); // move board(with all grids) to center 
        return newobj.transform.GetSiblingIndex();
    }

    public void ClearAllGrid(){               
        for(int i = 0; i < listGrid.Count; i++){
         Destroy(listGrid[i]);
        }
        listGrid.Clear();    
    }
    public void ClearBoard(){
        GameObject refObject;
        refObject = GameObject.Find("Canvas/Board(Clone)");
        Destroy(refObject);
    }
    
}