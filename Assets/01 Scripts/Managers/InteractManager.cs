using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : ManagerBase {
    private List<IInteractable> interactableInstances = new List<IInteractable>();

    int currentInteractIndex = 0;

    public void Interact() {
        if (interactableInstances.Count == 0) {
            Debug.LogWarning("No interacbable instance!");
            return;
        }

        if (currentInteractIndex >= interactableInstances.Count) currentInteractIndex = 0;

        interactableInstances[currentInteractIndex].Interact();
    }

    public void AddInteractable(IInteractable interactable) {
        interactableInstances.Add(interactable);
    }

    public void RemoveInteractable(IInteractable interactable) {
        interactableInstances.Remove(interactable);
    }
}
