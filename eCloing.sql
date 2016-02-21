--this is data modal for sCloning project, version 1.0 30-01-2016

------------------------------------------------------------      
--need manually create database ecloning for each instance--
------------------------------------------------------------
CREATE DATABASE ecloning;
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
	[Name] [nvarchar](256) NOT NULL UNIQUE,  --appAdmin, instituteAdmin, departAdmin, groupAdmin 
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
--user management
CREATE TABLE department --department level
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
	[des] TEXT,
	CONSTRAINT uq_department_name UNIQUE (name)
);
--research group in each department--
CREATE TABLE [group]
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	depart_id INT NOT NULL,
    name NVARCHAR(50) NOT NULL, 
	code TEXT, --for registration
	[des] TEXT,
	CONSTRAINT uq_group_depart_id_name UNIQUE (depart_id,name),
	CONSTRAINT fk_group_depart_id FOREIGN KEY (depart_id) REFERENCES department(id)
);
---lab people in each research group----
CREATE TABLE people
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	--group_id INT NOT NULL,
    first_name NVARCHAR(50) NOT NULL,
    mid_name  NVARCHAR(50),
    last_name  NVARCHAR(50) NOT NULL,
    email NVARCHAR(50) NOT NULL,
    func NVARCHAR(100),
	active bit,
	CONSTRAINT uq_people_email UNIQUE (email)
);

CREATE TABLE group_people
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	group_id INT NOT NULL,
    people_id INT NOT NULL,
	CONSTRAINT uq_group_people_group_id_people_id UNIQUE (group_id, people_id),
	CONSTRAINT fk_group_people_group_id FOREIGN KEY (group_id) REFERENCES [group](id),
	CONSTRAINT fk_group_people_people_id FOREIGN KEY (people_id) REFERENCES people(id),
);

--app license
CREATE TABLE app_license
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	institute_num INT NOT NULL, --the number of institute license, default  -1, meaning no limits
	institute_expired BIT,
	institute_expired_dt DATETIME,
	depart_id INT NOT NULL, 
	depart_num INT NOT NULL, --the number of each depart license, default  -1, meaning no limits
	depart_expired BIT,
	depart_expired_dt DATETIME,
	group_id INT NOT NULL,
	group_num INT NOT NULL, --the number of each group license, default 5.
	group_expired BIT,
	group_expired_dt DATETIME,
	CONSTRAINT fk_app_license_depart_id FOREIGN KEY (depart_id) REFERENCES department(id),
	CONSTRAINT fk_app_license_group_id FOREIGN KEY (group_id) REFERENCES [group](id),
	CONSTRAINT uq_app_license_depart_id_group_id UNIQUE (depart_id,group_id)
);

--technically license table for each person, like cloning, viral work, cellular work etc.
CREATE TABLE license
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    licence_name NVARCHAR(100) NOT NULL, --license name
    [des] TEXT  --remarks for license
);


--sub func for each people, licences for tech etc.
CREATE TABLE people_license
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	people_id INT NOT NULL,
    licence_id INT NOT NULL, --license id refer to license tables
    licence_num NVARCHAR(250),  --license id-num pair for each people
	CONSTRAINT fk_people_license_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT fk_people_license_licence_id FOREIGN KEY (licence_id) REFERENCES license(id),
);

---------------------------------------------------------------------------------------------
--DNA/RNA cloning


--plasmid table
CREATE TABLE plasmid
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(200) NOT NULL,
	[sequence] TEXT,
	expression_system NVARCHAR(100), --manmalian, baterial, yeast, worm, insert etc.
	expression_subsystem NVARCHAR(100), --in detail which bateria, cell or yeast --check addgene
	promotor NVARCHAR(100),
	polyA NVARCHAR(100),
	resistance NVARCHAR(200), --both bac and cellular
	reporter NVARCHAR(200),
	selection NVARCHAR(200), --selection marker
	[insert] NVARCHAR(200), --gene or oligo etc.
	usage NVARCHAR(200), --RNAi, luciferase, Cre/Lox etc. check addgene
	plasmid_type NVARCHAR(200),
	ref_plasmid INT,
	img_fn NVARCHAR(500), --upload image/ how the plasmid works
	addgene INT, --cite addgene plasmid# like, 12260
	d DATETIME,
	people_id INT NOT NULL, --the person who add this plasmid
	submitted_to_group BIT, --for submitted to general stock and prevent changes 
	shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
	shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
	[des] TEXT,
	CONSTRAINT fk_plasmid_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_plasmid_name_people_id UNIQUE (name,people_id)
); 


--1. Generic features
--2. Genes
--3. Regulatory
--4. Promoters
--5. Primers
--6.Terminators
--7.Origins
--8.ORFs

--main feature
CREATE TABLE plasmid_feature
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	feature NVARCHAR(200) NOT NULL,
	[des] TEXT,
);
INSERT INTO plasmid_feature
VALUES
('feature',  'Generic Features'),
('promoter', 'Promoters'),
('primer', 'Primers'),
('enzyme',  'Enzymes'),
('gene', 'Genes'),
('origin', 'Origins'),
('regulatory',  'Regulatory'),
('terminator',  'Terminators'),
('exact_feature', 'Exact Features'),
('orf', 'ORF');


--need use addgene giraffe
CREATE TABLE plasmid_map
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	plasmid_id INT NOT NULL,
	--type_id INT, --linear or circlular
	show_feature INT NOT NULL, -- 1 or 0
	feature NVARCHAR(200) NOT NULL,
	feature_id INT NOT NULL,
	start INT NOT NULL,
	[end] INT NOT NULL,
	cut INT, --enzyme cut only clude unique and 2 cuts
	label NVARCHAR(200) NOT NULL,
	clockwise INT NOT NULL, -- 1 or 0
	[des] TEXT,
	CONSTRAINT fk_plasmid_map_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id),
	CONSTRAINT fk_plasmid_map_feature_id FOREIGN KEY (feature_id) REFERENCES plasmid_feature(id)
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
	methylation BIT,
	CONSTRAINT uq_restriction_name UNIQUE (name)
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
	submitted_to_group BIT, --for submitted to general stock and prevent changes 
	shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
	shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_primer_tech_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_primer_name UNIQUE (name)
);


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
	submitted_to_group BIT, --for submitted to general stock and prevent changes 
	shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
	shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_oligo_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_oligo_name UNIQUE (name)
);

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
	submitted_to_group BIT, --for submitted to general stock and prevent changes 
	shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
	shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_probe_tech_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT fk_probe_forward_primer FOREIGN KEY (forward_primer) REFERENCES primer(id),
	CONSTRAINT fk_probe_reverse_primer FOREIGN KEY (reverse_primer) REFERENCES primer(id),
	CONSTRAINT uq_probe_name UNIQUE (name)
);

CREATE TABLE nuclease
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	[des] TEXT,
	CONSTRAINT uq_nuclease_name UNIQUE (name)
);

--create crispr group or virus group, plasmid group
CREATE TABLE [clone_group]
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	group_type NVARCHAR(100) NOT NULL, 
	name NVARCHAR(100) NOT NULL,
	version INT NOT NULL, --default to be 1 
	plasmid_id INT NOT NULL,
	oligo_id INT, 
	primer_id INT,
	nuclease_id INT, --enzyme or nuclease
	refgroup INT, -- for ref to another crispr group
	img_fn NVARCHAR(100), --for upload img scheme
	dt DATETIME DEFAULT GETDATE(),
	[des] TEXT,
	people_id INT,
	submitted_to_group BIT, --for submitted to general stock and prevent changes 
	shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
	shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
	CONSTRAINT fk_clone_group_people_id FOREIGN KEY (people_id) REFERENCES people(id),	
	CONSTRAINT fk_clone_group_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id),
	CONSTRAINT fk_clone_group_oligo_id FOREIGN KEY (oligo_id) REFERENCES oligo(id),
	CONSTRAINT fk_clone_group_pnuclease_id FOREIGN KEY (nuclease_id) REFERENCES nuclease(id),
	CONSTRAINT uq_crispr_group_name UNIQUE (name)
);


--ladder, for both DNA and protein
CREATE TABLE ladder
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	min_bp_kDa INT,
	max_bp_kda INT,
	company NVARCHAR(100),
	orderref NVARCHAR(100),
	CONSTRAINT uq_ladder_name UNIQUE (name),
);
--ladder size
CREATE TABLE ladder_size
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	ladder_id INT NOT NULL,
	size INT NOT NULL, --bp or kDa
	CONSTRAINT fk_padder_size_ladder_id FOREIGN KEY (ladder_id) REFERENCES ladder(id),
);


---------------------------------------------------------------
--storage, plasmids, bacterial, etc.

--container
CREATE TABLE container
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	category NVARCHAR(100), -- -20, -80, -180, liquid nitrogen
	[des] TEXT,
	CONSTRAINT uq_container_name UNIQUE (category),
);

CREATE TABLE minus20
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,

	--4 degree
	drawer_up INT NOT NULL, --drawer num
	-- minu 20 degree
	drawer_down INT NOT NULL,
	location NVARCHAR(100),
	[des] TEXT,
	CONSTRAINT uq_minus20_name UNIQUE (name),
);

CREATE TABLE minus80
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	tower_num INT NOT NULL, --how many towers
	drawer_num INT NOT NULL, --how many drawers in each tower
	box_num INT NOT NULL, --how many boxes in each drawer
	location NVARCHAR(100),
	[des] TEXT,
	CONSTRAINT uq_minus80_name UNIQUE (name),
);


CREATE TABLE minus180_liquid_nitrogen --for both
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	tower_num INT NOT NULL, --how many towers
	drawer_num INT NOT NULL, --how many drawers in each tower
	location NVARCHAR(100),
	[des] TEXT,
	CONSTRAINT uq_minus180_liquid_nitrogen_name UNIQUE (name),
);


------------------------------------------
--store plasmid


--store primers

--store oligo

--store enzyme, nuclease

--store bacteria

--store cells

--store virus
--should support tower, container, box and drawer moves
CREATE TABLE storage --for above all
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	type NVARCHAR(100) NOT NULL, --plamsid, primers etc. all above
	ref_id NVARCHAR(100) NOT NULL, --ref to all tables above, refer to a plasmid, so that all info can be linked, such as name
	container_id INT NOT NULL, 
	tower_id INT, -- -80, -180 and liquid nitrogen
	drawer_id INT, -- for all
	box_id INT,
	box_label NVARCHAR(500),
	vial_label NVARCHAR(500) NOT NULL,
	parent_link INT, --link to parents in the same table
	dt DATETIME, --freezing date
	[des] TEXT,
);


--------------------------------------------------------------
--general protocols, sop
--protocol for transduction, transfection, pcr, enzyme digestion, fragment purification and harvest, runing gel, blunt (bu ping), ligation, bacterial growth, cell growth, plasmid small/large-prepareation, cell freezing, prepare all reagents, cell division, cell expansion, virus packaging, production
--virus validation, virus reinjection, virus production, virus purification, virus freezing, etc
CREATE TABLE protocol 
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(500) NOT NULL,
	category NVARCHAR(100) NOT NULL, --groups
	purpose TEXT,
	version INT,
	versionref INT, --ref to previous version
	people_id INT,
	[des] TEXT,
	dt DATETIME,
	submitted_to_group BIT, --for submitted to general stock and prevent changes 
	shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
	shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
	CONSTRAINT fk_protocol_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_protocol_name_version UNIQUE ([name],[version])

);



---------------------------------------------------------------------------------------------------
--project design, should be automatically generated steps and cloning strategies
--project 
--suggested steps (different strategies)
--steps

--record experiments

--this need most of the work

--for main steps--
-- 1. molecular cloning
-- 2. cellular work (plasmid only)
-- 3. viral production
-- 4. viral infection in vitro
-- 5. viral inecton in vivo 
CREATE TABLE main_step
(
		id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		name NVARCHAR(200),
		[des] TEXT
);


--project/experiment
CREATE TABLE project
(
		id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		name NVARCHAR(200),
		aim TEXT,
		people_id INT NOT NULL,
		start_dt DATETIME,
		status NVARCHAR(100), --open, closed, finished
		finished_dt DATETIME, --date of closure or done
		dt DATETIME,
		[des] TEXT,
		shared_with_group BIT, --after approval changed to true and shared by all people in the same group 
		shared_with_people NVARCHAR(100), --shared with people id, not with the whole group, can be people from any group in the same institute, example 1-2-3
		CONSTRAINT fk_project_people_id FOREIGN KEY (people_id) REFERENCES people(id),
);


----------
--add table, clone_strategy
--need carefuly thinking
--should include: digestion strategies, like enzyme cut, whether need to blunt fragement, insert postion/replace, clockwise, gel confirmation, selection etc, 
--this need the plasmid map if using features or seq site if only using restriction cut.

----------------------
--add tables to record exp/data for each small experiment, think about all the work in the lab. should be expandable and flexible


--for form dropdown items--
CREATE TABLE dropdownitem
(
		id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		value NVARCHAR(100),
		[text] NVARCHAR(100),
		category NVARCHAR(100)
);
