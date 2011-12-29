-- Menel robot
local base_path = "Images/Units/Nuklearni/Infantry/"

local animations = {
    walk = {
        fps = 16,
        spritesheet = base_path .. "walk",
		frames = {
			{ x = 0, y = 0, width = 327, height = 433, offsetx = 0, offsety = 0 },
			{ x = 327, y = 0, width = 315, height = 427, offsetx = 11, offsety = -2 },
			{ x = 642, y = 0, width = 331, height = 432, offsetx = 0, offsety = -6 },
			{ x = 973, y = 0, width = 317, height = 427, offsetx = 11, offsety = -2 },
			{ x = 1290, y = 0, width = 327, height = 433, offsetx = 0, offsety = 0 },
			{ x = 0, y = 433, width = 315, height = 427, offsetx = 11, offsety = -2 },
			{ x = 315, y = 433, width = 331, height = 432, offsetx = 0, offsety = -6 },
			{ x = 646, y = 433, width = 317, height = 427, offsetx = 11, offsety = -2 },
		}
    },
    
    attack = {
        fps = 14,
        spritesheet = base_path .. "attack",
		frames = {
			{ x = 0, y = 0, width = 342, height = 419, offsetx = 0, offsety = 0 },
			{ x = 342, y = 0, width = 365, height = 400, offsetx = 0, offsety = 0 },
			{ x = 707, y = 0, width = 339, height = 380, offsetx = 0, offsety = 0 },
			{ x = 1046, y = 0, width = 310, height = 408, offsetx = 49, offsety = 0 },
			{ x = 1356, y = 0, width = 290, height = 408, offsetx = 49, offsety = 0 },
			{ x = 0, y = 419, width = 284, height = 408, offsetx = 49, offsety = 0 },
			{ x = 284, y = 419, width = 286, height = 380, offsetx = 46, offsety = 0 },
			{ x = 570, y = 419, width = 349, height = 407, offsetx = 25, offsety = 0 },
			{ x = 919, y = 419, width = 283, height = 481, offsetx = 87, offsety = 0 },
			{ x = 1202, y = 419, width = 410, height = 538, offsetx = 220, offsety = 0 },
			{ x = 0, y = 957, width = 515, height = 408, offsetx = 221, offsety = 0 },
			{ x = 515, y = 957, width = 534, height = 412, offsetx = 221, offsety = 0 },
			{ x = 1049, y = 957, width = 511, height = 416, offsetx = 221, offsety = 0 },
			{ x = 1560, y = 957, width = 472, height = 427, offsetx = 221, offsety = 0 },
			{ x = 2032, y = 957, width = 341, height = 433, offsetx = 183, offsety = 0 },
			{ x = 0, y = 1390, width = 345, height = 444, offsetx = 0, offsety = 0 },
			{ x = 345, y = 1390, width = 342, height = 427, offsetx = 0, offsety = 0 },
			{ x = 687, y = 1390, width = 342, height = 427, offsetx = 0, offsety = 0 },
			{ x = 1029, y = 1390, width = 342, height = 423, offsetx = 0, offsety = 0 },
			{ x = 1371, y = 1390, width = 342, height = 419, offsetx = 0, offsety = 0 },
			{ x = 0, y = 1834, width = 342, height = 419, offsetx = 0, offsety = 0 },
			{ x = 342, y = 1834, width = 342, height = 419, offsetx = 0, offsety = 0 },
		}
    },
    
    dying = {
        fps = 14,
        spritesheet = base_path .. "dying",
		frames = {
			{ x = 0, y = 0, width = 342, height = 419, offsetx = 0, offsety = 0 },
			{ x = 342, y = 0, width = 342, height = 426, offsetx = 0, offsety = 0 },
			{ x = 684, y = 0, width = 371, height = 433, offsetx = 0, offsety = 0 },
			{ x = 1055, y = 0, width = 342, height = 433, offsetx = 0, offsety = 0 },
			{ x = 1397, y = 0, width = 342, height = 433, offsetx = 0, offsety = 0 },
			{ x = 0, y = 433, width = 343, height = 424, offsetx = 0, offsety = 0 },
			{ x = 343, y = 433, width = 342, height = 408, offsetx = 0, offsety = 0 },
			{ x = 685, y = 433, width = 353, height = 359, offsetx = 0, offsety = 0 },
			{ x = 1038, y = 433, width = 330, height = 291, offsetx = 49, offsety = 0 },
			{ x = 1368, y = 433, width = 306, height = 304, offsetx = 48, offsety = 0 },
			{ x = 0, y = 857, width = 298, height = 305, offsetx = 46, offsety = 0 },
			{ x = 298, y = 857, width = 303, height = 305, offsetx = 46, offsety = 0 },
			{ x = 601, y = 857, width = 311, height = 305, offsetx = 46, offsety = 0 },
		}
    }
}

local unit = {
	name = "nuklearni_infantry",
	class = "infantry",
	animations = animations,	

	-- kind-specific parameters
	initial_hp = 100,
	speed = 0.01,
	attack_range = 1.5,
}

return unit