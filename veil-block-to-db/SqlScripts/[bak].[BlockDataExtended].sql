SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [bak].[BlockDataExtended](
	[XID] [bigint] IDENTITY(1,1) NOT NULL,
	[BlockID] [bigint] NULL,
	[PosBlocks24Hr] [int] NULL,
	[PosBlocks24HrPercent] [decimal](5, 2) NULL,
	[PowBlocks24Hr] [int] NULL,
	[PowBlocks24HrPercent] [decimal](5, 2) NULL,
	[X16rtBlocks24Hr] [int] NULL,
	[X16rtBlocks24HrPercent] [decimal](5, 2) NULL,
	[RandomXBlocks24Hr] [int] NULL,
	[RandomXBlocks24HrPercent] [decimal](5, 2) NULL,
	[ProgPowBlocks24Hr] [int] NULL,
	[ProgPowBlocks24HrPercent] [decimal](5, 2) NULL,
	[ShaBlocks24Hr] [int] NULL,
	[ShaBlocks24HrPercent] [decimal](5, 2) NULL,
 CONSTRAINT [PK_BlockDataExtended] PRIMARY KEY CLUSTERED 
(
	[XID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


