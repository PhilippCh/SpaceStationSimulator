using UnityEngine;
using System.Collections;

using SpaceStation;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SpaceStation.Station.Structure;
using SpaceStation.Util;

namespace SpaceStation.Game {

	public class GameSave {

		public void Load() {
			var regionManager = RegionManager.Instance;
			
			Logger.Info("Loading game...");
			
			regionManager.LoadRegion(Application.persistentDataPath + "/region.dat");
		}

		public void Save() {
			var regionManager = RegionManager.Instance;

			Logger.Info("Saving game...");

			regionManager.SaveRegion(Application.persistentDataPath + "/region.dat");

			Logger.Info("Successfully saved game to {0}", Application.persistentDataPath);
		}
	}

}