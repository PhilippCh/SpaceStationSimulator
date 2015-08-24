using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Object {

	public abstract class BaseObject {

		protected CellStorage cellReference;
		protected GameObject goReference;

		protected bool isVisible;

		public abstract void OnCreate(CellStorage data);
		public abstract void OnUpdateMetadata(CellStorage data);

		public virtual void Show() {
			this.isVisible = true;
		}

		public virtual void Hide() {
			this.isVisible = false;
		}

		protected virtual void RecycleGameObject() {
			if (this.goReference != null) {
				this.goReference.Recycle();
			}
		}
	}

}