using ATUIFDT.Core;

Core.Setup();
Core.SetBorders(ConsoleColor.Blue);
Core.AddToTextBuffer(1, 1, "Hello World!");
Core.AddToTextBuffer(1, 3, "This represents a simple program in our TUI :)");
Core.DrawBox(2, 4, 4, 4, ConsoleColor.Green);
// Task.Delay(1000).Wait();
// Core.CleanArea(2, 4, 4, 4, ConsoleColor.White);
Core.Button button = new Core.Button("Hello! there!", true);
Core.Button button2 = new Core.Button("I know more than sin is whatever it is", false);
Core.Button button3 = new Core.Button("Wow :)", false);

Core.Button btnRecieved = Core.ShowNGetOptions(new() { button, button2, button3 });
if (btnRecieved.text == button2.text)
{
    Core.AddToTextBuffer(3, 5, "Wow your english is bad lol", ConsoleColor.DarkYellow);
}
else
{
    Core.AddToTextBuffer(3, 5, "lolz", ConsoleColor.Red);
}

Core.ApplyTextChanges(); //ensure that you update text as its never automatic unless its shapes or such being drawn

Core.ShowNGetInput(3, 6);

Core.ShowNGetInput(4, 7, numberOnly: true);