USE [ecloning]
GO
SET IDENTITY_INSERT [dbo].[modifying_enzyme] ON 

INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (2, N'Klenow Fragment', N'DNA Polymerase', N'
    1. DNA blunting by fill-in 5''- overhangs
    2. Random-primed DNA labeling
    3. Labeling by fill-in 5''- overhangs of dsDNA
    4. DNA sequencing
    5. Site-specific mutagenesis
    6. Second strand cDNA synthesis
')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (3, N'DNA Polymerase I', N'DNA Polymerase', N'    1. DNA labeling by nick-translation
    2. Second strand cDNA synthesis')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (4, N'Klenow Fragment, exo-', N'DNA Polymerase', N'    1. Random-primed DNA labeling
    2. Labeling by fill-in 5''- overhangs of dsDNA
    3. Strand displacement amplification
    4. DNA sequencing')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (5, N'T4 DNA Polymerase', N'DNA Polymerase', N'    1. Blunting of DNA with 5''- or 3''- overhangs
    2. Blunting of PCR products with 3''-dA overhangs
    3. DNA labeling
    4. Site-directed mutagenesis
    5. Ligation-independent cloning')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (6, N'T7 DNA Polymerase', N'DNA Polymerase', N'    1. Purification of covalently closed circular DNA
    2. Primer extension reactions on long templates
    3. DNA 3''-end labeling
    4. Strand extensions in site directed mutagenesis
    5. Fill-in blunting of 5''-overhang DNA
    6. Second strand cDNA synthesis
    7. In situ detection of DNA fragmentation associated with apoptosis')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (7, N'Exonuclease I (Exo I)', N'Deoxyribonuclease (DNase)', N'    1. PCR product clean-up
    2. Removal of ssDNA from nucleic acid mixtures')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (8, N'Exonuclease III (Exo III)', N'Deoxyribonuclease (DNase)', N'    1. Creation of unidirectional deletions in DNA fragments
    2. Generation of ssDNA for sequencing
    3. Site-directed mutagenesis
    4. Preparation of strand specific probes')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (9, N'T4 Polynucleotide Kinase', N'Kinase', N'Catalyzes the transfer and exchange of Pi from the γ position of ATP to the 5′ -hydroxyl terminus of polynucleotides (double-and single-stranded DNA and RNA) and nucleoside 3′-monophosphates. Polynucleotide Kinase also catalyzes the removal of 3′-phosphoryl groups from 3′-phosphoryl polynucleotides, deoxynucleoside 3′-monophosphates and deoxynucleoside 3′-diphosphates (1).
Highlights:
    Isolated from a recombinant source
    5'' end labeling of DNA and RNA
    RNase and DNase free
    Supplied with 10X Reaction Buffer
')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (10, N' Shrimp Alkaline Phosphatase (rSAP) ', N'Phosphatase', N'Shrimp Alkaline Phosphatase (rSAP) is a heat labile alkaline phosphatase purified from a recombinant source. rSAP is identical to the native enzyme and contains no affinity tags or other modifications. rSAP nonspecifically catalyzes the dephosphorylation of 5′ and 3′ ends of DNA and RNA phosphomonoesters. Also, rSAP hydrolyses ribo-, as well as deoxyribonucleoside triphosphates (NTPs and dNTPs). rSAP is useful in many molecular biology applications such as the removal of phosphorylated ends of DNA and RNA for subsequent use in cloning or end-labeling of probes. In cloning, dephosphorylation prevents religation of linearized plasmid DNA. The enzyme acts on 5′ protruding, 5′ recessed and blunt ends. rSAP may also be used to degrade unincorporated dNTPs in PCR reactions to prepare templates for DNA sequencing or SNP analysis. rSAP is completely and irreversibly inactivated by heating at 65°C for 5 minutes, thereby making removal of rSAP prior to ligation or end-labeling unnecessary. ')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (11, N'Alkaline Phosphatase, Calf Intestinal (CIP)', N'Phosphatase', N'Alkaline Phosphatase, Calf Intestinal (CIP) nonspecifically catalyzes the dephosphorylation of 5′ and 3′ ends of DNA and RNA phosphomonoesters. Also, CIP hydrolyses ribo-, as well as deoxyribonucleoside triphosphates (NTPs and dNTPs). CIP is useful in many molecular biology applications such as the removal of phosphorylated ends of DNA and RNA for subsequent use in cloning or end-labeling of probes. In cloning, dephosphorylation prevents religation of linearized plasmid DNA. The enzyme acts on 5′ protruding, 5′ recessed and blunt ends. CIP may also be used to degrade unincorporated dNTPs in PCR reactions to prepare templates for DNA sequencing or SNP analysis. ')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (12, N' Cas9 Nuclease NLS, S. pyogenes ', N'Nuclease', N'Cas9 Nuclease NLS, S. pyogenes, is an RNA-guided endonuclease that catalyzes sitespecific cleavage of double stranded DNA. The location of the break is within the target sequence 3 bases from the NGG PAM (Protospacer Adjacent Motif) (1). The PAM sequence, NGG, must follow the targeted region on the opposite strand of the DNA with respect to the region complementary sgRNA sequence. Cas9 Nuclease NLS, S. pyogenes contains a single Simian virus 40 (SV40) T antigen nuclear localization sequence (NLS) on the C terminus of the protein. ')
INSERT [dbo].[modifying_enzyme] ([id], [name], [category], [application]) VALUES (13, N'Cas9 Nuclease, S. pyogenes', N'Nuclease', N'Cas9 Nuclease, S. pyogenes, is an RNA-guided endonuclease that catalyzes site-specific cleavage of double stranded DNA. The location of the break is within the target sequence 3 bases from the NGG PAM (Protospacer Adjacent Motif) (1). The PAM sequence, NGG, must follow the targeted region on the opposite strand of the DNA with respect to the region complementary sgRNA sequence.')
SET IDENTITY_INSERT [dbo].[modifying_enzyme] OFF
