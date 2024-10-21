using ChessChallenge.API;
using Raylib_cs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

public class EvilBot : IChessBot
{
    public int nodes = 0;
    public Move Think(Board board, Timer timer)
    {
        Move bestMove = board.GetLegalMoves()[0];
        Double Material = 0;
        Double MaxMaterial = Double.MinValue;
        Double MinMaterial = Double.MaxValue;
        bool isMaximizing = board.IsWhiteToMove;

        int depth = 4;

        // w/ Depth
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            Material = Eval(board, depth, Double.MinValue, Double.MaxValue, !isMaximizing);
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
            board.UndoMove(move);
        }

        nodes /= 1000;
        //if (isMaximizing) { Console.WriteLine("Max: " + Math.Round(MaxMaterial, 2) + " N: " + nodes + "k"); } else { Console.WriteLine("Min: " + Math.Round(MinMaterial, 2) + " N: " + nodes + "k"); } //Coment for Bot games
        //Console.WriteLine("StaticEval: " + MoveMaterial(board, 1));
        //nodes = 0;
        return bestMove;
    }

    public Double Eval(Board board, int depth, Double alpha, Double beta, bool Maximizing)
    {
        depth--;
        Double material = 0;
        Double maxMaterial = Double.MinValue;
        Double minMaterial = Double.MaxValue;

        if (depth <= 0 || board.IsInCheckmate() || board.IsDraw())
        {
            material = MoveMaterial(board, depth);

            return material;
        }

        if (Maximizing)
        {
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                material = Eval(board, depth, alpha, beta, false);
                maxMaterial = Math.Max(material, maxMaterial);
                board.UndoMove(move);

                alpha = Math.Max(material, alpha);
                if (beta <= alpha)
                {
                    break;
                }
            }
            //Console.WriteLine(maxMaterial + " D: " + depth);
            return maxMaterial;
        }
        else
        {
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                material = Eval(board, depth, alpha, beta, true);
                minMaterial = Math.Min(material, minMaterial);
                board.UndoMove(move);

                beta = Math.Min(material, beta);
                if (beta <= alpha)
                {
                    break;
                }
            }
            //Console.WriteLine(minMaterial + " D: " + depth);
            return minMaterial;
        }
    }

    public Double MoveMaterial(Board board, int depth)
    {
        //nodes++;
        Double Material = 0;
        int Row = 0;
        String FenBoard = board.GetFenString();
        bool isWhite = board.IsWhiteToMove;

        if (board.IsInCheckmate())
        {
            if (isWhite)
            {
                return -10000 / (depth + 1); //play first Mate
            }
            else
            {
                return 10000 / (depth + 1);
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