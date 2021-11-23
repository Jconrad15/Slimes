using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slimes
{
    public static class Chances
    {
        public static float HornChance = 0.7f;
        public static float MouthChance = 0.9f;
        public static float SpikesChance = 0.8f;

    }

    public class SlimeParameters
    {
        public float GroundSquishFactor { get; protected set; }
        public float Radius { get; protected set; }

        public Horn Horns { get; protected set; }
        public Mouth Mouth { get; protected set; }
        public List<Spikes> Spikes { get; protected set; }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public SlimeParameters()
        {
            GroundSquishFactor = Random.Range(0.7f, 0.8f);
            Radius = Random.Range(1f, 3f);

            // Check add Mouth
            if (Random.value < Chances.MouthChance)
            { Mouth = new Mouth(); }
            else
            { Mouth = null; }

            // Check add Horns
            if (Random.value < Chances.HornChance)
            { Horns = new Horn(); }
            else
            { Horns = null; }

            // Check add Spikes
            if (Random.value < Chances.SpikesChance)
            { AddSpike(); }



        }

        /// <summary>
        /// Adds spikes to the slime parameters.  Calls the relevant spike type constructor.
        /// </summary>
        private void AddSpike()
        {
            // Choose random type of spike
            SpikeNames spikeType = 
                (SpikeNames)Random.Range(0, (int)Enum.GetValues(typeof(SpikeNames)).Cast<SpikeNames>().Max() + 1);

            Debug.Log(spikeType);

            switch (spikeType)
            {
                case SpikeNames.Crystal:
                    int numCrystalSpikes = (int)Random.Range(Radius * 6, Radius * 8);
                    Spikes = new List<Spikes>();
                    for (int i = 0; i < numCrystalSpikes; i++)
                    {
                        Spikes.Add(new CrystalSpikes(Radius));
                    }
                    break;

                case SpikeNames.Sharp:
                    int numSharpSpikes = (int)Random.Range(Radius * 18, Radius * 22);
                    Debug.Log(numSharpSpikes);
                    Spikes = new List<Spikes>();
                    for (int i = 0; i < numSharpSpikes; i++)
                    {
                        Spikes.Add(new SharpSpikes(Radius));
                    }
                    break;

                default:
                    break;
            }


        }


    }

    public class Horn
    {
        public float HornThetaLowLimit { get; protected set; }
        public float HornThetaHighLimit { get; protected set; }
        public float HornPhiLowLimit { get; protected set; }
        public float HornPhiHighLimit { get; protected set; }
        public float HornHeight { get; protected set; }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public Horn()
        {
            HornThetaLowLimit = Mathf.PI / 6;
            HornThetaHighLimit = Mathf.PI / 8;
            HornPhiLowLimit = Mathf.PI / 6;
            HornPhiHighLimit = Mathf.PI / 8;
            HornHeight = Random.Range(0.2f, 1f);
        }


    }

    public class Mouth
    {
        public float MouthSize { get; protected set; }
        public float MouthIndentSize { get; protected set; }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public Mouth()
        {
            MouthSize = Random.Range(0.8f, 0.95f);
            MouthIndentSize = Random.Range(2f, 4f);
        }
    }


    public class Spikes
    {
        
    }

    public enum SpikeNames { Crystal, Sharp };

    public class CrystalSpikes : Spikes
    {
        public Vector2 centerXZ { get; protected set; }
        public float zSize { get; protected set; }
        public float xSize { get; protected set; }
        public float xMax { get; protected set; }
        public float xMin { get; protected set; }
        public float zMax { get; protected set; }
        public float zMin { get; protected set; }
        public float topSlope { get; protected set; }
        public float topExtensionAmount { get; protected set; }
        public float topPoint { get; protected set; }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public CrystalSpikes(float radius)
        {
            centerXZ = new Vector2(Random.Range(-0.8f * radius, 0.8f * radius),
                                   Random.Range(-0.8f * radius, 0.1f * radius));
            zSize = Random.Range(0.08f * radius, 0.15f * radius);
            xSize = Random.Range(0.08f * radius, 0.15f * radius);
            xMax = centerXZ.x + xSize;
            xMin = centerXZ.x - xSize;
            zMax = centerXZ.y + zSize;
            zMin = centerXZ.y - zSize;
            topSlope = Random.Range(0.4f, 1f);
            topExtensionAmount = Random.Range(0.3f, 0.6f);
            topPoint = radius + topExtensionAmount;
        }
    }

    public class SharpSpikes : Spikes
    {
        public Vector2 centerXZ { get; protected set; }
        public float zSize { get; protected set; }
        public float xSize { get; protected set; }
        public float xMax { get; protected set; }
        public float xMin { get; protected set; }
        public float zMax { get; protected set; }
        public float zMin { get; protected set; }
        public float spikeHeight { get; protected set; }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public SharpSpikes(float radius)
        {
            centerXZ = new Vector2(Random.Range(-0.75f * radius, 0.75f * radius),
                                   Random.Range(-0.75f * radius, 0.3f * radius));
            zSize = 0.05f;
            xSize = 0.05f;
            xMax = centerXZ.x + xSize;
            xMin = centerXZ.x - xSize;
            zMax = centerXZ.y + zSize;
            zMin = centerXZ.y - zSize;
            spikeHeight = radius * Random.Range(0.1f, 0.3f);
        }
    }




}