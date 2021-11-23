using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimes
{
    public class SlimeCreator : MonoBehaviour
    {
        [SerializeField]
        private GameObject sphere;

        [SerializeField]
        private Material slimeMaterial;

        public void CreateSlime()
        {
            SlimeParameters slimeParameters = new SlimeParameters();

            Mesh meshBody = CreateBodySphereMesh(slimeParameters);
        }

        private Mesh CreateBodySphereMesh(SlimeParameters slimeParameters)
        {
            // Body Parameters
            float radius = slimeParameters.Radius;
            int row = 50;
            int col = 100;
            float flatGridWidth = 8;
            float flatGridHeight = 6;


            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            _ = gameObject.AddComponent<MeshRenderer>();

            gameObject.GetComponent<Renderer>().material = slimeMaterial;
            Mesh meshBody = new Mesh();

            // Positions of vertices
             Vector3[] vertices = new Vector3[(row + 1) * (col + 1)];

            // Visual Spheres placed at vertices
            GameObject[] spheres = new GameObject[(row + 1) * (col + 1)];

            Vector2[] uv = new Vector2[(row + 1) * (col + 1)];
            Vector3[] normals = new Vector3[(row + 1) * (col + 1)];
            
            int[] triangles = new int[6 * row * col];
            
            // For all vertices, create vertex position and uv position grids
            for (int i = 0; i < vertices.Length; i++)
            {
                float x = i % (col + 1);
                float y = i / (col + 1);
                float x_pos = x / col * flatGridWidth;
                float y_pos = y / row * flatGridHeight;
                vertices[i] = new Vector3(x_pos, y_pos, 0);
                float u = x / col;
                float v = y / row;
                uv[i] = new Vector2(u, v);
            }

            // Create triangles
            for (int i = 0; i < 2 * row * col; i++)
            {
                int[] triIndex = new int[3];
                if (i % 2 == 0)
                {
                    triIndex[0] = i / 2 + i / (2 * col);
                    triIndex[1] = triIndex[0] + 1;
                    triIndex[2] = triIndex[0] + (col + 1);
                }
                else
                {
                    triIndex[0] = (i + 1) / 2 + i / (2 * col);
                    triIndex[1] = triIndex[0] + (col + 1);
                    triIndex[2] = triIndex[1] - 1;
                }

                triangles[i * 3] = triIndex[0];
                triangles[i * 3 + 1] = triIndex[1];
                triangles[i * 3 + 2] = triIndex[2];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                // Place visual Spheres
                spheres[i] = Instantiate(sphere, this.transform);

                // Transform vertex grid to sphere shape
                float theta = (vertices[i].x / flatGridWidth) * 2 * Mathf.PI; // 0 to 2Pi     0 points to x
                float phi = (vertices[i].y * Mathf.PI) / flatGridHeight;    // 0 to Pi      0 points to z

                Vector3 tempVector;
                tempVector.x = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
                tempVector.z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
                tempVector.y = radius * Mathf.Cos(phi);

                // Apply various rules to alter the sphere

                // Squish the sphere
                if (tempVector.y < 0)
                {
                    tempVector.y += Mathf.Abs(tempVector.y * slimeParameters.GroundSquishFactor);
                }

                // Check for EarSpikes
                if (slimeParameters.Horns != null)
                { tempVector = CreateEarSpikes(theta, phi, tempVector, slimeParameters.Horns); }

                // Check for Mouth
                if (slimeParameters.Mouth != null)
                { tempVector = CreateMouth(radius, tempVector, slimeParameters.Mouth); }


                // Create Spikes
                if (slimeParameters.Spikes != null)
                {
                    if (slimeParameters.Spikes[0].GetType() == typeof(CrystalSpikes))
                    {
                        // Create crystalline Spikes
                        tempVector = CreateCrystalSpikes(radius, tempVector, slimeParameters.Spikes);
                    }
                    if (slimeParameters.Spikes[0].GetType() == typeof(SharpSpikes))
                    {
                        // Create sharp Spikes
                        tempVector = CreateSharpSpikes(theta, phi, radius, tempVector, slimeParameters.Spikes);
                    }
                }











                /*                // Cut a circle:
                                // y = mz + b
                                float b = radius * 1.1f;
                                float m = 0.6f;
                                if ((m * tempVector.z + b) / tempVector.y < 1f &&
                                    tempVector.y > 0)
                                {
                                    tempVector.y = radius;
                                    tempVector.y = m * tempVector.z + b;
                                    spheres[i].GetComponent<Renderer>().material.color = Color.blue;
                                }*/




                vertices[i] = tempVector;
                spheres[i].transform.localPosition = tempVector;
                normals[i] = new Vector3(0, 1, 0);
            }

            // Set the mesh values
            meshBody.vertices = vertices;
            meshBody.normals = normals;
            meshBody.uv = uv;
            meshBody.triangles = triangles;
            meshFilter.mesh = meshBody;

            meshFilter.mesh.RecalculateNormals();

            return meshBody;
        }

        private static Vector3 CreateSharpSpikes(float theta, float phi, float radius, Vector3 tempVector, List<Spikes> allSpikes)
        {
            foreach (SharpSpikes ss in allSpikes)
            {
                if (tempVector.y > 0 &&
                    tempVector.x > ss.xMin && tempVector.x < ss.xMax && // top and bottom
                    tempVector.z > ss.zMin && tempVector.z < ss.zMax // left and right
                    )
                {
                    float distToCenter = Vector2.Distance(ss.centerXZ, new Vector2(tempVector.x, tempVector.z));

                    tempVector.x = (radius + ss.spikeHeight - distToCenter) * Mathf.Cos(theta) * Mathf.Sin(phi);
                    tempVector.z = (radius + ss.spikeHeight - distToCenter) * Mathf.Sin(theta) * Mathf.Sin(phi);
                    tempVector.y = (radius + ss.spikeHeight - distToCenter) * Mathf.Cos(phi);
                }
            }
            return tempVector;
        }

        private static Vector3 CreateCrystalSpikes(float radius, Vector3 tempVector, List<Spikes> allSpikes)
        {
            foreach (CrystalSpikes cs in allSpikes)
            {
                if (tempVector.y > 0 &&
                    tempVector.x > cs.xMin && tempVector.x < cs.xMax && // top and bottom of hexagon
                    tempVector.z > cs.zMin && tempVector.z < cs.zMax)
                {
                    // Apply top equation
                    if (tempVector.z > cs.centerXZ.y)
                    {
                        tempVector.y = -cs.topSlope * (tempVector.z - cs.centerXZ.y) + cs.topPoint;
                    }
                    else
                    {
                        tempVector.y = cs.topSlope * (tempVector.z - cs.centerXZ.y) + cs.topPoint;
                    }
                    // Also adjust height by distance from center of crystal
                    float distToCenter = Vector2.Distance(cs.centerXZ, new Vector2(tempVector.x, tempVector.z));
                    tempVector.y -= (distToCenter / radius);

                    return tempVector;
                }
            }

            return tempVector;
        }

        private static Vector3 CreateMouth(float radius, Vector3 tempVector, Mouth mouth)
        {
            if (tempVector.z > radius * mouth.MouthSize)
            {
                float value = (radius * mouth.MouthSize) - tempVector.z;
                tempVector.z += value * mouth.MouthIndentSize;
            }

            return tempVector;
        }

        private static Vector3 CreateEarSpikes(float theta, float phi, Vector3 tempVector, Horn earSpikes)
        {

            if (((theta < earSpikes.HornThetaLowLimit && theta > earSpikes.HornThetaHighLimit) ||                    // right spike
                (theta > Mathf.PI - earSpikes.HornThetaLowLimit && theta < Mathf.PI - earSpikes.HornThetaHighLimit)) // left spike
                &&
                (phi < earSpikes.HornPhiLowLimit && phi > earSpikes.HornPhiHighLimit))
            {
                tempVector.y += earSpikes.HornHeight;
            }

            return tempVector;
        }
    }
}