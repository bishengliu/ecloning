USE [ecloning]
GO
SET IDENTITY_INSERT [dbo].[modifying_company] ON 

INSERT [dbo].[modifying_company] ([id], [enzyme_id], [company_id]) VALUES (1, 2, 1)
INSERT [dbo].[modifying_company] ([id], [enzyme_id], [company_id]) VALUES (3, 3, 1)
INSERT [dbo].[modifying_company] ([id], [enzyme_id], [company_id]) VALUES (2, 4, 1)
SET IDENTITY_INSERT [dbo].[modifying_company] OFF
