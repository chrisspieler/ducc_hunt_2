using Sandbox;

public abstract class Equipment : Component
{
	public virtual void OnUnequip()
	{
		GameObject.Destroy();
	}
}
