public interface IPlayer
{
	void SetState(States newState);
	void InterpolatePose(string newPose, float weight);
	void InterpolatePose(string beginPose, string endPose, float weight);
	void SaveCurrentPose();
}
