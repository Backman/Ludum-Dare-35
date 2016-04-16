using UnityEngine;
using System.Collections;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction Attack;

	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;

	public PlayerAction Candy;
	public PlayerAction Lightning;
	public PlayerAction Crystal;
	public PlayerAction Avocado;

	public PlayerTwoAxisAction Move;

	public PlayerActions()
	{
		Attack = CreatePlayerAction("Attack");
		Left = CreatePlayerAction("Move Left");
		Right = CreatePlayerAction("Move Right");
		Up = CreatePlayerAction("Move Up");
		Down = CreatePlayerAction("Move Down");

		Candy = CreatePlayerAction("Candy");
		Lightning= CreatePlayerAction("Lightning");
		Crystal = CreatePlayerAction("Piss");
		Avocado = CreatePlayerAction("Poop");

		Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
	}

	public static PlayerActions CreateWithDefaultBindings()
	{
		var actions = new PlayerActions();

		actions.Attack.AddDefaultBinding(Key.Shift, Key.E);
		actions.Attack.AddDefaultBinding(InputControlType.Action1);
		actions.Attack.AddDefaultBinding(Mouse.LeftButton);

		actions.Candy.AddDefaultBinding(Key.I);
		actions.Lightning.AddDefaultBinding(Key.O);
		actions.Crystal.AddDefaultBinding(Key.K);
		actions.Avocado.AddDefaultBinding(Key.L);

		actions.Candy.AddDefaultBinding(InputControlType.Action1);
		actions.Lightning.AddDefaultBinding(InputControlType.Action2);
		actions.Crystal.AddDefaultBinding(InputControlType.Action3);
		actions.Avocado.AddDefaultBinding(InputControlType.Action4);

		actions.Up.AddDefaultBinding(Key.UpArrow);
		actions.Down.AddDefaultBinding(Key.DownArrow);
		actions.Left.AddDefaultBinding(Key.LeftArrow);
		actions.Right.AddDefaultBinding(Key.RightArrow);

		actions.Up.AddDefaultBinding(Key.W);
		actions.Down.AddDefaultBinding(Key.S);
		actions.Left.AddDefaultBinding(Key.A);
		actions.Right.AddDefaultBinding(Key.D);

		actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);

		actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		actions.Right.AddDefaultBinding(InputControlType.DPadRight);
		actions.Up.AddDefaultBinding(InputControlType.DPadUp);
		actions.Down.AddDefaultBinding(InputControlType.DPadDown);

		actions.ListenOptions.IncludeUnknownControllers = true;
		actions.ListenOptions.MaxAllowedBindings = 4;
		// actions.ListenOptions.MaxAllowedBindingsPerType = 1;
		// actions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
		// actions.ListenOptions.IncludeMouseButtons = true;

		actions.ListenOptions.OnBindingFound = (action, binding) =>
		{
			if (binding == new KeyBindingSource(Key.Escape))
			{
				action.StopListeningForBinding();
				return false;
			}
			return true;
		};

		actions.ListenOptions.OnBindingAdded += (action, binding) =>
		{
			Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
		};

		actions.ListenOptions.OnBindingRejected += (action, binding, reason) =>
		{
			Debug.Log("Binding rejected... " + reason);
		};

		return actions;
	}
}
