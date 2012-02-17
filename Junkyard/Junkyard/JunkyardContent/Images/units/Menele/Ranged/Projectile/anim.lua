-- Menel pocisk
local base_path = "Images/Units/Menele/Ranged/Projectile/"

local animations = {
    fly = {
        fps = 16,
        spritesheet = base_path .. "fly",
		frames = {
			{ x = 0, y = 0, offsetx = 342, offsety = 272, width = 85, height = 89 },
			{ x = 85, y = 0, offsetx = 342, offsety = 272, width = 106, height = 64 },
			{ x = 191, y = 0, offsetx = 342, offsety = 272, width = 86, height = 93 },
			{ x = 277, y = 0, offsetx = 342, offsety = 272, width = 81, height = 93 },
			{ x = 358, y = 0, offsetx = 342, offsety = 272, width = 100, height = 73 },
			{ x = 0, y = 93, offsetx = 342, offsety = 272, width = 70, height = 104 },
		}
    },    
}

local unit = {
	name = "menel_ranged_projectile",
	class = "projectile",
	animations = animations,

	-- kind-specific parameters
	gravity = 10.0,
}

return unit