public interface IPlayer
{
	void SetState(States newState);
	void InterpolatePose(string newPose, float weight);
	void InterpolatePose(string beginPose, string endPose, float weight);
	void TransitionPose(string poseName, float weight, bool transition = false);
	void SaveCurrentPose();
}
