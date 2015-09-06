using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using SpaceStation;
using SpaceStation.Util;
using SpaceStation.Station.Structure.Cell;
using System.Diagnostics;
using System.Reflection;
using SpaceStation.Station.Object;

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

		public const short InvalidObjectId = -100;
		public const short EmptyObjectId = -1;

		public WallObjectHelper WallObjectHelper;

		private Dictionary<short, Type> objectsById;
		private Dictionary<Type, short> objectsByType;

		private Dictionary<Type, CellMask> cellMasks;

		public GameRegistry() {
			this.objectsById = new Dictionary<short, Type>();
			this.objectsByType = new Dictionary<Type, short>();

			RegisterObjectsByAttribute();
		}

		public void PostAwake() {
			this.WallObjectHelper = new WallObjectHelper();
		}

		#region Objects

		public void RegisterObjectsByAttribute() {
			var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			var possible = (from System.Type type 
			                in types 
			                where type.IsSubclassOf(typeof(BaseObject)) 
			                select type).ToArray();

			foreach (var objectClass in possible) {

				foreach (Attribute attr in objectClass.GetCustomAttributes(true)) {
					var targetAttribute = attr as RegisterAsObject;
					
					if (targetAttribute != null) {
						RegisterObject(targetAttribute.ObjectType, targetAttribute.Id);
					}
				}
			}
		}

		public void RegisterObject<T>(short id) {
			this.RegisterObject(typeof(T), id);
		}

		public void RegisterObject(Type type, short id) {
			if (this.objectsById.ContainsKey(id)) {
				Logger.Warn("Registering Object {0} with id {1} failed, id is already assigned to {2}.", type.Name, id, this.objectsById[id].Name);
				return;
			}

			this.objectsById.Add(id, type);
			this.objectsByType.Add(type, id);

			Logger.Info("Registered object {0} with id {1}.", type.Name, id);
		}

		public Type GetObjectType(short id) {
			return this.objectsById.ContainsKey(id) ? this.objectsById[id] : null;
		}

		public short GetObjectId<T>() {
			if (!this.objectsByType.ContainsKey(typeof(T))) {
				Logger.Warn("Could not find object id for {0}.", typeof(T).Name);
				return InvalidObjectId;
			}

			return this.objectsByType[typeof(T)];
		}

		#endregion
	}

}