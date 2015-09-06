using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Object {

	[System.Serializable]
	public class SerializedObject {

		public short Id;
		public List<object> Properties = new List<object>();
	}

	public abstract class BaseObject {

		protected Rotation rotation;

		protected CellDefinition cellReference;
		protected GameObject goReference;

		public abstract void Update(IntVector3 position);

		public abstract SerializedObject Serialize();
		public abstract void Deserialize(IntVector3 position, SerializedObject serializedObject);

		public bool Is<T>() {
			return this.GetType() == typeof(T);
		}

		public void Destroy() {
			this.Recycle();
		}

		public void Recycle() {
			if (this.goReference != null) {
				this.goReference.Recycle();
			}
		}
	}

}