using System;
using System.Threading;

class Program
{
    // Snake
    static int width = 40;
    static int height = 20;
    static (int x, int y)[] snake = new (int, int)[1000];
    static int snakeLength = 1;
    static (int x, int y) food;
    static int score = 0;
    static ConsoleKey direction = ConsoleKey.RightArrow;
    static bool gameOver = false;

    // Saper
    static int saperWidth = 10;
    static int saperHeight = 10;
    static int mines = 10;
    static char[,] board = new char[saperWidth, saperHeight];
    static bool[,] revealed = new bool[saperWidth, saperHeight];
    static bool[,] mineField = new bool[saperWidth, saperHeight];
    static bool saperOver = false;

    static void Main()
    {
        Console.Title = "Wybierz grę";
        Console.CursorVisible = false;

        Console.WriteLine("Witaj w grach konsolowych!");
        Console.WriteLine("1. Snake 🐍");
        Console.WriteLine("2. Saper 💣");
        Console.Write("Wybierz grę (1/2): ");
        string choice = Console.ReadLine();

        if (choice == "1")
            RunSnake();
        else if (choice == "2")
            RunSaper();
        else
            Console.WriteLine("Nieznana opcja.");
    }

    // --- Snake ---
    static void RunSnake()
    {
        Console.Clear();
        Console.SetWindowSize(width + 2, height + 3);
        Console.SetBufferSize(width + 2, height + 3);
        StartGameSnake();

        while (!gameOver)
        {
            DrawSnake();
            InputSnake();
            LogicSnake();
            Thread.Sleep(100);
        }

        Console.Clear();
        Console.WriteLine("Game Over!");
        Console.WriteLine($"Final Score: {score}");
        Console.ReadKey();
    }

    static void StartGameSnake()
    {
        snakeLength = 1;
        snake[0] = (width / 2, height / 2);
        direction = ConsoleKey.RightArrow;
        score = 0;
        gameOver = false;
        SpawnFood();
    }

    static void SpawnFood()
    {
        Random rand = new Random();
        do
        {
            food = (rand.Next(1, width - 1), rand.Next(1, height - 1));
        } while (SnakeContains(food));
    }

    static bool SnakeContains((int x, int y) pos)
    {
        for (int i = 0; i < snakeLength; i++)
            if (snake[i] == pos)
                return true;
        return false;
    }

    static void DrawSnake()
    {
        Console.SetCursorPosition(0, 0);
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                if (x == 0 || y == 0 || x == width || y == height)
                    Console.Write("#");
                else if (x == food.x && y == food.y)
                    Console.Write("O");
                else if (x == snake[0].x && y == snake[0].y)
                    Console.Write("█");
                else
                {
                    bool body = false;
                    for (int i = 1; i < snakeLength; i++)
                    {
                        if (snake[i].x == x && snake[i].y == y)
                        {
                            Console.Write("█");
                            body = true;
                            break;
                        }
                    }
                    if (!body) Console.Write(" ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Score: {score}");
    }

    static void InputSnake()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if ((key == ConsoleKey.UpArrow && direction != ConsoleKey.DownArrow) ||
                (key == ConsoleKey.DownArrow && direction != ConsoleKey.UpArrow) ||
                (key == ConsoleKey.LeftArrow && direction != ConsoleKey.RightArrow) ||
                (key == ConsoleKey.RightArrow && direction != ConsoleKey.LeftArrow))
                direction = key;
        }
    }

    static void LogicSnake()
    {
        var head = snake[0];
        var newHead = head;
        switch (direction)
        {
            case ConsoleKey.UpArrow: newHead = (head.x, head.y - 1); break;
            case ConsoleKey.DownArrow: newHead = (head.x, head.y + 1); break;
            case ConsoleKey.LeftArrow: newHead = (head.x - 1, head.y); break;
            case ConsoleKey.RightArrow: newHead = (head.x + 1, head.y); break;
        }

        if (newHead.x <= 0 || newHead.x >= width || newHead.y <= 0 || newHead.y >= height || SnakeContains(newHead))
        {
            gameOver = true;
            return;
        }

        for (int i = snakeLength; i > 0; i--)
            snake[i] = snake[i - 1];

        snake[0] = newHead;

        if (newHead == food)
        {
            score += 10;
            snakeLength++;
            SpawnFood();
        }
    }

    // --- Saper ---
    static void RunSaper()
    {
        InitializeSaper();
        PlaceMines();

        while (!saperOver)
        {
            PrintSaper();
            Console.Write("Podaj współrzędne (x y): ");
            string[] input = Console.ReadLine().Split();
            if (input.Length != 2 || !int.TryParse(input[0], out int x) || !int.TryParse(input[1], out int y))
                continue;

            if (x < 0 || x >= saperWidth || y < 0 || y >= saperHeight)
                continue;

            Reveal(x, y);
            if (CheckWinSaper())
            {
                Console.WriteLine("🎉 Wygrałeś!");
                saperOver = true;
            }
        }

        ShowAllMines();
        PrintSaper();
        Console.WriteLine("Koniec gry.");
        Console.ReadKey();
    }

    static void InitializeSaper()
    {
        for (int i = 0; i < saperWidth; i++)
            for (int j = 0; j < saperHeight; j++)
            {
                board[i, j] = '#';
                revealed[i, j] = false;
                mineField[i, j] = false;
            }
        saperOver = false;
    }

    static void PlaceMines()
    {
        Random rand = new Random();
        int placed = 0;
        while (placed < mines)
        {
            int x = rand.Next(saperWidth);
            int y = rand.Next(saperHeight);
            if (!mineField[x, y])
            {
                mineField[x, y] = true;
                placed++;
            }
        }
    }

    // >>> TWOJA WERSJA PrintSaper <<<
    static void PrintSaper()
    {
        Console.Clear();

        // Nagłówek kolumn
        Console.Write("    "); // margines na numer wiersza
        for (int i = 0; i < saperWidth; i++)
        {
            Console.Write($"{i,2} ");
        }
        Console.WriteLine();

        // Linia oddzielająca nagłówek
        Console.Write("    ");
        for (int i = 0; i < saperWidth; i++)
        {
            Console.Write("---");
        }
        Console.WriteLine();

        // Wiersze z numeracją
        for (int j = 0; j < saperHeight; j++)
        {
            Console.Write($"{j,2} | ");
            for (int i = 0; i < saperWidth; i++)
            {
                if (revealed[i, j])
                    Console.Write($" {board[i, j]} ");
                else
                    Console.Write(" # ");
            }
            Console.WriteLine();
        }
    }

    static void Reveal(int x, int y)
    {
        if (revealed[x, y]) return;
        revealed[x, y] = true;

        if (mineField[x, y])
        {
            board[x, y] = '*';
            saperOver = true;
            Console.WriteLine("💥 Trafiłeś na minę!");
            return;
        }

        int count = CountMines(x, y);
        board[x, y] = count == 0 ? ' ' : count.ToString()[0];

        if (count == 0)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && ny >= 0 && nx < saperWidth && ny < saperHeight)
                        Reveal(nx, ny);
                }
        }
    }

    static int CountMines(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && ny >= 0 && nx < saperWidth && ny < saperHeight && mineField[nx, ny])
                    count++;
            }
        return count;
    }

    static void ShowAllMines()
    {
        for (int i = 0; i < saperWidth; i++)
            for (int j = 0; j < saperHeight; j++)
                if (mineField[i, j])
                    board[i, j] = '*';
    }

    static bool CheckWinSaper()
    {
        int revealedCount = 0;
        for (int i = 0; i < saperWidth; i++)
            for (int j = 0; j < saperHeight; j++)
                if (revealed[i, j])
                    revealedCount++;
        return revealedCount == (saperWidth * saperHeight - mines);
    }
}
