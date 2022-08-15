using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryGame;
namespace MemoryGame
{
    internal class InitializeGame
    {
        private static GameLogic m_LogicManager;
        
        public static void RunFirstGame()
        {
            m_LogicManager = new GameLogic();
            StartGame();
        }
        public static void StartGame()
        {
            
            if (!m_LogicManager.IsGameRestarted)
            {
                GetInitialUserInput();
            }
            
            GetGameBoardDimensionsInput();
            m_LogicManager.GameBoard = GameLogic.CreateBoard(m_LogicManager.Height, m_LogicManager.Width);
            DrawBoard();
            bool isRight = false;
            while(!m_LogicManager.AllShown){
                
                if(m_LogicManager.CurrentPlayer.Type == Player.ePlayerType.Computer)
                {
                    isRight = ComputerTurn(); 
                }
                else
                {
                    isRight = PlayerTurn();
                }
                if (isRight)
                {
                    Console.WriteLine("Great you found a match!");
                    Console.WriteLine("{0}'s current score is: {1}", m_LogicManager.CurrentPlayer.Name
                        , m_LogicManager.CurrentPlayer.Score);
                    Console.WriteLine("You get another turn!");
                    System.Threading.Thread.Sleep(2000);
                }
                else
                {
                    m_LogicManager.ChangePlayer();
                    Console.WriteLine("The turn goes to: {0}", m_LogicManager.CurrentPlayer.Name);
                    System.Threading.Thread.Sleep(2000);
                }
                DrawBoard();
                m_LogicManager.isAllTilesShown();
            }
            string ScoreBoard = String.Format(
@"             {0}       {1}
Final score:    {2}    -      {3}", m_LogicManager.Player1.Name, m_LogicManager.Player2.Name,
                               m_LogicManager.Player1.Score, m_LogicManager.Player2.Score);
            Console.WriteLine(ScoreBoard);
            whoWon();
            Console.WriteLine("Would you like to play another game?");
            Console.WriteLine("Please enter Y/N");
            string restartGame = Console.ReadLine();
            if (restartGame == "Y")
            {
                RestartGame();
            }
            EndGame();
        }
        private static void GetInitialUserInput()
        {
            Console.WriteLine("Please enter your name: ");
            string playerNameOne = Console.ReadLine();
            string playerNameTwo = "";
            Player playerOne = new Player(playerNameOne, 0);
            Player playerTwo;
            int choiceNum1;
            Console.WriteLine("Enter 1 to play against another player, or 2 to play against the computer.");
            string choice = Console.ReadLine();
            bool flag = int.TryParse(choice, out choiceNum1);
            while ((choiceNum1!= 1 && choiceNum1 != 2) || !flag)
            {
                Console.WriteLine("You have entered a wrong input, please try again:");
                choice = Console.ReadLine();
                flag = int.TryParse(choice, out choiceNum1);
            }
            if (choiceNum1 == 1)
            {
                Console.WriteLine("Great! You chose to play against another player.");
                Console.WriteLine("Please enter the 2nd player name: ");
                playerNameTwo = Console.ReadLine();
            }
            else if (choiceNum1 == 2)
            {
                Console.WriteLine("Great! You will now play against the computer.");
            }
            if(playerNameTwo == "")
            {
                playerTwo = new Player();
            }
            else
            {
                playerTwo = new Player(playerNameTwo, 0);
            }
            m_LogicManager.Player1 = playerOne;
            m_LogicManager.Player2 = playerTwo;
            m_LogicManager.CurrentPlayer = playerOne;
        }
        private static void GetGameBoardDimensionsInput()
        {
            int widthChoice = 0, heightChoice = 0;
            Console.WriteLine("Enter width(4 or 6): ");
            string widthInput = Console.ReadLine();
            bool flag = int.TryParse(widthInput, out widthChoice);
            while ((widthChoice != 4 && widthChoice != 6) || !flag)
            {
                Console.WriteLine("You have entered a wrong input, please enter 4 or 6: ");
                widthInput = Console.ReadLine();
                flag = int.TryParse(widthInput, out widthChoice);
            }
            Console.WriteLine("Enter height(4 or 6): ");
            string heightInput = Console.ReadLine();
            flag = int.TryParse(heightInput, out heightChoice);
            while ((heightChoice != 4 && heightChoice != 6) || !flag)
            {
                Console.WriteLine("You have entered a wrong input, please enter 4 or 6: ");
                heightInput = Console.ReadLine();
                flag = int.TryParse(heightInput, out heightChoice);
            }
            m_LogicManager.Width = widthChoice;
            m_LogicManager.Height = heightChoice;
        }
        private static void DrawBoard()
        {
            ClearWindow();
            int numOfEqualSigns = (m_LogicManager.Width * 4) + 1;
            string equalLine = string.Format("  {0}", new string('=', numOfEqualSigns));
            DrawLetterRow(m_LogicManager.Width);
            Console.WriteLine(equalLine);
            for (int i = 0; i < m_LogicManager.Height; i++)
            {
                DrawRow(i);
                Console.WriteLine(equalLine);
            }
            Console.WriteLine();
        }
        private static void DrawLetterRow(int i_Length)
        {
            StringBuilder Row = new StringBuilder(" ");
            for (int i = 0; i < i_Length; i++)
            {
                Row.Append(string.Format("   {0}", (char)(i + 'A')));
            }
            Console.WriteLine(Row.ToString());
        }
        private static void DrawRow(int i_IndexOfRow)
        {
            string startOfRow = string.Format("{0} |", i_IndexOfRow + 1);
            Console.Write(startOfRow);
            for (int i = 0; i < m_LogicManager.Width; i++)
            {
                string TileToPrint = string.Format(" {0} |", m_LogicManager.GameBoard[i_IndexOfRow, i].Shown ? m_LogicManager.GameBoard[i_IndexOfRow, i].Letter : ' ');
                Console.Write(TileToPrint);
            }
            Console.WriteLine();
        }
        private static void ClearWindow()
        {
            Ex02.ConsoleUtils.Screen.Clear();
        }  
        public static bool PlayerTurn()
        {
            bool playerIsRight = false;
            string firstTileCoordinate = "";
            string secondTileCoordinate = "";
            Console.Write("Enter a Column(Letter) and a Row(Digit) to pick a tile: ");
            InputValidation(true, ref firstTileCoordinate);
            Tile firstTile = GetTileAndShow(firstTileCoordinate);
            DrawBoard(); 
            Console.WriteLine("Please pick the second tile!");
            Console.Write("Enter a Column(Letter) and a Row(Digit) to pick a tile: ");
            InputValidation(true, ref secondTileCoordinate);
            Tile secondTile = GetTileAndShow(secondTileCoordinate);
            DrawBoard(); 
            char firstTileLetter = firstTile.Letter;
            char secondTileLetter = secondTile.Letter;
            if (firstTileLetter.Equals(secondTileLetter))
            {

                m_LogicManager.CurrentPlayer.Score += 1;
                playerIsRight = true;
                
            }
            else
            {
                Console.WriteLine("You failed! try to memorize the tiles!");
                System.Threading.Thread.Sleep(2000);
                DrawBoard();
                m_LogicManager.GameBoard[firstTile.Row, firstTile.Column].Shown = false;
                m_LogicManager.GameBoard[secondTile.Row, secondTile.Column].Shown = false;
                DrawBoard();
                System.Threading.Thread.Sleep(1000);


            }
            return playerIsRight;
        }

        public static bool ComputerTurnAI(char i_Letter)
        {
            bool isComputerRight = false;
            int i = m_LogicManager.CurrentPlayer.ComputersAI[i_Letter][0];
            int j = m_LogicManager.CurrentPlayer.ComputersAI[i_Letter][1];
            Console.WriteLine("COMPTURN AI:");
            Console.WriteLine(i);
            Console.WriteLine(j);
            m_LogicManager.GameBoard[i, j].Shown = true;
            if (i_Letter == m_LogicManager.GameBoard[i, j].Letter)
            {
                isComputerRight = true;
                m_LogicManager.CurrentPlayer.Score += 1;
            }
            else
            {
                m_LogicManager.GameBoard[i, j].Shown = false;
            }
            DrawBoard();
            
            return isComputerRight;
        }
        public static bool ComputerTurn()
        {
            foreach (KeyValuePair<char, int[]> kvp in m_LogicManager.CurrentPlayer.ComputersAI)
            {
                Console.WriteLine("Key = {0}, Value = {1},{2}", kvp.Key, kvp.Value[0], kvp.Value[1]);
                System.Threading.Thread.Sleep(1000);
            }
            int[] AiMemory = new int[2];
            bool isComputerRight = false;
            Random random = new Random();
            int randomColumn = random.Next(m_LogicManager.Width);
            int randomRow = random.Next(m_LogicManager.Height);
            while (m_LogicManager.GameBoard[randomRow, randomColumn].Shown)
            {
                randomColumn = random.Next(m_LogicManager.Width);
                randomRow = random.Next(m_LogicManager.Height);
            }
            AiMemory[0] = randomRow;
            AiMemory[1] = randomColumn;
            Tile tile1 = m_LogicManager.GameBoard[randomRow, randomColumn];
            m_LogicManager.GameBoard[randomRow, randomColumn].Shown = true;
            DrawBoard();
            System.Threading.Thread.Sleep(1000);
            if (!m_LogicManager.CurrentPlayer.ComputersAI.ContainsKey(tile1.Letter)){
                m_LogicManager.CurrentPlayer.ComputersAI.Add(tile1.Letter, AiMemory);
                Console.WriteLine("I ADDED: {0},{1},{2}", tile1.Letter, AiMemory[0], AiMemory[1]);
                System.Threading.Thread.Sleep(3000);
            }
            if (m_LogicManager.CurrentPlayer.ComputersAI.ContainsKey(tile1.Letter)
                && m_LogicManager.CurrentPlayer.ComputersAI[tile1.Letter]
                != AiMemory)
            {


                Console.WriteLine("Entered AI FUNC: key is contained in dict and coordinates diff");
                System.Threading.Thread.Sleep(1000);
                isComputerRight = ComputerTurnAI(tile1.Letter);
                if (!isComputerRight)
                {
                    m_LogicManager.GameBoard[randomRow, randomColumn].Shown = false;
                }
            }
            else
            {
                Console.WriteLine("before 2nd random");
                Console.WriteLine(randomColumn);
                Console.WriteLine(randomRow);
                System.Threading.Thread.Sleep(3000);

                randomColumn = random.Next(m_LogicManager.Width);
                randomRow = random.Next(m_LogicManager.Height);

                while (m_LogicManager.GameBoard[randomRow, randomColumn].Shown)
                {
                    randomColumn = random.Next(m_LogicManager.Width);
                    randomRow = random.Next(m_LogicManager.Height);
                }
                AiMemory[0] = randomRow;
                AiMemory[1] = randomColumn;
                Console.WriteLine("after 2nd random");
                Console.WriteLine(AiMemory[0]);
                Console.WriteLine(AiMemory[1]);
                System.Threading.Thread.Sleep(3000);

                Tile tile2 = m_LogicManager.GameBoard[randomRow, randomColumn];
                if (!m_LogicManager.CurrentPlayer.ComputersAI.ContainsKey(tile2.Letter)){
                    m_LogicManager.CurrentPlayer.ComputersAI.Add(tile2.Letter, AiMemory);
                    Console.WriteLine("I ADDED 2: {0},{1},{2}", tile2.Letter, AiMemory[0], AiMemory[1]);
                    Console.WriteLine("Dictionary after added the 2nd:");
                    foreach (KeyValuePair<char, int[]> kvp in m_LogicManager.CurrentPlayer.ComputersAI)
                    {
                        Console.WriteLine("Key = {0}, Value = {1},{2}", kvp.Key, kvp.Value[0], kvp.Value[1]);
                        System.Threading.Thread.Sleep(3000);
                    }
                    System.Threading.Thread.Sleep(3000);
                }
                
                m_LogicManager.GameBoard[randomRow, randomColumn].Shown = true;
                DrawBoard();
                if (tile1.Letter.Equals(tile2.Letter))
                {
                    m_LogicManager.CurrentPlayer.Score += 1;
                    isComputerRight = true;
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                    m_LogicManager.GameBoard[tile1.Row, tile1.Column].Shown = false;
                    m_LogicManager.GameBoard[tile2.Row, tile2.Column].Shown = false;
                    DrawBoard();
                    System.Threading.Thread.Sleep(1000);

                }
            }
            
            
            return isComputerRight;
        }
        public static void InputValidation(bool i_TilePick,ref string io_Coordinate)
        {
            while (i_TilePick)
            {
                io_Coordinate = Console.ReadLine();
                if(io_Coordinate.Equals("Q"))
                {
                    EndGame();
                }
                bool isValid = IsCoordinateValid(io_Coordinate);
                if (!isValid)
                {
                    Console.Write("Input is not valid." +
                        " Enter a Column(Letter) and a Row(number) to pick a tile: ");
                    continue;
                }
                bool isOutOfBounds = IsCoordinateOutOfBounds(io_Coordinate);
                if (isOutOfBounds)
                {
                    Console.Write("Tile is out of board bounds. Choose another tile: ");
                    continue;
                }

                bool isShown = IsCoordinateShown(io_Coordinate);
                if (isShown)
                {
                    Console.Write("Tile has been revealed already. Choose another tile: ");
                }
                else
                {
                    i_TilePick = false;
                }
            }
        }
        public static Tile GetTileAndShow(string i_Coordinate)
        {
            int i = GetRowFromInput(i_Coordinate);
            int j = GetColumnFromInput(i_Coordinate);
            Tile TileByCoordinate = m_LogicManager.GameBoard[i - 1, j];
            m_LogicManager.GameBoard[i - 1, j].Shown = true;
            return TileByCoordinate;

        }
        public static int GetRowFromInput(string i_Coordinate)
        {
            char row = i_Coordinate[1];
            int j;
            int.TryParse(row.ToString(), out j);
            return j;
        }
        public static int GetColumnFromInput(string i_Coordinate)
        {
            char column = i_Coordinate[0];
            char col = Char.ToUpper(column);
            int i = (int)(col - 65);
            return i;
        }
        public static bool IsCoordinateValid(string i_Coordinate)
        {
            bool isValid;
            if (i_Coordinate.Length != 2)
            {
                isValid = false;
            }
            else
            {
                char firstCoordinate = i_Coordinate[0];
                char secondCoordinate = i_Coordinate[1];
                if (Char.IsLetter(firstCoordinate) && Char.IsDigit(secondCoordinate))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        public static bool IsCoordinateOutOfBounds(string i_Coordinate)
        {
            bool isOutOfBounds;
            int i = GetRowFromInput(i_Coordinate);
            int j = GetColumnFromInput(i_Coordinate);
            if (i > m_LogicManager.GameBoard.GetLength(0) || j > m_LogicManager.GameBoard.GetLength(1))
            {
                isOutOfBounds = true;
            }
            else
            {
                isOutOfBounds = false;
            }
            
            return isOutOfBounds;
        }
        public static bool IsCoordinateShown(string i_Coordinate)
        {
            bool isCoordinateShown;
            int i = GetRowFromInput(i_Coordinate);
            int j = GetColumnFromInput(i_Coordinate);
            Tile chosenTile = m_LogicManager.GameBoard[i - 1, j];
            if (chosenTile.Shown)
            {
                isCoordinateShown = true;
            }
            else
            {
                isCoordinateShown = false;
            }
            return isCoordinateShown;
        }
        private static void RestartGame()
        {
            ClearWindow();
            m_LogicManager.IsGameRestarted = true;
            StartGame();
        }
        private static void EndGame()
        {
            ClearWindow();
            Console.WriteLine("Bye Bye!");
            System.Threading.Thread.Sleep(2000);
            Environment.Exit(0);

        }
        private static void whoWon()
        {
            if(m_LogicManager.Player1.Score > m_LogicManager.Player2.Score)
            {
                Console.WriteLine("{0} won the game!", m_LogicManager.Player1.Name);
            }
            else if(m_LogicManager.Player1.Score == m_LogicManager.Player2.Score)
            {
                Console.WriteLine("Its a TIE!");
            }
            else
            {
                Console.WriteLine("{0} won the game!", m_LogicManager.Player2.Name);
            }
        }
    }
}


