-- Menel robot
local base_path = "Images/Units/Menele/Junkie/"

local animations = {
    walk = {
        fps = 16,
        spritesheet = base_path .. "walk",
		frames = {
			{ x = 0, y = 0, width = 339, height = 430, offsetx = 0, offsety = 0 },
			{ x = 339, y = 0, width = 369, height = 427, offsetx = 7, offsety = 3 },
			{ x = 708, y = 0, width = 369, height = 431, offsetx = 1, offsety = 6 },
			{ x = 1077, y = 0, width = 361, height = 431, offsetx = 7, offsety = 3 },
		}
    },
    
    attack = {
        fps = 16,
        spritesheet = base_path .. "attack",
		frames = {
			{ x = 0, y = 0, width = 339, height = 457, offsetx = 0, offsety = 0 },
			{ x = 339, y = 0, width = 382, height = 437, offsetx = 7, offsety = 20 },
			{ x = 721, y = 0, width = 432, height = 387, offsetx = 1, offsety = 77 },
			{ x = 1153, y = 0, width = 361, height = 450, offsetx = 7, offsety = 11 },
		}        
    },
    
    dying = {
        fps = 12,
        spritesheet = base_path .. "dying",
		frames = {
			{ x = 0, y = 0, width = 310, height = 445, offsetx = 0, offsety = 0 },
			{ x = 310, y = 0, width = 332, height = 429, offsetx = 88, offsety = 0 },
			{ x = 642, y = 0, width = 548, height = 372, offsetx = 0, offsety = 0 },
			{ x = 1190, y = 0, width = 653, height = 363, offsetx = 0, offsety = 0 },
			{ x = 1843, y = 0, width = 698, height = 367, offsetx = 0, offsety = 0 },
			{ x = 0, y = 445, width = 703, height = 367, offsetx = 0, offsety = 0 },
			{ x = 703, y = 445, width = 703, height = 367, offsetx = 0, offsety = 0 },
			{ x = 1406, y = 445, width = 703, height = 367, offsetx = 0, offsety = 0 },
		}
    }
}

local unit = {
	name = "menel_junkie",
	class = "infantry",
	animations = animations,

	-- kind-specific parameters
	initial_hp = 50,
	speed = 0.08,
	attack_range = 1.2,
}

return unit