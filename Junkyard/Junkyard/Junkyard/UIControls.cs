﻿using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Junkyard
{
    public abstract class Widget
    {
        public GameScreen Screen { get; protected set; }
        protected ContentManager contentManager;
        protected SpriteBatch spriteBatch;
        public Widget(GameScreen screen, ContentManager content)
        {
            Screen = screen;
            spriteBatch = screen.ScreenManager.SpriteBatch;
            contentManager = content;
            LoadContent();
        }

        protected virtual void LoadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }

    public delegate void LayoutMatchesHandler(Widget source, string layoutName);

    class PuzzleBoardWidget : Widget
    {
        public BoardLayoutFinder LayoutFinder { get; set; }
        public Color CurrentColor { get; set; }
        public Color SelectedColor { get; set; }
        public Point SelectedTile {
            get
            {
                return selectedTile;
            }
            set
            {
                selectedTile = value;
            }
        }
        private bool isSomethingSelected;
        private Point currentTile;
        private Point selectedTile;

        public Point Position { get; set; }
        public Point Dimensions { get; set; }
        private Texture2D background, border, selected;
        private Texture2D wood, metal, fire, grog;
        public PuzzleBoard Board { get; set; }

        private const int tilePaddingX = 8, tilePaddingY = 8;
        private Point tileSize = new Point(101, 101);
        private Point leftTop = new Point(54, 54);

        private float selectedScale = 1.0f, minSelectedScale = 0.8f, scaleDiff = 0.01f;
        private int selectedBounceTime = 20, elapsedTime = 0;

        private float fadeOut = 1.0f, fadeOutSpeed = 0.02f;
        private int fadeOutTime = 20, fadeOutElapsedTime = 0;
        private bool isFadingOut = false;

        public event LayoutMatchesHandler LayoutMatches;

        public void Left()
        {
            if (currentTile.X > 0)
                currentTile.X--;
        }

        public void Right()
        {
            if (currentTile.X < Board.Dimensions.X - 1)
                currentTile.X++;
        }

        public void Up()
        {
            if (currentTile.Y > 0)
                currentTile.Y--;
        }

        public void Down()
        {
            if (currentTile.Y < Board.Dimensions.Y - 1)
                currentTile.Y++;
        }

        public void Select()
        {
            if (Board.Blocks[currentTile.X, currentTile.Y] == BlockType.Empty)
                return;

            if (isSomethingSelected)
            {
                Board.Swap(selectedTile, currentTile);                
                isSomethingSelected = false;

                // try to match a known layout
                if (LayoutFinder != null)
                {
                    var layouts = LayoutFinder.Scan(Board);
                    if (LayoutMatches != null)
                    {
                        foreach (string layoutName in layouts)
                        {
                            LayoutMatches(this, layoutName);
                        }
                    }
                }

            }
            else if (!isFadingOut)
            {
                SelectedTile = currentTile;
                isSomethingSelected = true;
            }
        }

        public void Randomize()
        {
            if (!isSomethingSelected)
            {
                fadeOutElapsedTime = 0;
                isFadingOut = true;
            }
        }        

        public PuzzleBoardWidget(GameScreen screen, ContentManager content, Point position, Point dimensions) : base(screen, content)
        {            
            Position = position;            
            Dimensions = dimensions;
            currentTile = new Point(0, 0);
            isSomethingSelected = false;
            SelectedColor = Color.Red;
            CurrentColor = Color.Yellow;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            background = contentManager.Load<Texture2D>("Images/GUI/board_bg");
            border = contentManager.Load<Texture2D>("Images/GUI/board_border");
            selected = contentManager.Load<Texture2D>("Images/GUI/selected");
            wood = contentManager.Load<Texture2D>("Images/GUI/wood");
            metal = contentManager.Load<Texture2D>("Images/GUI/aluminium");
            fire = contentManager.Load<Texture2D>("Images/GUI/fire");
            grog = contentManager.Load<Texture2D>("Images/GUI/grog");            
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
                        fadeOut = 1.0f;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // scale and translate the widget
            Matrix scaleMatrix = Matrix.CreateScale((float)(Dimensions.X) / background.Width, (float)(Dimensions.Y) / background.Height, 1) 
                * Matrix.CreateTranslation(new Vector3((float)Position.X, (float)Position.Y, 0));
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
                            Rectangle rect = new Rectangle(
                                leftTop.X + x * (tileSize.X + tilePaddingX),
                                leftTop.Y + y * (tileSize.Y + tilePaddingY), 
                                tileSize.X, 
                                tileSize.Y
                            );
                            Color color = Color.White;
                            if (isSomethingSelected && SelectedTile.X == x && selectedTile.Y == y)
                                color = SelectedColor;
                            else if (currentTile.X == x && currentTile.Y == y)
                                color = CurrentColor;

                            spriteBatch.Draw(tileTexture, rect, color * fadeOut);
                        }
                    }
                
                // mark the selected tile
                Rectangle selectedRect = new Rectangle(
                    leftTop.X + currentTile.X * (tileSize.X + tilePaddingX) + (int)(tileSize.X - selected.Width * selectedScale) / 2,
                    leftTop.Y + currentTile.Y * (tileSize.Y + tilePaddingY) + (int)(tileSize.Y - selected.Height * selectedScale) / 2,
                    (int)(selected.Width * selectedScale),
                    (int)(selected.Height * selectedScale)
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
    }
}