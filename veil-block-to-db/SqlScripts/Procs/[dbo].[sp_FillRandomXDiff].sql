SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_FillRandomXDiff] 
AS
BEGIN

	DECLARE @StartBlock bigint = (SELECT ISNULL(MAX(BLOCKID),0) + 1 FROM bak.RandomXDiffHistory);
	DECLARE @BlockType int = 32; 
	SELECT BDO.BlockID, 
	CASE WHEN BDO.BlockType = @BlockType THEN BDO.PowDiff ELSE 0.00 END AS 'PowDiff'
	INTO #FilteredDiffBlocks
	FROM [Veil].bak.[BlockData] BDO WITH (nolock)
	WHERE BDO.BlockID >= @StartBlock - 1

	INSERT INTO bak.RandomXDiffHistory(BlockID,PowDiff,Ma720Blocks,Ma1440Blocks, Ma4320Blocks, Ma10080Blocks)	
		SELECT BDO.BlockID,
		BDO.PowDiff,	
		ISNULL((SELECT AVG(BDI.PowDiff) FROM bak.BlockData BDI WHERE BDI.BlockID <= BDO.BlockID AND BDI.BlockID >(BDO.BlockID - 720) AND BDI.BlockType = @BlockType),0)  AS 'Ma720Blocks' ,		
		ISNULL((SELECT AVG(BDI.PowDiff) FROM bak.BlockData BDI WHERE BDI.BlockID <= BDO.BlockID AND BDI.BlockID >(BDO.BlockID - 1440) AND BDI.BlockType = @BlockType),0)  AS 'Ma1440Blocks',		
		ISNULL((SELECT AVG(BDI.PowDiff) FROM bak.BlockData BDI WHERE BDI.BlockID <= BDO.BlockID AND BDI.BlockID >(BDO.BlockID - 4320) AND BDI.BlockType = @BlockType),0)  AS 'Ma4320Blocks',
		0  AS 'Ma10080Blocks'
		FROM #FilteredDiffBlocks BDO WITH (nolock)
		WHERE BDO.BlockID >= @StartBlock

	DROP TABLE #FilteredDiffBlocks;
END
GO


