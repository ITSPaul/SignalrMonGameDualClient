
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AnimatedSprite;
using System.Collections.Generic;
using Utilities;
using System.Linq;
using System;
using CameraNS;

namespace MonoGameClientAss12015
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private static Object gameLock = new Object();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D _background;
        Player player;
        Player opponent;
        public static HubConnection myConnection;
        public static IHubProxy proxy;
        Vector2 oldPos;
        Vector2 _worldBound = new Vector2(5000, 5000);
        Vector2 _viewCentre;
        // Holds all the game collectables both this players and the opponents
        List<Collectable> _collectables = new List<Collectable>();
        List<Barrier> _barriers = new List<Barrier>();

        Dictionary<string, float> characterStrength = 
            new Dictionary<string, float>() { { "Sprites\\body", 5.0f }, {"Sprites\\body2", 10.0f } };
        Dictionary<string, float> characterSpeed =
                new Dictionary<string, float>() { { "Sprites\\body", 10.0f }, { "Sprites\\body2", 5.0f } };

        private Texture2D _tx1;
        private Texture2D _tx0;
        Camera _playerCam;
        bool playing = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
#if ANDROID

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#endif

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            new InputEngineNS.InputEngine(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            int characterChoice = Utilities.Utility.NextRandom(1);
            myConnection = new HubConnection("http://localhost:50574/");
            //myConnection = new HubConnection("http://cgmonogameserver2015.azurewebsites.net/");
            myConnection.StateChanged += MyConnection_StateChanged;
            proxy = myConnection.CreateHubProxy("MultiHubV2");
            myConnection.Start().Wait();

            _background = Content.Load<Texture2D>(@"Backgrounds\sky_pissyellowstripes");
            font = Content.Load<SpriteFont>(@"SpriteFonts\message");
            _tx0 = Content.Load<Texture2D>(@"Sprites\body");
            _tx1 = Content.Load<Texture2D>(@"Sprites\body2");
            characterChoice = Utility.NextRandom(1);

            if (characterChoice == 0) // first player is stronger and slower
            {
                player = new Player(_tx0,
                                    Vector2.Zero, 5.0f, font, 1, 0.5f, _worldBound);
            }
            else if (characterChoice == 1) // first player is stronger and slower
            {
                player = new Player(_tx1,
                                    Vector2.Zero, 10.0f, font, 1, 0.5f, _worldBound);
            }

            _viewCentre = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            // Create a player in the current Viewport in the current client
            player.position =
            //_viewCentre;
            new Vector2(
                            Utility.NextRandom(GraphicsDevice.Viewport.Width),
                            Utility.NextRandom(GraphicsDevice.Viewport.Height));
            player.clientID = myConnection.ConnectionId;
            player.speed = characterSpeed[player.textureName];
            _playerCam = new Camera(Vector2.Zero, _worldBound);
            // request to join the game
            proxy.Invoke("join", new object[] { player.clientID, player.textureName, (int)player.position.X, (int)player.position.Y });
            proxy.On<int>("playerNumber", (n)
                 =>
                     {
                         player.playerId = "Player " + n.ToString();
                     }
                    );
            proxy.On<int>("opponentNumber", (n)
                 =>
            {
                if (opponent != null)
                    opponent.playerId = "Player " + n.ToString();
            }
                );
            // recieve a Joined player and setup an opponent player for that player
            proxy.On<string, string, int, int>("opponentJoined", (JoinedId, CharacterName, x, y)
                 =>
            {
                if (opponent == null && JoinedId != player.clientID)
                {
                    Texture2D tx = Content.Load<Texture2D>(@"" + CharacterName);
                    opponent = new Player(tx,
                                Vector2.Zero, characterStrength[CharacterName], font, 1, 0.5f,_worldBound);
                    opponent.position = new Vector2(x, y);
                    opponent.clientID = JoinedId;
                    opponent.speed = characterSpeed[opponent.textureName];
                    //opponent.playerId = "Player " + count.ToString();
                    // Add the first player to join all the clients as it had no others when started
                    proxy.Invoke("addMe", new object[] { player.clientID,
                                                player.textureName,
                                                (int)player.position.X, (int)player.position.Y });
                }
            });
            // recieve an opponent moved
            proxy.On<int, int>("opponentMoved", (x, y)
                => { if (opponent != null) opponent.position = new Vector2(x, y); });
            // recieve a tidy up after an opponent has left
            proxy.On("opponentLeft", ()
                => { opponent = null; });
            // recieve a setup setup message and setup the game and start to play
            proxy.On("setup", ()
                => { createCollectables();
                    createBarriers();
                    playing = true;
                });
            // recieve an opponents barrier
            proxy.On<int, int>("createOpponentBarrier", (x, y) =>
            {
                    _tx1 = Content.Load<Texture2D>(@"Sprites\grassGreen");
                    Barrier b = new Barrier(_tx1, Vector2.Zero, font, 1, 0.1f);
                    b.position = new Vector2(x, y);
                    b.owner = opponent.clientID;
                    lock (gameLock)
                    {
                        _barriers.Add(b);
                    }
            });
            // recieve an opponents collectable
            proxy.On<int, int>("createOpponentCollectable", (x, y) =>
            {
                    _tx1 = Content.Load<Texture2D>(@"Sprites\bomb");
                    Collectable c = new Collectable(_tx1, Vector2.Zero, font, 1, 0.1f);
                    c.position = new Vector2(x, y);
                    c.owner = opponent.clientID;
                lock (gameLock)
                {
                    _collectables.Add(c);
                }
            });
            // recieve a Remove message and remove an opponents artifacts from this game
            proxy.On<string>("Remove", (OpponentID) =>
            {
                lock (gameLock)
                {

                    var collectablesTodelete = (from c in _collectables
                                            where c.owner == OpponentID
                                            select c).ToList();
                    foreach (Collectable c in collectablesTodelete)
                    {
                            _collectables.Remove(c);
                         }
                }
                lock (gameLock)
                   {
                    var barriersTodelete = (from c in _barriers
                                            where c.owner == OpponentID
                                            select c).ToList();
                        foreach (Barrier b in barriersTodelete)
                            {
                                    _barriers.Remove(b);
                                }
                     }
                 }); 
            // if a barrier is hit by a player projectile then update the opponents representation in this client
            proxy.On<int, int, string>("opponentBarrierHit",
                        (x, y, owner) =>
                        {
                            lock (gameLock)
                            {
                                // as long as no two barriers have the same position
                                Barrier hitb = _barriers
                                .Where(b => b.position.X == x && b.position.Y == y && b.owner != owner)
                                .FirstOrDefault();
                                if (hitb != null)
                                {
                                    hitb.NumberOfHits++;
                                }
                            }
                        });
            // if an opponent collects a collectaable update the opponents representation in this client
            proxy.On<int, int, string>("opponentCollected",
                        (x, y, owner) =>
                        {
                            lock (gameLock)
                            {
                                // as long as no two barriers have the same position
                                var collected = _collectables
                                .Where(c => c.position.X == x && c.position.Y == y && c.owner != owner);
                                if (collected.Count() > 0)
                                {
                                    foreach (Collectable c in collected)
                                    {
                                        _collectables.Remove(c);
                                    }
                                }
                            }
                        });

            // TODO: use this.Content to load your game content here
        }
        // Create a barrier locally and inform listening clients that it is in the game
        private void createBarriers()
        {
            for (int i = 0; i < 1; i++)
            {
                _tx1 = Content.Load<Texture2D>(@"Sprites\grassBlack");
                Barrier b = new Barrier(_tx1, Vector2.Zero, font, 1, 1.0f);
                b.position = new Vector2(
                                Utility.NextRandom((int)_worldBound.X - _tx1.Width),
                                Utility.NextRandom((int)_worldBound.Y - _tx1.Height));
                b.owner = player.clientID;
                lock (gameLock)
                {
                    _barriers.Add(b);
                }
                proxy.Invoke("setUpOpponentBarrier", new object[] { (int)b.position.X, (int)b.position.Y });
            }
        }


        // Same as barriers above
        private void createCollectables()
        {
            for (int i = 0; i < 5; i++)
            {
                _tx1 = Content.Load<Texture2D>(@"Sprites\coin");
                Collectable c = new Collectable(_tx1, Vector2.Zero, font, 1, 1.0f);
                c.position = new Vector2(
                                Utility.NextRandom((int)_worldBound.X- _tx1.Width),
                                Utility.NextRandom((int)_worldBound.Y - _tx1.Height));
                c.owner = player.clientID;
                lock (gameLock)
                {
                    _collectables.Add(c);
                }
                proxy.Invoke("setUpOpponentCollectable", new object[] { (int)c.position.X, (int)c.position.Y });
            }

        }

        private void MyConnection_StateChanged(StateChange connectionState)
        {
            if(connectionState.OldState == ConnectionState.Reconnecting && connectionState.NewState == ConnectionState.Connected)
            {
                proxy.Invoke("changeConnection" ,new object[] {player.clientID });
                player.clientID = myConnection.ConnectionId;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        /// 
        protected override void EndRun()
        {
            proxy.Invoke("leave");
            base.EndRun();
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // To ensure that collections are not accessed while they are being setup

            if (playing)
            {
                lock (gameLock)
                {
                    foreach (Barrier b in _barriers)
                        b.Update(gameTime);
                }
                lock (gameLock)
                {
                    foreach (Collectable c in _collectables)
                        c.Update(gameTime);
                }
                checkBarrierHits();
                checkCollectedItems();
            }
            if (myConnection.State == ConnectionState.Connected)
            {
                oldPos = player.position;
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                if (player != null)
                {
                    player.Update(gameTime,myConnection.ConnectionId);
                   // _playerCam.follow(player.position, GraphicsDevice.Viewport);
                }
                    if (opponent != null)
                        opponent.Update(gameTime,myConnection.ConnectionId);
                    _playerCam.follow(player.position, GraphicsDevice.Viewport);

                // TODO: Add your update logic here
            }
            base.Update(gameTime);
        }

        private void checkCollectedItems()
        {
            // see if a player is collecting anything
            IEnumerable<Collectable> collected = null;
            lock (gameLock)
            {
                collected = _collectables
                .Where(c => c.Visible && c.collisionDetect(player) && c.owner == player.clientID).ToList();
            }
            // if there has been anything collected then remove it
            if (collected.Count() > 0)
            {
                foreach (var  c in collected)
                {
                    player.score += c.value;
                    lock (gameLock)
                    {
                        _collectables.Remove(c);
                    }
                    // inform the others that it is collected
                    proxy.Invoke("collected",
                        new object[] { c.position.X, c.position.Y });
                }
            }

        }



        private void checkBarrierHits()
        {
            // get any hits on opponents barriers
            IEnumerable<Barrier> hits = null;
            lock (gameLock)
            {
                if (player.PlayerProjectile != null)
                {
                    hits = _barriers
                    .Where(barrier => barrier.owner != player.clientID
                    && player.PlayerProjectile.collisionDetect(barrier));

                    // Apply the hits 
                    if (hits.Count() > 0)
                    {
                        player.PlayerProjectile.ProjectileState = Projectile.PROJECTILE_STATE.EXPOLODING;
                        foreach (Barrier barrier in hits)
                        {
                            barrier.NumberOfHits++;
                            proxy.Invoke("barrierHit",
                                new object[] { barrier.position.X, barrier.position.Y });
                        }
                        // destroy any barriers that need destroying irrespective of player
                        var destroyed = _barriers.Where(b => b.NumberOfHits > 2);
                        if (destroyed.Count() > 0)
                        {
                            foreach (var b in destroyed)
                                _barriers.Remove(b);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    null, null, null, null, _playerCam.CurrentCameraTranslation);

            spriteBatch.Draw(_background, new
                Rectangle(0, 0, (int)_worldBound.X, (int)_worldBound.Y), Color.White);

            if (myConnection.State == ConnectionState.Connected)
            {
                if (player != null)
                {
                    player.DrawWithMessage(spriteBatch, font);
                    // Inform player moved here as Draw calls will be as quick as possible
                    if (player.position != oldPos)
                        proxy.Invoke("playerMoved", new object[] { (int)player.position.X, (int)player.position.Y }).Wait();
                }
                if (opponent != null)
                    opponent.DrawWithMessage(spriteBatch, font);
                if (playing)
                {
                    foreach (Barrier b in _barriers)
                        b.Draw(spriteBatch);
                    lock (gameLock)
                    {
                        foreach (Collectable c in _collectables)
                            c.Draw(spriteBatch);
                    }
                }
            }
            spriteBatch.End();
                // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }
    }
}
