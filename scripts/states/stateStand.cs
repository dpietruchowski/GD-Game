using State;
using StateContext;

public class StateStand
{
    const float maxVelocity = 0.5f;
    public override void Update(Vector2 velocity)
    {
        var length = velocity.Length();
        if (length > maxVelocity) {
            player.SetState(States.Walking);
            return;
        }
        player.InterpolatePose("T-Pose", (maxVelocity - length)*2);
    }
}