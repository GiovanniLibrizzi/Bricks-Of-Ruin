using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CPGDGameJam.Game {
    class Util {

        public static float Lerp(float firstFloat, float secondFloat, float by) {
            return firstFloat * (1 - by) + secondFloat * by;
        }


        public static void Log(string message) {
            System.Diagnostics.Debug.WriteLine(message);
        }

        //public static float Clamp(float value, float min, float max) {
        //    return Math.Min(max, Math.Max(value, min));
        //}

        public static Rectangle RemoveRectPos(Rectangle rect) {
            return new Rectangle(0, 0, rect.Width, rect.Height);
        }
        
    }


    public struct Vector2Int {

        public int x, y;

        public Vector2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }

    }

}
