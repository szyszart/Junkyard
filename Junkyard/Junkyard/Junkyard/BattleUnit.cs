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
        
        protected int acumulatedDmg = 0;

        public BattleUnit()
        {
            Stealth = false;
        }

        public BattleUnit(Simulation simulation, Player player)
        {
            Simulation = simulation;
            Player = player;
        }

        // Determines whether enemy units will see this unit as a hostile entity.
        // Set to false to represent an entity that is not a real battle unit (e.g. a projectile).
        public virtual bool Stealth { get; set; }

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

        public Infantry(Simulation simulation, Player player, Texture2D texture, Vector3 position)
            : base(simulation, player, texture, position)
        {            
        }

        public override void OnSpawn()
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
            if (Player != null)
            {
                Avatar.Flipped = (direction < 0);
            }

        }
    }

    class Ranged : AnimatedBattleUnit
    {
        public float AttackRange { get; set; }
        public float Speed { get; set; }

        protected float direction = 1.0f;
        protected bool dying = false;

        protected bool thrown = false;

        public Ranged()
        {
        }

        public Ranged(Simulation simulation, Player player, Texture2D texture, Vector3 position)
            : base(simulation, player, texture, position)
        {            
        }

        public override void OnSpawn()
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

                if (currentAnimation == "attack" && currentFrame == 7 && !thrown)
                {
                    Projectile projectile = (Projectile)Simulation.Spawn("menel_ranged_projectile");
                    projectile.Avatar.Position = this.Avatar.Position;
                    projectile.Gravity = 10.0f;
                    projectile.Player = this.Player;
                    projectile.Simulation = this.Simulation;
                    projectile.ProjectileHit += ProjectileHit;

                    if (foe != null)
                    {                        
                        projectile.SetTarget(foe);
                    }
                    else
                    {
                        projectile.Velocity = new Vector3(direction * 5.5f, 0.5f, 0.0f);
                    }

                    Simulation.Add(projectile);
                    thrown = true;
                }

                if (AnimationEnded)
                {
                    ResetAnimation();
                    thrown = false;
                }
            }
            base.OnTick(time);
        }

        protected void ProjectileHit(Projectile p)
        {
            if (p.Target != null)
                Simulation.Attack(p.Target, this);
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
            if (Player != null)
            {
                Avatar.Flipped = (direction < 0);
            }
        }
    }

    class Meteor : AnimatedBattleUnit
    {
        private static Random random = new Random();
        private const float MaxOffset = 4.0f;

        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public float Range { get; set; }
        protected bool dying = false;
        protected float direction;

        public Meteor()
        {
            Velocity = Vector3.Zero;
            
            // we don't want it to be treated as a battle unit
            Stealth = true;
        }

        public Meteor(Simulation simulation, Player player)
            : this()
        {
            Simulation = simulation;
            Player = player;
        }

        protected void AssertFacingTowardsFoe()
        {
            direction = Player.Direction;
            Avatar.Flipped = (direction < 0);
        }

        public override void OnSpawn()
        {
            AssertAnimation("fly");
            Avatar.Position += ((float)random.NextDouble() - 0.5f) * MaxOffset * Vector3.UnitX;
        }

        public override void OnTick(GameTime time)
        {
            AssertFacingTowardsFoe();
 
            float bottom = Avatar.Position.Y + Avatar.PixelScale.Y * Avatar.TexRect.Height;
            if (!dying && bottom <= Simulation.GroundLevel)
            {
                dying = true;
                BattleUnit foe = Simulation.GetNearestFoe(this);
                if (foe != null)
                {
                    float dist = Math.Abs(foe.Avatar.Position.X - Avatar.Position.X);
                    if (dist <= 2 * Range)
                    {
                        Simulation.Attack(foe, this);
                        Simulation.Attack(foe, this);
                    }
                }
            }

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
                AssertAnimation("fly");
                if (AnimationEnded)
                    ResetAnimation();

                float elapsedTime = time.ElapsedGameTime.Milliseconds / 1000.0f;
                Velocity += elapsedTime * Acceleration;
                Avatar.Position += elapsedTime * new Vector3(direction * Velocity.X, Velocity.Y, Velocity.Z);
            }

            base.OnTick(time);
        }
    }

    delegate void ProjectileHitHandler(Projectile sender);

    class Projectile : AnimatedBattleUnit
    {
        protected float direction = 1.0f;
        protected bool active = true;
        public float Gravity { get; set; }
        public Vector3 Velocity { get; set; }
        public BattleUnit Target { get; set; }        
        public event ProjectileHitHandler ProjectileHit;

        protected virtual void OnHit()
        {
            if (ProjectileHit != null)
                ProjectileHit(this);
        }

        public void SetTarget(BattleUnit u)
        {
            Target = u;
            float uWidth = u.Avatar.PixelScale.X * u.Avatar.TexRect.Width;

            if (u.Avatar.Flipped)
                uWidth = -uWidth;

            float xDistance = u.Avatar.Position.X - Avatar.Position.X + 2 * uWidth;
            SetTarget(xDistance);
        }

        public void SetTarget(float xDistance)
        {
            double angle = Math.PI * 0.25;
            float v = (float)Math.Sqrt((Math.Abs(xDistance) * Gravity) / Math.Sin(2 * angle));
            direction = Math.Sign(xDistance);
            Velocity = new Vector3(direction * v * (float)Math.Cos(angle), v * (float)Math.Sin(angle), 0);            
        }

        public Projectile()
        {
            Gravity = 10.0f;
            Velocity = Vector3.Zero;
            
            // we don't want it to be treated as a battle unit
            Stealth = true;
        }

        public Projectile(Simulation simulation, Player player)
            : this()
        {
            Simulation = simulation;
            Player = player;
        }

        public override void OnSpawn()
        {
            AssertAnimation("fly");
        }

        public override void OnTick(GameTime time)
        {
            AssertFacingTowardsFoe();
            AssertAnimation("fly");
            if (AnimationEnded)
                ResetAnimation();

            float elapsedTime = time.ElapsedGameTime.Milliseconds / 1000.0f;

            Velocity += elapsedTime * new Vector3(0, -Gravity, 0);
            Avatar.Position += elapsedTime * Velocity;

            if (active && Avatar.Position.Y < Simulation.GroundLevel)            
            {
                OnHit();
                active = false;                
            }

            if (Avatar.Position.Y < -5.0f)
                Simulation.Remove(this);

            base.OnTick(time);
        }

        protected void AssertFacingTowardsFoe()
        {
            direction = Player.Direction;
            if (Player != null)
            {
                Avatar.Flipped = (direction < 0);
            }
        }
    }
}