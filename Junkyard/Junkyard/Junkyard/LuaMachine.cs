using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using LuaInterface;

namespace Junkyard
{
    public class LuaMachine
    {
        public static Lua state = new Lua();

        public static Vector3 TableToVector(LuaInterface.LuaTable tbl)
        {
            return new Vector3((float)(double)tbl[1], (float)(double)tbl[2], (float)(double)tbl[3]);
        }
    }
}
