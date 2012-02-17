luanet.load_assembly("Microsoft.Xna.Framework")
MathHelper=luanet.import_type("Microsoft.Xna.Framework.MathHelper")

math.randomseed(os.time())

local losowe = { "but01", "pudla01", "rurka01", "worek01", "worek02", "znak01" }

staticElements = {	
	plan01 = {
		assetName = "Images/Maps/Underworld/plan01",
		pos = { 0.0, 2.35, -1.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 12.0, 4.0, 1.0 },
	},
	plan02 = {
		assetName = "Images/Maps/Underworld/plan02",
		pos = { 0.0, 2.1, -2.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 12.0, 4.0, 1.0 },
	},
	plan03 = {
		assetName = "Images/Maps/Underworld/plan03",
		pos = { 0.0, 2.60, -10.0 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 18.0, 10.0, 1.0 },
	},	
}

statki = {
	ship1 = {
		assetName = "Images/Others/nuklon",
		pos = { -7.5, 0.1, -1.2 },
		yawpitchroll = { 0, 0, 0 },
		scale = { 2.0, 2.0, 1.0 },		
	},
	ship2 = {
		assetName = "Images/Others/statek_menele",
		pos = { 7.5, 0.0, -1.2 },
		yawpitchroll = { MathHelper.ToRadians(180), MathHelper.ToRadians(5), 0 },
		scale = { 2.0, 2.0, 1.0 },
	}
}
initialY = -1.6;
	
return staticElements,statki,initialY
