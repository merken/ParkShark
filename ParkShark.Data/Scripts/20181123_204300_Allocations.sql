USE [ParkSharkDb]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Allocations](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[MemberId] [int] NOT NULL,
	[ParkingLotId] [int] NOT NULL,
	[StartDateTime] [datetime2](7) NOT NULL,
	[EndDateTime] [datetime2](7) NULL,
	[LicensePlateNumber] [nvarchar](255) NOT NULL,
	[LicensePlateCountry] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Allocations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Allocations] ADD  CONSTRAINT [DF_Allocations_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Allocations]  WITH CHECK ADD  CONSTRAINT [FK_Allocations_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO

ALTER TABLE [dbo].[Allocations] CHECK CONSTRAINT [FK_Allocations_Members]
GO

ALTER TABLE [dbo].[Allocations]  WITH CHECK ADD  CONSTRAINT [FK_Allocations_ParkingLots] FOREIGN KEY([ParkingLotId])
REFERENCES [dbo].[ParkingLots] ([Id])
GO

ALTER TABLE [dbo].[Allocations] CHECK CONSTRAINT [FK_Allocations_ParkingLots]
GO