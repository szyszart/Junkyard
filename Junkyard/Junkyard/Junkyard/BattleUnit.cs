using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Junkyard
{
    class Simulation
    {
        public bool EnemyInRange(BattleUnit unit, float range)
        {
            // implement me!
            return false;
        }
    }

    class AnimationData
    {        
        public SpriteSheetDimensions Dimensions { get; set; }
        public Texture2D SpriteSheet { get; set; }
        public AnimationData(SpriteSheetDimensions dimensions, Texture2D sheet)
        {            
            Dimensions = dimensions;
            SpriteSheet = sheet;
        }
    }    

    class BattleUnitSkin
    {
        protected Dictionary<string, AnimationData> animations;
        
        public BattleUnitSkin()
        {
            animations = new Dictionary<string, AnimationData>();
        }

        public AnimationData GetAnimation(string name)
        {
            return animations[name];
        }

        public void SetAnimation(string name, AnimationData data)
        {
            animations[name] = data;
        }
    }

    abstract class BattleUnit
    {
        public float HP { get; set; }
        public BattleUnit()
        {
            HP = 100.0f;
        }
    }

    class SimpleBattleUnit : BattleUnit
    {
        class StateDescription
        {
            public string PreAnim;
            public string LoopAnim;
            public string PostAnim;
            public StateDescription(string pre, string loop, string post)
            {
                PreAnim = pre;
                LoopAnim = loop;
                PostAnim = post;
            }
        }

        static private Dictionary<SimpleBattleUnitState, StateDescription> stateDescriptions = new Dictionary<SimpleBattleUnitState, StateDescription>() 
        {
            { SimpleBattleUnitState.Idle, new StateDescription(null, "idle", null) },
            { SimpleBattleUnitState.Walk, new StateDescription("prewalk", "walk", "postwalk") },
            { SimpleBattleUnitState.Attack, new StateDescription(null, "attack", null) },
            { SimpleBattleUnitState.Die, new StateDescription(null, "die", null) }
        };

        public const float MeleeRange = 0.5f;
        public enum SimpleBattleUnitState
        {
            Idle,
            Walk,
            Attack,
            Die,
            Done
        }
        protected SimpleBattleUnitState state = SimpleBattleUnitState.Idle;
        protected SimpleBattleUnitState next;
        public FrameSprite3D Avatar { get; protected set; }

        public BattleUnitSkin Skin { get; protected set; }
        public Simulation Simulation { get; protected set; }
        protected Vector3 Destination { get; set; }
        protected Vector3 Direction { get; set; }

        protected AnimationData currentAnimation, nextAnimation;

        public SimpleBattleUnit(Simulation simulation, BattleUnitSkin skin, Vector3 position, Vector3 dest)
        {
            Simulation = simulation;
            Skin = skin;
            Destination = dest;
            Direction = (Destination - position);
            Direction.Normalize();

            nextAnimation = currentAnimation = skin.GetAnimation("walk");
            Avatar = new FrameSprite3D(currentAnimation.SpriteSheet, position, currentAnimation.Dimensions);
            Avatar.Reset();
        }        

        protected bool HasNotYetArrived
        {
            get
            {
                return (Vector3.Distance(Avatar.Position, Destination) > 0.1f);
            }
        }

        public void Update(GameTime gameTime)
        {          
            Avatar.NextFrame();
            if (Avatar.AnimationFinished)
            {
                if (currentAnimation != nextAnimation)
                {
                    currentAnimation = nextAnimation;
                    Avatar.Texture = currentAnimation.SpriteSheet;
                    Avatar.GridDimensions = currentAnimation.Dimensions;                    
                }
                Avatar.Reset();
            }
        }        
    }

}