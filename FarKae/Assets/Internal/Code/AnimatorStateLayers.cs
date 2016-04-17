using UnityEngine;
using System.Collections;

public class AnimatorStateLayers
{
	public int baseLayer;
	public int attackLayer;
	public int hitLayer;
	public int blockLayer;
	public int deathLayer;

	Animator _animator;

	public AnimatorStateLayers(Animator animator)
	{
		_animator = animator;
		baseLayer = animator.GetLayerIndex("Base Layer");
		attackLayer = animator.GetLayerIndex("Attack");
		hitLayer = animator.GetLayerIndex("Hit");
		blockLayer = animator.GetLayerIndex("Block");
		deathLayer = animator.GetLayerIndex("Death");
	}

	public bool CheckState(string name, int layer)
	{
		return _animator.GetCurrentAnimatorStateInfo(layer).IsName(name);
	}
}
