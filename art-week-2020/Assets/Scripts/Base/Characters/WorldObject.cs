using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Characters;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    protected Water water;
    protected float inertia;

    public virtual void Awake()
    {
        water = GameObject.Find("Water3D").GetComponent<Water>();
        inertia = 50;
    }

    private void Update()
    {
        if (!IsPlayer())
        {
            transform.rotation = GetWavesRotation();
            if (!IsIA())
            {
                var player = GameObject.Find("Player").GetComponentInChildren<Player>();
                var direction = player.GetDirection();
                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction * -1, player.Speed * Time.deltaTime);
            }
        }
    }

    public Quaternion GetWavesRotation()
    {
        Vector3 normal;
        Vector3 pos;
        water.GetSurfacePosAndNormalForWPos(transform.position, out pos, out normal);
        return Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, normal) * transform.rotation, 1 / inertia);
    }

    public virtual bool IsPlayer() { return false; }
	public virtual bool IsIA() { return false; }
	public virtual bool IsObstacle() { return false; }
}
