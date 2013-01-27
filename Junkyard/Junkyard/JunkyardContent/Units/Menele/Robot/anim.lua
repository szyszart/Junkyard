-- Menel robot
local base_path = "Images/Units/Menele/Robot/"

local animations = {
    walk = {
        fps = 12,
        spritesheet = base_path .. "walk",
        frames = {
        	{ x = 0, y = 0, width = 665, height = 1091, offsetx = 0, offsety = 0 },
        	{ x = 665, y = 0, width = 656, height = 1088, offsetx = 7, offsety = 8 },
        	{ x = 1321, y = 0, width = 647, height = 1101, offsetx = 25, offsety = 14 },
        	{ x = 0, y = 1101, width = 639, height = 1117, offsetx = 23, offsety = 8 },
        	{ x = 639, y = 1101, width = 586, height = 1114, offsetx = 80, offsety = 0 },
        	{ x = 1225, y = 1101, width = 609, height = 1117, offsetx = 65, offsety = 8 },
        	{ x = 0, y = 2218, width = 782, height = 1101, offsetx = -49, offsety = 14 },
        	{ x = 782, y = 2218, width = 688, height = 1094, offsetx = -23, offsety = 8 },
        }
    },
    
    attack = {
        fps = 14,
        spritesheet = base_path .. "attack",
        frames = {
        	{ x = 0, y = 0, width = 672, height = 1122, offsetx = 0, offsety = 0 },
        	{ x = 672, y = 0, width = 749, height = 1066, offsetx = 26, offsety = 0 },
        	{ x = 1421, y = 0, width = 1022, height = 986, offsetx = 35, offsety = 0 },
        	{ x = 0, y = 1122, width = 854, height = 1060, offsetx = 34, offsety = 0 },
        	{ x = 854, y = 1122, width = 900, height = 1065, offsetx = 70, offsety = 0 },
        	{ x = 1754, y = 1122, width = 1104, height = 783, offsetx = 42, offsety = 0 },
        	{ x = 0, y = 2187, width = 1090, height = 702, offsetx = 22, offsety = 0 },
        	{ x = 1090, y = 2187, width = 1102, height = 683, offsetx = 34, offsety = 0 },
        	{ x = 2192, y = 2187, width = 1034, height = 698, offsetx = 46, offsety = 0 },
        	{ x = 0, y = 2889, width = 945, height = 814, offsetx = 62, offsety = 0 },
        	{ x = 945, y = 2889, width = 871, height = 910, offsetx = 78, offsety = 0 },
        	{ x = 1816, y = 2889, width = 763, height = 1043, offsetx = 78, offsety = 0 },
        }
    },
    
    dying = {
        fps = 14,
        spritesheet = base_path .. "dying",
        frames = {
        	{ x = 0, y = 0, width = 672, height = 1122, offsetx = 0, offsety = 0 },
        	{ x = 672, y = 0, width = 719, height = 1032, offsetx = -2, offsety = 0 },
        	{ x = 1391, y = 0, width = 990, height = 936, offsetx = -122, offsety = 0 },
        	{ x = 0, y = 1122, width = 1072, height = 693, offsetx = -92, offsety = 0 },
        	{ x = 1072, y = 1122, width = 1172, height = 556, offsetx = -88, offsety = 0 },
        	{ x = 2244, y = 1122, width = 1288, height = 386, offsetx = -132, offsety = 0 },
        	{ x = 0, y = 1815, width = 1295, height = 370, offsetx = -96, offsety = 0 },
        }
    }
}

local unit = {
	name = "menel_robot",
	class = "infantry",
	animations = animations,

	-- kind-specific parameters
	initial_hp = 300,
	speed = 0.005,
	attack_range = 2.6,
}

return unit