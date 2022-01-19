using System;

namespace Checkers
{
    class Game
    {
        public int[,] board;
        // black - czarne pionki, white - białe pionki, -1 - białe pole (nie używane), 0 - puste czarne pole
        int black = 2, white = 1;
        static int order = 0; // zmienna ktora bedzie okreslala kolejnosc gry, jesli order % 2 == 0 gra bialy w przeciwnym wypadku czarny,
                              // zmienna bedzie aktualizowana z kazdym poprawnym wywolaniem funkcji move - rozgrywke rozpoczynaja biale
        public Game() // konstruktor tworzacy plansze startowa, przy utworzeniu nowej instancji klasy
        {
            this.board = new int[,]{{ -1,black,-1,black,-1,black,-1,black},
                    { black,-1,black,-1,black,-1,black,-1},
                    { -1,black,-1,black,-1,black,-1,black},
                    { 0,-1,0,-1,0,-1,0,-1 },
                    { -1,0,-1,0,-1,0,-1,0},
                    { white,-1,white,-1,white,-1,white,-1},
                    { -1,white,-1,white,-1,white,-1,white},
                    { white,-1,white,-1,white,-1,white,-1},
            };
        }
        public void DisplayBoard()
        {
            string name;
            if (order % 2 == 0) name = "białych";
            else name = "czarnych";
            Console.WriteLine($"              Ruch {name}.");
            for (int i = 0; i < 8; i++)
            {
                if (i == 0)
                {
                    Console.WriteLine("\n     | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | ");
                    Console.WriteLine("      ________________________________");
                }
                for (int j = 0; j < 8; j++)
                {
                    if (j == 0) Console.Write($"  {i + 1}  |");
                    switch (board[i, j])
                    {
                        case -1:
                            Console.Write("   |");
                            break;
                        case 0:
                            Console.Write("   |");
                            break;
                        case 1: // white
                            Console.Write(" ■ |");
                            break;
                        case 2: // black
                            Console.Write(" O |"); // █
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("   --|-------------------------------|");
            }
        }
        public void Move(int i0, int j0, int i, int j) // (i0,j0) wspolrzedne wybranego pionka, (i,j) wspolrzedne docelowe
        {
            int flag = 0;
            int pawn;
            if (order % 2 == 0) pawn = white;
            else pawn = black;
            try
            {
                // wybor zlego pionka - zla kolejnosc
                if ((board[i0,j0] == white && pawn == black) || (board[i0,j0] == black && pawn == white))
                {
                    flag = 1; goto error;
                }
                // bicie pojedyncze
                else if ((Math.Abs(j0 - j) == 2) && ((pawn == white && i0 - i == 2) || (pawn == black && i - i0 == 2)))
                {
                    if (board[i, j] == 0)
                    {
                        int mv_i = 0, mv_j = 0; // zmienne pomocnicze pozwalające określić gdzie jest atakowany pionek
                        if (pawn == white && j > j0) { mv_i = -1; mv_j = 1; }
                        else if (pawn == white && j < j0) { mv_i = -1; mv_j = -1; }
                        else if (pawn == black && j > j0) { mv_i = 1; mv_j = 1; }
                        else if (pawn == black && j < j0) { mv_i = 1; mv_j = -1; }
                        
                        if (mv_i != 0)
                        {
                            board[i0 + mv_i, j0 + mv_j] = 0; // usuwanie atakowanego pionka
                            board[i, j] = board[i0, j0];
                            board[i0, j0] = 0;
                            order++;
                        }
                        else
                        {
                            flag = 2; goto error;
                        }
                        
                    }
                    else
                    {
                        flag = 2; goto error;
                    }
                }
                // sprawdzenie normalnego ruchu do przodu -> diagonalnie o jedno pole
                else if ((pawn == white && i0 - i != 1) || (pawn == black && i - i0 != 1) || Math.Abs(j0 - j) != 1)
                {
                    flag = 2; goto error;
                }
                else if (board[i, j] == 0) // można wykonać ruch tylko w wolne miejsce
                {
                    board[i, j] = board[i0, j0];
                    board[i0, j0] = 0;
                    order++;
                }
                else
                {
                    flag = 3; goto error;
                }
            }
            catch
            {
                flag = 4; goto error;
            }

        error:
            if (flag == 1) 
            {
                string name;
                if (pawn == white) name = "białych";
                else name = "czarnych";
                Console.WriteLine($"Teraz kolej {name}! Spróbuj ponownie...\n");
            } 
            else if (flag == 2) Console.WriteLine("Niepoprawny ruch! Spróbuj ponownie...");
            else if (flag == 3) Console.WriteLine("Wybrane pole jest zajęte! Spróbuj ponownie...");
            else if (flag == 4) Console.WriteLine("Wyjście poza zakres planszy! Spróbuj ponownie...");
        }
        public bool IsFinished()
        {
            int countBlack = 0, countWhite = 0;
            for (int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (board[i, j] == black) countBlack++;
                    else if (board[i, j] == white) countWhite++;
                }
            }
            if (countBlack > 0 && countWhite > 0) return false;
            string name;
            if (countBlack == 0) name = "białe";
            else name = "czarne";
            Console.WriteLine($"Koniec gry! Wygrywają pionki {name}.");
            return true;
        }
        public void Capture() {}
    }
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            int i0, j0, i, j;
            Console.WriteLine("            ■ - pionki białe.");
            Console.WriteLine("            O - pionki czarne.\n");
            while (!game.IsFinished())
            {
                game.DisplayBoard();
                try
                {
                    Console.WriteLine("\nWybierz pionek, którym chcesz się poruszyć.\n");
                    Console.Write("Numer wiersza:\n> "); i0 = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Numer kolumny:\n> "); j0 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Wybierz pole, na które chcesz przesunąć wybrany pionek.\n");
                    Console.Write("Numer wiersza:\n> "); i = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Numer kolumny:\n> "); j = Convert.ToInt32(Console.ReadLine());
                    Console.Clear();
                    game.Move(i0 - 1, j0 - 1, i - 1, j - 1);
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("Złe dane wejściowe! Spróbuj ponownie...");
                }// charmap

            }
        }
    }
}
