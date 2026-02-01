using System;
using System.Numerics;
using Godot;

public partial class TalkNpc : RigidBody2D
{
    // Animation player for showing movement
    private AnimationPlayer _animationPlayer;

    // The Main character - used to check distance for interactions
    private CharacterBody2D _mainChar;
    private MainCharControl _mainScript;

    // Interaction text - enable when player is nearby but is not already interacting
    private Control _interactText;

    // Speech text - enable when player is interacting
    private ColorRect _textBox;

    // Whether the object has been interacted with - set by player
    public bool isInteracted = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("idle");

        _interactText = GetNode<Control>("IntTextControl");
        _textBox = GetNode<ColorRect>("TextBox");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Get MainChar body if not already set
        // This requires the Party be on the same depth as the NPC
        if (_mainChar == null)
        {
            GD.Print("Character not found");
            _mainChar = GetNode<CharacterBody2D>("../Party/MainChar");
            _mainScript = _mainChar as MainCharControl;
        }
        else
        {
            // Check distance to MainChar
            Godot.Vector2 charLoc = _mainChar.GlobalTransform.Origin;
            float distance = charLoc.DistanceTo(GlobalTransform.Origin);

            // If close enough, show interaction text
            if (distance < 70.0f)
            {
                if (_interactText.Visible == false)
                {
                    _interactText.Visible = true;
                }

                if (isInteracted)
                {
                    _textBox.Visible = true;
                    _interactText.Visible = false;
                }

                _mainScript.intObject = this;
                _mainScript.intTextBox = _textBox.GetNode<SpeechText>("SpeechText");
            }
            else
            {
                if (_interactText.Visible == true)
                {
                    _interactText.Visible = false;
                }

                isInteracted = false;
                _textBox.Visible = false;
            }
        }
    }
}
