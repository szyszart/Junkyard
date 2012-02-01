-- Menel strzelec
local base_path = "Images/Units/Menele/Ranged/"

local animations = {
    walk = {
        fps = 16,
        spritesheet = base_path .. "walk",
		frames = {
			{ x = 0, y = 0, width = 232, height = 498, offsetx = 0, offsety = 0 },
			{ x = 232, y = 0, width = 254, height = 489, offsetx = 3, offsety = 11 },
			{ x = 486, y = 0, width = 306, height = 478, offsetx = -23, offsety = 20 },
			{ x = 792, y = 0, width = 253, height = 488, offsetx = 3, offsety = 11 },
			{ x = 1045, y = 0, width = 232, height = 499, offsetx = 0, offsety = 0 },
			{ x = 0, y = 499, width = 252, height = 488, offsetx = 3, offsety = 11 },
			{ x = 252, y = 499, width = 303, height = 479, offsetx = -23, offsety = 20 },
			{ x = 555, y = 499, width = 254, height = 488, offsetx = 3, offsety = 11 },
		}
    },
    
    attack = {
        fps = 16,
        spritesheet = base_path .. "attack",
		frames = {
			{ x = 0, y = 0, width = 295, height = 493, offsetx = 0, offsety = 0 },
			{ x = 295, y = 0, width = 240, height = 493, offsetx = 55, offsety = 0 },
			{ x = 535, y = 0, width = 234, height = 498, offsetx = 61, offsety = -5 },
			{ x = 769, y = 0, width = 268, height = 514, offsetx = 27, offsety = -21 },
			{ x = 1037, y = 0, width = 254, height = 506, offsetx = 41, offsety = -13 },
			{ x = 0, y = 514, width = 256, height = 516, offsetx = 39, offsety = -23 },
			{ x = 256, y = 514, width = 289, height = 506, offsetx = 56, offsety = -13 },
			{ x = 545, y = 514, width = 419, height = 513, offsetx = 47, offsety = -20 },
			{ x = 964, y = 514, width = 357, height = 510, offsetx = 50, offsety = -17 },
			{ x = 1321, y = 514, width = 315, height = 513, offsetx = 47, offsety = -20 },
			{ x = 0, y = 1030, width = 240, height = 498, offsetx = 61, offsety = -5 },
			{ x = 240, y = 1030, width = 240, height = 493, offsetx = 55, offsety = 0 },
			{ x = 480, y = 1030, width = 293, height = 506, offsetx = 2, offsety = -13 },
		}  
    },
    
    dying = {
        fps = 16,
        spritesheet = base_path .. "dying",
		frames = {
			{ x = 0, y = 0, width = 295, height = 493, offsetx = 0, offsety = 0 },
			{ x = 295, y = 0, width = 234, height = 496, offsetx = 61, offsety = 0 },
			{ x = 529, y = 0, width = 234, height = 496, offsetx = 61, offsety = 0 },
			{ x = 763, y = 0, width = 262, height = 507, offsetx = 33, offsety = 0 },
			{ x = 1025, y = 0, width = 230, height = 448, offsetx = 74, offsety = 0 },
			{ x = 0, y = 507, width = 243, height = 481, offsetx = 98, offsety = 0 },
			{ x = 243, y = 507, width = 236, height = 481, offsetx = 98, offsety = 0 },
			{ x = 479, y = 507, width = 405, height = 408, offsetx = 96, offsety = 0 },
			{ x = 884, y = 507, width = 475, height = 130, offsetx = 99, offsety = 0 },
			{ x = 1359, y = 507, width = 491, height = 106, offsetx = 96, offsety = 0 },
		}
    }
}

local unit = {
	name = "menele_ranged",
	class = "ranged",
	animations = animations,

	-- kind-specific parameters
	initial_hp = 50,
	attack_range = 5.0,
	speed = 0.01,
}

return unit