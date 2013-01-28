luanet.load_assembly("Microsoft.Xna.Framework")
MathHelper=luanet.import_type("Microsoft.Xna.Framework.MathHelper")

math.randomseed(os.time())

local losowe = { "but01", "pudla01", "rurka01", "worek01", "worek02", "znak01" }

staticElements = {
	powietrze = {
		assetName = "Images/Others/powietrze_male",
		pos = { 0, 5.0, -20.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 24.02, 24.02, 1.0 }
	},	
	--[[
	powietrze_tlo = {
		assetName = "Images/Others/powietrze_male",
		pos = { 0, -1.2, -21.0 },
		yawpitchroll = { 0, -math.pi/2, 0 },
		scale = { 20.0, 20.0, 1.0 }
	},		

	--lightbulb = {
		--assetName = "Images/Others/lightbulb",
		--pos = { 0, 0, 0 },
		--yawpitchroll = { 0, 0, 0 },
		--scale = { 0.1, 0.1, 1.0 }
	--},

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
		pos = { 0.0, 2.31, -1.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 18.0, 5.0, 1.0 },
	},
	plan02 = {
		assetName = "Images/Maps/Menelichy/plan02",
		pos = { 0.0, 1.15, -17.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 24.0, 10.0, 1.0 },
	},
	plan03 = {
		assetName = "Images/Maps/Menelichy/plan03",
		pos = { 0.0, 1.4, -19.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 24.0, 10.0, 1.0 },
	},	
}

for _,name in pairs(losowe) do
	staticElements[name] = 
		{
			assetName = "Images/Maps/Menelichy/"..name,
			pos = { math.random()*20 - 10, math.random() - 2, -0.9},
			yawpitchroll = { 0, 0, 0 },
			scale = { 0.5, 0.5, 0.5 }
		}
end

statki = {
	ship1 = {
		assetName = "Images/Others/kapitan",
		pos = { -7.5, 0.0, -1.2 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 2.0, 2.0, 1.0 },
		normalMap = "Images/Others/kapitan_norm"
	},
	ship2 = {
		assetName = "Images/Others/kapitan",
		pos = { 7.5, 0.5, -1.2 },
		yawpitchroll = { MathHelper.ToRadians(180), MathHelper.ToRadians(5), 0 },
		scale = { 2.0, 2.0, 1.0 },
		normalMap = "Images/Others/kapitan_norm"
	}
}
initialY = -1.1;
	
return staticElements,statki,initialY
