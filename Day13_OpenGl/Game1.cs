using LibCompute;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Day13_OpenGl
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D _empty, _wall, _block, _paddle, _ball;

        private IOPipe _io;
        private IntcodeComputer _computer;
        private bool _ioRequested;

        private Dictionary<Vector2, Tile> _board = new Dictionary<Vector2, Tile>();
        private SpriteFont _font;

        private bool _blockCounted = false;
        private int _score = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1344,
                PreferredBackBufferHeight = 608,

                SynchronizeWithVerticalRetrace = true
            };

            Content.RootDirectory = "Content";

            _io = new IOPipe();
            _computer = new IntcodeComputer("Arcade", "input", _io);
            _io.FireEveryNbOutput = 3;

            _io.IntOuputted += OnComputerOutput;

            _io.ReadingInt += OnComputerRequestingInput;

            _computer.MemoryWriteAt(0, 2);
        }

        private void OnComputerRequestingInput(object sender, EventArgs args)
        {
            _ioRequested = true;
            _io.InputInt(ComputePaddleMovement());
        }

        private int ComputePaddleMovement()
        {
            var state = Keyboard.GetState();
            var paddle = _board.First(x => x.Value.ID == BLOCK_ID.PADDLE);
            var ball = _board.First(x => x.Value.ID == BLOCK_ID.BALL);
            return Math.Sign(ball.Value.X - paddle.Value.X);
        }

        private void OnComputerOutput(object sender, EventArgs args)
        {
            var x = (int)_io.ReadOutputInt();
            var y = (int)_io.ReadOutputInt();

            if (x == -1 && y == 0) //score update
            {
                _score = (int)_io.ReadOutputInt();
            }
            else
            {
                var id = (BLOCK_ID)_io.ReadOutputInt();

                if (!_board.ContainsKey(new Vector2(x, y)))
                    _board.Add(new Vector2(x, y), null);

                Texture2D texture = null;

                switch (id)
                {
                    case BLOCK_ID.BALL:
                        texture = _ball;
                        break;

                    case BLOCK_ID.BLOCK:
                        texture = _block;
                        break;

                    case BLOCK_ID.EMPTY:
                        texture = _empty;
                        break;

                    case BLOCK_ID.PADDLE:
                        texture = _paddle;
                        break;

                    case BLOCK_ID.WALL:
                        texture = _wall;
                        break;

                    default:
                        throw new ArgumentException("Unknown tile type");
                }

                _board[new Vector2(x, y)] = new Tile(x, y, id) { Texture = texture };
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _empty = Content.Load<Texture2D>("empty_0");
            _wall = Content.Load<Texture2D>("wall_1");
            _block = Content.Load<Texture2D>("block_2");
            _paddle = Content.Load<Texture2D>("paddle_3");
            _ball = Content.Load<Texture2D>("ball_4");
            _font = Content.Load<SpriteFont>("font");

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            while (!_computer.ProgramTerminated && !_ioRequested)
                _computer.Step();

            if (!_blockCounted)
            {
                Window.Title = $"Answer to part 1: {_board.Count(x => x.Value.ID == BLOCK_ID.BLOCK)}";
                _blockCounted = true;
            }

            _ioRequested = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (var tile in _board)
            {
                var pos = tile.Key;
                var adjustedPos = new Vector2(pos.X * _wall.Width, pos.Y * _wall.Height);
                if (tile.Value.Texture is null)
                    continue;
                spriteBatch.Draw(tile.Value.Texture, adjustedPos, Color.White);
            }

            spriteBatch.DrawString(_font, $"Score: {_score}", new Vector2(0), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}