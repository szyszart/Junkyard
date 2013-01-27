-- Menel knur
local base_path = "Images/Units/Menele/Boar/"

local animations = {
    fly = {
        fps = 16,
        spritesheet = base_path .. "fly",
        frames = {
            { x = 0, y = 0, width = 425, height = 341, offsetx = 0, offsety = 0 },
            { x = 425, y = 0, width = 438, height = 342, offsetx = -13, offsety = -1 },
            { x = 863, y = 0, width = 442, height = 341, offsetx = -17, offsety = 0 },
            { x = 1305, y = 0, width = 438, height = 342, offsetx = -13, offsety = -1 },
        }, 
    },
      
    dying = {
        fps = 14,
        spritesheet = base_path .. "dying",
		frames = {
			{ x = 0, y = 0, width = 531, height = 301, offsetx = 0, offsety = 0 },
			{ x = 531, y = 0, width = 720, height = 378, offsetx = -88, offsety = 0 },
			{ x = 1251, y = 0, width = 899, height = 418, offsetx = -195, offsety = 0 },
			{ x = 0, y = 418, width = 1028, height = 514, offsetx = -268, offsety = 0 },
			{ x = 1028, y = 418, width = 1042, height = 572, offsetx = -256, offsety = 0 },
			{ x = 2070, y = 418, width = 1114, height = 588, offsetx = -291, offsety = 0 },
			{ x = 0, y = 1006, width = 1068, height = 512, offsetx = -270, offsety = 0 },
			{ x = 1068, y = 1006, width = 918, height = 403, offsetx = -297, offsety = 0 },
			{ x = 1986, y = 1006, width = 929, height = 267, offsetx = -308, offsety = 0 },
			{ x = 0, y = 1518, width = 944, height = 255, offsetx = -323, offsety = 0 },
			{ x = 944, y = 1518, width = 939, height = 260, offsetx = -318, offsety = 0 },
		},
    }
}

local unit = {
	name = "menel_boar",
	class = "meteor",
	animations = animations,

	-- kind-specific parameters
	height = 6.0,
    velocity = { 5.0, -5.0, 0.0 },    
	acceleration = { 5.0, -10.0, 0.0 },
	range = 1.0, 
}

return unit