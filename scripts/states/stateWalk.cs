using State;

public class StateWalk
{    
    float distance = 0;
    bool transition = true;
    const float maxVelocity = 3.0f;
    const float minVelocity = 0.5f;
    String[] poses = {"Walk1", "Walk2", "Walk3" ,"Walk4"}; 
    public override void Update(Vector2 velocity)
    {
        var length = velocity.Length();
        if (length > maxVelocity) {
            player.SetState(States.Running);
            return;
        } else if (length < minVelocity) {
            player.SetState(States.Standing);
            return;
        }
        float weight = distance;
        if (transition) {
            player.InterpolatePose(poses[0], weight);
            if (weight >= 1.0f) {
                transition = false;
                distance = 0;
            }
        } else {
            InterpolatePose(weight);
        }
        distance += length;
    }

    public override void OnStateSet() 
    {
        player.SaveCurrentPose();
        distance = 0;
        transition = true;
    }

    void InterpolatePose(float weight)
    {
		float val = Math.Abs((weight*2.1f) % 4 - 0.01f);
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 3 ? 0 : begin + 1;
		player.InterpolatePose(poses[begin], poses[end], realWeight);
    }
}