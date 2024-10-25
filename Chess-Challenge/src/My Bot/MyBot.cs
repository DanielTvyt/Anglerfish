using ChessChallenge.API;
using Microsoft.CodeAnalysis.Diagnostics;
using Raylib_cs;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

public class MyBot : IChessBot
{
    public int nodes = 0;
    //public int tBhits = 0;
    Hashtable tableBase = new Hashtable();
    public Move Think(Board board, Timer timer)
    {
        Move bestMove = board.GetLegalMoves()[0];
        Move move;
        int amountMoves = board.GetLegalMoves().Length;
        float[] Material = new float[amountMoves];
        float MaxMaterial = float.MinValue;
        float MinMaterial = float.MaxValue;
        bool isMaximizing = board.IsWhiteToMove;
        byte depth = 2; //start depth -1
        float alpha = float.MinValue;
        float beta = float.MaxValue;
        Move[] legalMoves = board.GetLegalMoves();

        int x = 0;
        foreach (Move move1 in legalMoves)
        {
            Material[x] = 0;
            x++;
        }
        while (timer.MillisecondsElapsedThisTurn < 80 && timer.MillisecondsRemaining > 5000 || depth < 0)
        {
            depth++;
            var res = Material.Select((v, i) => new { v, i })
            .OrderBy(p => p.v)
            .Select(p => p.i)
            .ToList();
            Array.Reverse(Material);

            MaxMaterial = float.MinValue;
            MinMaterial = float.MaxValue;
            alpha = float.MinValue;
            beta = float.MaxValue;

            //Console.WriteLine(bestMove + " D: "+ depth);
            for (int y = 0; y < amountMoves; y++)
            {
                int i = (int)res[y];
                //Console.WriteLine("i " + i + "M: " + Material[i]);
                move = legalMoves[i];
                board.MakeMove(move);

                Material[i] = Eval(board, depth, alpha, beta, !isMaximizing);

                if (isMaximizing)
                {
                    alpha = Math.Max(alpha, Material[i]);

                    if (Material[i] > MaxMaterial)
                    {
                        MaxMaterial = Material[i];
                        bestMove = move;
                    }
                }
                else
                {
                    beta = Math.Min(beta, Material[i]);

                    if (Material[i] < MinMaterial)
                    {
                        MinMaterial = Material[i];
                        bestMove = move;
                    }
                }
                board.UndoMove(move);
            }
        }

        //Console.WriteLine("N/ms: " + nodes/timer.MillisecondsElapsedThisTurn + " TB Hits: " + tBhits);
        nodes /= 1000;
        if (isMaximizing) { Console.WriteLine("Max: " + Math.Round(MaxMaterial, 2) + " N: " + nodes +"k d: " + depth); } else {Console.WriteLine("Min: " + Math.Round(MinMaterial, 2) + " N: " + nodes + "k d: " + depth); }
        nodes = 0;
        return bestMove;
    }

    public float Eval (Board board, byte depth, float alpha, float beta,  bool Maximizing)
    {
        depth--;
        float material;
        float maxMaterial = float.MinValue;
        float minMaterial = float.MaxValue;

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

    public float MoveMaterial(Board board, byte depth)
    {
        nodes++;
        bool isWhite = board.IsWhiteToMove;

        if (board.IsInCheckmate())
        {
            if (isWhite)
            {
                return -1000 * (depth + 1); //play first Mate
            }
            else
            {
                return 1000 * (depth + 1);
            }
        }
        if (board.IsDraw())
        {
            return 0;
        }

        string FenBoard = board.GetFenString();
        
        //ulong zobristKey = board.ZobristKey;
        //if (tableBase.ContainsKey(zobristKey))
        //{
        //    //tBhits++;
        //    return (float)Convert.ToDouble(tableBase[zobristKey]);
        //}

        float Material = 0;
        byte Row = 0;
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
                    Material -= 1 + Row * 0.1f;
                    break;
                case 'b':
                    Material -= 3.5f;
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
                    Material += 1 + (7 - Row) * 0.1f;
                    break;
                case 'B':
                    Material += 3.5f;
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
                Material -= 0.25f;
            }
            else
            {
                Material += 0.25f;
            }
        }
        //tableBase.Add(zobristKey, Material);
        return Material;
    } 
}