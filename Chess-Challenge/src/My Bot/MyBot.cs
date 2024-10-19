using ChessChallenge.API;
using Raylib_cs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Reflection;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Double Material = 0;
        Double MaxMaterial = Double.MinValue;
        Double MinMaterial = Double.MaxValue;
        //Double Material2 = 0;
        //Double MaxMaterial2 = 1000;
        int Depth = 4;

        Move bestMove = board.GetLegalMoves()[0];

        ///* w/ Depth
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            Material = Eval(board, Depth, true, true);
            if (board.IsWhiteToMove)
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
                    MaxMaterial = Material;
                    bestMove = move;
                }
            }
            
            Material = 0;
            board.UndoMove(move);
        }
        //*/
        
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
        
        Console.WriteLine("Max: " + MaxMaterial); //Coment for Bot games
        MaxMaterial = Double.MinValue;
        return bestMove;
    }

    public Double Eval (Board board, int Depth, bool Maximizing, bool isFirst)
    {
        Double material = 0.69;
        Double maxMaterial = Double.MinValue;
        Double minMaterial = Double.MaxValue;

        foreach (Move move in board.GetLegalMoves())
        {

            if (!isFirst)
            {
                board.MakeMove(move);
            }

            if (board.IsInCheckmate())
            {
                board.UndoMove(move);
                if (Maximizing)
                {
                    return 10000;
                }
                else
                {
                    return -10000;
                } 
            }

            //material = MoveMaterial(board);

            if (Depth > 0)
            {
                Depth--;
                material = (MoveMaterial(board) + Eval(board, Depth, !Maximizing, false)) / 2;
            }
            if (Depth == 0)
            {
                
                if (Maximizing)
                {
                    if (material > maxMaterial)
                    {
                        maxMaterial = material;
                    }
                }
                else
                {
                    if (material < minMaterial)
                    {
                        minMaterial = material;
                    }
                }
            }
            Console.WriteLine(material + " D: " + Depth);
            if (!isFirst)
            {
                board.UndoMove(move);
            }
            material = 0;
        }

        if (Maximizing)
        {
            return maxMaterial;
        }
        else
        {
            return minMaterial;
        }
    }

    public Double MoveMaterial(Board board)
    {
        Double Material = 0;
        int Row = 0;
        String FenBoard = board.GetFenString();


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
                    Material += 1 + Row * 0.1;
                    break;
                case 'b':
                    Material += 3;
                    break;
                case 'n':
                    Material += 3;
                    break;
                case 'r':
                    Material += 5;
                    break;
                case 'q':
                    Material += 9;
                    break;

                case 'P':
                    Material -= 1 + (8 - Row) * 0.1;
                    break;
                case 'B':
                    Material -= 3;
                    break;
                case 'N':
                    Material -= 3;
                    break;
                case 'R':
                    Material -= 5;
                    break;
                case 'Q':
                    Material -= 9;
                    break;
            }
        }
        if (!board.IsWhiteToMove)
        {
            Material *= -1;
        }
        
        return Material;
    } 
 }