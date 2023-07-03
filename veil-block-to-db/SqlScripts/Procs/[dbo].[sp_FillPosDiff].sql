SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_FillPosDiff] 
AS
BEGIN

	DECLARE @StartBlock bigint = (SELECT ISNULL(MAX(BLOCKID),0) + 1 FROM bak.PosDiffHistory);

	INSERT INTO bak.PosDiffHistory(BlockID,BlockDate,PosDiff,Ma720Blocks,Ma1440Blocks, Ma4320Blocks, Ma10080Blocks)
		SELECT BlockID, BlockDate, PosDiff AS 'PosDiff',
		ISNULL((SELECT AVG(BDI.PosDiff) 
		FROM [Veil].bak.[BlockData] BDI WITH (nolock)  
		WHERE BDI.BlockID <= BDO.BlockID and BDI.PosDiff > 0
		AND BDI.BlockID >= (BDO.BlockID - 720)),0) AS 'Ma720Blocks',
		ISNULL((SELECT AVG(BDI.PosDiff) 
			FROM [Veil].bak.[BlockData] BDI WITH (nolock)  
			WHERE BDI.BlockID <= BDO.BlockID  and BDI.PosDiff > 0
			AND BDI.BlockID >= (BDO.BlockID - 1440)),0) AS 'Ma1440Blocks',
		ISNULL((SELECT AVG(BDI.PosDiff) 
			FROM [Veil].bak.[BlockData] BDI WITH (nolock)  
			WHERE BDI.BlockID <= BDO.BlockID  and BDI.PosDiff > 0
			AND BDI.BlockID >= (BDO.BlockID - 4320)),0) AS 'Ma4320Blocks',
		0 AS 'Ma10080Blocks'
		FROM [Veil].bak.[BlockData] BDO WITH (nolock)
		WHERE BlockID >= @StartBlock
		AND BlockID <= (@StartBlock + 100000)
		ORDER BY BlockID
		
END
GO


