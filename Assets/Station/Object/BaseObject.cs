using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Station.Object {

	public abstract class BaseObject {

		protected CellDefinition cellReference;
		protected GameObject goReference;

		public abstract void Update(IntVector3 position);
	}

}