using UnityEngine;
using System.Collections;

public class DiamondSquareGenerator : MonoBehaviour {
	public float diamondSquareDelta = 5f;
	public float diamondSquareBlend = 100f;
	public PointLight pointLight;
	public GameObject waterPlane;
	public int seed = 12345;
	public bool random = false;
	private Terrain terrain;
	private float maxHeight;
	private float minHeight;

	// Use this for initialization
	void Start () {
		terrain = (Terrain) GetComponent(typeof(Terrain));
		if (terrain == null) {
			Debug.Log ("Cannot find terrain object!");
			return;
		}
		if (!random) {
			Random.seed = seed;
		}
		generateTerrain ();
	}
	
	// Update is called once per frame
	void Update () {
		terrain.materialTemplate.SetColor("_PointLightColor", this.pointLight.color);
		terrain.materialTemplate.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
	}
	public void generateTerrain() {
		TerrainData terrainData = terrain.terrainData;
		assignHeights (terrainData);
		calculateMaxMinHeight (terrainData);
		float waterPlaneHeight = minHeight + ((maxHeight - minHeight) / 2.0f);
		waterPlane.transform.position = new Vector3 (waterPlane.transform.position.x, waterPlaneHeight, waterPlane.transform.position.z);
		assignSplatMap (terrainData);
	}

	private void calculateMaxMinHeight(TerrainData terrainData) {
		for (int y = 0; y < terrainData.alphamapHeight; y++) {
			for (int x = 0; x < terrainData.alphamapWidth; x++) {
				// Normalise x/y coordinates to range 0-1 
				float yNormalized = (float)y / (float)terrainData.alphamapHeight;
				float xNormalized = (float)x / (float)terrainData.alphamapWidth;

				// Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
				float height = terrainData.GetHeight (Mathf.RoundToInt (yNormalized * terrainData.heightmapHeight), Mathf.RoundToInt (xNormalized * terrainData.heightmapWidth));

				if (height > maxHeight) {
					maxHeight = height;
				}
				if (height < minHeight) {
					minHeight = height;
				}
			}
		}
	}

	private void assignSplatMap(TerrainData terrainData) {

		// Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
		float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
		for (int y = 0; y < terrainData.alphamapHeight; y++)
		{
			for (int x = 0; x < terrainData.alphamapWidth; x++)
			{
				// Normalise x/y coordinates to range 0-1 
				float yNormalized = (float)y/(float)terrainData.alphamapHeight;
				float xNormalized = (float)x/(float)terrainData.alphamapWidth;

				// Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
				float height = terrainData.GetHeight(Mathf.RoundToInt(yNormalized * terrainData.heightmapHeight),Mathf.RoundToInt(xNormalized * terrainData.heightmapWidth) );

				// Setup an array to record the mix of texture weights at this point
				float[] splatWeights = new float[terrainData.alphamapLayers];

				int randomTexture = Random.Range (0, 2);
				float randomBoundHeight = Random.Range (0, 30);

				float heightRange = maxHeight - minHeight;
				float iceRangeMin = maxHeight - heightRange * 0.25f;
				float mudRangeMin = iceRangeMin - heightRange * 0.25f;
				float grassRangeMin = minHeight;

				if (height >= iceRangeMin - randomBoundHeight) {
					splatWeights[2] = 1f;
				}

				if (height < (iceRangeMin + randomBoundHeight) && height >= mudRangeMin) {
					splatWeights[randomTexture] = 10f;
				}

				if (height < (mudRangeMin + randomBoundHeight) && height >= grassRangeMin) {
					splatWeights[0] = 1f;
				}

			
				// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
				float z = splatWeights[0] + splatWeights[1] + splatWeights[2];

				// Loop through each terrain texture
				for(int i = 0; i<terrainData.alphamapLayers; i++){

					// Normalize so that sum of all texture weights = 1
					splatWeights[i] /= z;

					// Assign this point to the splatmap array
					splatmapData[x, y, i] = splatWeights[i];
				}
			}
		}
		// Finally assign the new splatmap to the terrainData:
		terrainData.SetAlphamaps(0, 0, splatmapData);
	}

	

	private void assignHeights(TerrainData terrainData) {
		int terrainWidth = terrainData.heightmapWidth;
		int terrainHeight = terrainData.heightmapHeight;

		float[,] heightMap = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
		float[,] generatedHeightMap = (float[,]) heightMap.Clone();

		// Set the number of iterations and pass the height array to the appropriate generator script...
		generatedHeightMap = generateDiamondSquare(generatedHeightMap, new Vector2(terrainWidth, terrainHeight));

		// Apply it to the terrain object...
		for (int y = 0; y < terrainHeight; y++) {
			for (int x = 0; x < terrainWidth; x++) {
				float newHeightAtPoint = generatedHeightMap[x, y];
				heightMap[x, y] = newHeightAtPoint;
			}
		}
		terrainData.SetHeights(0, 0, heightMap);
	}

	private float[,] generateDiamondSquare(float[,] heightMap, Vector2 arraySize) {
		int terrainWidth = (int) arraySize.x;
		int terrainHeight = (int) arraySize.y;
		float heightRange = 1.5f;
		int step = terrainWidth - 1;

		// initialize 4 corners
		heightMap[0, 0] = 0.5f;
		heightMap[terrainWidth - 1, 0] = 0.5f;
		heightMap[0, terrainHeight - 1] = 0.5f;
		heightMap[terrainWidth - 1, terrainHeight - 1] = 0.5f;

		while (step > 1) {
			
			// calculate the next half
			int halfStep = Mathf.RoundToInt (step / 2);

			// diamond
			for (int x = 0; x < terrainWidth - 1; x += step){
				for (int y = 0; y < terrainHeight - 1; y += step){
					int nextX = x + halfStep;
					int nextY = y + halfStep;

					Vector2[] points = new Vector2[4];
					points[0] = new Vector2(x, y);
					points[1] = new Vector2(x + step, y);
					points[2] = new Vector2(x, y + step);
					points[3] = new Vector2(x + step, y + step);

					dsCalculateHeight(heightMap, arraySize, nextX, nextY, points, heightRange);
				}
			}
			// square
			for (int x = 0; x < terrainWidth - 1; x += step) {
				for (int y = 0; y < terrainHeight - 1; y += step) {
					int nextX = x + halfStep;
					int nextY = y + halfStep;

					Vector2[] points1 = new Vector2[4];
					points1[0] = new Vector2(nextX - halfStep, y);
					points1[1] = new Vector2(nextX, y - halfStep);
					points1[2] = new Vector2(nextX + halfStep, y);
					points1[3] = new Vector2(nextX, y + halfStep);

					Vector2[] points2 = new Vector2[4];
					points2[0] = new Vector2(x - halfStep, nextY);
					points2[1] = new Vector2(x, nextY - halfStep);
					points2[2] = new Vector2(x + halfStep, nextY);
					points2[3] = new Vector2(x, nextY + halfStep);

					dsCalculateHeight(heightMap, arraySize, nextX, y, points1, heightRange);
					dsCalculateHeight(heightMap, arraySize, x, nextY, points2, heightRange);
				}
			}
			heightRange *= diamondSquareDelta;
			step = halfStep;
		}
		return heightMap;
	}

	private void dsCalculateHeight(float[,] heightMap, Vector2 arraySize, int x, int y, Vector2[] points, float heightRange) {
		int terrainWidth = (int) arraySize.x;
		int terrainHeight = (int) arraySize.y;
		float h = 0.0f;

		// calculate the average height
		for (int i = 0; i < 4; i++){
			if (points[i].x < 0) {
				points[i].x += (terrainWidth - 1);
			} else if (points[i].x > terrainWidth) {
				points[i].x -= (terrainWidth - 1);
			} else if (points[i].y < 0) {
				points[i].y += terrainHeight - 1;
			} else if (points[i].y > terrainHeight) {
				points[i].y -= terrainHeight - 1;
			}
			h += (float) (heightMap[(int) points[i].x, (int) points[i].y] / 4);
		}
		// apply random seed and height range
		h += (Random.value * heightRange - heightRange / 2);

		// nomalise h within 0 and 1
		if (h < 0.0f) {
			h = 0.0f;
		} else if (h > 1.0f) {
			h = 1.0f;
		}

		// add to height map
		heightMap[x, y] = h;

		// handle the first four corner points
		if (x == 0) {
			heightMap[terrainWidth - 1, y] = h;
		} else if (x == terrainWidth - 1) {
			heightMap[0, y] = h;
		} else if (y == 0) {
			heightMap[x, terrainHeight - 1] = h;
		} else if (y == terrainHeight - 1) {
			heightMap[x, 0] = h;
		}
	}


}
