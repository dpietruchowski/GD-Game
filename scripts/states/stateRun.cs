using State;

public class StateRun
{    
    float distance = 0;
    bool transition = true;
    const float minVelocity = 3.0f;
    String[] poses = {"Run1", "Run2", "Run3"}; 
    public override void Update(Vector2 velocity)
    {
        var length = velocity.Length();
        if (length < minVelocity) {
            player.SetState(States.Walking);
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
		float val = Math.Abs((weight*1.0f) % 3 - 0.01f);
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 2 ? 0 : begin + 1;
		player.InterpolatePose(poses[begin], poses[end], realWeight);
    }
}