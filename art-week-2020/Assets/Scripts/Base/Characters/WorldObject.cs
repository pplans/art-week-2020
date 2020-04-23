using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
	public virtual bool IsPlayer() { return false; }
	public virtual bool IsIA() { return false; }
	public virtual bool IsObstacle() { return false; }
}
