using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    #region Some generic UI stuff

    public abstract class Widget
    {
        #region Private fields

        protected ContentManager contentManager;
        protected SpriteBatch spriteBatch;

        #endregion
        #region Properties

        public GameScreen Screen { get; protected set; }

        #endregion
        #region Ctors

        public Widget(GameScreen screen, ContentManager content)
        {
            Screen = screen;
            spriteBatch = screen.ScreenManager.SpriteBatch;
            contentManager = content;
            LoadContent();
        }

        #endregion
        #region Public methods

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        #endregion
        #region Protected methods

        protected virtual void LoadContent()
        {
        }

        #endregion
    }

    #endregion
    #region Puzzle board widget

    public delegate void LayoutMatchesHandler(Widget source, LayoutInstance layout);

    public delegate void LayoutAcceptedHandler(Widget source, LayoutInstance layout);

    internal class PuzzleBoardWidget : Widget
    {
        #region Constants

        private const int LayoutsEnabledRows = 2;
        private const int tilePaddingX = 8, tilePaddingY = 8;

        #endregion
        #region Private fields

        public List<LayoutInstance> LayoutsMatched;
        private Texture2D background;
        private PuzzleBoard board;
        private Texture2D border;
        private Point currentTile;
        private int elapsedTime;

        private float fadeOut = 1.0f;
        private int fadeOutElapsedTime;
        private float fadeOutSpeed = 0.02f;
        private int fadeOutTime = 20;
        private Texture2D fire, grog;
        private bool isFadingOut;
        private bool isSomethingSelected;

        private Point leftTop = new Point(54, 54);
        private Texture2D metal;

        private float minSelectedScale = 0.8f, scaleDiff = 0.01f;
        private Texture2D selected;
        private int selectedBounceTime = 20;
        private float selectedScale = 1.0f;
        private Point selectedTile;
        private Point tileSize = new Point(101, 101);
        private Texture2D wood;

        #endregion
        #region Properties

        public PuzzleBoard Board
        {
            get { return board; }
            set
            {
                board = value;
                Refresh();
            }
        }

        public Color CurrentColor { get; set; }
        public Point Dimensions { get; set; }
        public BoardLayoutFinder LayoutFinder { get; set; }
        public Point Position { get; set; }
        public Color SelectedColor { get; set; }

        public Point SelectedTile
        {
            get { return selectedTile; }
            set { selectedTile = value; }
        }

        #endregion
        #region Events

        public event LayoutAcceptedHandler LayoutAccepted;
        public event LayoutMatchesHandler LayoutMatches;

        #endregion
        #region Ctors

        public PuzzleBoardWidget(GameScreen screen, ContentManager content, Point position, Point dimensions)
            : base(screen, content)
        {
            LayoutsMatched = new List<LayoutInstance>();

            Position = position;
            Dimensions = dimensions;
            currentTile = new Point(0, 0);
            isSomethingSelected = false;
            SelectedColor = Color.Red;
            CurrentColor = Color.Yellow;
        }

        #endregion
        #region Overrides

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // scale and translate the widget
            Matrix scaleMatrix = Matrix.CreateScale((float) (Dimensions.X)/background.Width,
                                                    (float) (Dimensions.Y)/background.Height, 1)
                                 *Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, scaleMatrix);

            spriteBatch.Draw(background, new Rectangle(0, 0, background.Width, background.Height), Color.White);

            if (Board != null)
            {
                for (int y = 0; y < Board.Dimensions.Y; y++)
                    for (int x = 0; x < Board.Dimensions.X; x++)
                    {
                        Texture2D tileTexture = null;
                        switch (Board.Blocks[x, y])
                        {
                            case BlockType.Fire:
                                tileTexture = fire;
                                break;
                            case BlockType.Grog:
                                tileTexture = grog;
                                break;
                            case BlockType.Metal:
                                tileTexture = metal;
                                break;
                            case BlockType.Wood:
                                tileTexture = wood;
                                break;
                        }
                        if (tileTexture != null)
                        {
                            var rect = new Rectangle(
                                leftTop.X + x*(tileSize.X + tilePaddingX),
                                leftTop.Y + y*(tileSize.Y + tilePaddingY),
                                tileSize.X,
                                tileSize.Y
                                );
                            Color color = Color.White;
                            if (isSomethingSelected && SelectedTile.X == x && selectedTile.Y == y)
                                color = SelectedColor;
                            else if (currentTile.X == x && currentTile.Y == y)
                                color = CurrentColor;

                            spriteBatch.Draw(tileTexture, rect, color*fadeOut);
                        }
                    }

                // draw the backgrounds for each matched layout
                foreach (LayoutInstance i in LayoutsMatched)
                {
                    Texture2D sprites = i.Layout.Thumbnails;
                    if (sprites == null)
                        continue;

                    Point corner = i.Location;
                    Point size = i.Layout.BlockSize;

                    int h = (i.Layout.Top == null) ? 1 : 2;
                    int w = i.Layout.Bottom.Length;

                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                        {
                            Point p = i.Layout.ThumbnailBlocks[w*y + x];
                            var srcRect = new Rectangle(p.X*size.X, p.Y*size.Y, size.X, size.Y);
                            var destRest = new Rectangle(
                                leftTop.X + (corner.X + x)*(tileSize.X + tilePaddingX),
                                leftTop.Y + (corner.Y + y)*(tileSize.Y + tilePaddingY),
                                tileSize.X,
                                tileSize.Y
                                );
                            spriteBatch.Draw(sprites, destRest, srcRect, Color.White*fadeOut);
                        }
                }


                // mark the selected tile
                var selectedRect = new Rectangle(
                    leftTop.X + currentTile.X*(tileSize.X + tilePaddingX) +
                    (int) (tileSize.X - selected.Width*selectedScale)/2,
                    leftTop.Y + currentTile.Y*(tileSize.Y + tilePaddingY) +
                    (int) (tileSize.Y - selected.Height*selectedScale)/2,
                    (int) (selected.Width*selectedScale),
                    (int) (selected.Height*selectedScale)
                    );
                spriteBatch.Draw(selected, selectedRect, SelectedColor);
            }

            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime >= selectedBounceTime)
            {
                elapsedTime -= selectedBounceTime;

                selectedScale += scaleDiff;
                if (selectedScale < minSelectedScale)
                {
                    selectedScale = minSelectedScale;
                    scaleDiff = -scaleDiff;
                }
                else if (selectedScale > 1.0)
                {
                    selectedScale = 1.0f;
                    scaleDiff = -scaleDiff;
                }
            }
            spriteBatch.Draw(border, new Rectangle(0, 0, border.Width, border.Height), Color.White);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isFadingOut)
            {
                fadeOutElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (fadeOutElapsedTime >= fadeOutTime)
                {
                    fadeOut -= fadeOutSpeed;
                    if (fadeOut <= 0.0f)
                    {
                        isFadingOut = false;

                        Board.Randomize();
                        LayoutsMatched.Clear();
                        Refresh();

                        fadeOut = 1.0f;
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            background = contentManager.Load<Texture2D>("Images/GUI/board_bg");
            border = contentManager.Load<Texture2D>("Images/GUI/board_border");
            selected = contentManager.Load<Texture2D>("Images/GUI/selected");
            wood = contentManager.Load<Texture2D>("Images/GUI/meat01");
            metal = contentManager.Load<Texture2D>("Images/GUI/aluminium");
            fire = contentManager.Load<Texture2D>("Images/GUI/fire");
            grog = contentManager.Load<Texture2D>("Images/GUI/grog");
        }

        #endregion
        #region Public methods

        public void Accept()
        {
            if (LayoutsMatched.Count > 0)
            {
                foreach (LayoutInstance i in LayoutsMatched)
                {
                    LayoutAccepted(this, i);
                }
                LayoutsMatched.Clear();
                Board.MoveDown(2);
                Refresh();
            }
        }

        public void Down()
        {
            if (currentTile.Y < Board.Dimensions.Y - 1)
                currentTile.Y++;
        }

        public void Left()
        {
            if (currentTile.X > 0)
                currentTile.X--;
        }

        public void Randomize()
        {
            if (!isSomethingSelected)
            {
                fadeOutElapsedTime = 0;
                isFadingOut = true;
            }
        }

        public void Right()
        {
            if (currentTile.X < Board.Dimensions.X - 1)
                currentTile.X++;
        }

        public void Select()
        {
            if (Board.Blocks[currentTile.X, currentTile.Y] == BlockType.Empty)
                return;

            if (isSomethingSelected)
            {
                Board.Swap(selectedTile, currentTile);
                isSomethingSelected = false;
                Refresh();
            }
            else if (!isFadingOut)
            {
                SelectedTile = currentTile;
                isSomethingSelected = true;
            }
        }

        public void Up()
        {
            if (currentTile.Y > 0)
                currentTile.Y--;
        }

        #endregion
        #region Protected methods

        protected void Refresh()
        {
            // try to match a known layout
            if (LayoutFinder != null)
            {
                IEnumerable<LayoutInstance> layouts = LayoutFinder.Scan(Board);
                LayoutsMatched.AddRange(layouts);

                if (LayoutMatches != null)
                {
                    foreach (LayoutInstance i in layouts)
                    {
                        LayoutMatches(this, i);
                    }
                }
            }
        }

        #endregion
    }

    #endregion
}