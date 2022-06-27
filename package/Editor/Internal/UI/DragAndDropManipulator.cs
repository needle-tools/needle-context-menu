using UnityEngine;
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

	private Vector2 targetStartPosition { get; set; }
	private Vector3 pointerStartPosition { get; set; }
	private bool enabled { get; set; }
	private VisualElement root { get; }

	private void PointerDownHandler(PointerDownEvent evt)
	{
		targetStartPosition = target.transform.position;
		pointerStartPosition = evt.position;
		target.CapturePointer(evt.pointerId);
		target.style.position = Position.Absolute;
		root.Add(target);
		enabled = true;
	}

	private void PointerMoveHandler(PointerMoveEvent evt)
	{
		if (enabled && target.HasPointerCapture(evt.pointerId))
		{
			var pointerDelta = evt.position - pointerStartPosition;

			target.transform.position = new Vector2(
				Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
				Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
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
			// var closestPos = Vector3.zero;
			// if (closestOverlappingSlot != null)
			// {
			// 	closestPos = RootSpaceOfSlot(closestOverlappingSlot);
			// 	closestPos = new Vector2(closestPos.x, closestPos.y);
			// }
			// target.transform.position =
			// 	closestOverlappingSlot != null ? closestPos : targetStartPosition;

			enabled = false;
			closestOverlappingSlot?.Add(target);
			target.style.position = Position.Relative;
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