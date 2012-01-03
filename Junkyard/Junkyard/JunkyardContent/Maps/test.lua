luanet.load_assembly("Microsoft.Xna.Framework")
MathHelper=luanet.import_type("Microsoft.Xna.Framework.MathHelper")

math.randomseed(os.time())

local losowe = { "but01", "pudla01", "rurka01", "worek01", "worek02", "znak01" }

local staticElements = {
	powietrze = {
		assetName = "Images/Others/powietrze_male",
		pos = { 0, 5.0, -20.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 20.02, 11.71, 1.0 }
	},
	powietrze_tlo = {
		assetName = "Images/Others/powietrze_male",
		pos = { 0, -1.2, -21.0 },
		yawpitchroll = { 0, -math.pi/2, 0 },
		scale = { 20.0, 20.0, 1.0 }
	},
	lightbulb = {
		assetName = "Images/Others/lightbulb",
		pos = { 0, 0, 0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 0.1, 0.1, 1.0 }
	},
	--[[
	blisko = {
		assetName = "Images/Others/blisko",
		pos = { 0.0, 0.0, -1.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 10.0, 2.0, 1.0 },
		normalMap = "Images/Others/blisko_norm"		
	},
	--]]
	plan01 = {
		assetName = "Images/Maps/Menelichy/plan01",
		pos = { 0.0, -1.55, -1.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 10.0, 1.0, 1.0 },
	},
	oddalenie = {
		assetName = "Images/Others/oddalenie",
		pos = { (math.random(100)-40), - 1, -10.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 20.0, 6.0, 1.0 }
	}	
}

for _,name in pairs(losowe) do
	staticElements[name] = 
		{
			assetName = "Images/Maps/Menelichy/"..name,
			pos = { math.random()*20 - 10, math.random() - 2, -1},
			yawpitchroll = { 0, 0, 0 },
			scale = { 0.5, 0.5, 0.5 }
		}
end

local statki = {
	ship1 = {
		assetName = "Images/Others/kapitan",
		pos = { -8.0, 0.0, -1.2 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 2.0, 2.0, 1.0 },
		normalMap = "Images/Others/kapitan_norm"
	},
	ship2 = {
		assetName = "Images/Others/kapitan",
		pos = { 8.0, 0.5, -1.2 },
		yawpitchroll = { MathHelper.ToRadians(180), MathHelper.ToRadians(5), 0 },
		scale = { 2.0, 2.0, 1.0 },
		normalMap = "Images/Others/kapitan_norm"
	}
}
	
return staticElements,statki