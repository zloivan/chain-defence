using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChainDefense.PathFinding
{
    public class PathManager : MonoBehaviour
    {
        public static PathManager Instance { get; set; }

        [SerializeField] private List<Transform> _waypoints;
        [SerializeField] private Transform _spawnPosition;

        private void Awake() =>
            Instance = this;

        public Vector3 GetWaypointPosition(int index) =>
            _waypoints[index].position;

        public int GetWaypointCount() =>
            _waypoints.Count;

        public Vector3 GetSpawnPosition() =>
            _spawnPosition.position;

        private void OnDrawGizmos()
        {
            if (_waypoints == null || _waypoints.Count < 2)
                return;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < _waypoints.Count - 1; i++)
            {
                var a = _waypoints[i];
                var b = _waypoints[i + 1];
                if (a == null || b == null)
                    continue;

                Gizmos.DrawLine(a.position, b.position);
            }
        }
    }
}