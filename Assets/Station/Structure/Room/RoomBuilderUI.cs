using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpaceStation.Station.Structure.Room {

	/* BEGIN OLD DEFINITIONS */

	public enum CellType {
		EMPTY,
		FLOOR,
		WALL
	}

	public class RoomDefinition {
		public Rect bounds;
		public CellType[,] cells;
	}

	/* END OLD DEFINITIONS */

	public class RoomBuilderUI : MonoBehaviour {

		public int TileSize = 24;
		public int BuildArea = 64;

		private Dictionary<CellType, GUIStyle> styles;

		public RoomBuilderUI() {
			styles = new Dictionary<CellType, GUIStyle>();
		}

		private Texture2D CreateColorTexture(Color color) {
			var texture = new Texture2D(1, 1);
			
			texture.SetPixel(0, 0, color);
			texture.Apply();

			return texture;
		}

		private void SetupStyles() {
			CreateSimpleStyle(CellType.WALL, Color.black, Color.yellow);
			CreateSimpleStyle(CellType.FLOOR, Color.black, Color.green);
			CreateSimpleStyle(CellType.EMPTY, Color.white, Color.blue);
		}

		private void CreateSimpleStyle(CellType cellType, Color textColor, Color backgroundColor) {
			var style = new GUIStyle(GUI.skin.GetStyle("Label"));

			style.alignment = TextAnchor.MiddleCenter;
			style.normal.background = CreateColorTexture(backgroundColor);
			style.normal.textColor = textColor;

			styles.Add(cellType, style);
		}

		/* OLD PART BEGINS HERE */

		Vector2 originalPos, currentPos;
		Vector2 prevPos = new Vector2(-1, -1);
		bool drawing = false;
		bool wasDown = false;
		bool finished = false;

		IntVector2 size;

		CellType[,] cells;

		List<RoomDefinition> rooms = new List<RoomDefinition>();
		RoomDefinition overlayRoom;

		private void OnGUI() {

			if (styles.Count == 0) {
				SetupStyles();
			}

			// Draw existing temporary rooms
			foreach (RoomDefinition room in rooms) {
				DrawRoom(new IntVector2((int) room.bounds.x, (int)room.bounds.y), room.cells);
			}

			// Draw current temporary room
			if (drawing) {
				Vector2 usePos = originalPos;

				GUI.Label(new Rect(currentPos.x * TileSize, currentPos.y * TileSize, 150, 20), currentPos.ToString());
				GUI.Label(new Rect(currentPos.x * TileSize, currentPos.y * TileSize + 20, 150, 20), originalPos.ToString());

				if (currentPos.x < originalPos.x) {
					usePos.x = currentPos.x;
				}

				if (currentPos.y < originalPos.y) {
					usePos.y = currentPos.y;
				}

				DrawRoom(new IntVector2(usePos.x, usePos.y), cells);
			}

			if (overlayRoom != null) {
				var adjustedBounds = new Rect(overlayRoom.bounds.position * TileSize, overlayRoom.bounds.size * TileSize);
				var buttonPos = new Rect(adjustedBounds.position + adjustedBounds.size / 2, new Vector2(40, 30));

				buttonPos.x -= 20;
				buttonPos.y -= 15;

				if (GUI.Button(buttonPos, "X")) {
					rooms.Remove(overlayRoom);
				}
			}

			if (finished) {
				DrawRoom(IntVector2.zero, cells);
			}
		}

		private void DrawRoom(IntVector2 origin, CellType[,] cells) {
			if (cells == null) {
				return;
			}

			for (int x = 0; x < cells.GetLength(0); x++) {
				for (int z = 0; z < cells.GetLength(1); z++) {
					var rect = new Rect((origin.x + x) * TileSize, (origin.z + z) * TileSize, TileSize, TileSize);
					var formattedName = cells[x, z].ToString().Substring(0, 1);

					GUI.Label(rect, formattedName, styles[cells[x, z]]);
				}
			}
		}

		private void Update() {
			if (Input.GetMouseButtonDown(0)) {
				originalPos = Input.mousePosition.ToVector2();
				originalPos.x = Mathf.Floor(originalPos.x / TileSize);
				originalPos.y = Mathf.Floor((Screen.height - originalPos.y) / TileSize);

				wasDown = true;

				if (finished) {
					finished = false;
					this.cells = null;
				}
			}

			drawing = Input.GetMouseButton(0);

			if (Input.GetMouseButton(0)) {

				currentPos = Input.mousePosition.ToVector2();
				currentPos.x = Mathf.Floor(currentPos.x / TileSize);
				currentPos.y = Mathf.Floor((Screen.height - currentPos.y) / TileSize);

				if (currentPos != prevPos) {
					size = new IntVector2((int)Mathf.Abs(currentPos.x - originalPos.x), (int)Mathf.Abs(currentPos.y - originalPos.y));
					
					cells = new CellType[size.x, size.z];

					for (int x = 0; x < size.x; x++) {
						for (int z = 0; z < size.z; z++) {
							if (x == 0 || x == size.x - 1 || z == 0 || z == size.z - 1) {
								cells[x, z] = CellType.WALL;
							} else {
								cells[x, z] = CellType.FLOOR;
							}
						}
					}
				}

				prevPos = currentPos;
			} else if (!Input.GetMouseButton(0) && wasDown) {
				if (Vector2.Distance(currentPos, originalPos) < 1) {
					overlayRoom = null;
					
					foreach (RoomDefinition room in rooms) {
						var mousePos = Input.mousePosition;
						var adjustedBounds = new Rect(room.bounds.position * TileSize, room.bounds.size * TileSize);
						
						mousePos.y = Screen.height - mousePos.y;
						
						if (adjustedBounds.Contains(mousePos)) {
							overlayRoom = room;
						}
					}

					if (overlayRoom != null) {
						Logger.Info("Update", "Selected room {0}", overlayRoom.bounds.ToString());
					}
				} else if (cells != null) {
					var xOrigin = currentPos.x < originalPos.x ? currentPos.x : originalPos.x;
					var zOrigin = currentPos.y < originalPos.y ? currentPos.y : originalPos.y;
					
					if (size.x != 0 && size.z != 0) {
						// Assemble the room and add to list
						AddTemporaryRoom((int) xOrigin, (int) zOrigin, size.x, size.z, cells);
						
						cells = null;
					}
					
				}

				wasDown = false;
			}
		}

		public void CreateRooms() {
			var bounds = rooms[0].bounds;

			// First, find the bounding box encasing all rooms
			foreach (RoomDefinition room in rooms) {
				bounds.xMin = room.bounds.xMin < bounds.xMin ? room.bounds.xMin : bounds.xMin;
				bounds.xMax = room.bounds.xMax > bounds.xMax ? room.bounds.xMax : bounds.xMax;

				bounds.yMin = room.bounds.yMin < bounds.yMin ? room.bounds.yMin : bounds.yMin;
				bounds.yMax = room.bounds.yMax > bounds.yMax ? room.bounds.yMax : bounds.yMax;
			}

			Logger.Info("CreateRooms", "Global bounds found at {0}", bounds.ToString());

			CellType[,] finalCells = new CellType[(int)bounds.width, (int)bounds.height];

			foreach (RoomDefinition room in rooms) {

				for (int x = 0; x < room.cells.GetLength(0); x++) {
					for (int z = 0; z < room.cells.GetLength(1); z++) {
						finalCells[(int)(room.bounds.xMin - bounds.xMin + x), (int)(room.bounds.yMin - bounds.yMin + z)] = room.cells[x, z];
					}
				}
			}

			this.rooms.Clear();

			this.cells = finalCells;
			this.finished = true;
		}

		public void Reset() {
			this.rooms.Clear();
			this.cells = null;

			Logger.Info("Reset", "Reset room drawing");
		}

		private void AddTemporaryRoom(int x, int y, int width, int length, CellType[,] cells) {
			rooms.Add(new RoomDefinition() {
				bounds = new Rect(x, y, width, length),
				cells = cells
			});

			Logger.Info("AddRoom", "Temporary room added at {0} {1}", x.ToString(), y.ToString());
		}
	}

}