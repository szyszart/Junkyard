-- Puzzle board layouts

local layouts = {
	thumbnails = "Images/Units/thumbnails",
	block_size = {512, 512},

	{
		top			= "ggmm",
		bottom		= "mmgg",
		unit		= "menel_boar",
		thumbnails	= {
			{0, 0}, {0, 0}, {0, 0}, {0, 0}, 
			{0, 0}, {0, 0}, {0, 0}, {0, 0},
		}
	},	

	{
		top			= nil,
		bottom		= "ww",
		unit		= "menele_ranged",
		thumbnails	= {
			{0, 0}, {0, 0}
		}
	},	

	{
		top			= nil,
		bottom		= "ff",
		unit		= "nuklearni_infantry",
		thumbnails	= {
			{0, 0}, {0, 0}
		}
	},

	{
		top			= "ww",
		bottom		= "gg",
		unit		= "menel_infantry",
		thumbnails	= {
			{3, 0}, {4, 0}, {2, 0}, {2, 0}
		}
	},

	{
		top			= "mm",
		bottom		= "mm",
		unit		= "menel_robot",
		thumbnails	= {
			{0, 0}, {1, 0}, {3, 0}, {4, 0}
		}
	},

	{
		top			= "gg",
		bottom		= "gg",
		unit		= "menel_junkie",
		thumbnails	= {
			{3, 0}, {4, 0},	{3, 0}, {4, 0}
		}
	},
}

return layouts