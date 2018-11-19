USE [ParkSharkDb]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BuildingTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_BuildingTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/*RESEED, Make sure the first ID starts with 1*/
DBCC CHECKIDENT('[BuildingTypes]', RESEED, 1)
GO

INSERT INTO [dbo].[BuildingTypes] VALUES('Underground')
GO

INSERT INTO [dbo].[BuildingTypes] VALUES('Aboveground')
GO

/*Add NULLable building type id FK column to ParkingLots */
ALTER TABLE [dbo].[ParkingLots] ADD [BuildingTypeId] int NULL
GO

/*Migrate old data from string column to FK reference, this is why we needed the RESEED*/
UPDATE [dbo].[ParkingLots] SET [BuildingTypeId] = 1 WHERE [BuildingType] = 'Underground'
GO

UPDATE [dbo].[ParkingLots] SET [BuildingTypeId] = 2 WHERE [BuildingType] = 'Aboveground'
GO

/*Alter building type id FK column to NOT NULL*/
ALTER TABLE [dbo].[ParkingLots] ALTER COLUMN [BuildingTypeId] int NOT NULL
GO

/*Create the FK relation*/
ALTER TABLE [dbo].[ParkingLots]  WITH CHECK ADD  CONSTRAINT [FK_ParkingLots_BuildingTypes] FOREIGN KEY([BuildingTypeId])
REFERENCES [dbo].[BuildingTypes] ([Id])
GO

/*Check the FK relation*/
ALTER TABLE [dbo].[ParkingLots] CHECK CONSTRAINT [FK_ParkingLots_Contacts]
GO

/*Drop old column*/
ALTER TABLE [dbo].[ParkingLots] DROP COLUMN [BuildingType]
GO