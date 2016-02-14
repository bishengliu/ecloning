--this is data modal for sCloning project, version 1.0 30-01-2016

------------------------------------------------------------      
--need manually create database ecloning for each instance--
------------------------------------------------------------

-----------------------------------------user account management---------------------------------------------------------------------



/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7-5-2015 11:14:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 7-5-2015 11:15:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NOT NULL UNIQUE,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)  TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 7-5-2015 11:14:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL UNIQUE,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) 

GO



/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 7-5-2015 11:14:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)  TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO


/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 7-5-2015 11:15:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO

ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO


/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 7-5-2015 11:15:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO

ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO

ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO


--add some unique constraints

alter table dbo.AspNetUsers Add constraint UQ_AspNetUsers_Email UNIQUE (Email);
alter table dbo.AspNetRoles Add constraint UQ_AspNetRoles_Name UNIQUE (Name);



--------------------------------------------tables for  eCloning------------------------------------------------------------
--each instance should be made for each institute
--departments for an institute---
CREATE TABLE department --department level
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL, 
	active BIT,
	[des] TEXT,
);
--research group in each department--
CREATE TABLE group_people
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	depart_id INT NOT NULL,
    name NVARCHAR(50) NOT NULL, 
	active BIT,
	[des] TEXT,
	CONSTRAINT fk_people_group_depart_id FOREIGN KEY (depart_id) REFERENCES department(id)
);
---lab people in each research group----
CREATE TABLE people
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	group_id INT NOT NULL,
    first_name NVARCHAR(50) NOT NULL,
    mid_name  NVARCHAR(50),
    last_name  NVARCHAR(50) NOT NULL,
    email NVARCHAR(50) NOT NULL,
    func NVARCHAR(100),
	active bit,
	CONSTRAINT fk_people_group_id FOREIGN KEY (group_id) REFERENCES group_people(id)
);

--plasmid table
CREATE TABLE plasmid
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(200) NOT NULL,
	[sequence] TEXT,
	resist_bac NVARCHAR(100),
	resist_cell NVARCHAR(100),
	reporter NVARCHAR(100),
	expression_system NVARCHAR(100), --bacteria, cellular, yeast, etc
	subsystem NVARCHAR(100), --in detail which bateria, cell or yeast
	ref_plasmid INT,
	d DATETIME,
	people_id INT NOT NULL,
	[des] TEXT,
	CONSTRAINT fk_plasmid_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_plasmid_name_people_id UNIQUE (name,people_id)
); 


--restriction
CREATE TABLE restriction
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	forward_seq NVARCHAR(20) NOT NULL,
	forward_cut INT NOT NULL,
	reverse_seq NVARCHAR(20) NOT NULL,
	reverse_cut INT NOT NULL,
);

--pcr primers
CREATE TABLE [primer]
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	[sequence] NVARCHAR(200) NOT NULL, 
	company NVARCHAR(100),
	orderref NVARCHAR(100),
	location NVARCHAR(100),--ref to the the container system 
	[usage] NVARCHAR(100), --pcr, qpcr, cloing, sequencing
	purity NVARCHAR(100), --desalted (default), page
	modification NVARCHAR(100), --Non (default), phosphorylated
	people_id INT,
	[des] TEXT,
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_primer_tech_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_primer_name UNIQUE (name)
)


CREATE TABLE [oligo]
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	[sequence] NVARCHAR(500) NOT NULL, 
	company NVARCHAR(100),
	orderref NVARCHAR(100),
	location NVARCHAR(100),
	[usage] NVARCHAR(100), --pcr, qpcr, cloing, sequencing
	purity NVARCHAR(100), --desalted (default), page
	modification NVARCHAR(100), --Non (default), phosphorylated
	people_id INT,
	[des] TEXT,
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_oligo_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_oligo_name UNIQUE (name)
)

CREATE TABLE [probe] --pcr probe
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	[sequence] NVARCHAR(500) NOT NULL, 
	forward_primer INT,
	reverse_primer INT,
	[usage] NVARCHAR(100), --pcr, qpcr, cloing, sequencing
	company NVARCHAR(100), --Roche(default), Applied BioSystems
	orderref NVARCHAR(100),
	location NVARCHAR(100),
	people_id INT,
	[des] TEXT,
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_probe_tech_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT fk_probe_forward_primer FOREIGN KEY (forward_primer) REFERENCES primer(id),
	CONSTRAINT fk_probe_reverse_primer FOREIGN KEY (reverse_primer) REFERENCES primer(id),
	CONSTRAINT uq_probe_name UNIQUE (name)
)



--for form dropdown items--
CREATE TABLE dropdownitem
(
		id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		value NVARCHAR(100),
		[text] NVARCHAR(100),
		category NVARCHAR(100)
);

