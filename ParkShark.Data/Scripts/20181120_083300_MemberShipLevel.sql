USE [ParkSharkDb]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MemberShipLevels](
	[Name] [nvarchar](50) NOT NULL,
	[MonthlyCost] [decimal](18, 2) NOT NULL,
	[AllocationReduction] [decimal](18, 2) NOT NULL,
	[MaximumDurationInMinutes] [decimal](18, 0) NOT NULL,
 CONSTRAINT [PK_MemberShipLevels] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[MemberShipLevels] VALUES('Bronze', 0, 0, 240)
GO

INSERT INTO [dbo].[MemberShipLevels] VALUES('Silver', 10, 20, 360)
GO

INSERT INTO [dbo].[MemberShipLevels] VALUES('Gold', 40, 30, 1440)
GO

ALTER TABLE [dbo].[Members] ADD MemberShipLevel [nvarchar](50) NULL
GO

/*Default set to Bronze*/
UPDATE  [dbo].[Members] SET MemberShipLevel = 'Bronze'
GO

ALTER TABLE [dbo].[Members] ALTER COLUMN MemberShipLevel [nvarchar](50) NOT NULL
GO

ALTER TABLE [dbo].[Members]  WITH CHECK ADD  CONSTRAINT [FK_Members_MemberShipLevel] FOREIGN KEY([MemberShipLevel])
REFERENCES [dbo].[MemberShipLevels] ([Name])
GO

ALTER TABLE [dbo].[Members] CHECK CONSTRAINT [FK_Members_MemberShipLevel]
GO
