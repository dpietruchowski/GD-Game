using State;

enum States
{
    Standing,
    Walking,
    Running
}
public interface IPlayer
{
    void SetState(States newState);
    void InterpolatePose(String newPose, float weight);
    void InterpolatePose(String beginPose, String endPose, float weight);
    void SaveCurrentPose();
}