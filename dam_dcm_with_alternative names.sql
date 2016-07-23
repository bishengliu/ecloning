USE [ecloning]
GO
/****** Object:  Table [dbo].[Dam]    Script Date: 7/23/2016 7:57:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dam](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[COverlapping] [bit] NOT NULL,
	[CBlocked] [bit] NOT NULL,
	[appending] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Dcm]    Script Date: 7/23/2016 7:57:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dcm](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[COverlapping] [bit] NOT NULL,
	[CBlocked] [bit] NOT NULL,
	[prefixing] [nvarchar](100) NULL,
	[appending] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Dam] ON 

INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (1, N'BclI', 1, 1, NULL)
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (2, N'BspPI (AlwI)', 1, 1, NULL)
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (3, N'MboI', 1, 1, NULL)
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (4, N'DpnII', 1, 1, NULL)
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (6, N'Bsu15I (ClaI)', 0, 1, N'C')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (7, N'HphI', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (8, N'MboII', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (9, N'TaqI', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (10, N'XbaI', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (11, N'Bsp68I (NruI)', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (12, N'Hpy188I', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (13, N'Hpy188III', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (15, N'PagI (BspHI)', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (16, N'Kpn2I (BspEI)', 0, 1, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (17, N'BspDI', 0, 1, N'C')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (18, N'BdaI', 0, 0, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (19, N'BseJI (BsaBI)', 0, 0, N'C')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (20, N'Hin4I', 0, 0, N'C')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (21, N'BsaB1', 0, 0, N'C')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (23, N'PfoI', 0, 0, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (24, N'SmoI (SmlI)', 0, 0, N'TC')
INSERT [dbo].[Dam] ([id], [name], [COverlapping], [CBlocked], [appending]) VALUES (25, N'BcgI', 0, 0, N'TC')
SET IDENTITY_INSERT [dbo].[Dam] OFF
SET IDENTITY_INSERT [dbo].[Dcm] ON 

INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (1, N'EcoRII', 1, 1, NULL, NULL)
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (2, N'Bme1390I (ScrFI)', 0, 1, NULL, NULL)
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (3, N'Bsp120I (PspOMI)', 0, 1, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (4, N'CaiI (AlwNI)', 0, 1, N'null', N'null')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (5, N'CfrI', 0, 1, NULL, N'GG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (6, N'Cfr13I (Sau96I)', 0, 1, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (7, N'Eco47I (AvaII)', 0, 1, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (8, N'Eco57MI', 0, 1, N'C', NULL)
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (9, N'Eco147I (StuI)', 0, 1, NULL, N'GG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (10, N'Eco0109I', 0, 1, NULL, N'GG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (11, N'GsuI (BpmI)', 0, 1, NULL, N'G')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (12, N'MlsI (MscI)', 0, 1, NULL, N'GG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (13, N'PfoI', 0, 1, NULL, NULL)
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (14, N'Psp5II (PpuMI)', 0, 1, NULL, N'GG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (15, N'Van91I (PflMI)', 0, 1, NULL, N'GG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (16, N'Acc65I (Asp718I)', 0, 0, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (17, N'ApaI', 0, 0, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (18, N'BseLI (BslI)', 0, 0, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (19, N'BshNI', 0, 0, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (20, N'BshNI (BanI)', 0, 0, N'CCW', NULL)
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (21, N'Hin1I (BsaHI)', 0, 0, NULL, N'WGG')
INSERT [dbo].[Dcm] ([id], [name], [COverlapping], [CBlocked], [prefixing], [appending]) VALUES (22, N'SfiI', 0, 0, NULL, N'WGG')
SET IDENTITY_INSERT [dbo].[Dcm] OFF
