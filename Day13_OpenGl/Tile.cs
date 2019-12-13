using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day13_OpenGl
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }

        public BLOCK_ID ID { get; set; }

        public Tile(int x, int y, BLOCK_ID id)
        {
            X = x;
            Y = y;
            ID = id;
        }

        public Texture2D Texture { get; set; }
    }

    public enum BLOCK_ID
    {
        EMPTY = 0,
        WALL = 1,
        BLOCK = 2,
        PADDLE = 3,
        BALL = 4
    }
}