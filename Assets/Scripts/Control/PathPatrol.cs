using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PathPatrol : MonoBehaviour
    {
        public int i = 0;
        void OnDrawGizmos() {
            int childCount = transform.childCount;
            const float waypointGizmoRadius = 0.3f;

            for (i = 0; i < childCount; i++) {
                int j = GetNextIndex(i);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
                Gizmos.DrawSphere(GetWayPoint(i), waypointGizmoRadius);
            }

        }

        public int GetNextIndex(int i) {
            if (i+1 == transform.childCount) {
                return 0;
            }
            return i+1;
        }

        public Vector3 GetWayPoint(int i) {
            return transform.GetChild(i).position;
        }

    }
}
