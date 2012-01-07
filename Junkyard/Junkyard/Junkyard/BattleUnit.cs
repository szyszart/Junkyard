using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Junkyard
{ 
    public class BattleUnit : IDrawable
    {
        public Simulation Simulation { get; set; }
        public Player Player { get; set; }

        public int Hp { get; set; }
        public bool reallyDead = false;

        public ScaledSprite3D Avatar { get; protected set; }

        // only for DEBUGGING PURPOSES        
        protected int acumulatedDmg = 0;

        public BattleUnit()
        {
        }

        public BattleUnit(Simulation simulation, Player player)
        {
            Simulation = simulation;
            Player = player;
        }

        public virtual void OnSpawn() { }
        public virtual void OnTick(GameTime time) { }
        public virtual void OnDamage(int dmg) { acumulatedDmg += dmg; }
        public virtual void OnDeath() { }
        public virtual void OnRemove() { }

        public virtual void Draw(Effect effect) { }
    }

    public class AnimatedBattleUnit : BattleUnit
    {
        public Dictionary<string, Animation> Animations = new Dictionary<string,Animation>();
               
        protected string currentAnimation;
        protected int currentFrame = -1;
        protected float frameDuration;
        protected float timeElapsed = 0;

        public AnimatedBattleUnit()
        {
            Avatar = new ScaledSprite3D();
        }
        
        public AnimatedBattleUnit(Simulation simulation, Player player, Texture2D texture, Vector3 position)
            : base(simulation, player) 
        {
            Avatar = new ScaledSprite3D(texture, position);
            Avatar.Position = position;
        }

        public override void OnTick(GameTime time)
        {
            timeElapsed += time.ElapsedGameTime.Milliseconds;
            if (timeElapsed >= frameDuration)
            {
                timeElapsed -= frameDuration;
                NextFrame();
            }

            base.OnTick(time);
        }

        protected void NextFrame()
        {
            if (!AnimationEnded)
                currentFrame++;
        }

        protected void AssertAnimation(string name)
        {
            if (currentAnimation != name)
            {
                Avatar.Texture = Animations[name].SpriteSheet;
                ResetAnimation();
                currentAnimation = name;
                frameDuration = (1000.0f / Animations[currentAnimation].FramesPerSecond);
                timeElapsed = 0;
            }
        }

        protected void ResetAnimation()
        {
            currentFrame = 0;
        }

        protected bool AnimationEnded
        {
            get
            {
                if (currentAnimation != null && Animations.ContainsKey(currentAnimation))
                    return (currentFrame >= Animations[currentAnimation].FrameCount);
                else
                    return true;                
            }
        }

        public override void Draw(Effect effect)
        {
            if (currentFrame < Animations[currentAnimation].FrameCount)
            {
                Avatar.TexRect = Animations[currentAnimation].Frames[currentFrame].Rectangle;
                Avatar.Offset = Animations[currentAnimation].Frames[currentFrame].Offset;
            }
            Avatar.Draw(effect);
        }
    }

    public class Infantry : AnimatedBattleUnit
    {       
        public float AttackRange { get; set; }
        public float Speed { get; set; }

        protected float direction = 1.0f;                
        protected bool dying = false;

        public Infantry()
        {
        }

        public Infantry(Simulation simulation, Player player, Texture2D texture, Vector3 position) : base(simulation, player, texture, position)
        {
            AssertAnimation("walk");            
        }

        public override void OnTick(GameTime time)
        {            
            AssertFacingTowardsFoe();

            if (dying)
            {
                AssertAnimation("dying");
                if (AnimationEnded)
                {
                    reallyDead = true;
                }
            }
            else
            {
                BattleUnit foe = Simulation.GetNearestFoe(this);
                float dist = (foe != null) ? Vector3.Distance(foe.Avatar.Position, Avatar.Position) : float.PositiveInfinity;
                if (foe != null && dist <= AttackRange && Math.Sign(direction) * (foe.Avatar.Position.X - Avatar.Position.X) >= 0.25 * AttackRange)
                {
                    AssertAnimation("attack");
                }
                else if (currentAnimation != "attack" || AnimationEnded)
                {
                    AssertAnimation("walk");
                    Move();
                }

                Hp -= acumulatedDmg;
                acumulatedDmg = 0;

                if (Hp <= 0)
                {
                    dying = true;
                }

                if (currentAnimation == "attack" && AnimationEnded)
                {
                    if (foe != null)
                    {
                        Simulation.Attack(foe, this);
                    }
                }

                if (AnimationEnded)
                    ResetAnimation();
            }
            base.OnTick(time);
        }

        public override void OnDeath()
        {
            AssertAnimation("death");
            dying = true;
        }

        protected void Move()
        {            
            Avatar.Position += direction * Speed * Vector3.UnitX;
        }

        protected void AssertFacingTowardsFoe()
        {
            direction = Player.Direction;
            if (Player != null) {
                Avatar.Flipped = (direction < 0);
            }
            
        }
    }

}