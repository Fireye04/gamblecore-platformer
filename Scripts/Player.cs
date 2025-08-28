using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D {
    public const float Speed = 300.0f;
    public const float JumpVelocity = -200.0f;

    public AnimationPlayer anim;
    public Timer timer;

    public bool pointingRight = true;

    // BOON Handlers
    public int getTotalDoubleJumps() {
        HashSet<string> bs = GameState.GetGSInstance().boons;
        if (bs.Contains("Double Jump 2")) {
            return 2;
        } else if (bs.Contains("Double Jump 1")) {
            return 1;
        }
        return 0;
    }

    public int totalDoubleJumps;

    public int jumpsLeft;

    public bool getDashEnabled() {
        HashSet<string> bs = GameState.GetGSInstance().boons;
        return bs.Contains("Dash");
    }
    public bool canDash;

    public bool dashing;

    public override void _Ready() {
        jumpsLeft = getTotalDoubleJumps();
        totalDoubleJumps = getTotalDoubleJumps();
        canDash = getDashEnabled();
        anim = GetNode<AnimationPlayer>("%Anim");
        timer = GetNode<Timer>("%Timer");
    }

    // handle movement
    public override void _PhysicsProcess(double delta) {
        Vector2 velocity = Velocity;

        // Reset double jumps on floor
        if (jumpsLeft < totalDoubleJumps && IsOnFloor()) {
            jumpsLeft = totalDoubleJumps;
        }

        // Add the gravity.
        if (!IsOnFloor()) {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") &&
            (IsOnFloor() || jumpsLeft > 0)) {
            if (!IsOnFloor()) {
                jumpsLeft -= 1;
            }
            velocity.Y = JumpVelocity;
        }

        if (dashing) {
            if (pointingRight) {
                velocity.X = 2 * Speed;
            } else {
                velocity.X = -2 * Speed;
            }
        } else {
            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom
            // gameplay actions.
            Vector2 direction = Input.GetVector("left", "right", "up", "down");
            if (direction != Vector2.Zero) {
                velocity.X = direction.X * Speed;
                pointingRight = velocity.X > 0;
            } else {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // Handle special inputs
    public override void _Input(InputEvent @event) {
        // TODO: If cannot dash, communicate to player
        if (@event.IsActionPressed("dash") && canDash) {
            if (pointingRight) {
                anim.Play("Dash");
            } else {
                anim.Play("Dash_Left");
            }
        }
    }

    public void dash() { dashing = true; }

    public void endDash() {
        dashing = false;
        timer.Start();
    }

    // Dash timer finished
    private void OnTimerTimeout() { canDash = true; }

    // Has hit damage object
    private void OnHitBoxBodyShapeEntered(Godot.Rid rid, Node2D body,
                                          int shape_index,
                                          int local_shape_index) {

        GameState gs = GameState.GetGSInstance();
        gs.resetValues(false);
        gs.CallDeferred(GameState.MethodName.play);
    }
}
