SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [bak].[DenomEfficiency](
	[XID] [bigint] IDENTITY(1,1) NOT NULL,
	[BlockID] [bigint] NOT NULL,
	[BlockDate] [datetime] NOT NULL,
	[Supply10] [decimal](4, 2) NOT NULL,
	[Reward10] [decimal](4, 2) NOT NULL,
	[Efficiency10] [decimal](18, 2) NOT NULL,
	[Supply100] [decimal](4, 2) NOT NULL,
	[Reward100] [decimal](4, 2) NOT NULL,
	[Efficiency100] [decimal](18, 2) NOT NULL,
	[Supply1000] [decimal](4, 2) NOT NULL,
	[Reward1000] [decimal](4, 2) NOT NULL,
	[Efficiency1000] [decimal](18, 2) NOT NULL,
	[Supply10000] [decimal](4, 2) NOT NULL,
	[Reward10000] [decimal](4, 2) NOT NULL,
	[Efficiency10000] [decimal](18, 2) NOT NULL,
	[BlocksBack] [int] NOT NULL,
 CONSTRAINT [PK_DenomEfficiency] PRIMARY KEY CLUSTERED 
(
	[XID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


