using ChessChallenge.API;
using Raylib_cs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

public class MyBot : IChessBot
{
    public int nodes = 0;
    public Move Think(Board board, Timer timer)
    {
        Move bestMove = board.GetLegalMoves()[0];
        Double Material = 0;
        Double MaxMaterial = Double.MinValue;
        Double MinMaterial = Double.MaxValue;
        bool isMaximizing = board.IsWhiteToMove;
        
        int depth = 2;

        // w/ Depth
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            Material = Eval(board, depth, isMaximizing);
            if (isMaximizing)
            {
                if (Material > MaxMaterial)
                {
                    MaxMaterial = Material;
                    bestMove = move;
                }
            }
            else
            {
                if (Material < MinMaterial)
                {
                    MinMaterial = Material;
                    bestMove = move;
                }
            }
            
            //Material = 0;
            board.UndoMove(move);
        }

        /*
        foreach (Move move in allMoves)
        {
            board.MakeMove(move);

            // Always play checkmate in one
            if (board.IsInCheckmate())
            {
                bestMove = move;
                break;
            }

            

            Material = MoveMaterial(board);

            if (board.IsInCheck())
            {
                Material += 0.1;
            }


            if (board.IsDraw())
            {
                Material = 0;
            }

            foreach (Move move2 in board.GetLegalMoves())
            {
                board.MakeMove(move2);
            
                Material2 = MoveMaterial(board);
            
                if (board.IsInCheckmate())
                {
                    Material2 += 100;
                }
            
                if (board.IsInCheck())
                {
                    Material2 += 0.1;
                }
            
                if (board.IsDraw())
                {
                    Material2 = 0;
                }
            
                if (Material2 < MaxMaterial2)
                {
                    MaxMaterial2 = Material2 * -1;
                }
            
                Material2 = 0;
                board.UndoMove(move2);
            }
            Console.WriteLine(MaxMaterial2);
            Material = (Material + MaxMaterial2) / 2;
            MaxMaterial2 = 1000;

            if (Material > MaxMaterial)
            {
                MaxMaterial = Material;
                bestMove = move;
            }

            Material = 0;
            board.UndoMove(move);
        }
        */

        if (isMaximizing) { Console.WriteLine("Max: " + Math.Round(MaxMaterial, 2) + " N: " + nodes); } else {Console.WriteLine("Min: " + Math.Round(MinMaterial, 2) + " N: " + nodes); } //Coment for Bot games
        //Console.WriteLine("StaticEval: " + MoveMaterial(board));
        nodes = 0;
        return bestMove;
    }

    public Double Eval (Board board, int depth, bool Maximizing)
    {
        depth--;
        Double material = 0;
        Double maxMaterial = Double.MinValue;
        Double minMaterial = Double.MaxValue;

        if (depth <= 0 || board.IsInCheckmate() || board.IsDraw())
        {
            material = MoveMaterial(board);

            return material;
        }

        if (Maximizing)
        {
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                material = Eval(board, depth, false);
                maxMaterial = Math.Max(material, maxMaterial);
                board.UndoMove(move);
            }
            //Console.WriteLine(maxMaterial + " D: " + depth);
            return maxMaterial;
        }
        else
        {
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                material = Eval(board, depth, true);
                minMaterial = Math.Min(material, minMaterial);
                board.UndoMove(move);
            }
            //Console.WriteLine(minMaterial + " D: " + depth);
            return minMaterial;
        }
    }

    public Double MoveMaterial(Board board)
    {
        nodes++;
        Double Material = 0;
        int Row = 0;
        String FenBoard = board.GetFenString();
        bool isWhite = board.IsWhiteToMove;

        if (board.IsInCheckmate())
        {
            if (isWhite)
            {
                return -1000;
            }
            else
            {
                return 1000;
            }
        }
        if (board.IsDraw())
        {
            return 0;
        }
     

        for (int i = 0; i < FenBoard.Length; i++)
        {
            switch (FenBoard[i])
            {
                case ' ':
                    i = 100;
                    break;
                case '/':
                    Row++;
                    break;
                case 'p':
                    Material -= 1 + Row * 0.1;
                    break;
                case 'b':
                    Material -= 3;
                    break;
                case 'n':
                    Material -= 3;
                    break;
                case 'r':
                    Material -= 5;
                    break;
                case 'q':
                    Material -= 9;
                    break;

                case 'P':
                    Material += 1 + (7 - Row) * 0.1;
                    break;
                case 'B':
                    Material += 3;
                    break;
                case 'N':
                    Material += 3;
                    break;
                case 'R':
                    Material += 5;
                    break;
                case 'Q':
                    Material += 9;
                    break;
            }
        }

        if (board.IsInCheck())
        {
            if (isWhite)
            {
                Material -= 0.25;
            }
            else
            {
                Material += 0.25;
            }
        }
        
        return Material;
    } 
 }