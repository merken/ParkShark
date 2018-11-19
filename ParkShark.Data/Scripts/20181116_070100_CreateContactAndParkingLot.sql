USE [ParkSharkDb]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[MobilePhone] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Street] [nvarchar](255) NOT NULL,
	[StreetNumber] [nvarchar](50) NOT NULL,
	[PostalCode] [nvarchar](15) NOT NULL,
	[PostalName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ParkingLots](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[DivisionId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[BuildingType] [nvarchar](100) NOT NULL,
	[PricePerHour] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_ParkingLots] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ParkingLots]  WITH CHECK ADD  CONSTRAINT [FK_ParkingLots_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([Id])
GO

ALTER TABLE [dbo].[ParkingLots] CHECK CONSTRAINT [FK_ParkingLots_Contacts]
GO

ALTER TABLE [dbo].[ParkingLots]  WITH CHECK ADD  CONSTRAINT [FK_ParkingLots_Divisions] FOREIGN KEY([DivisionId])
REFERENCES [dbo].[Divisions] ([Id])
GO

ALTER TABLE [dbo].[ParkingLots] CHECK CONSTRAINT [FK_ParkingLots_Divisions]
GO

ALTER TABLE [dbo].[ParkingLots] ADD [Capacity] [decimal](18, 2) NOT NULL
GO