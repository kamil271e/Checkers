using System;

namespace Checkers
{
    class Game
    {
        public int[,] board; // plansza
        // black - czarne pionki, white - białe pionki, -1 - białe pole (nie używane), 0 - puste czarne pole
        int black = 2, white = 1;
        int blackQueen = 4, whiteQueen = 3; // damki w odpowiednich kolorach
        int pawn; // aktualnie wybrany pionek
        static int order = 0; // zmienna ktora bedzie okreslala kolejnosc gry, jesli order % 2 == 0 gra bialy w przeciwnym wypadku czarny,
                              // zmienna bedzie aktualizowana z kazdym poprawnym wywolaniem funkcji move - rozgrywke rozpoczynaja biale
        public Game() // konstruktor tworzacy plansze startowa, przy utworzeniu nowej instancji klasy
        {
            board = new int[,]{{ -1,0,-1,0,-1,black,-1,black},
                    { 0,-1,white,-1,0,-1,0,-1},
                    { -1,0,-1,black,-1,0,-1,black},
                    { 0,-1,0,-1,0,-1,0,-1 },
                    { -1,0,-1,0,-1,0,-1,0},
                    { 0,-1,0,-1,0,-1,white,-1},
                    { -1,black,-1,0,-1,white,-1,white},
                    { 0,-1,0,-1,0,-1,0,-1},
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
                            Console.Write("   |"); break;
                        case 0:
                            Console.Write("   |"); break;
                        case 1: // white
                            Console.Write(" ■ |"); break;
                        case 2: // black
                            Console.Write(" o |"); break;
                        case 3: // whiteQueen
                            Console.Write(" █ |"); break;
                        case 4: // blackQueen
                            Console.Write(" O |"); break;
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
            if (order % 2 == 0) pawn = white;
            else pawn = black;
            try
            {
                // WYBRANO MIEJSCE PUSTE
                if (board[i0,j0] <= 0) { flag = 6; goto error; }
                
                // WYBOR ZLEGO PIONKA - ZLA KOLEJNOSC
                else if ((pawn == white && board[i0,j0] != white && board[i0,j0] != whiteQueen) || (pawn == black && board[i0,j0] != black && board[i0, j0] != blackQueen))
                {
                    flag = 1; goto error;
                }
                
                // WYBOR ZLEGO POLA DOCELOWEGO - ZAJETE LUB NIEUZYWANE W GRZE
                else if (board[i,j] != 0)
                {
                    if (board[i,j] != -1) { flag = 3; goto error; }
                    else { flag = 5; goto error; }
                }
                
                // DAMKA
                else if ((board[i0,j0] == whiteQueen) || (board[i0,j0] == blackQueen))
                {
                    if (Math.Abs(i - i0) == Math.Abs(j - j0))
                    {
                        int mv_i = 0, mv_j = 0; // zmienne okreslajace kierunek poruszania sie damki
                        InitDirVariablesQueen(ref mv_i, ref mv_j, i0, j0, i, j);

                        int k;
                        for (k = 1; k < Math.Abs(i - i0) - 1; k++)
                        {
                            if (board[i0 + mv_i * k, j0 + mv_j * k] != 0) { flag = 2; goto error; } // sprawdzenie czy na drodze nie ma innego pionka
                        }
                        int opponent = board[i0 + mv_i * k, j0 + mv_j * k];

                        // opponent to puste pole - wykonujemy zwykly ruch
                        if (opponent == 0)
                        {
                            Capture(i0, j0, i, j, -1, -1);
                        }
                        else
                        {
                            // sprawdzenie czy w odpowiednim miejscu nie stoi czasem nasz wlasny pionek
                            if ((board[i0, j0] == whiteQueen && opponent != black && opponent != blackQueen) || (board[i0, j0] == blackQueen && opponent != white && opponent != whiteQueen))
                            {
                                flag = 2; goto error;
                            }
                            // bicie
                            Capture(i0, j0, i, j, i0 + mv_i * k, j0 + mv_j * k);
                        }
                    }
                    else
                    {
                        flag = 2; goto error;
                    }
                }
                
                // PODWOJNE BICIE
                else if (Math.Abs(i0-i) == 4 && (Math.Abs(j0-j) == 0 || Math.Abs(j0-j) == 4))
                {
                    //
                }
                
                // POJEDYNCZE BICIE
                else if ((Math.Abs(j0 - j) == 2) && ((pawn == white && i0 - i == 2) || (pawn == black && i - i0 == 2)))
                {
                    int mv_i = 0, mv_j = 0; // zmienne pomocnicze pozwalające określić gdzie jest atakowany pionek
                    InitDirVariables(ref mv_i, ref mv_j, i0, j0, i, j);

                    int opponent = board[i0 + mv_i, j0 + mv_j];
                    // sprawdzenie czy na pewno przeciwnik znajduje sie w dobrym miejscu
                    if ((pawn == white && (opponent == black || opponent == blackQueen)) || (pawn == black && (opponent == white || opponent == whiteQueen)))
                    {
                        Capture(i0, j0, i, j, i0 + mv_i, j0 + mv_j);
                        if (pawn == white && i == 0) board[i, j] = whiteQueen;
                        else if (pawn == black && i == 7) board[i, j] = blackQueen;
                    }
                    else
                    {
                        flag = 2; goto error;
                    }
                }
                
                // SPRAWDZENIE POPRAWNOSCI RUCHU NORMALNEGO TJ. DIAGONALNIE I DO PRZODU
                else if ((pawn == white && i0 - i != 1) || (pawn == black && i - i0 != 1) || Math.Abs(j0 - j) != 1)
                {
                    flag = 2; goto error;
                }
               
                // ZWYKŁY RUCH
                else
                {
                    Capture(i0, j0, i, j, -1, -1);
                    if (pawn == white && i == 0) board[i, j] = whiteQueen;
                    else if (pawn == black && i == 7) board[i, j] = blackQueen;
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
            else if (flag == 5) Console.WriteLine("Nie można wybrać tego pola! Spróbuj ponownie...");
            else if (flag == 6) Console.WriteLine("Na wybranym miejscu nie ma pionka! Spróbuj ponownie...");
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
        public void InitDirVariablesQueen(ref int mv_i, ref int mv_j, int i0, int j0, int i, int j)
        {
            // 4 mozliwosci :
            if (i0 > i && j0 > j) { mv_i = -1; mv_j = -1; } // lewo gora // poruszamy sie zmniejszajac 'i' i zmniejszajac 'j'
            else if (i0 > i && j0 < j) { mv_i = -1; mv_j = 1; } // prawo gora // poruszamy sie zmniejszajac 'i' i zwiekszajac 'j'
            else if (i0 < i && j0 < j) { mv_i = 1; mv_j = 1; } // prawo dol // poruszamy sie zwiekszajac 'i' i zwiekszajac 'j'
            else if (i0 < i && j0 > j) { mv_i = 1; mv_j = -1; } // lewo dol // poruszamy sie zwiekszajac 'i' i zmniejszajac 'j'

        }
        public void InitDirVariables(ref int mv_i, ref int mv_j, int i0, int j0, int i, int j)
        {
            if (pawn == white && j > j0) { mv_i = -1; mv_j = 1; }
            else if (pawn == white && j < j0) { mv_i = -1; mv_j = -1; }
            else if (pawn == black && j > j0) { mv_i = 1; mv_j = 1; }
            else if (pawn == black && j < j0) { mv_i = 1; mv_j = -1; }
        }
        public void Capture(int i0, int j0, int i, int j, int ix, int jx)
        {
            if (ix != -1 && jx != -1) board[ix, jx] = 0; // usuwanie atakowanego pionka
            board[i, j] = board[i0, j0];
            board[i0, j0] = 0;
            order++;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            int i0, j0, i, j;
            Console.WriteLine("   ■ - pionki białe. █ - biała damka.");
            Console.WriteLine("   o - pionki czarne. O - czarna damka\n");
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
