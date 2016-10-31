using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundCollector.Utils
{
    class DrawUtils
    {
        public static Texture2D CreateCircle(Int32 radius, GraphicsDevice graphicsDevice)
        {
            Int32 outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(graphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (Int32 i = 0; i < data.Length; i++)
            {
                data[i] = Color.Transparent;
            }

            // Work out the minimum step necessary using trigonometry + sine approximation.
            Double angleStep = 1f / radius;

            for (Double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                Int32 x = (Int32)Math.Round(radius + radius * Math.Cos(angle));
                Int32 y = (Int32)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }
    }
}
