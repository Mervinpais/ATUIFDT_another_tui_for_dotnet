using ATUIFDT.Core;
using static ATUIFDT.Core.Core;

Window window = new Window(50, 1, 100, 15);
ClearScreen();
window.Setup();
window.SetBorders(ConsoleColor.Blue);
window.DrawBox(3, 2, 10, 5, ConsoleColor.Green);
window.AddToTextBuffer(1, 1, "Hello World!");
window.AddToTextBuffer(1, 3, "This represents a simple program in our TUI :)");
window.ApplyTextChanges();
// Task.Delay(1000).Wait();
// Core.CleanArea(2, 4, 4, 4, ConsoleColor.White);
Window.Button button = new Window.Button("Some option 1", true);
Window.Button button2 = new Window.Button("SOME OPTION 2!", false);
Window.Button button3 = new Window.Button("some option 3?", false);

Window.Button btnRecieved = window.ShowNGetOptions([button, button2, button3]);
if (btnRecieved.text == button2.text)
{
    window.AddToTextBuffer(3, 5, "So you are a middle guy eh?", ConsoleColor.Yellow);
}
else
{
    window.AddToTextBuffer(3, 5, "lolz epic", ConsoleColor.Gray);
}

window.AddToTextBuffer(3, 6, "Enter some input");
window.ApplyTextChanges(); //ensure that you update text as its never automatic unless its shapes or input or other such being drawn
window.ShowNGetInput(3, 7);

window.AddToTextBuffer(3, 8, "Enter some numeric input");
window.ApplyTextChanges();
window.ShowNGetInput(3, 9, numberOnly: true);

window.AddToTextBuffer(3, 10, "Enter some password input");
window.ApplyTextChanges();
window.ShowNGetInput(3, 11, passwordText: true);

Window window2 = new Window(1,1,20,10);
window2.Setup();
window2.SetBorders(ConsoleColor.Blue);
window2.DrawBox(3, 2, 10, 5, ConsoleColor.Green);

WaitForExit();