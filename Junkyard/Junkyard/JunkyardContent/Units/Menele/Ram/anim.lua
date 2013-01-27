-- Menel taran
local base_path = "Images/Units/Menele/Ram/"

local animations = {
    walk = {
        fps = 16,
        spritesheet = base_path .. "walk",        
        frames = {
        	{ x = 0, y = 0, width = 690, height = 538, offsetx = 0, offsety = 0 },
        	{ x = 690, y = 0, width = 698, height = 536, offsetx = 0, offsety = 0 },
        	{ x = 1388, y = 0, width = 708, height = 541, offsetx = 0, offsety = 0 },
        	{ x = 0, y = 541, width = 697, height = 538, offsetx = 0, offsety = 0 },
        	{ x = 697, y = 541, width = 690, height = 540, offsetx = 0, offsety = 0 },
        	{ x = 1387, y = 541, width = 698, height = 537, offsetx = 0, offsety = 0 },
        	{ x = 0, y = 1081, width = 701, height = 542, offsetx = 0, offsety = 0 },
        	{ x = 701, y = 1081, width = 697, height = 539, offsetx = 0, offsety = 0 },
        }         
    },
    
    attack = {
        fps = 18,
        spritesheet = base_path .. "attack",
        frames = {
        	{ x = 0, y = 0, width = 589, height = 536, offsetx = 0, offsety = 0 },
        	{ x = 589, y = 0, width = 564, height = 542, offsetx = 0, offsety = -6 },
        	{ x = 1153, y = 0, width = 589, height = 545, offsetx = 0, offsety = -9 },
        	{ x = 1742, y = 0, width = 567, height = 532, offsetx = 0, offsety = 4 },
        	{ x = 0, y = 545, width = 675, height = 541, offsetx = 0, offsety = -5 },
        	{ x = 675, y = 545, width = 925, height = 588, offsetx = 0, offsety = -52 },
        	{ x = 1600, y = 545, width = 936, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 2536, y = 545, width = 965, height = 598, offsetx = 0, offsety = -62 },
        	{ x = 0, y = 1149, width = 936, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 936, y = 1149, width = 925, height = 588, offsetx = 0, offsety = -52 },
        	{ x = 1861, y = 1149, width = 936, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 2797, y = 1149, width = 965, height = 598, offsetx = 0, offsety = -62 },
        	{ x = 0, y = 1753, width = 925, height = 588, offsetx = 0, offsety = -52 },
        	{ x = 925, y = 1753, width = 936, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 1861, y = 1753, width = 965, height = 598, offsetx = 0, offsety = -62 },
        	{ x = 2826, y = 1753, width = 936, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 0, y = 2357, width = 925, height = 588, offsetx = 0, offsety = -52 },
        	{ x = 925, y = 2357, width = 936, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 1861, y = 2357, width = 965, height = 598, offsetx = 0, offsety = -62 },
        	{ x = 2826, y = 2357, width = 992, height = 604, offsetx = 0, offsety = -68 },
        	{ x = 0, y = 2961, width = 991, height = 601, offsetx = 0, offsety = -64 },
        	{ x = 991, y = 2961, width = 990, height = 602, offsetx = 0, offsety = -66 },
        	{ x = 1981, y = 2961, width = 897, height = 602, offsetx = 0, offsety = -66 },
        	{ x = 2878, y = 2961, width = 589, height = 536, offsetx = 0, offsety = 0 },
        }
    },
    
    dying = {
        fps = 14,
        spritesheet = base_path .. "dying",
        frames = {
        	{ x = 0, y = 0, width = 589, height = 536, offsetx = 0, offsety = 0 },
        	{ x = 589, y = 0, width = 611, height = 551, offsetx = 0, offsety = 0 },
        	{ x = 1200, y = 0, width = 624, height = 561, offsetx = 0, offsety = 0 },
        	{ x = 0, y = 561, width = 703, height = 598, offsetx = 0, offsety = 0 },
        	{ x = 703, y = 561, width = 876, height = 605, offsetx = -74, offsety = 0 },
        	{ x = 1579, y = 561, width = 1010, height = 604, offsetx = -111, offsety = 0 },
        	{ x = 0, y = 1166, width = 1163, height = 618, offsetx = -159, offsety = 0 },
        	{ x = 1163, y = 1166, width = 1229, height = 598, offsetx = -218, offsety = 0 },
        	{ x = 2392, y = 1166, width = 1229, height = 411, offsetx = -218, offsety = 0 },
        	{ x = 0, y = 1784, width = 1229, height = 332, offsetx = -218, offsety = 0 },
        	{ x = 1229, y = 1784, width = 1229, height = 311, offsetx = -218, offsety = 0 },
        	{ x = 2458, y = 1784, width = 1229, height = 320, offsetx = -218, offsety = 0 },
        }  
    }
}

local unit = {
	name = "menel_ram",
	class = "infantry",
	animations = animations,

	-- kind-specific parameters
	initial_hp = 400,
	speed = 0.02,
	attack_range = 2.0,
}

return unit