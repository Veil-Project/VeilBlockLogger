SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [bak].[BlockData](
	[XID] [bigint] IDENTITY(1,1) NOT NULL,
	[BlockID] [bigint] NOT NULL,
	[BlockType] [int] NOT NULL,
	[BlockTimestamp] [bigint] NOT NULL,
	[BlockDate] [datetime] NOT NULL,
	[BlockHash] [varchar](75) NOT NULL,
	[PosDiff] [float] NULL,
	[PowDiff] [float] NULL,
	[TxCount] [int] NULL,
	[MoneySupply] [bigint] NOT NULL,
 CONSTRAINT [PK_BlockData] PRIMARY KEY CLUSTERED 
(
	[XID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


