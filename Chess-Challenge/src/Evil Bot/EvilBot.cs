﻿using ChessChallenge.API;
using Raylib_cs;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

public class EvilBot : IChessBot
{
    public int nodes = 0;
    //public int tBhits = 0;
    //Hashtable tableBase = new Hashtable();
    public Move Think(Board board, Timer timer)
    {
        Move[] legalMoves = board.GetLegalMoves();
        Move bestMove = legalMoves[0];
        Move move;
        int amountMoves = legalMoves.Length;
        float[] Material = new float[amountMoves];
        float MaxMaterial = float.MinValue;
        float MinMaterial = float.MaxValue;
        float alpha = float.MinValue;
        float beta = float.MaxValue;
        bool isMaximizing = board.IsWhiteToMove;

        byte depth = 2; //start depth -1

        while (timer.MillisecondsElapsedThisTurn < 80 && timer.MillisecondsRemaining > 5000 || depth < 2)
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
        //if (isMaximizing) { Console.WriteLine("Max: " + Math.Round(MaxMaterial, 2) + " N: " + nodes + "k d: " + depth); } else { Console.WriteLine("Min: " + Math.Round(MinMaterial, 2) + " N: " + nodes + "k d: " + depth); }
        nodes = 0;
        return bestMove;
    }

    public float Eval(Board board, byte depth, float alpha, float beta, bool Maximizing)
    {
        depth--;
        float material;

        if (depth <= 0 || board.IsInCheckmate() || board.IsDraw())
        {
            material = MoveMaterial(board, depth);

            return material;
        }

        if (Maximizing)
        {
            float maxMaterial = float.MinValue;
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
            return maxMaterial;
        }
        else
        {
            float minMaterial = float.MaxValue;
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
        byte file = 0;
        for (int i = 0; i < FenBoard.Length; i++)
        {
            file++;
            switch (FenBoard[i])
            {
                case ' ':
                    i = 100;
                    break;
                case '/':
                    file = 0;
                    Row++;
                    break;
                case 'p':
                    Material -= (1 + Row * 0.05f);
                    break;
                case 'b':
                    Material -= 3.5f;
                    break;
                case 'n':
                    Material -= (float)(3 + -0.01 * Math.Pow((file - 4.5), 2) - 0.01 * Math.Pow((Row - 3.5), 2));
                    break;
                case 'r':
                    Material -= (float)(5.5 - 0.02 * Math.Pow((6 - Row), 2));
                    break;
                case 'q':
                    Material -= 9;
                    break;

                case 'P':
                    Material += 1 + (7 - Row) * 0.05f;
                    break;
                case 'B':
                    Material += 3.5f;
                    break;
                case 'N':
                    Material += (float)(3 + -0.01 * Math.Pow((file - 4.5), 2) - 0.01 * Math.Pow((Row - 3.5), 2));
                    break;
                case 'R':
                    Material += (float)(5.5 - 0.02 * Math.Pow((2 - Row), 2)); ;
                    break;
                case 'Q':
                    Material += 9;
                    break;
                case '2':
                    file += 1;
                    break;
                case '3':
                    file += 2;
                    break;
                case '4':
                    file += 3;
                    break;
                case '5':
                    file += 4;
                    break;
                case '6':
                    file += 5;
                    break;
                case '7':
                    file += 6;
                    break;
            }
        }

        if (isWhite)
        {
            //Material -= board.GetLegalMoves().Length * 0.01f;
            if (board.IsInCheck())
            {
                Material -= 0.25f;
            }
        }
        else
        {
            //Material += board.GetLegalMoves().Length * 0.01f;
            if (board.IsInCheck())
            {
                Material += 0.25f;
            }
        }
        //tableBase.Add(zobristKey, Material);
        return Material;
    }
}