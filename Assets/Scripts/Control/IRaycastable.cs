using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public interface IRaycastable {
        CursorType GetCursor();
        bool HandleRaycast(PlayerController callingController);
    }
}
