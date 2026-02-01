using System;
using Godot;

public partial class SpeechText : RichTextLabel
{
    public int currentLine = 0;

    public void ScrollDown()
    {
        currentLine = Math.Min(currentLine + 1, GetLineCount() - 1);
        if (currentLine == GetLineCount() - 1)
        {
            currentLine = 0;
        }
        ScrollToLine(currentLine);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
