using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator
{
	public DragAndDropManipulator(VisualElement target, VisualElement root)
	{
		this.target = target;
		this.root = root;
	}

	protected override void RegisterCallbacksOnTarget()
	{
		target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
		target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
		target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
		target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler); 
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
		target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
		target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
		target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
	}

	private readonly VisualElement root;
	private VisualElement startSlot;
	private Vector3 pointerStartPosition;
	private Vector3 offset;
	private bool enabled;

	private void PointerDownHandler(PointerDownEvent evt)
	{
		target.CapturePointer(evt.pointerId);
		startSlot = target.parent;

		enabled = true;

		var worldPos = target.worldBound.position;
		root.Add(target);
		target.style.position = Position.Absolute;
		
		target.transform.position = root.WorldToLocal(worldPos);
		pointerStartPosition = target.transform.position;
		offset = target.transform.position - evt.position;
	}

	private void PointerMoveHandler(PointerMoveEvent evt)
	{
		if (enabled && target.HasPointerCapture(evt.pointerId))
		{
			var pos = evt.position + offset;
			var pointerDelta = pos - pointerStartPosition;
			pos = new Vector2(
				Mathf.Clamp(pointerDelta.x + pointerStartPosition.x, 0, root.panel.visualTree.worldBound.width),
				Mathf.Clamp(pointerDelta.y + pointerStartPosition.y, 0, root.panel.visualTree.worldBound.height));
			;
			target.transform.position = pos;
		}
	}

	private void PointerUpHandler(PointerUpEvent evt)
	{
		if (enabled && target.HasPointerCapture(evt.pointerId))
		{
			target.ReleasePointer(evt.pointerId);
		}
	}

	private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
	{
		if (enabled)
		{
			var slotsContainer = root.Q<VisualElement>("slots");
			var allSlots =
				slotsContainer.Query<VisualElement>(className: "slot");
			var overlappingSlots =
				allSlots.Where(OverlapsTarget);
			var closestOverlappingSlot =
				FindClosestSlot(overlappingSlots);

			if (closestOverlappingSlot == null) closestOverlappingSlot = startSlot;

			enabled = false;
			closestOverlappingSlot?.Add(target);
			target.style.position = Position.Relative;
			target.transform.position = Vector3.zero;
			target.BringToFront();
		}
	}

	private bool OverlapsTarget(VisualElement slot)
	{
		return target.worldBound.Overlaps(slot.worldBound);
	}

	private VisualElement FindClosestSlot(UQueryBuilder<VisualElement> slots)
	{
		var slotsList = slots.ToList();
		var bestDistanceSq = float.MaxValue;
		VisualElement closest = null;
		foreach (var slot in slotsList)
		{
			var displacement =
				RootSpaceOfSlot(slot) - target.transform.position;
			var distanceSq = displacement.sqrMagnitude;
			if (distanceSq < bestDistanceSq)
			{
				bestDistanceSq = distanceSq;
				closest = slot;
			}
		}
		return closest;
	}

	private Vector3 RootSpaceOfSlot(VisualElement slot)
	{
		var slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
		return root.WorldToLocal(slotWorldSpace);
	}
}