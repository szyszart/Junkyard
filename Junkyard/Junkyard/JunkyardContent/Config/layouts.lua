-- Puzzle board layouts

local layouts = {
	thumbnails = "Images/Units/thumbnails",
	block_size = {104, 102},

	{
		top			= "ggww",
		bottom		= "wwgg",
		unit		= "menel_boar",
		thumbnails	= {
			{14, 0}, {15, 0}, {16, 0}, {17, 0}, 
			{14, 1}, {15, 1}, {16, 1}, {17, 1}, 
		}
	},	

	{
		top			= nil,
		bottom		= "ggww",
		unit		= "menel_ram",
		thumbnails	= {
			{6, 0}, {7, 0}, {8, 0}, {9, 0},
		}
	},

	{
		top			= nil,
		bottom		= "wf",
		unit		= "menele_ranged",
		thumbnails	= {
			{2, 0}, {3, 0}
		}
	},	

	{
		top			= nil,
		bottom		= "fm",
		unit		= "nuklearni_infantry",
		thumbnails	= {
			{0, 1}, {1, 1}
		}
	},

	{
		top			= nil,
		bottom		= "gf",
		unit		= "menel_infantry",
		thumbnails	= {
			{0, 0}, {1, 0},
		}
	},

	{
		top			= "mmmm",
		bottom		= "mggm",
		unit		= "menel_robot",
		thumbnails	= {
			{10, 0}, {11, 0}, {12, 0}, {13, 0},
			{10, 1}, {11, 1}, {12, 1}, {13, 1}
		}
	},

	{
		top			= "gf",
		bottom		= "fg",
		unit		= "menel_junkie",
		thumbnails	= {
			{4, 0}, {5, 0},	{4, 1}, {5, 1}
		}
	},
}

return layouts