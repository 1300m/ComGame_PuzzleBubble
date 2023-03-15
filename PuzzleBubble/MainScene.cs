using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PuzzleBubble
{
    public class MainScene : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public const int GAMEWIDTH = 400;
        public const int GAMEHEIGHT = 700;

        public const int _BUBBLESIZE = 50;

        int[,] _gameTable;

        Texture2D _bubble, _rect, _cannon;
        SpriteFont _font;

        int _bubbleShootCount;
        int _ceilingDown;

        Color _colorRnd, _nextColorRnd;

        bool _win, _lose;

        Vector2 _bubblePos;
        float _moveSpeed;
        float _tick;

        enum GameState
        {
            PrepareShooting,
            Shooting,
            GameEnded
        }
        GameState _currentGameState;

        MouseState _currentMouseState, _previousMouseState;
        KeyboardState _currentKeyState, _previouskeyState;

        public MainScene()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = GAMEWIDTH + 400;
            _graphics.PreferredBackBufferHeight = GAMEHEIGHT;
            _graphics.ApplyChanges();

            Reset();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _bubble = Content.Load<Texture2D>("BubbleBomb");
            _cannon = Content.Load<Texture2D>("Cannon");
            _font = Content.Load<SpriteFont>("GameFont");

            _rect = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            _rect.SetData(data);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentKeyState = Keyboard.GetState();
            _previousMouseState = _currentMouseState;

            switch (_currentGameState)
            {
                case GameState.PrepareShooting:
                    //Ceiling Dropping
                    if (_bubbleShootCount % 3 == 0 && _bubbleShootCount != 0)
                    {
                        for (int i = 10; i >= _ceilingDown; i--)
                        {
                            for (int j = 7; j >= 0; j--)
                            {
                                if (i != _ceilingDown)
                                {
                                    _gameTable[i, j] = _gameTable[i - 1, j];
                                }
                                else
                                {
                                    _gameTable[i, j] = 0;
                                }
                            }
                        }

                        _ceilingDown++;
                    }

                    

                    if (_bubbleShootCount > 0)
                    {
                        _bubblePos = new Vector2(GAMEWIDTH / 2 + 175, GAMEHEIGHT - 50);
                        _colorRnd = _nextColorRnd;
                        _nextColorRnd = ColorRandom();
                    }

                    _currentGameState = GameState.Shooting;

                    _win = CheckedWin();
                    _lose = CheckedLose();

                    if (_lose || _win)
                    {
                        _currentGameState = GameState.GameEnded;
                    }
                    break;
                case GameState.Shooting:
                    //Shoot!
                    _currentMouseState = Mouse.GetState();
                    if ((_currentMouseState.LeftButton == ButtonState.Pressed &&
                        _previousMouseState.LeftButton == ButtonState.Released) ||
                        _currentKeyState.IsKeyDown(Keys.Space) && !_currentKeyState.Equals(_previouskeyState))
                    {
                        _bubbleShootCount++;

                        _currentGameState = GameState.PrepareShooting;
                    }
                    break;
                case GameState.GameEnded:
                    if (_currentKeyState.IsKeyDown(Keys.Space) && !_currentKeyState.Equals(_previouskeyState))
                    {
                        _currentGameState = GameState.PrepareShooting;
                        Reset();
                    }
                    break;
            }

            _previouskeyState = _currentKeyState;

            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            //BackGround drawing (เดี๋ยวเปลี่ยนทีหลัง)
            _spriteBatch.Draw(_rect, new Vector2(200, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(GAMEWIDTH, GAMEHEIGHT), SpriteEffects.None, 0f);

            ///Line
            _spriteBatch.Draw(_rect, new Vector2(200, GAMEHEIGHT - 100), null, Color.Brown, 0f, Vector2.Zero, new Vector2(GAMEWIDTH, 5), SpriteEffects.None, 0f);

            //Bubble
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch(_gameTable[i, j])
                    {
                        case 1:
                            _spriteBatch.Draw(_bubble, new Vector2(_BUBBLESIZE * j + 200, _BUBBLESIZE * i + 100), null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            break;
                        case 2:
                            _spriteBatch.Draw(_bubble, new Vector2(_BUBBLESIZE * j + 200, _BUBBLESIZE * i + 100), null, Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            break;
                        case 3:
                            _spriteBatch.Draw(_bubble, new Vector2(_BUBBLESIZE * j + 200, _BUBBLESIZE * i +100), null, Color.Blue, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            break;
                        case 4:
                            _spriteBatch.Draw(_bubble, new Vector2(_BUBBLESIZE * j + 200, _BUBBLESIZE * i + 100), null, Color.Green, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            break;
                    }
                }
            }

            //Ceiling
            _spriteBatch.Draw(_rect, new Vector2(200, -(GAMEHEIGHT - 100) + (_BUBBLESIZE * _ceilingDown)), null, Color.DimGray, 0f, Vector2.Zero, new Vector2(GAMEWIDTH, GAMEHEIGHT), SpriteEffects.None, 0f);

            //Cannon
            _spriteBatch.Draw(_cannon, new Vector2(GAMEWIDTH / 2 + 160, GAMEHEIGHT - 100), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            _spriteBatch.Draw(_bubble, _bubblePos, _colorRnd);
            _spriteBatch.Draw(_bubble, new Vector2(GAMEWIDTH / 2 + 75, GAMEHEIGHT - 50), _nextColorRnd);

            switch (_currentGameState)
            {
                case GameState.PrepareShooting:
                    break;
                case GameState.Shooting:
                    break;
                case GameState.GameEnded:
                    if (_lose)
                    {
                        Vector2 fontSize1 = _font.MeasureString("LOSE");
                        _spriteBatch.DrawString(_font, "LOSE", new Vector2((GAMEWIDTH - fontSize1.X) / 2 + 200, (GAMEHEIGHT - fontSize1.Y) / 2 - 30), Color.Red);
                        Vector2 fontSize2 = _font.MeasureString("Press SPACE to restart");
                        _spriteBatch.DrawString(_font, "Press SPACE to restart", new Vector2((GAMEWIDTH - fontSize2.X) / 2 + 200, (GAMEHEIGHT - fontSize2.Y) / 2 + 10), Color.OrangeRed);
                    }
                    else
                    {
                        Vector2 fontSize3 = _font.MeasureString("WIN!");
                        _spriteBatch.DrawString(_font, "WIN!", new Vector2((GAMEWIDTH - fontSize3.X) / 2 + 200, (GAMEHEIGHT - fontSize3.Y) / 2), Color.LightYellow);
                    }
                    break;
            }

            _spriteBatch.End();
            _graphics.BeginDraw();

            base.Draw(gameTime);
        }

        protected void Reset()
        {


            _gameTable = new int[11, 8]
            {
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0}
            };
            _bubbleShootCount = 0;
            _ceilingDown = 0;
            _colorRnd = ColorRandom();
            _nextColorRnd = ColorRandom();
            _lose = false;
            _win = false;
            _moveSpeed = 10f;

            _bubblePos = new Vector2(GAMEWIDTH / 2 + 175, GAMEHEIGHT - 50);

            _currentGameState = GameState.PrepareShooting;
            _currentMouseState = Mouse.GetState();
        }

        protected Color ColorRandom()
        {
            Random rnd = new Random();
            int _rndNum = rnd.Next(4) + 1;

            switch (_rndNum)
            {
                case 1:
                    return Color.Red;
                case 2:
                    return Color.Yellow;
                case 3:
                    return Color.Blue;
                case 4:
                    return Color.Green;
                default:
                    return Color.White;
            }
        }

        protected bool CheckedWin()
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_gameTable[i, j] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool CheckedLose()
        {
            for (int j = 0; j < 8; j++)
            {
                if (_gameTable[10, j] != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}