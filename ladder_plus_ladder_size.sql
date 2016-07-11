USE [ecloning]
GO
SET IDENTITY_INSERT [dbo].[ladder] ON 

INSERT [dbo].[ladder] ([id], [ladder_type], [name], [min_bp_kDa], [max_bp_kda], [company_id], [orderref], [a_values], [b_value]) VALUES (4, N'DNA', N'1 kb DNA Ladder', 500, 10002, 1, N'N3232L', NULL, NULL)
INSERT [dbo].[ladder] ([id], [ladder_type], [name], [min_bp_kDa], [max_bp_kda], [company_id], [orderref], [a_values], [b_value]) VALUES (5, N'DNA', N'100 bp DNA Ladder', 100, 1517, 1, N'N3231L', NULL, NULL)
SET IDENTITY_INSERT [dbo].[ladder] OFF
SET IDENTITY_INSERT [dbo].[ladder_size] ON 

INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (14, 4, 10002, 0.029, 42)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (15, 4, 8001, 0.058, 42)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (16, 4, 6001, 0.107, 50)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (17, 4, 5001, 0.146, 42)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (18, 4, 4001, 0.194, 33)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (19, 4, 3001, 0.291, 125)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (20, 4, 2000, 0.427, 48)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (21, 4, 1500, 0.544, 36)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (22, 4, 1000, 0.709, 42)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (23, 4, 517, 0.942, 21)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (24, 4, 500, 0.951, 21)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (25, 5, 1517, 0.039, 45)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (26, 5, 1200, 0.143, 35)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (27, 5, 1000, 0.247, 95)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (28, 5, 900, 0.286, 27)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (29, 5, 800, 0.338, 24)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (30, 5, 700, 0.403, 21)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (31, 5, 600, 0.455, 18)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (32, 5, 517, 0.532, 97)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (33, 5, 400, 0.61, 38)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (34, 5, 300, 0.701, 29)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (35, 5, 200, 0.818, 25)
INSERT [dbo].[ladder_size] ([id], [ladder_id], [size], [Rf], [mass]) VALUES (36, 5, 100, 0.922, 48)
SET IDENTITY_INSERT [dbo].[ladder_size] OFF
