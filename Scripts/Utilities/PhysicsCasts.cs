using System;
using Godot;

public readonly struct CastHit
{
	public readonly bool NonEmpty {get;}
	public readonly Vector3 HitPosition {get;}
	public readonly Vector3 Normal {get;}
	//public readonly int FaceIndex {get;}
	public readonly CollisionObject3D Collider {get;}
	public readonly uint ColliderId {get;}
	//public readonly int /*Shape3D*/ Shape {get;}
	public readonly Rid rid; // its a ulong, low-end ID
	public readonly string ColliderOwnerName {get;}

	public CastHit(Godot.Collections.Dictionary intersectResult)
	{
		NonEmpty = true;
		HitPosition = (Vector3) intersectResult["position"];
		Normal = (Vector3) intersectResult["normal"];
		//FaceIndex = (int) intersectResult["face_index"];
		Collider = (CollisionObject3D) intersectResult["collider"];
		ColliderId = (uint) intersectResult["collider_id"];
		//Shape = (int) intersectResult["shape"];
		rid = (Rid) intersectResult["rid"];
		ColliderOwnerName = intersectResult["collider"].ToString();
	}

	public CastHit()
	{
		NonEmpty = false;
		HitPosition = Vector3.Zero;
		Normal = Vector3.Zero;
		//FaceIndex = 0;
		Collider = null;
		ColliderId = 0;
		//Shape = 0;
		rid = new Rid();
		ColliderOwnerName = "";
	}
}

public partial class PhysicsCasts : Node3D
{

	// collision result keys:
	// "position", "normal", "face_index", "collider_id", "collider", "shape", "rid"

	/// <summary>
	/// This is to shorten the, too low-level Raycast method of godot.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <returns></returns>
	public static CastHit CastLine(Node3D sender, Vector3 from, Vector3 to, uint layerMask = 4294967295, bool collideWithAreas = false)
	{
		PhysicsDirectSpaceState3D spaceState = sender.GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, layerMask);
		
		query.CollideWithAreas = collideWithAreas;

		var result = spaceState.IntersectRay(query);

		if(result.Count > 0)
			return new CastHit(result);
		else
			return new CastHit();
	}

	public static uint GetCollisionMask(params uint[] layers)
	{
		uint layermask = 0;

		foreach (int layer in layers)
			layermask += 1u << (layer - 1);

		return layermask;
	}
}