using Godot;
using System;

public class Bone : Spatial
{
	public override void _Ready()
	{
		
	}
	
	public void MoveToBonePosition(Skeleton skel, string boneName)
	{
		this.Transform = GetBoneGlobalTransform(skel, boneName);
	}
	
	public Transform GetBoneGlobalTransform(Skeleton skel, string boneName)
	{
		var boneIdx = skel.FindBone(boneName);
		var parent = skel.GetBoneParent(boneIdx);
		var parentBoneTransform = skel.GetBoneGlobalPose(parent);
		var bonePose = skel.GetBoneRest(boneIdx);
		var boneTransform = skel.GetBoneGlobalPose(boneIdx);
		var transform = new Transform();
		transform.basis = parentBoneTransform.basis;
		transform.origin = parentBoneTransform.origin + parentBoneTransform.basis.Xform(bonePose.origin);
		return transform;
	}
	public Transform GetBoneRestTransform(Skeleton skel, string boneName)
	{
		var boneIdx = skel.FindBone(boneName);
		var parent = skel.GetBoneParent(boneIdx);
		var parentBoneTransform = skel.GetBoneGlobalPose(parent);
		var bonePose = new Transform();
		bonePose.basis = this.Transform.basis;
		bonePose.origin = parentBoneTransform.basis.XformInv(this.Transform.origin - parentBoneTransform.origin);
		return bonePose;
	}
}
