using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Mover mover;
        Fighter fighter;
        Health health;

        [SerializeField] float maxPathLength = 40f;

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        // Start is called before the first frame update
        void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI()) return;
            if (health.IsDead()) {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
            
        }

        bool InteractWithUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        // this is responsible for pickups and attacking enemies
        bool InteractWithComponent() {

            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables) {
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursor());
                        return true;
                    }
                } 
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted() {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++) {
                distances[i] = hits[i].distance ;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        bool InteractWithMovement()
        {
            
            Vector3 moveToLocation;
            bool hasHit = RaycastNavmesh(out moveToLocation);
            if (hasHit) // if it hits something
            {
                if (Input.GetMouseButton(1))
                {
                    mover.StartMoveAction(moveToLocation, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        bool RaycastNavmesh(out Vector3 moveToLocation) {

            moveToLocation = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);             
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasFound = NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas);
            if (!hasFound) return false;            
            moveToLocation = navMeshHit.position;

            NavMeshPath path = new NavMeshPath(); // *
            // the line below will store info into the path* variable
            bool hasPath = NavMesh.CalculatePath(transform.position, moveToLocation, NavMesh.AllAreas, path);
            if (!hasPath) return false; //can't calculate path
            if (path.status != NavMeshPathStatus.PathComplete) return false; // can't complete the path
            if (GetPathLength(path) > maxPathLength) return false; // limit how far the NavMeshAgent can cover;
            return true;
        }

        float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length <= 2) return 0;
            for (int i = 0; i < path.corners.Length-1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }
            return total;
        }

        void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(CursorType type) {

            foreach (CursorMapping mapping in cursorMappings) {
                if (mapping.type == type) {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
