using UnityEngine;
using System.Collections;

using SpaceStation;
using SpaceStation.Util;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpaceStation.Station.Structure.Room {

	/* TODO: Unify this with Station.Structure.Cell.CellType */
	public enum CellType {
		EMPTY,
		FLOOR,
		WALL
	}

	public class RoomBuilderUI : MonoBehaviour {

		private struct MouseData {
			public IntVector2 originPosition;
			public IntVector2 currentPosition;
			public IntVector2 lastPosition;

			public bool HasMoved() {
				return currentPosition.x != lastPosition.x || currentPosition.z != lastPosition.z;
			}

			public bool isPressed;
		}

		private enum SelectionMode {
			ROOM,
			SINGLE
		}

		public int TileSize = 24;
		public int AreaSize = 16;

		private Dictionary<CellType, GUIStyle> styles;

		private CellType[,] cells;

		private CellType[,] temporaryCells;
		private IntVector2 temporaryOrigin;
		private IntVector2 temporarySize;

		private MouseData mouseData;

		public RoomBuilderUI() {
			styles = new Dictionary<CellType, GUIStyle>();
		}

		/**
		 * Prepares all UI to show the room editing dialog.
		 * This should be the central entry point for all other scripts referencing the room builder.
		 * 
		 * @param center - Center point of the editable region
		 */
		public void ShowBuildDialog(IntVector3 center) {
			this.cells = new CellType[AreaSize, AreaSize];
			this.temporaryCells = null;
		}

		/**
		 * Creates a simple 1x1 texture containing a single color value.
		 * 
		 * @param color - Color for the texture
		 */
		private Texture2D CreateColorTexture(Color color) {
			var texture = new Texture2D(1, 1);
			
			texture.SetPixel(0, 0, color);
			texture.Apply();

			return texture;
		}

		/**
		 * Sets up basic styles for visualization of different tile types.
		 */
		private void SetupStyles() {
			CreateSimpleStyle(CellType.WALL, Color.black, Color.yellow);
			CreateSimpleStyle(CellType.FLOOR, Color.black, Color.green);
			CreateSimpleStyle(CellType.EMPTY, Color.white, Color.blue);
		}

		/**
		 * Creates a simple style for a given cell type.
		 * 
		 * @param cellType - Cell type to create the style for
		 * @param textColor - Text color of the tile
		 * @param backgroundColor - Background color of the tile
		 */
		private void CreateSimpleStyle(CellType cellType, Color textColor, Color backgroundColor) {
			var style = new GUIStyle(GUI.skin.GetStyle("Label"));

			style.alignment = TextAnchor.MiddleCenter;
			style.normal.background = CreateColorTexture(backgroundColor);
			style.normal.textColor = textColor;

			styles.Add(cellType, style);
		}

		/**
		 * Draws a single tile at the specified coordinate multiplied by tile size.
		 * 
		 * @param x - X coordinate of the tile
		 * @param z - z coordinate of the tile
		 * @param cellType - Type of the cell
		 */
		private void DrawTile(int x, int z, CellType cellType) {
			var rect = new Rect(x * TileSize, z * TileSize, TileSize, TileSize);
			var formattedName = cellType.ToString().Substring(0, 1);

			GUI.Label(rect, formattedName, styles[cellType]);
		}

		/**
		 * Converts screen space to tile grid coordinates.
		 * 
		 * @param original - Screen space coordinates
		 */
		private IntVector2 ConvertScreenToTileCoords(Vector3 original) {
			var coordinates = new IntVector2(original.x, original.y);

			coordinates.x = coordinates.x / TileSize;
			coordinates.z = (Screen.height - coordinates.z) / TileSize;

			return coordinates;
		}

		private void SetCells(IntVector2 origin, CellType[,] cells) {

			for (int x = 0; x < cells.GetLength(0); x++) {
				for (int z = 0; z < cells.GetLength(1); z++) {
					var spacePosition = new IntVector2(origin.x + x, origin.z + z);

					if (spacePosition.x > 0 && spacePosition.x < AreaSize && 
					    spacePosition.z > 0 && spacePosition.z < AreaSize) {

						this.cells[spacePosition.x, spacePosition.z] = cells[x, z];
					}
				}
			}
		}

		/**
		 * Resets cell data.
		 */
		public void Reset() {
			ShowBuildDialog(new IntVector3(128, 128, 128));
			
			Logger.Info("Reset", "Reset room drawing");
		}

		/**
		 * Initializes builder dialog.
		 */
		private void Awake() {
			ShowBuildDialog(new IntVector3(128, 128, 128));
		}

		/**
		 * Renders all builder-related UI
		 */
		private void OnGUI() {
			if (styles.Count == 0) {
				SetupStyles();
			}

			/* Draw editable area */
			for (int x = 0; x < this.cells.GetLength(0); x++) {
				for (int z = 0; z < this.cells.GetLength(1); z++) {
					DrawTile(x, z, this.cells[x, z]);
				}
			}

			/* Draw temporary room (if present) */
			if (this.temporaryCells != null) {

				for (int x = 0; x < this.temporaryCells.GetLength(0); x++) {
					for (int z = 0; z < this.temporaryCells.GetLength(1); z++) {
						DrawTile(temporaryOrigin.x + x, temporaryOrigin.z + z, this.temporaryCells[x, z]);
					}
				}
			}
		}

		/**
		 * Updates mouse input and triggers corresponding edit actions.
		 */
		private void Update() {

			/* Set origin coordinates if new click was detected */
			if (Input.GetMouseButtonDown(0)) {
				mouseData.originPosition = ConvertScreenToTileCoords(Input.mousePosition);
				mouseData.isPressed = true;
			}

			/* Drag to create a temporary room (created upon release) */
			if (Input.GetMouseButton(0)) {
				mouseData.currentPosition = ConvertScreenToTileCoords(Input.mousePosition);

				if (mouseData.HasMoved()) {
					temporarySize.x = Mathf.Abs(mouseData.currentPosition.x - mouseData.originPosition.x);
					temporarySize.z = Mathf.Abs(mouseData.currentPosition.z - mouseData.originPosition.z);

					this.temporaryCells = new CellType[temporarySize.x, temporarySize.z];

					for (int x = 0; x < temporarySize.x; x++) {
						for (int z = 0; z < temporarySize.z; z++) {
							var isWall = (x == 0 || x == temporarySize.x - 1 || z == 0 || z == temporarySize.z - 1);

							this.temporaryCells[x, z] = isWall ? CellType.WALL : CellType.FLOOR;
						}
					}
			
					this.temporaryOrigin = mouseData.originPosition;

					/* Make origin adjustments for negative room sizes */
					if (mouseData.currentPosition.x < mouseData.originPosition.x) {
						this.temporaryOrigin.x = mouseData.currentPosition.x;
					}
					
					if (mouseData.currentPosition.z < mouseData.originPosition.z) {
						this.temporaryOrigin.z = mouseData.currentPosition.z;
					}

					mouseData.lastPosition = mouseData.currentPosition;
				}
			} else if (mouseData.isPressed && this.temporaryCells != null) {

				if (temporarySize.x != 0 && temporarySize.z != 0) {
					SetCells(this.temporaryOrigin, this.temporaryCells);
				}

				mouseData.isPressed = false;
				this.temporaryCells = null;
			}
		}
	}

}