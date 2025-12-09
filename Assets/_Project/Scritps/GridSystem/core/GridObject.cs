using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChainDefense.GridSystem.core
{
    public class GridObject<T> where T : MonoBehaviour
    {
        private readonly GridPosition _gridPosition;
        private readonly GridSystemBase<GridObject<T>> _gridSystem;
        private readonly List<T> _unitList;

        public GridObject(GridSystemBase<GridObject<T>> gridSystem, GridPosition gridPosition)
        {
            _gridPosition = gridPosition;
            _gridSystem = gridSystem;
            _unitList = new List<T>();
        }

        public override string ToString() =>
            $"{_gridPosition}\n" + string.Join("\n", _unitList.Select(e=>e.ToString()));

        public void Add(T unit) => _unitList.Add(unit);

        public List<T> GetList() => _unitList;

        public void Remove(T unit)
        {
            if (!_unitList.Contains(unit)) 
                return;
            
            _unitList.Remove(unit);
        }
        
        public void ClearObjectList() => _unitList.Clear();
        public bool HasAny() => _unitList.Count > 0;

        public T Get() => HasAny() ? _unitList[0] : null;

        public void Set(T unit)
        {
            _unitList.Clear();
            _unitList.Add(unit);
        }
    }
}