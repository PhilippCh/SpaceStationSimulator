using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using System.Collections.Generic;

namespace SpaceStation.Station.Structure.Room {

	public enum CellType {
		EMPTY,
		FLOOR,
		WALL
	}

	public class CellViz {
		public GUIStyle Style;
		public string Icon;
	}

	public class RoomDefinition {
		public Rect bounds;
		public CellType[,] cells;
	}

	public class RoomBuilderUI : MonoBehaviour {

		Texture2D wallTexture, floorTexture, emptyTexture, deleteTexture;
		GUIStyle floorStyle, wallStyle, emptyStyle;
		int tileSize = 24;

		Vector2 originalPos, currentPos;
		Vector2 prevPos = new Vector2(-1, -1);
		bool drawing = false;
		bool wasDown = false;
		bool connectRooms = false;
		bool finished = false;

		IntVector2 size;

		CellType[,] cells;
		Dictionary<CellType, CellViz> cellVisualizations = new Dictionary<CellType, CellViz>();

		List<RoomDefinition> rooms = new List<RoomDefinition>();
		RoomDefinition overlayRoom;

		private void Awake() {
			InitializeTexture(ref wallTexture, Color.yellow);
			InitializeTexture(ref floorTexture, Color.green);
			InitializeTexture(ref emptyTexture, Color.blue);
			InitializeTexture(ref deleteTexture, new Color(1, 0, 0, .125f));
		}

		private void InitializeTexture(ref Texture2D texture, Color color) {
			texture = new Texture2D(1, 1);

			texture.SetPixel(0, 0, color);
			texture.Apply();
		}

		private void OnGUI() {
			connectRooms = GUI.Toggle(new Rect(10, 10, 350, 20), connectRooms, " Connect rooms when building");

			if (floorStyle == null) {
				floorStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
				wallStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
				emptyStyle = new GUIStyle(GUI.skin.GetStyle("Label"));

				floorStyle.alignment = wallStyle.alignment = emptyStyle.alignment = TextAnchor.MiddleCenter;
				floorStyle.normal.textColor = wallStyle.normal.textColor = Color.black;

				floorStyle.normal.background = floorTexture;
				wallStyle.normal.background = wallTexture;
				emptyStyle.normal.background = emptyTexture;

				cellVisualizations.Add(CellType.EMPTY, new CellViz() {
					Style = emptyStyle,
					Icon = "E"
				});
				
				cellVisualizations.Add(CellType.FLOOR, new CellViz() {
					Style = floorStyle,
					Icon = "F"
				});
				
				cellVisualizations.Add(CellType.WALL, new CellViz() {
					Style = wallStyle,
					Icon = "W"
				});
			}

			// Draw existing temporary rooms
			foreach (RoomDefinition room in rooms) {
				DrawRoom(new IntVector2((int) room.bounds.x, (int)room.bounds.y), room.cells);
			}

			// Draw current temporary room
			if (drawing) {
				Vector2 usePos = originalPos;

				GUI.Label(new Rect(currentPos.x * tileSize, currentPos.y * tileSize, 150, 20), currentPos.ToString());
				GUI.Label(new Rect(currentPos.x * tileSize, currentPos.y * tileSize + 20, 150, 20), originalPos.ToString());

				if (currentPos.x < originalPos.x) {
					usePos.x = currentPos.x;
				}

				if (currentPos.y < originalPos.y) {
					usePos.y = currentPos.y;
				}

				DrawRoom(new IntVector2(usePos.x, usePos.y), cells);
			}

			if (overlayRoom != null) {
				var adjustedBounds = new Rect(overlayRoom.bounds.position * tileSize, overlayRoom.bounds.size * tileSize);
				var buttonPos = new Rect(adjustedBounds.position + adjustedBounds.size / 2, new Vector2(40, 30));

				buttonPos.x -= 20;
				buttonPos.y -= 15;

				GUI.Box(adjustedBounds, deleteTexture);

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
					var rect = new Rect((origin.x + x) * tileSize, (origin.z + z) * tileSize, tileSize, tileSize);
					var style = cellVisualizations[cells[x, z]].Style;

					GUI.Label(rect, cellVisualizations[cells[x, z]].Icon, style);
				}
			}
		}

		private void Update() {
			if (Input.GetMouseButtonDown(0)) {
				originalPos = Input.mousePosition.ToVector2();
				originalPos.x = Mathf.Floor(originalPos.x / tileSize);
				originalPos.y = Mathf.Floor((Screen.height - originalPos.y) / tileSize);

				wasDown = true;

				if (finished) {
					finished = false;
					this.cells = null;
				}
			}

			drawing = Input.GetMouseButton(0);

			if (Input.GetMouseButton(0)) {

				currentPos = Input.mousePosition.ToVector2();
				currentPos.x = Mathf.Floor(currentPos.x / tileSize);
				currentPos.y = Mathf.Floor((Screen.height - currentPos.y) / tileSize);

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
						var adjustedBounds = new Rect(room.bounds.position * tileSize, room.bounds.size * tileSize);
						
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

			if (connectRooms) {
				finalCells = ConnectRooms(finalCells);
			}

			this.rooms.Clear();

			this.cells = finalCells;
			this.finished = true;
		}

		private CellType[,] ConnectRooms(CellType[,] cells) {

			Logger.Info("ConnectRooms", "Removing inner walls");

			for (int x = 0; x < cells.GetLength(0); x++) {
				for (int z = 0; z < cells.GetLength(1); z++) {
					bool isOuterWall = false;

					if (cells[x, z] != CellType.WALL) {
						continue;
					}

					if (x == 0 || x == cells.GetLength(0) - 1 || z == 0 || z == cells.GetLength(1) - 1) {
						continue;
					} else {
						var neighbors = new CellType[8];

						// Top row
						neighbors[0] = cells[x - 1, z - 1];
						neighbors[1] = cells[x, z - 1];
						neighbors[2] = cells[x + 1, z - 1];

						// Center row
						neighbors[3] = cells[x - 1, z];
						neighbors[4] = cells[x + 1, z];

						// Bottom row
						neighbors[5] = cells[x - 1, z + 1];
						neighbors[6] = cells[x, z + 1];
						neighbors[7] = cells[x + 1, z + 1];

						for (int i = 0; i < neighbors.Length; i++) {
							if (neighbors[i] == CellType.EMPTY) {
								isOuterWall = true;
								break;
							}
						}

						if (!isOuterWall) {
							cells[x, z] = CellType.FLOOR;
						}
					}
				}
			}

			return cells;
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