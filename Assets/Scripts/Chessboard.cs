using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
	# region Declare
	[Header("Art stuff")]
	[SerializeField] private Material tileMaterial;
	
	private const int TileCountX = 8;
	private const int TileCountY = 8;
	private GameObject[,] tiles;
	private Camera currentCamera;
	private Vector2Int currentHover;
	# endregion
	
	private void Awake() {
		GenerateAllTiles(1, TileCountX,TileCountY);
	}

	private void Update() {
		if (!currentCamera) {
			currentCamera = Camera.current;
			return;
		}
		RaycastHit info;
		Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile"))) {
			// Get the index of tile I have hit
			Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);
			// If we are hovering a tile after not hovering any tiles
			if (currentHover == -Vector2Int.one) {
				currentHover = hitPosition;
				tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
			}
			// If we were already hovering a tile, change the previous one
			if (currentHover != hitPosition) {
				tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
				currentHover = hitPosition;
				tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
			}
		}
	}

	#region Generate board
	// Create all tiles in board
	private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY) {
		tiles = new GameObject[tileCountX, tileCountY];
		for (int i = 0; i < tileCountX; i++) {
			for (int j = 0; j < tileCountY; j++) {
				tiles[i, j] = GenerateSingleTile(tileSize, i, j);
			}
		}
	}
	// Create single tile then return it
	private GameObject GenerateSingleTile(float tileSize, int x, int y) {
		GameObject tileObject = new GameObject($"X: {x}, Y: {y}");
		tileObject.transform.parent = transform;

		Mesh mesh = new Mesh();
		tileObject.AddComponent<MeshFilter>().mesh = mesh;
		tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

		Vector3[] verticles = new Vector3[4];
		verticles[0] = new Vector3(x * tileSize, 0, y * tileSize);
		verticles[1] = new Vector3(x * tileSize, 0, (y + 1) * tileSize);
		verticles[2] = new Vector3((x + 1) * tileSize, 0, y * tileSize);
		verticles[3] = new Vector3((x + 1) * tileSize, 0, (y + 1) * tileSize);

		int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
		mesh.vertices = verticles;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		tileObject.layer = LayerMask.NameToLayer("Tile");
		tileObject.AddComponent<BoxCollider>();
		
		return tileObject;
	}
	#endregion

	private Vector2Int LookupTileIndex(GameObject hitInfo) {
		for (int x = 0; x < TileCountX; x++) {
			for (int y = 0; y < TileCountY; y++) {
				if (tiles[x, y] == hitInfo) {
					return new Vector2Int(x, y);
				}
			}
		}
		return -Vector2Int.one; // Invalid
	}
}
