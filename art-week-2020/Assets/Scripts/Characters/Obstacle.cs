using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : WorldObject
{
	#region UnityEvents
	#endregion

	#region Methods
	public override bool IsObstacle() { return true; }
	#endregion
}
