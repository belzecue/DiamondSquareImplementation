Implementation:

	Terrain generation:
		Firstly I instantiate a unity standard terrain object in my scene, then I process and change the terrainData using diamond square algorithm. So for every xy coordinates of the terrain I calculate a new height using diamond square. Each iteration I execute 2 nested loop, 1 for diamond and 1 for square. I keep halving the step by 2 for the entire width of the terrain; and using the step and halfstep we can calculate the average height for the coordinates for squares and diamonds. Multiply the height range with a random value to create variations.

		For applying the texture at different height of the terrain, I add in a custom material and imported 3 textures into my terrain object. At runtime, I assigned each coordinates of the terrain with different texture according to height. I created an splatmap and populate it with texture details of the terrain and then I reassigned to the terrain splatmap. The top 25% of the terrain is painted with white ice texture. The middle 15% of the terrain is a mixture of all three textures. The bottom is painted with grass texture. I also used random values in between the distinction of different parts of paint. So in between the ice texture and middle part, there is a mixture of both to have a smoother connection between the two parts.
	Camera
		I used the standard input methods of GetAxis() to control the xy rotation of the camera using Mouse X and Mouse Y
		I used increment/decrement z rotation of the camera by using GetKey()
		I used the standard input methods of GetAxis() to control the xz position of the camera
		To avoid going under the terrain, I sampled the height of the terrain on every update and check if terrain height is bigger than camera's y position
		To avoid going out of bounds, I calculate the xy coordinates of the 4 sides of the terrain and check if camera is out of bounds
	Sun
		I instantiated a point light sphere object and let it rotate around the terrain to simulate the effect of the sun 
	Water plane
		I instantiated a circular plane that is shaded with custom watershader which I wrote to have a simple transparent effect and using the water bump map of unity's standard assets
	Phoung Shading
		The terrain is shaded with custum shader using phone shading technique. According to the colour and the position of the point light, I applied fragment shading by calculating and combining ambient intensities, diffuse reflections and specular reflections components to shade the fragment. The final fragment colour is combined with the texture of the terrain.

External sources:
	Use of unity standard assets:
		- grass texture - GrassHillAlbedo
		- grass rocky texture - GrassRockyAlbedo
		- water bump map - WaterBump
	Code reference for phong shading
		- University of Melbourne, Alexandre Mutel
	Code reference for diamond square algorithm
		- wikipedia: https://en.wikipedia.org/wiki/Diamond-square_algorithm
		- Unity TerrainToolkit
	Code reference for applying colour/texture for terrain
		- Alastair Aitchison: https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/
	Code reference for water shader
		- Tim Michels: http://answers.unity3d.com/questions/355414/how-to-add-transparency-to-a-shader.html
