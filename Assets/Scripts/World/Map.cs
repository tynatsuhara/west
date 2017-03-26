using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Map {

	public const int CHUNK_SIZE = 100;
	public const int MAX_LOCATIONS_PER_CHUNK = 30;

	public static Dictionary<SerializableVector3, Chunk> chunks;

	public Map() {
		chunks = new Dictionary<SerializableVector3, Chunk>();
		GenerateChunk(Vector3.zero);
	}

	private static void GenerateChunk(Vector3 v) {
		Chunk c = new Chunk(v);

		for (int i = 0; i < MAX_LOCATIONS_PER_CHUNK; i++) {
			float x = Random.Range(0, 1f * CHUNK_SIZE);
			float y = Random.Range(0, 1f * CHUNK_SIZE);
			Location l = new Location(c, new Vector3(x, y, 0f));
		}
	}


	[System.Serializable]
	public class Chunk {
		public SerializableVector3 gridLocation;
		public static Dictionary<SerializableVector3, Chunk> chunks;

		public Chunk(Vector3 gridLocation) {
			this.gridLocation = new SerializableVector3(gridLocation);
		}
	}


	[System.Serializable]
	public class Location {
		public System.Guid guid;
		public Chunk chunk;
		private SerializableVector3 chunkCoordinates;
		public Vector3 worldCoord {
			get {
				float chunkX = chunk.gridLocation.val.x;
				float chunkY = chunk.gridLocation.val.y;
				return new Vector3(chunkX * CHUNK_SIZE + chunkCoordinates.val.x, 
								   chunkY * CHUNK_SIZE + chunkCoordinates.val.y, 0f);
			}
		}

		public Location(Chunk chunk, Vector3 chunkCoordinates) {
			this.chunk = chunk;
			this.chunkCoordinates = new SerializableVector3(chunkCoordinates);
		}
	}
 }