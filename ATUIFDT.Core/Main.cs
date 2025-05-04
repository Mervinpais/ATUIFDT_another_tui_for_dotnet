using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.VisualBasic;

namespace ATUIFDT.Core;

public static class Core
{
    public static void ClearScreen()
    {
        Console.Clear();
    }

    public static void WaitForExit()
    {
        Console.SetCursorPosition(2, Console.WindowHeight - 1);
        Console.ResetColor();
        Console.WriteLine("Press any key to exit");
        Console.ReadKey(true);
        Environment.Exit(0);
    }

    public class Window
    {
        int startX = -1;
        int startY = -1;
        int endX = -1;
        int endY = -1;
        int height;
        int width;
        (ConsoleColor, char)[,] buffer = new (ConsoleColor, char)[0, 0];

        public Window(int StartX, int StartY, int EndX, int EndY, (ConsoleColor, char)[,] Buffer = null)
        {
            startX = StartX; startY = StartY;
            endX = EndX; endY = EndY;
            //height = Height; width = Width; Height and width will be set automatically on setup
            if (Buffer != null)
            {
                buffer = Buffer;
            }
        }

        public void Setup()
        {
            Console.ResetColor();

            SetWindowSize();
            ApplyTextChanges();
        }

        public void SetBorders(ConsoleColor consoleColor)
        {
            DrawBox(0, 0, width, height - 1, consoleColor);
        }

        void SetWindowSize()
        {
            if (startX == -1) { startX = 0; }
            if (startY == -1) { startY = 0; }
            if (endX == -1) { endX = Console.WindowWidth - 1; }
            if (endY == -1) { endY = Console.WindowHeight; }
            height = endY - startY;
            width = endX - startX;
            buffer = new (ConsoleColor, char)[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    buffer[y, x] = (ConsoleColor.Black, ' ');
                }
            }
        }

        public void ApplyTextChanges()
        {
            //Console.WriteLine(string.Join(Environment.NewLine, buffer));
            for (int yPos = 0; yPos < height; yPos++)
            {
                for (int xPos = 0; xPos < width; xPos++)
                {
                    var (color, ch) = buffer[yPos, xPos];
                    Console.ForegroundColor = color;
                    Console.SetCursorPosition(startX + xPos, startY + yPos);
                    Console.Write(ch);
                }
                Console.Write(Environment.NewLine);
            }
            Console.ResetColor();
        }

        public void AddToTextBuffer_charOnly(int posX, int posY, char charact, ConsoleColor consoleColor = ConsoleColor.White)
        {
            buffer[posY, posX] = (consoleColor, charact);
        }

        public void AddToTextBuffer(int posX, int posY, string text, ConsoleColor consoleColor = ConsoleColor.White)
        {
            foreach (char c in text)
            {
                if (posX >= width)
                {
                    posX = 0;
                    posY++;
                }

                if (posY >= height)
                {
                    posY = height - 1;
                }

                AddToTextBuffer_charOnly(posX, posY, c, consoleColor);
                posX++;
            }
        }

        public void RemoveFromTextBuffer_charOnly(int posX, int posY)
        {
            buffer[posY, posX] = (Console.ForegroundColor, ' ');
        }

        public void DrawBox(int posX, int posY, int sizeX, int sizeY, ConsoleColor consoleColor = ConsoleColor.White)
        {
            AddToTextBuffer(posX, posY, "┏".PadRight(sizeX - 1, '━') + "┓", consoleColor);
            for (var i = 1; i < sizeY; i++)
            {
                AddToTextBuffer(posX, posY + i, "┃", consoleColor);
                AddToTextBuffer(posX + sizeX - 1, posY + i, "┃", consoleColor);
            }
            AddToTextBuffer(posX, sizeY + 1, "┗".PadRight(sizeX - 1, '━') + "┛", consoleColor);
            ApplyTextChanges();
        }

        public void CleanArea(int posX, int posY, int sizeX, int sizeY)
        {
            AddToTextBuffer(posX, posY, " ".PadRight(sizeX - 1, ' ') + " ");
            for (var i = 1; i < sizeY; i++)
            {
                AddToTextBuffer(posX, posY + i, " ");
                AddToTextBuffer(posX + sizeX - 1, posY + i, " ");
            }
            AddToTextBuffer(posX, sizeY + 1, " ".PadRight(sizeX - 1, ' ') + " ");
            ApplyTextChanges();
        }

        public Button ShowNGetOptions(List<Button> buttons)
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            int ypositionForButtons = height - buttons.Count - 2;
            int selectedBTN_idx = 0;
            while (cki.Key != ConsoleKey.Escape)
            {
                Console.SetCursorPosition(startX + 2, ypositionForButtons);
                for (int i = 0; i < buttons.Count; i++)
                {
                    Button btn = buttons[i];
                    if (btn.selected == false)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        selectedBTN_idx = i;
                    }
                    Console.WriteLine(btn.text);
                    Console.SetCursorPosition(startX + 2, Console.CursorTop);
                }
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.UpArrow)
                {
                    buttons[selectedBTN_idx].selected = false;
                    selectedBTN_idx--;
                }
                else if (cki.Key == ConsoleKey.DownArrow)
                {
                    buttons[selectedBTN_idx].selected = false;
                    selectedBTN_idx++;
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    buttons[selectedBTN_idx].selected = true;
                    break;
                }
                if (selectedBTN_idx < 0) selectedBTN_idx = 0;
                if (selectedBTN_idx > buttons.Count - 1) selectedBTN_idx = buttons.Count - 1;
                buttons[selectedBTN_idx].selected = true;
            }
            Console.ResetColor();
            return buttons[selectedBTN_idx];
        }

        public class Button
        {
            public string text;
            public bool selected = false;
            public Button(string Text, bool Selected)
            {
                text = Text;
                selected = Selected;
            }
        }

        public string ShowNGetInput(int posX, int posY, int stringMaxLength = 24, bool numberOnly = false, bool passwordText = false)
        {
            Console.SetCursorPosition(posX, posY);
            string? input = "";
            ConsoleKeyInfo cki = new();
            int new_posX = posX;

            if ((posX + startX > startX + width) || (posY + startY > startY + height)) { throw new Exception("Invalid position for Input!"); }

            while (cki.Key != ConsoleKey.Enter)
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter) break; // to ensure the input will not be destroyed on enter, so we can just see the text :)

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (new_posX > posX && new_posX < width)
                    {
                        new_posX--;
                    }
                    RemoveFromTextBuffer_charOnly(new_posX + 1, posY); //to remove the '|' char from previous position
                    RemoveFromTextBuffer_charOnly(new_posX, posY); AddToTextBuffer_charOnly(new_posX, posY, '|'); //add the | character here as we backspace
                    input = input.Substring(0, input.Length - 1); //to fix the bug with the max string length
                    Console.SetCursorPosition(new_posX, posY);
                    ApplyTextChanges();
                    continue;
                }

                //for valid input, below;

                if (input.Length >= stringMaxLength) //ensure we are in our bounds
                {
                    continue;
                }

                if (passwordText && (numberOnly || !numberOnly)) //even if its a number only or not...
                {
                    input += '*';
                    AddToTextBuffer_charOnly(new_posX, posY, '*');
                    AddToTextBuffer_charOnly(new_posX + 1, posY, '|');
                }
                else if (numberOnly)
                {
                    if (cki.KeyChar == '1' || cki.KeyChar == '2' || cki.KeyChar == '3' || cki.KeyChar == '4'
                        || cki.KeyChar == '4' || cki.KeyChar == '5' || cki.KeyChar == '6' || cki.KeyChar == '7'
                        || cki.KeyChar == '8' || cki.KeyChar == '9' || cki.KeyChar == '0')
                    {
                        input += cki.KeyChar;
                        AddToTextBuffer_charOnly(new_posX, posY, cki.KeyChar);
                        AddToTextBuffer_charOnly(new_posX + 1, posY, '|');
                    }
                    else
                    {
                        if (new_posX > posX && new_posX < width)
                        {
                            new_posX--;
                        }
                        Console.SetCursorPosition(new_posX, posY);
                        continue;
                    }
                }
                else
                {
                    input += cki.KeyChar;
                    AddToTextBuffer_charOnly(new_posX, posY, cki.KeyChar);
                    AddToTextBuffer_charOnly(new_posX + 1, posY, '|');
                }
                new_posX++;
                if (new_posX >= width) { posY++; new_posX = posX; }
                ApplyTextChanges();
            }
            if (input == null) { input = ""; }

            return input;
        }
    }
}