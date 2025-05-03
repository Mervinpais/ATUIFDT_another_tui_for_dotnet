using ATUIFDT.Core;
using static ATUIFDT.Core.Core;

Setup();
SetBorders(ConsoleColor.Blue);
AddToTextBuffer(1, 1, "Hello World!");
AddToTextBuffer(1, 3, "This represents a simple program in our TUI :)");
DrawBox(2, 4, 4, 4, ConsoleColor.Green);
// Task.Delay(1000).Wait();
// Core.CleanArea(2, 4, 4, 4, ConsoleColor.White);
Button button = new Button("Some option 1", true);
Button button2 = new Button("SOME OPTION 2!", false);
Button button3 = new Button("some option 3?", false);

Button btnRecieved = ShowNGetOptions([button, button2, button3]);
if (btnRecieved.text == button2.text)
{
    AddToTextBuffer(3, 5, "So you are a middle guy eh?", ConsoleColor.Yellow);
}
else
{
    AddToTextBuffer(3, 5, "lolz epic", ConsoleColor.Gray);
}

AddToTextBuffer(3, 6, "Enter some input");
ApplyTextChanges(); //ensure that you update text as its never automatic unless its shapes or input or other such being drawn
ShowNGetInput(3, 7);

AddToTextBuffer(3, 8, "Enter some numeric input");
ApplyTextChanges();
ShowNGetInput(3, 9, numberOnly: true);

AddToTextBuffer(3, 10, "Enter some password input");
ApplyTextChanges();
ShowNGetInput(3, 11, passwordText: true);