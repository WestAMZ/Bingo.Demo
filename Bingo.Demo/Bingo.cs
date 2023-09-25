using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bingo.Demo
{
    public class BingoGame
    {
        private int[,] bingoBoard;
        private int[,] scoreBoard;
        private List<int> returnedNumbers = new List<int>();
        private System.Timers.Timer gameTimer;
        int currentTurn = 0;
        // Delay de 1 seg entre cada turno
        private const int TURN_DELAY = 1000;
        private const int MIN_NUMBER = 1;
        private const int MAX_NUMBER = 40;

        public BingoGame(int rows_cols)
        {
            initBoard(rows_cols);
            Console.WriteLine("Board Generated");
        }
        private void initBoard(int rows_cols)
        {
            var includedNumbers = new List<int>();
            bingoBoard = new int[rows_cols, rows_cols];
            scoreBoard = new int[rows_cols, rows_cols];
            for (int i = 0; i < bingoBoard.GetLength(0); i++) 
            {
                for (int j = 0; j < bingoBoard.GetLength(1); j++) 
                {
                    int newNumber;
                    do
                    {
                        newNumber = GetRandom();

                    } while (includedNumbers.Contains(newNumber));
                    
                    bingoBoard[i,j] = newNumber;
                    scoreBoard[i,j] = 0;
                    includedNumbers.Add(newNumber);
                }
            }
        }
        private int GetRandom(int min = MIN_NUMBER, int max = MAX_NUMBER + 1) 
        {
            return new Random().Next(min, max);
        }
        public void PrintBoard() 
        {
            for (int i = 0;  i < bingoBoard.GetLength(0); i++)
            {
                Console.WriteLine("----------------------------------------------");
                for (int j = 0; j < bingoBoard.GetLength(1) ; j++)
                {
                    if (returnedNumbers.Contains(bingoBoard[i, j]))
                    {
                        Console.Write("X\t");
                    }
                    else 
                    {
                        Console.Write($"{bingoBoard[i, j]}\t");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("----------------------------------------------");
        }
        public void StartGame() 
        {
            Console.WriteLine("Iniciando Juego ...");
            gameTimer = new System.Timers.Timer(interval: TURN_DELAY);
            gameTimer.Elapsed += (sender, e) => PlayTurn();
            gameTimer.Start();

        }
        void PlayTurn() 
        {
            int number;
            currentTurn++;

            do {
                number = GetRandom();
            } while (returnedNumbers.Contains(number));

            Console.WriteLine();
            Console.WriteLine($"El número es -- {number} -- Turno [{currentTurn}]");
            returnedNumbers.Add(number);
            Index index = GetIndex(number);

            if (index != null) 
            {
                scoreBoard[index.index1, index.index2] = 1;
            }

            PrintBoard();

            var isGameEnded = EvaluateGame();

            if (currentTurn == 40 || isGameEnded) 
            {
                Console.WriteLine("Juego Terminado");
                if(isGameEnded)
                    Console.WriteLine(" ** Ganó el juego :) **");
                gameTimer.Dispose();
            }
        }
        private bool EvaluateGame() 
        {
            /*
             
            Evaluciones relizadas de acuerdo a lo descrito, ejemplo con matríz 3x3

            Los indices para matriz 3x3 dada serían a como sigue
            [0,0][0,1][0,2]
            [1,0][1,1][1,2]
            [2,0][2,1][2,2]


            por lo que para las distintas evaluaciones tendriamos

            -------------------------
            transversal 1
            0,0
            1,1
            2,2

            es decir [i,i]

            -------------------------
            transversal 2
            0,2
            1,1
            2,0

            es decir [i, (tamaño - 1) -i]
             
            */


            var pathTransversal1 = "";
            var pathTransversal2 = "";
            var pathX = "";
            var pathY = "";

            int multiplyTranversal1 = 1;
            int multiplyTranversal2 = 1;

            for (int i = 0; i < scoreBoard.GetLength(0); i++)
            {
                int multiplyResultx = 1;
                int multiplyResulty = 1;
                for (int j = 0; j < scoreBoard.GetLength(1); j++)
                {
                    // validando fila
                    multiplyResultx *= scoreBoard[i, j];
                    pathX += $"[{i},{j}] ,";
                    // validando columna
                    multiplyResulty *= scoreBoard[j, i];
                    pathY += $"[{j},{i}] ,";
                }
                if (multiplyResultx == 1 || multiplyResulty == 1) 
                {
                    Console.WriteLine($"Path Ganador: { (multiplyResultx == 1 ? pathX : pathY) }");
                    return true;
                }

                //*
                //    *
                //        *
                multiplyTranversal1 *= scoreBoard[i, i];
                pathTransversal1 += $"[{i},{i}] ,";
                
                //          *  
                //     *
                //*
                multiplyTranversal2 *= multiplyTranversal2 * scoreBoard[i, (scoreBoard.GetLength(0) -1 ) - i];
                pathTransversal2 += $"[{i},{(scoreBoard.GetLength(0) - 1) - i}] ,";
            }
            if (multiplyTranversal1 == 1 || multiplyTranversal2 == 1)
            {
                Console.WriteLine($"Path Ganador: {(multiplyTranversal1 == 1 ? pathTransversal1 : pathTransversal2)}");
                return true;
            }

            return false;
        }
        private Index GetIndex(int number) 
        {
            for (int i = 0; i < bingoBoard.GetLength(0); i++)
            {
                for (int j = 0; j < bingoBoard.GetLength(1); j++)
                {
                    if (bingoBoard[i, j] == number) 
                    {
                        return new Index { index1 = i, index2 = j };
                    }
                }
            }
            return null;
        }
        internal class Index 
        {
            public int index1 { get; set; }
            public int index2 { get; set; }
        }
    }
}
