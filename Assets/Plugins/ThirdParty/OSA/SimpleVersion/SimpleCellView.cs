using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using UnityEngine;

namespace OSA {
    public interface ISimpleCellView {
        void UpdateCell(CellViewsHolder holder, object data);
    }

    public class SimpleCellView : MonoBehaviour, ISimpleCellView {
        public virtual void UpdateCell(CellViewsHolder holder, object data) {
        }
    }
}