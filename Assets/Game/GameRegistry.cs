using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;

namespace SpaceStation.Game {

	public class GameRegistry {

		private static GameRegistry instance;
		public static GameRegistry Instance { 
			get {
				if (instance == null) {
					instance = new GameRegistry();
				}

				return instance;
			}
		}

		public const short EmptyId = -1;

		private Dictionary<short, Type> objectsById;
		private Dictionary<Type, short> objectsByType;

		public GameRegistry() {
			this.objectsById = new Dictionary<short, Type>();
			this.objectsByType = new Dictionary<Type, short>();
		}

		public void RegisterObject<T>(short id) {
			if (this.objectsById.ContainsKey(id)) {
				Logger.Warn("RegisterObject", "Registering Object {0} with id {1} failed, id is already assigned to {2}.", typeof(T).Name, id, this.objectsById[id].Name);
				return;
			}

			this.objectsById.Add(id, typeof(T));
			this.objectsByType.Add(typeof(T), id);
		}

		public Type GetObjectType(short id) {
			return this.objectsById.ContainsKey(id) ? this.objectsById[id] : null;
		}

		public short GetObjectId<T>() {
			return this.objectsByType.ContainsKey(typeof(T)) ? this.objectsByType[typeof(T)] : (short) -1;
		}
	}

}