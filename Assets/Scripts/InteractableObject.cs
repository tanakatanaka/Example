using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreatorKitCode
{
    public abstract class InteractableObject : HighlightableObject
    {
        public abstract bool IsInteractable { get; }

        public abstract void InteractWith(CharacterData target);
    }
}
