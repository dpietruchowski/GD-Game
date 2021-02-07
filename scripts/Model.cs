using Godot;
using Godot.Collections;
using System;

public class Model : Skeleton
{
	Dictionary lastPose;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public Dictionary GetPose(String name)
	{
		return (Dictionary)GetNode<Spatial>("PoseLib").Call("get_pose", name);
	}
	
	public void SaveCurrentPose()
	{
		lastPose = GetCurrentPose();
	}
	
	public Dictionary GetCurrentPose()
	{
		var pose = new Dictionary();
		for(int i = 0; i < this.GetBoneCount(); ++i) {
			Transform t = this.GetBoneRest(i);
			pose.Add(i, t);
		}
		return pose;
	}
	
	public void ApplyPose(String name)
	{
		var pose = GetPose(name);
		ApplyPose(pose);
	}
	
	public void ApplyPose(Dictionary pose)
	{
		for(int i = 0; i < this.GetBoneCount(); ++i) {
			if (pose.Contains(i)) {
				Transform t = (Transform)pose[i];
				this.SetBoneRest(i, t);
			}
		}
	}
	
	public void SetInterpolatedPose(String endName, float weight)
	{
		if (weight < 0.05 || lastPose == null) {
			SaveCurrentPose();
		}
		var endPose = GetPose(endName);
		SetInterpolatedPose(lastPose, endPose, weight);
	}
	
	public void SetInterpolatedPose(String beginName, String endName, float weight)
	{
		var beginPose = GetPose(beginName);
		var endPose = GetPose(endName);
		SetInterpolatedPose(beginPose, endPose, weight);
	}
	
	public void SetInterpolatedPose(Dictionary beginPose, Dictionary endPose, float weight)
	{
		for(int i = 0; i < this.GetBoneCount(); ++i) {
			if (beginPose.Contains(i) && endPose.Contains(i)) {
				Transform beginTransform = (Transform)beginPose[i];
				Transform endTransform = (Transform)endPose[i];
				Transform t = beginTransform.InterpolateWith(endTransform, weight);
				this.SetBoneRest(i, t);
			}
		}
	}
}
