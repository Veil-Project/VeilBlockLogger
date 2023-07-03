SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetDiffLineGraphData] 
	@BlockType int,
	@DateRange int
AS
BEGIN

	DECLARE @BlocksBack int = 4320; -- 7 Days
	--DECLARE @BlockType int = 3; -- 7 Days
	--DECLARE @DateRange int = 4; -- 7 Days

	IF(@DateRange = 2)
	BEGIN 
		SET @BlocksBack = 43200; -- 30 Days
	END
	IF(@DateRange = 3)
	BEGIN 
		SET @BlocksBack = 86400; -- 60 Days
	END
	IF(@DateRange = 4)
	BEGIN 
		SET @BlocksBack = 129600; -- 90 Days
	END
	IF(@DateRange = 5)
	BEGIN 
		SET @BlocksBack = 259200; -- 90 Days
	END
		

	IF (@BlockType = 3)
	BEGIN
		SELECT BD.BlockID, BD.BlockDate, 
		ISNULL(BD.PosDiff,0.0) AS 'Diff',
		ISNULL(CAST(DH.Ma720Blocks AS decimal(18,2)),0.0) AS 'Ma720Blocks',
		ISNULL(CAST(DH.Ma1440Blocks AS decimal(18,2)),0.0) AS 'Ma1440Blocks',
		ISNULL(CAST(DH.Ma4320Blocks AS decimal(18,2)),0.0) AS 'Ma4320Blocks'
		FROM [Veil].bak.[BlockData] BD (nolock) 
		JOIN [Veil].bak.PosDiffHistory DH (nolock) ON DH.BlockID=BD.BlockID 
		WHERE BD.BlockID > ((SELECT MAX(BlockID) FROM [Veil].bak.[BlockData]) - @BlocksBack)		
		ORDER BY BD.BlockID 
		RETURN
	END 

	IF (@BlockType = 2) -- Progpow
		BEGIN
		SELECT BD.BlockID, BD.BlockDate, 
		DH.PowDiff AS 'Diff',
		ISNULL(CAST(DH.Ma720Blocks AS decimal(18,2)),0.0) AS 'Ma720Blocks',
		ISNULL(CAST(DH.Ma1440Blocks AS decimal(18,2)),0.0) AS 'Ma1440Blocks',
		ISNULL(CAST(DH.Ma4320Blocks AS decimal(18,2)),0.0) AS 'Ma4320Blocks'
		FROM [Veil].bak.[BlockData] BD (nolock) 
		JOIN [Veil].bak.PowDiffHistory DH (nolock) ON DH.BlockID=BD.BlockID
		WHERE BD.BlockID > ((SELECT MAX(BlockID) FROM [Veil].bak.[BlockData]) - @BlocksBack)		
		ORDER BY BD.BlockID 
		RETURN
	END
	

	IF (@BlockType = 34) -- Progpow
	BEGIN
		SELECT BD.BlockID, BD.BlockDate, 
		DH.PowDiff AS 'Diff',
		ISNULL(CAST(DH.Ma720Blocks AS decimal(18,4)),0.0) AS 'Ma720Blocks',
		ISNULL(CAST(DH.Ma1440Blocks AS decimal(18,4)),0.0) AS 'Ma1440Blocks',
		ISNULL(CAST(DH.Ma4320Blocks AS decimal(18,4)),0.0) AS 'Ma4320Blocks'
		FROM [Veil].bak.ProgpowDiffHistory DH  (nolock) 
		JOIN [Veil].bak.[BlockData] BD (nolock) ON DH.BlockID=BD.BlockID
		WHERE BD.BlockID > ((SELECT MAX(BlockID) FROM [Veil].bak.[BlockData]) - @BlocksBack)		
		ORDER BY BD.BlockID 
		RETURN
	END	

	IF (@BlockType = 32) -- RandomX
	BEGIN
		SELECT BD.BlockID, BD.BlockDate, 
		DH.PowDiff AS 'Diff',
		ISNULL(CAST(DH.Ma720Blocks AS decimal(18,10)),0.0) AS 'Ma720Blocks',
		ISNULL(CAST(DH.Ma1440Blocks AS decimal(18,10)),0.0) AS 'Ma1440Blocks',
		ISNULL(CAST(DH.Ma4320Blocks AS decimal(18,10)),0.0) AS 'Ma4320Blocks'
		FROM [Veil].bak.RandomXDiffHistory DH (nolock)
		JOIN  [Veil].bak.[BlockData] BD (nolock)  ON DH.BlockID=BD.BlockID
		WHERE BD.BlockID > ((SELECT MAX(BlockID) FROM [Veil].bak.[BlockData]) - @BlocksBack)		
		ORDER BY BD.BlockID 
		RETURN
	END 	

	IF (@BlockType = 31) -- Sha
	BEGIN
		SELECT BD.BlockID, BD.BlockDate, 
		DH.PowDiff AS 'Diff',
		ISNULL(CAST(DH.Ma720Blocks AS decimal(18,8)),0.0) AS 'Ma720Blocks',
		ISNULL(CAST(DH.Ma1440Blocks AS decimal(18,8)),0.0) AS 'Ma1440Blocks',
		ISNULL(CAST(DH.Ma4320Blocks AS decimal(18,8)),0.0) AS 'Ma4320Blocks'
		FROM [Veil].bak.ShaDiffHistory DH (nolock)
		JOIN  [Veil].bak.[BlockData] BD (nolock) ON DH.BlockID=BD.BlockID
		WHERE BD.BlockID > ((SELECT MAX(BlockID) FROM [Veil].bak.[BlockData]) - @BlocksBack)		
		ORDER BY BD.BlockID 
		RETURN
	END

END
GO


