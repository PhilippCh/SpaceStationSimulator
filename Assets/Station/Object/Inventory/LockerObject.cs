using UnityEngine;
using System.Collections;

using SpaceStation;

namespace SpaceStation.Station.Object {

	public class LockerObject : BaseObject {

		#region implemented abstract members of BaseObject

		public override void Update(SpaceStation.Util.IntVector3 position) {
			throw new System.NotImplementedException();
		}

		public override SerializedObject Serialize() {
			throw new System.NotImplementedException();
		}

		public override void Deserialize(SpaceStation.Util.IntVector3 position, SerializedObject serializedObject) {
			throw new System.NotImplementedException();
		}

		#endregion
	}

}