using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[System.Serializable]
public class Square 
{
    public int row;
    public int column;
    public int side;
    public int value = 0;
    public Square(){}
    public Square(int row, int column, int side){
        this.row = row;
        this.column = column;
        this.side = side;
        this.value = 0;
    }
}
public class AIScript : MonoBehaviour
{   
    public GameController gameController;
    private static int gridValue = 3;
    private int side;
    
    private int[] getPoint = {0,0,0,0};   
    // điểm máy nhận được nếu đánh 1 nước (cho cả player+computer)
    public static int[] weight = { 10000, 1000, 30, 12 }; // cho lần lượt 5/4/3/2 nước thẳng hàng ( theo cả chiều dọc/ngang/2 đường chéo)

    public static readonly int maxDeep = 2;
    public static int count = 0;
    private static int[,] mainBoard;


    void Awake(){
      gameController.GetComponent<GameController>().setAIController(this);  
    }
    public void setGameController(GameController controller){
        gameController = controller;
    }

    // initialize
    public void setGridValue(int value){
      gridValue = value;
      
    }

    // call minimax for the first time
    public int GameState(int[,] board, int sideValue)  // computerside = O(1) ; = X(-1)
    {
        mainBoard = board;
        side = sideValue;
        return maxValue(mainBoard, Int32.MinValue, Int32.MaxValue, 0);
    }

    private int maxValue(int[,] board, int alpha, int beta, int depth)
    {
        int valueMax = Int32.MinValue;
        if(isCutOff(board,depth)){            
            return EvaluateState(board);// terminal-state
        }
        List<Square> successors = getSuccessors(board); // get list of possible move
        List<Square> successorssort = new List<Square>();
                
        foreach(Square square in successors){ // get value for each move       
            if(gameController.GetPlayerSide() == "X"){
                square.side = 1;
                board[square.row,square.column] = 1;//sidevalue
                square.value = Evaluate(board,1);
            }else{
                square.side = -1;
                board[square.row,square.column] = -1;//sidevalue
                square.value = Evaluate(board,-1);                
            }
            successorssort.Add(square);
            Debug.Log(square.row+":"+square.column+"="+ square.value);
            board[square.row,square.column] = 0;
        }
        //successorssort.Sort();
        //successorssort = successorssort.OrderBy(square =>square.value).ToList<Square>;
        Square best = GetBestSquare(successorssort);
        Square bestMove = null;
        //minimax
        foreach(Square square1 in successorssort){
               board[square1.row,square1.column] = square1.side; 
               int tryMove = minValue(board,alpha,beta,depth+1);
               if(valueMax < tryMove){
                   valueMax = tryMove;
                   bestMove = square1;
               }
               if(valueMax >= beta){
                    board[square1.row,square1.column] = 0;
                    return square1.row*gridValue + square1.column;                    
               }
               alpha =  Mathf.Max(alpha,valueMax);
               board[square1.row,square1.column] = 0; 
        }
        Debug.Log("last return");
        if(bestMove.value >= best.value) return bestMove.row*gridValue + bestMove.column; 
        else return best.row*gridValue + best.column;
    }

    private Square GetBestSquare(List<Square> list){
        Square max = list[0];
        foreach(Square get in list){
            if (get.value > max.value) max = get;
        }
        Debug.Log("max square: " + max.value);
        return max;
    }



    private int minValue(int[,] board, int alpha, int beta, int depth){
        int valueMin = Int32.MaxValue;
        if(isCutOff(board,depth)){            
            return EvaluateState(board);// terminal-state
        }
        List<Square> successors = getSuccessors(board);
        List<Square> successorssort = new List<Square>();
        
        
        foreach(Square square in successors){
            
            if(gameController.GetPlayerSide() == "X"){
                board[square.row,square.column] = 1;
                square.value = Evaluate(board,1);
            }else{
                board[square.row,square.column] = -1;
                square.value = Evaluate(board,-1);                
            }
            successorssort.Add(square);
            board[square.row,square.column] = 0;
        }
        //successorssort.Sort();
        Square bestMove = null;
        foreach(Square square1 in successorssort){
               board[square1.row,square1.column] = square1.side; 
               int tryMove = maxValue(board,alpha,beta,depth+1);
               if(valueMin > tryMove){
                   valueMin = tryMove;
                   bestMove = square1;
               }
               if(valueMin <= alpha){
                    board[square1.row,square1.column] = 0;
               // Debug.Log("square1: " + (square1.row*gridValue + square1.column-1));
                    return square1.row*gridValue + square1.column;                    
               }
               beta =  Mathf.Min(beta,valueMin);
               board[square1.row,square1.column] = 0; 
        }
       // Debug.Log("bestmove: "+ (bestMove.row*gridValue + bestMove.column-1));
        return bestMove.row*gridValue + bestMove.column; 
    }

    public List<Square> getSuccessors (int[,] board){
        List<Square> list = new List<Square>();          
        for(int i = 0; i <gridValue; i++){
            for(int j = 0; j < gridValue;j++){
                Square a = new Square(); 
                if(board[i,j] == 0){                    
                    a.row = i;
                    a.column = j;
                    a.side = 0;
                    a.value = 0;
                    list.Add(a);
                }
            }
        }
        return list;
    }


    public int EvaluateState(int[,] board) 
    {
        int x = Evaluate(board,-1);
        int o = Evaluate(board,1);
        if(gameController.getCurrentSide() == "X"){
            x *= 2;
        }else o *=2;

        //Debug.Log("evaluate state");
        return Math.Abs(x-o);
    }

    public bool isCutOff(int[,] board, int depth){
        if(depth >= maxDeep) return true;
        if(gameController.moveCount>= gameController.maxMoveCount) return true;
        if(gameController.getGameOverState()) return true;
        return false;
    }

    public int Evaluate(int[,] board,int sideValue){ 
        // tong cac diem? co dc 
        for(int i =0;i <4;i++) getPoint[i] = 0;
        CountStraight(board,sideValue);
        int point =getPoint[0]*weight[0] + getPoint[1]*weight[1] + getPoint[2]*weight[2] + getPoint[3]*weight[3];
        //Debug.Log(getPoint[0] + " " + getPoint[1] + " " + getPoint[2] + "  " + getPoint[3] );
        //Debug.Log("point = " + point );
        //Debug.Log(sideValue);
        return point;        
    }

    private void CountStraight(int[,] board, int sideValue){        
        for(int i = 0; i<gridValue;i++){
            for(int j = 0; j<gridValue;j++){
                if(board[i,j] == sideValue){
                    
                    CountLine(CountOnRow(board,i,j,sideValue));
                    CountLine(CountOnColumn(board,i,j,sideValue));
                    CountLine(CountOnDiagonal1(board,i,j,sideValue));
                    CountLine(CountOnDiagonal2(board,i,j,sideValue));
                }
            }
        }
    }   
    private void CountLine(int a){
        if(a==5) getPoint[0]++;
        if(a==4) getPoint[1]++;
        if(a==3) getPoint[2]++;
        if(a==2) getPoint[3]++;
    }
    private int CountOnRow(int[,] board, int i, int j , int sideValue){ // i not changed
        int count = 1;
        int a = j;
        while(j >0 ){
            if(board[i,j-1] == sideValue){ 
                count++;
                j--;
            }else break;
        }
        while(a<gridValue-1){
            if(board[i,a+1] == sideValue){
                count++;
                a++;
            }else break;
        }
        return count;
    }
    private int CountOnColumn(int[,] board, int i, int j , int sideValue){ // j not changed
        int count = 1;
        int a = i;
        while(i >0 ){
            if(board[i-1,j] == sideValue){ 
                count++;
                i--;
            }else break;
        }
        while(a<gridValue-1){
            if(board[a+1,j] == sideValue){
                count++;
                a++;
            }else break;
        }
        return count;
    }
    private int CountOnDiagonal1(int[,] board, int i, int j , int sideValue){ // theo đường này /
        int count = 1;
        int a = i;
        int b = j;
        while(a <gridValue-1 && b >0 ){
            if(board[a+1,b-1] == sideValue){ 
                count++;
                a++;
                b--;
            }else break;
        }
        while(i >0 && j<gridValue-1){
            if(board[i-1,j+1] == sideValue){
                count++;
                i--;
                j++;
            }else break;
        }
        return count;
    }
    private int CountOnDiagonal2(int[,] board, int i, int j , int sideValue){ // theo đường này \
        int count = 1;
        int a = i;
        int b = j;
        while(a >0 && b>0){
            if(board[a-1,b-1] == sideValue){ 
                count++;
                a--;
                b--;
            }else break;
        }
        while(a<gridValue-1 && b <gridValue-1){
            if(board[a+1,b+1] == sideValue){
                count++;
                a++;
                b++;
            }else break;
        }
        return count;
    }
}

        



