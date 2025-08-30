using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{

    [Export]
    public Node2D Spawnpoint;

    public const float Speed = 200.0f;
    public const float JumpVelocity = -300.0f;

    // Time after leaving the ground that jumps are accepted. For my own sanity.
    [Export]
    protected float CoyoteTime = 0.1f;

    // When enabled, dashes can be cut short by releasing the dash key.
    [Export]
    protected bool useDashCutting = true;

    public AnimationPlayer anim;
    public Timer timer;
    public AnimatedSprite2D sprite;
    public InteractionBox iBox;

    public bool pointingRight = true;

    // BOON Handlers
    public int getTotalDoubleJumps()
    {
        HashSet<string> bs = GameState.GetGSInstance().boons;
        if (bs.Contains("Double Jump 2"))
        {
            return 2;
        }
        else if (bs.Contains("Double Jump 1"))
        {
            return 1;
        }
        return 0;
    }

    public int totalDoubleJumps;

    public int jumpsLeft;

    public bool getDashEnabled()
    {
        HashSet<string> bs = GameState.GetGSInstance().boons;
        return bs.Contains("Dash");
    }

    public bool isDirectionalDashEnabled()
    {
        HashSet<string> bs = GameState.GetGSInstance().boons;
        return bs.Contains("DirectionalDash");

    }

    public int getMaxDashes()
    {
        HashSet<string> bs = GameState.GetGSInstance().boons;
        return bs.Contains("DoubleDirectionalDash") ? 2 : 1;
    }


    public bool dashing;

    private bool _dashEnabled;

    private int _dashCharges = 0;

    private Vector2 _dashVelocity;

    private Vector2 _moveDirection;

    private ulong _lastGroundedTime;

    public override void _Ready()
    {
        jumpsLeft = getTotalDoubleJumps();
        totalDoubleJumps = getTotalDoubleJumps();
        _dashEnabled = getDashEnabled();
        _dashCharges = getMaxDashes();
        anim = GetNode<AnimationPlayer>("%Anim");
        timer = GetNode<Timer>("%Timer");
        sprite = GetNode<AnimatedSprite2D>("%Sprite");
        iBox = GetNode<InteractionBox>("%InteractionBox");
        Transform = Spawnpoint.Transform;
    }

    // handle movement
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Reset double jumps on floor
        if (jumpsLeft < totalDoubleJumps && IsOnFloor())
        {
            jumpsLeft = totalDoubleJumps;
        }
        // Update for CanJump state. 
        if (IsOnFloor())
        {
            _lastGroundedTime = Time.GetTicksMsec();
        }

        if (dashing)
        {
            velocity = _dashVelocity;
        }
        else
        {
            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
                sprite.Play("falling");
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("jump") &&
                (CanJump() || jumpsLeft > 0))
            {
                if (!CanJump())
                {
                    jumpsLeft -= 1;
                }
                velocity.Y = JumpVelocity;
                _lastGroundedTime = 0;
            }

            _moveDirection = Input.GetVector("left", "right", "up", "down");
            if (_moveDirection != Vector2.Zero)
            {
                velocity.X = _moveDirection.X * Speed;

                // Handle walk anims
                setDirection(_moveDirection);
                if (IsOnFloor())
                {
                    sprite.Play("run");
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                if (IsOnFloor())
                {
                    sprite.Play("idle");
                }
            }
        }

        // Handle interaction prompt update on move if there are multiple in
        // range
        if ((velocity.X != 0 || velocity.Y != 0) &&
            iBox.interactablesInRange.Count > 1)
        {

            GameState.GetGSInstance().EmitSignal(
                GameState.SignalName.InteractionUpdate,
                iBox.find_nearest_interactable());
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // Handle special inputs
    public override void _Input(InputEvent @event)
    {
        // TODO: If cannot dash, communicate to player
        if (@event.IsActionPressed("dash") && canDash())
        {
            anim.Play("Dash");
            _dashVelocity = new Vector2((pointingRight ? 2f : -2f) * Speed, 0);

            if (isDirectionalDashEnabled())
            {
                _dashVelocity = Speed * 2f * _moveDirection;
            }
            _dashCharges--;
        }

        if (dashing && Input.IsActionJustReleased("dash"))
        {
            endDash();
        }

        // Interaction
        if (@event.IsActionPressed("interact") &&
            iBox.interactablesInRange.Count > 0)
        {
            IInteractable target =
                (IInteractable)iBox.find_nearest_interactable();
            if (target.canInteract())
            {
                target.interact();
                GameState.GetGSInstance().EmitSignal(
                    GameState.SignalName.InteractionUpdate,
                    iBox.find_nearest_interactable());
            }
        }
    }

    public bool CanJump()
    {
        return IsOnFloor() || Time.GetTicksMsec() - _lastGroundedTime < CoyoteTime * 1000f;
    }

    public void setDirection(Vector2 dir)
    {
        pointingRight = dir.X > 0;
        sprite.FlipH = !pointingRight;
    }

    private bool canDash()
    {
        return _dashEnabled && !dashing && _dashCharges > 0;
    }

    public void dash()
    {
        dashing = true;
    }

    public void endDash()
    {
        dashing = false;
        Velocity = Vector2.Zero;
        timer.Start();
    }

    // Dash timer finished
    private void OnTimerTimeout()
    {
        _dashCharges = getMaxDashes();
    }

    // Has hit damage object
    private void OnHitBoxBodyShapeEntered(Godot.Rid rid, Node2D body,
                                          int shape_index,
                                          int local_shape_index)
    {

        GameState gs = GameState.GetGSInstance();
        gs.lives -= 1;
        if (gs.lives > 0)
        {
            Transform = Spawnpoint.Transform;
        }
        else
        {
            gs.loseRound();
        }
    }

    // Has hit key
    private void OnKeyBoxAreaShapeEntered(Godot.Rid rid, Node2D body,
                                          int shape_index,
                                          int local_shape_index)
    {
        GameState.GetGSInstance().keys += 1;
        body.QueueFree();
    }
    private void OnInteractionBoxAreaShapeEntered(Godot.Rid rid, Node2D body,
                                                  int shape_index,
                                                  int local_shape_index)
    { }
}
