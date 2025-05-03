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
    static int height;
    static int width;
    private static bool redrawAfterInput = true;
    static (ConsoleColor, char)[,] buffer = new (ConsoleColor, char)[0, 0];
    static public void Setup()
    {
        SetWindowSize();
        ApplyTextChanges();
    }

    static public void SetBorders(ConsoleColor consoleColor)
    {
        AddToTextBuffer(0, 0, "┏".PadRight(width, '━') + "┓", consoleColor);
        for (var i = 0; i < height - 2; i++)
        {
            AddToTextBuffer(0, i + 1, "┃", consoleColor);
            AddToTextBuffer(width - 1, i + 1, "┃", consoleColor);
        }
        AddToTextBuffer(0, height - 1, "┗".PadRight(width, '━') + "┛", consoleColor);
        ApplyTextChanges();
    }

    static void SetWindowSize()
    {
        height = Console.WindowHeight - 1;
        width = Console.WindowWidth;
        buffer = new (ConsoleColor, char)[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                buffer[y, x] = (ConsoleColor.Black, ' ');
            }
        }
    }

    public static void ApplyTextChanges()
    {
        //Console.WriteLine(string.Join(Environment.NewLine, buffer));
        Console.Clear();
        for (int yPos = 0; yPos < height; yPos++)
        {
            for (int xPos = 0; xPos < width; xPos++)
            {
                var (color, ch) = buffer[yPos, xPos];
                Console.ForegroundColor = color;
                Console.Write(ch);
            }
            Console.Write(Environment.NewLine);
        }
        Console.ResetColor();
    }

    public static void AddToTextBuffer_charOnly(int posX, int posY, char charact, ConsoleColor consoleColor = ConsoleColor.White)
    {
        buffer[posY, posX] = (consoleColor, charact);
    }

    public static void AddToTextBuffer(int posX, int posY, string text, ConsoleColor consoleColor = ConsoleColor.White)
    {
        foreach (char c in text)
        {
            if (posX >= width)
            {
                posX = 0;
                posY++;
            }

            if (posY >= height)
                break;

            AddToTextBuffer_charOnly(posX, posY, c, consoleColor);
            posX++;
        }
    }

    public static void DrawBox(int posX, int posY, int sizeX, int sizeY, ConsoleColor consoleColor = ConsoleColor.White)
    {
        AddToTextBuffer(posX, posY, "┏".PadRight(sizeX, '━') + "┓", consoleColor);
        for (var i = 0; i < sizeY - 2; i++)
        {
            AddToTextBuffer(posX, posY + i + 1, "┃", consoleColor);
            AddToTextBuffer(posX + sizeX, posY + i + 1, "┃", consoleColor);
        }
        AddToTextBuffer(posX, posY + sizeY - 1, "┗".PadRight(sizeX, '━') + "┛", consoleColor);
        ApplyTextChanges();
    }

    public static void CleanArea(int posX, int posY, int sizeX, int sizeY, ConsoleColor consoleColor = ConsoleColor.White)
    {
        AddToTextBuffer(posX, posY, " ".PadRight(sizeX, ' ') + " ", consoleColor);
        for (var i = 0; i < sizeY - 2; i++)
        {
            AddToTextBuffer(posX, posY + i + 1, " ".PadRight(sizeX + 1, ' '), consoleColor);
        }
        AddToTextBuffer(posX, posY + sizeY - 1, " ".PadRight(sizeX, ' ') + " ", consoleColor);
        ApplyTextChanges();
    }

    public static Button ShowNGetOptions(List<Button> buttons)
    {
        ConsoleKeyInfo cki = new ConsoleKeyInfo();
        int ypositionForButtons = height - buttons.Count - 2;
        int selectedBTN_idx = 0;
        while (cki.Key != ConsoleKey.Escape)
        {
            Console.SetCursorPosition(2, ypositionForButtons);
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
                Console.SetCursorPosition(2, Console.CursorTop);
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

    public static string ShowNGetInput(int posX, int posY, int stringMaxLength = 24, bool numberOnly = false, bool passwordText = false)
    {
        Console.SetCursorPosition(posX, posY);
        string? input = "";
        ConsoleKeyInfo cki = new();
        int new_posX = posX;
        while (cki.Key != ConsoleKey.Enter)
        {
            if (input.Length >= stringMaxLength)
            {
                continue;
            }
            cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.Enter) break; // to ensure the input will not be destroyed on enter, so we can just see the text :)

            if (cki.Key == ConsoleKey.Backspace)
            {
                
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
