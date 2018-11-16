USE [ParkSharkDb]
GO

ALTER TABLE [dbo].[Divisions] ADD [ParentDivisionId] [int] NULL
GO

ALTER TABLE [dbo].[Divisions]  WITH CHECK ADD  CONSTRAINT [FK_Divisions_Divisions] FOREIGN KEY([ParentDivisionId])
REFERENCES [dbo].[Divisions] ([Id])
GO

ALTER TABLE [dbo].[Divisions] CHECK CONSTRAINT [FK_Divisions_Divisions]
GO

ALTER TABLE [dbo].[ParkingLots] ADD [Capacity] [decimal](18, 2) NOT NULL
GO