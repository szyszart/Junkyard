-- Menel piechur
local base_path = "Images/Units/Menele/Infantry/"

local animations = {
    walk = {
        fps = 16,
        spritesheet = base_path .. "walk",
        frames = {
        	{ x = 0, y = 0, width = 272, height = 400, offsetx = 0, offsety = 0 },
        	{ x = 272, y = 0, width = 270, height = 395, offsetx = 7, offsety = 5 },
        	{ x = 542, y = 0, width = 271, height = 391, offsetx = 10, offsety = 11 },
        	{ x = 813, y = 0, width = 270, height = 397, offsetx = 7, offsety = 4 },
        	{ x = 1083, y = 0, width = 272, height = 400, offsetx = 0, offsety = -2 },
        	{ x = 0, y = 400, width = 274, height = 393, offsetx = -7, offsety = 5 },
        	{ x = 274, y = 400, width = 271, height = 388, offsetx = -9, offsety = 12 },
        	{ x = 545, y = 400, width = 274, height = 396, offsetx = -7, offsety = 5 },
        },        
    },
    
    attack = {
        fps = 14,
        spritesheet = base_path .. "attack",
        frames = {
        	{ x = 0, y = 0, width = 208, height = 440, offsetx = 20, offsety = 0 },
        	{ x = 208, y = 0, width = 173, height = 437, offsetx = 43, offsety = 0 },
        	{ x = 381, y = 0, width = 219, height = 431, offsetx = 42, offsety = 6 },
        	{ x = 600, y = 0, width = 290, height = 434, offsetx = 43, offsety = 5 },
        	{ x = 890, y = 0, width = 372, height = 433, offsetx = 3, offsety = 6 },
        	{ x = 0, y = 440, width = 380, height = 435, offsetx = -6, offsety = 6 },
        	{ x = 380, y = 440, width = 427, height = 435, offsetx = -17, offsety = 6 },
        	{ x = 807, y = 440, width = 474, height = 455, offsetx = -18, offsety = 6 },
        	{ x = 1281, y = 440, width = 504, height = 483, offsetx = -4, offsety = 6 },
        	{ x = 1785, y = 440, width = 483, height = 503, offsetx = 18, offsety = 6 },
        	{ x = 0, y = 943, width = 163, height = 432, offsetx = 28, offsety = 8 },
        	{ x = 163, y = 943, width = 141, height = 433, offsetx = 49, offsety = 7 },
        	{ x = 304, y = 943, width = 158, height = 433, offsetx = 49, offsety = 7 },
        	{ x = 462, y = 943, width = 225, height = 440, offsetx = 5, offsety = 0 },
        },
    },
    
    dying = {
        fps = 14,
        spritesheet = base_path .. "dying",
        frames = {
        	{ x = 0, y = 0, width = 170, height = 437, offsetx = 0, offsety = 0 },
        	{ x = 170, y = 0, width = 360, height = 483, offsetx = -47, offsety = 0 },
        	{ x = 530, y = 0, width = 233, height = 412, offsetx = -90, offsety = 0 },
        	{ x = 763, y = 0, width = 233, height = 442, offsetx = -169, offsety = 0 },
        	{ x = 996, y = 0, width = 326, height = 484, offsetx = -320, offsety = 0 },
        	{ x = 0, y = 484, width = 392, height = 399, offsetx = -401, offsety = 0 },
        	{ x = 392, y = 484, width = 406, height = 284, offsetx = -432, offsety = 0 },
        	{ x = 798, y = 484, width = 406, height = 171, offsetx = -432, offsety = 0 },
        	{ x = 1204, y = 484, width = 406, height = 164, offsetx = -432, offsety = 0 },
        	{ x = 1610, y = 484, width = 406, height = 151, offsetx = -432, offsety = 0 },
        	{ x = 0, y = 883, width = 406, height = 124, offsetx = -432, offsety = 0 },
        },        
    }
}

local unit = {
	name = "menel_infantry",
	class = "infantry",
	animations = animations,

	-- kind-specific parameters
	initial_hp = 100,
	speed = 0.01,
	attack_range = 1.2,
}

return unit