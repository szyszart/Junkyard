using LuaInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class LuaMachine
    {
        #region Private fields

        public static Lua Instance = new Lua();

        #endregion
        #region Public static methods

        public static Sprite3D LoadAsset(LuaTable el, ContentManager content)
        {
            var texture = content.Load<Texture2D>((string) el["assetName"]);

            var pos = el["pos"] as LuaTable;
            var ypr = el["yawpitchroll"] as LuaTable;
            var scale = el["scale"] as LuaTable;
            if (pos == null || ypr == null || scale == null)
            {
                throw new ContentLoadException();
            }
            var asset = new Sprite3D(
                texture,
                null,
                TableToVector(pos),
                Quaternion.CreateFromYawPitchRoll((float) (double) ypr[1], (float) (double) ypr[2],
                                                  (float) (double) ypr[3]),
                TableToVector(scale)
                );

            var normalMap = el["normalMap"];
            if (normalMap != null)
                asset.NormalMap = content.Load<Texture2D>((string) normalMap);

            return asset;
        }

        public static Point LuaTableToPoint(LuaTable tbl)
        {
            return new Point((int) (double) tbl[1], (int) (double) tbl[2]);
        }

        public static Vector3 TableToVector(LuaTable tbl)
        {
            return new Vector3((float) (double) tbl[1], (float) (double) tbl[2], (float) (double) tbl[3]);
        }

        #endregion
    }
}