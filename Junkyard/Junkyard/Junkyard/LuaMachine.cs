using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using LuaInterface;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class LuaMachine
    {
        public static Lua Instance = new Lua();

        public static Point LuaTableToPoint(LuaTable tbl)
        {
            return new Point((int)(double)tbl[1], (int)(double)tbl[2]);
        }

        public static Vector3 TableToVector(LuaInterface.LuaTable tbl)
        {
            return new Vector3((float)(double)tbl[1], (float)(double)tbl[2], (float)(double)tbl[3]);
        }

        public static Sprite3D LoadAsset(LuaTable el, ContentManager content)
        {
            Texture2D texture =  content.Load<Texture2D>((string)el["assetName"]);
                           
            var pos = el["pos"] as LuaTable;
            var ypr = el["yawpitchroll"] as LuaTable;
            var scale = el["scale"] as LuaTable;
            var asset = new Sprite3D(
                texture,
                null,
                TableToVector(pos),
                Quaternion.CreateFromYawPitchRoll((float)(double)ypr[1], (float)(double)ypr[2], (float)(double)ypr[3]),
                TableToVector(scale)
                );

            var normalMap = el["normalMap"];
            if (normalMap != null)
                asset.NormalMap = content.Load<Texture2D>((string)normalMap);

            return asset;            
        }
    }
}
