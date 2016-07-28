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

---also need to add this to web.config
--    <add name="DefaultConnection" providerName="System.Data.SqlClient" connectionString="server=NB0162;database=ecloning;user id=mcca;password=mcca;Connect Timeout=36000" />


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
INSERT INTO [department](name,[des]) VALUES 
( 'AppAdmin', 'SystemAdmin'),
( 'Institute Admin', 'Institute Admin');

--research group in each department--
CREATE TABLE [group]
(
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	depart_id INT NOT NULL,
    name NVARCHAR(50) NOT NULL, 
	email NVARCHAR(100), 
	code TEXT, --for registration
	[des] TEXT,
	CONSTRAINT uq_group_depart_id_name UNIQUE (depart_id,name),
	CONSTRAINT fk_group_depart_id FOREIGN KEY (depart_id) REFERENCES department(id)
);

INSERT INTO [group](depart_id,name,[des]) VALUES 
( 1, 'AppAdmin','SystemAdmin'); --must be added first!
-- ( 2, 'Institute Admin','Administrator');


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
	seq_length INT, --for creating maps
	expression_system NVARCHAR(100), --manmalian, baterial, yeast, worm, insert etc.
	expression_subsystem NVARCHAR(100), --in detail which bateria, cell or yeast --check addgene
	promotor NVARCHAR(100),
	polyA NVARCHAR(100),
	resistance NVARCHAR(200), --both bac and cellular
	reporter NVARCHAR(200),
	selection NVARCHAR(200), --selection marker
	[insert] NVARCHAR(200), --gene or oligo etc.
	insert_species NVARCHAR(200),
	usage NVARCHAR(200), --RNAi, luciferase, Cre/Lox etc. check addgene
	plasmid_type NVARCHAR(200),
	ref_plasmid NVARCHAR(100),
	img_fn NVARCHAR(500), --upload image/ how the plasmid works
	addgene INT, --cite addgene plasmid# like, 12260
	d DATETIME,
	people_id INT NOT NULL, --the person who add this plasmid
	linear BIT, -- the plasmid is linear?	
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
('feature',  'Generic Feature'),
('promoter', 'Promoter'),
('primer', 'Primer'),
('enzyme',  'Enzyme'),
('gene', 'Gene'),
('origin', 'Origin'),
('regulatory',  'Regulatory'),
('terminator',  'Terminator'),
('exact_feature', 'Exact Feature'),
('orf', 'ORF');

--table to hold commonly used plasmid feature for auto genereation plasmid map
--main feature
CREATE TABLE common_feature
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	feature_id INT NOT NULL,
	label NVARCHAR(200) NOT NULL,
	[sequence] text NOT NULL,
	[des] TEXT,
	group_id INT NOT NULL,
	people_id INT, --the person who add this feature
	CONSTRAINT fk_common_feature_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT fk_common_feature_group_id FOREIGN KEY (group_id) REFERENCES [group](id),
	CONSTRAINT fk_common_feature_feature_id FOREIGN KEY (feature_id) REFERENCES plasmid_feature(id),
	CONSTRAINT uq_common_feature_label_group_id UNIQUE (label, group_id)
);


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
	common_id INT, --ref to common_feature id;
	clockwise INT NOT NULL, -- 1 or 0
	locked INT, -- 1 or 0
	[des] TEXT,
	CONSTRAINT fk_plasmid_map_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id),
	CONSTRAINT fk_plasmid_map_ccommon_id FOREIGN KEY (common_id) REFERENCES common_feature(id),
	CONSTRAINT fk_plasmid_map_feature_id FOREIGN KEY (feature_id) REFERENCES plasmid_feature(id)
);


--used to restore feature for automatically updating features when seq is provided
CREATE TABLE plasmid_map_backup
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
	common_id INT, --ref to common_feature id;
	clockwise INT NOT NULL, -- 1 or 0
	[des] TEXT,
	CONSTRAINT fk_plasmid_map_backup_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id),
	CONSTRAINT fk_plasmid_map_backup_ccommon_id FOREIGN KEY (common_id) REFERENCES common_feature(id),
	CONSTRAINT fk_plasmid_map_backup_feature_id FOREIGN KEY (feature_id) REFERENCES plasmid_feature(id)
);


--prevent the enzyme table to be changed by normal users
--restriction
CREATE TABLE restri_enzyme
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	forward_seq NVARCHAR(20) NOT NULL, --5' to 3'
	forward_cut INT NOT NULL, --cut after the num, first letter starts with 1, --5' to 3' ---- for keep most left cut position
	reverse_cut INT NOT NULL, --cut after num, first letter starts with 1, --3' to 5' ---- for keep most left cut position
	staractitivity BIT,
	inactivation INT, -- inactivation 20min at 65C (is 1), 80C  (is 2) or NO (0) 
	dam BIT,
	dcm BIT,
	cpg BIT,
	forward_cut2 INT, -- for keep most right cut position
	reverse_cut2 INT, -- for keep most right cut position
	--common BIT,
	CONSTRAINT uq_restriction_name UNIQUE (name)
);

--dam: 5'-GmATC-3' and 3'-CTmAG-5'
--dcm: 5'-CmCAGG-3' and 3'-GGTmCC-5' //and // 5'-CmCTGG-3' and 3'-GGAmCC-5'

CREATE TABLE seq_code
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(10) NOT NULL,
	necleotide NVARCHAR(10) NOT NULL,	
);


--insert data
INSERT INTO seq_code (name, necleotide) 
VALUES
('R', 'G'),
('R', 'A'),
('K', 'G'),
('K', 'T'),
('B', 'C'),
('B', 'G'),
('B', 'T'),
('Y', 'C'),
('Y', 'T'),
('S', 'C'),
('S', 'G'),
('D', 'A'),
('D', 'G'),
('D', 'T'),
('W', 'A'),
('W', 'T'),
('H', 'A'),
('H', 'C'),
('H', 'T'),
('N', 'A'),
('N', 'T'),
('N', 'G'),
('N', 'C'),
('M', 'A'),
('M', 'C'),
('V', 'A'),
('V', 'C'),
('V', 'G');


CREATE TABLE letter_code
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(10) NOT NULL,
	necleotide NVARCHAR(20) NOT NULL,	
);

--insert data
INSERT INTO letter_code (name, necleotide) 
VALUES
('R', 'G, A'),
('K', 'G, T'),
('B', 'C, G, T'),
('Y', 'C, T'),
('S', 'C, G'),
('D', 'A, G, T'),
('W', 'A, T'),
('H', 'A, C, T'),
('N', 'A, T, G, C'),
('M', 'A, C'),
('V', 'A, C, G');

--common used non ATGC letters
--R = G or A
--K= G or T
-- B = C, G or T
--Y = C or T
--S == C or G
--D = A, G or T
--W = A or T
--H = A, C or T
--N = G, A T or C
--M = A or C
--V = A, C or G

--need to add protein letter code

CREATE TABLE company
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	shortName NVARCHAR(100) NOT NULL,
	fullName NVARCHAR(500) NOT NULL,
	[des] TEXT
);

CREATE TABLE buffer
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	company_id INT NOT NULL,
	name NVARCHAR(100) NOT NULL,
	composition NVARCHAR(500) NOT NULL,
	show_activity BIT, -- show activity for restriction activity
	show_activity2 BIT, -- show activity for restriction activity
	[des] TEXT,
	CONSTRAINT fk_buffer_company_id FOREIGN KEY (company_id) REFERENCES company(id)
);

CREATE TABLE activity_restriction --activity of restriction enzyme
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	enzyme_id INT NOT NULL,
	company_id INT NOT NULL,
	buffer_id INT NOT NULL,
	temprature INT NOT NULL,  -- temp in 0C
	activity INT NOT NULL, --activity: 0 is <10%, 1 is 10%, 2 is 10-25%, 3 is 25%,  4 is 25%-50%, 5 is 50%, 6 is 50-75%,  7 is 75%, 8 is 75-100%, 9 is 100%
	CONSTRAINT fk_activity_restriction_enzyme_id FOREIGN KEY (enzyme_id) REFERENCES restri_enzyme(id),
	CONSTRAINT fk_activity_restriction_company_id FOREIGN KEY (company_id) REFERENCES company(id),
	CONSTRAINT fk_activity_restriction_buffer_id FOREIGN KEY (buffer_id) REFERENCES buffer(id)
);

--add table t store commonly used restriction enzymes and also used to auto generate plasmid map
CREATE TABLE common_restriction
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	enzyme_id INT NOT NULL,
	company_id INT NOT NULL,
	group_id INT NOT NULL,
	CONSTRAINT fk_common_restriction_enzyme_id FOREIGN KEY (enzyme_id) REFERENCES restri_enzyme(id),
	CONSTRAINT fk_common_restriction_company_id FOREIGN KEY (company_id) REFERENCES company(id),
	CONSTRAINT fk_common_restriction_group_id FOREIGN KEY (group_id) REFERENCES [group](id),
	CONSTRAINT uq_common_restriction_enzyme_id_group_id_company_id UNIQUE (enzyme_id,group_id,company_id)
);

 
CREATE TABLE modifying_enzyme
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(250) NOT NULL,
	category NVARCHAR(250) NOT NULL, -- low level categoty
	[application] TEXT
);


CREATE TABLE activity_modifying --activity of modifying enzyme
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	enzyme_id INT NOT NULL,
	company_id INT NOT NULL,
	buffer_id INT NOT NULL,
	temprature INT NOT NULL,  -- temp in 0C
	activity INT NOT NULL, --activity: 0 is <10%, 1 is 10%, 2 is 10-25%, 3 is 25%,  4 is 25%-50%, 5 is 50%, 6 is 50-75%,  7 is 75%, 8 is 75-100%, 9 is 100%
	CONSTRAINT fk_activity_modifying_enzyme_id FOREIGN KEY (enzyme_id) REFERENCES modifying_enzyme(id),
	CONSTRAINT fk_activity_modifying_company_id FOREIGN KEY (company_id) REFERENCES company(id),
	CONSTRAINT fk_activity_modifying_buffer_id FOREIGN KEY (buffer_id) REFERENCES buffer(id)
);



--add table t store commonly used modifying enzymes and also used to auto generate plasmid map
CREATE TABLE common_modifying
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	enzyme_id INT NOT NULL,
	company_id INT NOT NULL,
	group_id INT NOT NULL,
	CONSTRAINT fk_common_modifying_enzyme_id FOREIGN KEY (enzyme_id) REFERENCES modifying_enzyme(id),
	CONSTRAINT fk_common_modifying_company_id FOREIGN KEY (company_id) REFERENCES company(id),
	CONSTRAINT fk_common_modifying_group_id FOREIGN KEY (group_id) REFERENCES [group](id),
	CONSTRAINT uq_common_modifying_enzyme_id_group_id_company_id UNIQUE (enzyme_id,group_id,company_id)
);


-- link restriciton enzyme to company
CREATE TABLE restriction_company
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	enzyme_id INT NOT NULL,
	company_id INT NOT NULL,
	CONSTRAINT fk_restriction_company_enzyme_id FOREIGN KEY (enzyme_id) REFERENCES restri_enzyme(id),
	CONSTRAINT fk_restriction_company_company_id FOREIGN KEY (company_id) REFERENCES company(id),
	CONSTRAINT uq_restriction_company_enzyme_id_company_id UNIQUE (enzyme_id,company_id)
);


--link modifying enzyme to company_id
CREATE TABLE modifying_company
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	enzyme_id INT NOT NULL,
	company_id INT NOT NULL,
	CONSTRAINT fk_modifying_company_enzyme_id FOREIGN KEY (enzyme_id) REFERENCES modifying_enzyme(id),
	CONSTRAINT fk_modifying_company_company_id FOREIGN KEY (company_id) REFERENCES company(id),
	CONSTRAINT uq_modifying_company_enzyme_id_company_id UNIQUE (enzyme_id,company_id)
);


--dam enzyme table
--GmATC
--first check the appending letter and then locate where is "GA" or "GAT" is, if it is appending with "TC", or "C" then it is affected!!!
CREATE TABLE Dam
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	COverlapping BIT NOT NULL, --completely overlapping, true = completely, false= paritially
	CBlocked BIT NOT NULL, --complately blocked or paritally impaired, true = completely, false= paritially	
	appending NVARCHAR(100) --appending letter to check	
);
/*
INSERT INTO [Dam]
VALUES
--completely overlapping and completely blocked
('BclI', 1, 1, null),
('BspPI', 1, 1, null),
('MboI', 1, 1, null),
('DpnII', 1, 1, null),
('AlwI', 1, 1, null),


--partially overpping and completely blocked
('Bsu15I', 0, 1, 'C'),
('HphI', 0, 1, 'TC'),
('MboII', 0, 1, 'TC'),
('TaqI', 0, 1, 'TC'),
('XbaI', 0, 1, 'TC'),
('NruI', 0, 1, 'TC'),
('Hpy188I', 0, 1, 'TC'),
('Hpy188III', 0, 1, 'TC'),
('ClaI', 0, 1, 'C'),
('BspHI', 0, 1, 'TC'),
('BspEI', 0, 1, 'TC'),
('BspDI', 0, 1, 'C'),

--in middle
('BdaI', 0, 0, 'TC'),
('BseJI', 0, 0, 'C'),
('Hin4I', 0, 0, 'C'),
('BsaB1', 0, 0, 'C'),


--partially overlapping and partially blocked
('PagI', 0, 0, 'TC'),
('PfoI', 0, 0, 'TC'),
('SmoI', 0, 0, 'TC'),
--need to be careful to deel with, in middle
('BcgI', 0, 0, 'TC');
*/

--CCAGG or CCTGG
CREATE TABLE Dcm
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(100) NOT NULL,
	COverlapping BIT NOT NULL, --completely overlapping, true = completely, false= paritially
	CBlocked BIT NOT NULL, --complately blocked or paritally impaired, true = completely, false= paritially
	prefixing NVARCHAR(100), --appending letter to check	
	appending NVARCHAR(100) --appending letter to check	
);
/*
INSERT INTO [Dcm]
VALUES
--completely overlapping and completely blocked
('EcoRII', 1, 1, null,null),

--partially overpping and completely blocked
('Bme1390I', 0, 1, null, null), --in middle
('Bsp120I', 0, 1, null,'WGG'), 
('CaiI', 0, 1, null,null), 
('CfrI', 0, 1, null,'GG'), 
('Cfr13I', 0, 1, null,'WGG'),
('Eco47I', 0, 1, null,'WGG'),
('Eco57MI', 0, 1, 'C',null),
('Eco147I', 0, 1, null,'GG'),
('Eco0109I', 0, 1, null,'GG'),
('GsuI', 0, 1, null,'G'),
('MlsI', 0, 1, null,'GG'),
('PfoI', 0, 1, null,null), --in middle
('Psp5II', 0, 1, null,'GG'),
('Van91I', 0, 1, null,'GG'),

--partially overlapping and partially blocked
('Acc65I', 0, 0, null,'WGG'),
('ApaI', 0, 0, null,'WGG'),
('BseLI', 0, 0, null,'WGG'),
('BshNI', 0, 0, null,'WGG'),
('Eco31I', 0, 0, 'CCW',null),
('Hin1I', 0, 0, null,'WGG'),
('SfiI', 0, 0, null,'WGG');
*/
--table to keep the dam and dcm on plasmid map
CREATE TABLE methylation
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	plasmid_id INT NOT NULL,
	cut INT NOT NULL,
	clockwise INT NOT NULL,
	name NVARCHAR(100) NOT NULL,
	dam_complete BIT NOT NULL,
	dam_impaired BIT NOT NULL,
	dcm_complete BIT NOT NULL,
	dcm_impaired BIT NOT NULL,
	CONSTRAINT fk_methylation_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id)
);
--to backup the methylation data
CREATE TABLE methylation_backup
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	plasmid_id INT NOT NULL,
	cut INT NOT NULL,
	clockwise INT NOT NULL,
	name NVARCHAR(100) NOT NULL,
	dam_complete BIT NOT NULL,
	dam_impaired BIT NOT NULL,
	dcm_complete BIT NOT NULL,
	dcm_impaired BIT NOT NULL,
	CONSTRAINT fk_methylation_backup_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id)
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
	CONSTRAINT uq_primer_name_people_id UNIQUE (name, people_id)
	--CONSTRAINT uq_primer_name UNIQUE (name)
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
	dt DATETIME DEFAULT GETDATE(),
	CONSTRAINT fk_probe_tech_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT fk_probe_forward_primer FOREIGN KEY (forward_primer) REFERENCES primer(id),
	CONSTRAINT fk_probe_reverse_primer FOREIGN KEY (reverse_primer) REFERENCES primer(id),
	CONSTRAINT uq_probe_name UNIQUE (name)
);

--plasmid bundle table
CREATE TABLE plasmid_bundle
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	bundle_id INT NOT NULL,
	name NVARCHAR(200) NOT NULL, --bundle name
	[des] TEXT, --bundle remarks
	member_type NVARCHAR(100) NOT NULL, --the resource type of the member, like plasmid, primer, etc. most of the time, it is plasmid
	member_id INT NOT NULL, --ref to the member id
	member_role TEXT, --why the member is inculded?
	--ref_bundle INT NOT NULL, --default to be 0, 0 is the top level
	img_fn NVARCHAR(200), --for upload img scheme
	dt DATETIME DEFAULT GETDATE(),
	people_id INT NOT NULL, --who created this bundle
	CONSTRAINT fk_plasmid_bundle_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	--CONSTRAINT uq_plasmid_bundle_name_people_id UNIQUE (name, people_id) -- in the same group it is allowed with the same bundle name
);
--=======================================================================================================================================
--------------------------------------------------------
--this table keep the used save fragement of a plasmid, not the real clone fragment
--plasmid fragment will be wiped away when the seq of the plasmid is changed
--this table will store all the info about 1) how the fragment is genereated from a plasmid;
CREATE TABLE fragment
(
    id INT NOT NULL PRIMARY KEY IDENTITY(100,1),
    name NVARCHAR(200) NOT NULL, -- the name of the fragment, follow: plasmid id-enzyme-start-end position

    --info about how the fragment is generated
    plasmid_id INT, --from plasmids
    fragment_id INT, --from fragments
    parantal BIT NOT NULL, -- if the fragment has parental plasmid or fragment   
    enzyme_id NVARCHAR(100), --what enzymes are used, 1 or 2 enzyme in most cases. it should be a "," seperated string of enzyme ids
    company_id NVARCHAR(100), --the same order as enzyme, it should be a "," seperated string of company ids
    buffer_id NVARCHAR(100), --the same order as enzyme, it should be a "," seperated string of buffer ids

    --info about the fragment itself
    --fill below if the fragment is generated from a known plasmid or fragment
    forward_start INT,
    forward_end INT,
    --keep this info if the fragment is added manually without parental plasmid or fragment
    forward_size INT,
    forward_seq TEXT,
    --fill below if the fragment is generated from a known plasmid or fragment or fragment
    rc_start INT, --reverse complement
    rc_end INT,
    --keep this info if the fragment is added manually without parental plasmid or fragment
    rc_size INT, --reverse complement
    rc_seq TEXT,
    rc_left_overhand INT,  --set the first letter of forward seq to be 1 and the "rc_left_overhand" is a number relative to 1: -1 means one letter to left and 1 means one letter to the right, 0 means no overhang
    rc_right_overhand INT,

    ladder_id INT, -- to restore the digest setting
    people_id INT NOT NULL, --name + people_id shoud be unique
    dt DATETIME NOT NULL DEFAULT GETDATE(),
    [des] TEXT,
    CONSTRAINT fk_plasmid_fragment_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id),
    CONSTRAINT fk_plasmid_fragment_people_id FOREIGN KEY (people_id) REFERENCES people(id),
    CONSTRAINT uq_plasmid_fragment_name_people_id UNIQUE (name, people_id)
);


--only for manually added fragments
--need use addgene giraffe
CREATE TABLE fragment_map
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	fragment_id INT NOT NULL,
	--type_id INT, --linear or circlular
	show_feature INT NOT NULL, -- 1 or 0
	feature NVARCHAR(200) NOT NULL,
	feature_id INT NOT NULL,
	start INT NOT NULL,
	[end] INT NOT NULL,
	cut INT, --enzyme cut only clude unique and 2 cuts
	common_id INT, --ref to common_feature id;
	clockwise INT NOT NULL, -- 1 or 0
	locked INT, -- 1 or 0
	[des] TEXT,
	CONSTRAINT fk_fragment_map_fragment_id FOREIGN KEY (fragment_id) REFERENCES fragment(id),
	CONSTRAINT fk_fragment_map_common_id FOREIGN KEY (common_id) REFERENCES common_feature(id),
	CONSTRAINT fk_fragment_map_feature_id FOREIGN KEY (feature_id) REFERENCES plasmid_feature(id)
);
--table to keep the dam and dcm on fragment map
CREATE TABLE fragment_methylation
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	fragment_id INT NOT NULL,
	cut INT NOT NULL,
	clockwise INT NOT NULL,
	name NVARCHAR(100) NOT NULL,
	dam_complete BIT NOT NULL,
	dam_impaired BIT NOT NULL,
	dcm_complete BIT NOT NULL,
	dcm_impaired BIT NOT NULL,
	CONSTRAINT fk_fragment_methylation_fragment_id FOREIGN KEY (fragment_id) REFERENCES fragment(id)
);
-----------------------------------------------
--=======================================================================================================================================

--table for group sharing
CREATE TABLE group_shared
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	group_id INT NOT NULL,
	resource_id INT NOT NULL, --plasmid, oligo, primer etc, anything that can be shared
	category NVARCHAR(50), --plasmid, primer, oligo, probe. fragment etc
	sratus NVARCHAR(50), --submitted, appproved
	CONSTRAINT fk_group_shared_group_id FOREIGN KEY (group_id) REFERENCES [group](id),
	CONSTRAINT uq_group_id_resource_id_category UNIQUE (group_id, resource_id, category)
);

--ladder, for both DNA and protein
CREATE TABLE ladder
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	ladder_type NVARCHAR(50) NOT NULL, --DNA, RNA or protein
	name NVARCHAR(100) NOT NULL,
	min_bp_kDa INT NOT NULL,
	max_bp_kda INT NOT NULL,
	company_id INT NOT NULL,
	orderref NVARCHAR(100),
	a_values float, --y = ax+b
	b_value float, --y = ax+b
	CONSTRAINT uq_ladder_company_id_name UNIQUE (company_id, name),
	CONSTRAINT fk_ladder_company_id FOREIGN KEY (company_id) REFERENCES company(id)
);
--ladder size
CREATE TABLE ladder_size
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	ladder_id INT NOT NULL,
	size INT NOT NULL, --bp or kDa
	mass INT NOT NULL,
	Rf float NOT NULL,
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

--------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------tables for adding experiments---------------------------------------------------------------------------
CREATE TABLE protocol 
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(500) NOT NULL,
	upload NVARCHAR(500),
	version INT,
	versionref INT, --ref to previous version
	people_id INT,
	[des] TEXT,
	type_id INT, --for keep the linked protocol together, should be the same for the linked ptotocol
	dt DATETIME,
	CONSTRAINT fk_protocol_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT uq_protocol_name_version UNIQUE ([name],[version])
);

CREATE TABLE experiment
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(200),
	[des] TEXT,
	people_id INT NOT NULL,
	dt DATETIME, --date of creation
	CONSTRAINT fk_experiment_people_id FOREIGN KEY (people_id) REFERENCES people(id),
);

--ligation
CREATE TABLE exp_type
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(200),
	[des] TEXT
);

INSERT INTO exp_type VALUES
('Pick Colony', 'Pick Colony for expanding the plasmid'),
('Plasmid Transduction', 'Plasmid Transduction'),
('Plasmid Transfection', 'Plasmid Transfection'),
('Plasmid Miniprep','Plasmid Miniprep'),
('Restriction Enzyme Digestion', 'Plasmid Enzyme Digestion'), 
('Fragment Gel Extraction', 'Fragment Gel Extraction'),
('PCR', 'PCR'), 
('Ligation', 'ligate 2 fragments into a plasmid'),
('Restriction Enzyme Digestion', 'Plasmid Enzyme Digestion'), 
('Plasmid Maxiprep', 'Plasmid Maxiprepp');


--experiment types
--general types 
CREATE TABLE exp_step
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	name NVARCHAR(200),
	exp_id INT NOT NULL,
	type_id INT NOT NULL, --ligation exp_type id
	step_id INT NOT NULL,
	protocol_id INT,
	[des] TEXT,
	people_id INT NOT NULL,
	dt DATETIME, --date of creation
	CONSTRAINT fk_exp_step_people_id FOREIGN KEY (people_id) REFERENCES people(id),
	CONSTRAINT fk_exp_step_exp_id FOREIGN KEY (exp_id) REFERENCES experiment(id),
	CONSTRAINT fk_exp_step_type_id FOREIGN KEY (type_id) REFERENCES exp_type(id),
	CONSTRAINT fk_exp_step_protocol_id FOREIGN KEY (protocol_id) REFERENCES protocol(id),
	CONSTRAINT uq_exp_step_exp_id_step_id UNIQUE ([exp_id],[step_id])
);

CREATE TABLE exp_step_material
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	exp_id INT NOT NULL,
	exp_step_id INT NOT NULL, --link to ligation exp
	forward_primer_id INT, 
	reverse_primer_id INT,
	probe_id INT,
	emzyme_id NVARCHAR(200), --for digestion, ligarion, etc--link to enzyme ids, can be multiple enzymes, thus should be string and seprated by  '-'
	plasmid_id INT, --if plasmid is used
	frag1_id INT, --1st fragment for ligation
	frag2_id INT,--2nd fragment for ligation
	[des] TEXT, --comment
	dt DATETIME, 
	CONSTRAINT fk_exp_step_material_exp_id FOREIGN KEY (exp_id) REFERENCES experiment(id),
	CONSTRAINT fk_exp_step_material_forward_primer_id FOREIGN KEY (forward_primer_id) REFERENCES primer(id),
	CONSTRAINT fk_exp_step_material_reverse_primer_id FOREIGN KEY (reverse_primer_id) REFERENCES primer(id),
	CONSTRAINT fk_exp_step_material_probe_id FOREIGN KEY (probe_id) REFERENCES probe(id),
	CONSTRAINT fk_exp_step_material_plasmid_id FOREIGN KEY (plasmid_id) REFERENCES plasmid(id),
	CONSTRAINT fk_exp_step_material_frag1_id FOREIGN KEY (frag1_id) REFERENCES fragment(id),
	CONSTRAINT fk_exp_step_material_frag2_id FOREIGN KEY (frag2_id) REFERENCES fragment(id)
);
CREATE TABLE exp_step_result
(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	exp_id INT NOT NULL,
	exp_step_id INT NOT NULL, --link to ligation exp
	result_id INT NOT NULL,
	result_upload NVARCHAR(200),
	result_des TEXT,
	dt DATETIME, 
	CONSTRAINT fk_exp_step_result_exp_id FOREIGN KEY (exp_id) REFERENCES experiment(id)
);

--for form dropdown items--
CREATE TABLE dropdownitem
(
		id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		value NVARCHAR(100),
		[text] NVARCHAR(100),
		category NVARCHAR(100)
);
INSERT INTO dropdownitem VALUES

--plasmid expression system
('Mammalian', 'Mammalian', 'ExpSystem'),
('Bacteria', 'Bacteria', 'ExpSystem'),
('Insect', 'Insect', 'ExpSystem'),
('Worms', 'Worms', 'ExpSystem'),
('Yeast', 'Yeast', 'Yeast'),
('Plant', 'Plant', 'ExpSystem'),

--plasmid expeimental usage
('Mouse Targeting', 'Mouse Targeting', 'PlasmidUse'),
('Lentiviral', 'Lentiviral', 'PlasmidUse'),
('Retroviral', 'Retroviral', 'PlasmidUse'),
('Adenoviral', 'Adenoviral', 'PlasmidUse'),
('AAV', 'AAV', 'PlasmidUse'),
('RNAi', 'RNAi', 'PlasmidUse'),
('Cre/Lox', 'Cre/Lox', 'PlasmidUse'),
('CRISPR', 'CRISPR', 'PlasmidUse'),
('TALEN', 'TALEN', 'PlasmidUse'),
('Luciferase', 'Luciferase', 'PlasmidUse'),
('Synthetic Biology', 'Synthetic Biology', 'PlasmidUse'),
('Other', 'Other', 'PlasmidUse'),
('Unspecified', 'Unspecified', 'PlasmidUse'),

--plasmid type
('Encodes one insert', 'Encodes one insert', 'PlasmidType'),
('Encodes gRNA/shRNA', 'Encodes gRNA/shRNA', 'PlasmidType'),
('Empty backbone', 'Empty backbone', 'PlasmidType'),
('Encodes multiple inserts', 'Encodes multiple inserts', 'PlasmidType'),
('Pooled library', 'Pooled library', 'PlasmidType'),
('Bacterial strain', 'Bacterial strain', 'PlasmidType'),

--plasmid species
('Human', 'Human', 'InsertSpecies'),
('Mouse', 'Mouse', 'InsertSpecies'),
('Rat', 'Rat', 'InsertSpecies'),
('Chicken', 'Chicken', 'InsertSpecies'),
('Bovine', 'Bovine', 'InsertSpecies'),
('Frog', 'Frog', 'InsertSpecies'),
('Zebrafish', 'Zebrafish', 'InsertSpecies'),
('Fly', 'Fly', 'InsertSpecies'),
('Nematode', 'Nematode', 'InsertSpecies'),
('Budding Yeast', 'Budding Yeast', 'InsertSpecies'),
('Fission Yeast', 'Fission Yeast', 'InsertSpecies'),
('Mustard Weed', 'Mustard Weed', 'InsertSpecies'),
('Synthetic', 'Synthetic', 'InsertSpecies'),
('Other', 'Other', 'InsertSpecies'),

--slectable marker
('Neomycin', 'Neomycin', 'SelectMarker'),
('Puromycin', 'Puromycin', 'SelectMarker'),
('Hygromycin', 'Hygromycin', 'SelectMarker'),
('Zeocin', 'Zeocin', 'SelectMarker'),
('Blasticidin', 'Blasticidin', 'SelectMarker'),
('Gentamicin', 'Gentamicin', 'SelectMarker'),
('TRP1', 'TRP1', 'SelectMarker'),
('LEU2', 'LEU2', 'SelectMarker'),
('URA3', 'URA3', 'SelectMarker'),
('HIS3', 'HIS3', 'SelectMarker'),
('Basta', 'Basta', 'SelectMarker'),
('Other', 'Other', 'SelectMarker'),


--Resistance
('Ampicillin', 'Ampicillin', 'Resistance'),
('Kanamycin', 'Kanamycin', 'Resistance'),
('Tetracycline', 'Tetracycline', 'Resistance'),
('Chloramphenicol', 'Chloramphenicol', 'Resistance'),
('Streptomycin', 'Streptomycin', 'Resistance'),
('Hygromycin', 'Hygromycin', 'Resistance'),
('Gentamycin', 'Gentamycin', 'Resistance'),
('Bleocin(Zeocin)', 'Bleocin(Zeocin)', 'Resistance'),
('Spectinomycin', 'Spectinomycin', 'Resistance'),
('Nourseothricin(clonNat)', 'Nourseothricin(clonNat)', 'Resistance'),
('Other', 'Other', 'Resistance'),

--promotors
('ACT5C', 'ACT5C', 'Promotor'),
('COPIA', 'COPIA', 'Promotor'),
('UBC', 'UBC', 'Promotor'),
('SV40', 'SV40', 'Promotor'),
('EF1a', 'EF1a', 'Promotor'),
('SFFV', 'SFFV', 'Promotor'),
('PGK', 'PGK', 'Promotor'),
('CMV', 'CMV', 'Promotor'),
('CAG', 'CAG', 'Promotor'),
('Other', 'Other', 'Promotor'),

--reporter
('GFP', 'GFP', 'Reporter'),
('DsRed', 'DsRed', 'Reporter'),
('Katushka', 'Katushka', 'Reporter'),
('mCherry', 'mCherry', 'Reporter'),
('BFP', 'BFP', 'Reporter'),
('Luciferase', 'Luciferase', 'Reporter'),
('LacZ', 'LacZ', 'Reporter'),
('Other', 'Other', 'Reporter'),

--POLY A
('rBGpA', 'rBGpA', 'PolyA'),
('TKpA', 'TKpA', 'PolyA'),
('bGHpA', 'bGHpA', 'PolyA'),
('SV40pA', 'SV40pA', 'PolyA'),
('Other', 'Other', 'PolyA'),

--0/1--
--true /false
('1', 'Yes', 'YN01'),
('0', 'No', 'YN01'),

--true /false
('true', 'Yes', 'TF'),
('false', 'No', 'TF'),

--star activity
('0', 'No', 'StarActivity'),
('1', '65°C, 20 min', 'StarActivity'),
('2', '80°C, 20 min', 'StarActivity'),

-- modifying enzyme category

('DNA Ligase', 'DNA Ligase', 'MEnzyme'),
('RNA Ligase', 'RNA Ligase', 'MEnzyme'),
('DNA Polymerase', 'DNA Polymerase', 'MEnzyme'),
('RNA Polymerase', 'RNA Polymerase', 'MEnzyme'),
('RNase Inhibitor', 'RNase Inhibitor', 'MEnzyme'),
('Phosphatase', 'Phosphatase', 'MEnzyme'),
('Kinase', 'Kinase', 'MEnzyme'),
('Ribonuclease (RNase)', 'Ribonuclease (RNase)', 'MEnzyme'),
('Deoxyribonuclease (DNase)', 'Deoxyribonuclease (DNase)', 'MEnzyme'),
('Other', 'Other', 'MEnzyme'),
('DNA Labeling', 'DNA Labeling', 'MEnzyme'),
('ssDNA Binding Proteins', 'ssDNA Binding Proteins', 'MEnzyme'),
('Nuclease', 'Nuclease', 'MEnzyme'),
('Methyltransferase', 'Methyltransferase', 'MEnzyme'),
('DNA Repair Protein', 'DNA Repair Protein', 'MEnzyme'),

-- buffer activity
('0', '< 10%', 'BufferActivity'),
('1', '10%', 'BufferActivity'),
('2', '< 10 - 25%', 'BufferActivity'),
('3', '25%', 'BufferActivity'),
('4', '25 - 50%', 'BufferActivity'),
('5', '50%', 'BufferActivity'),
('6', '50 - 75%', 'BufferActivity'),
('7', '75%', 'BufferActivity'),
('8', '75 - 100%', 'BufferActivity'),
('9', '100%', 'BufferActivity'),

--ladder type
('DNA', 'DNA', 'ladder'),
('RNA', 'RNA', 'ladder'),
('Protein', 'Protein', 'ladder')

--app roles
('GroupLeader', 'Group Leader', 'appRole'),
('Assistant', 'Group Assistant', 'appRole'),
('InstAdmin', 'Institute Administrator', 'appRole')

;










