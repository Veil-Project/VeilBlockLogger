SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetBlockTally] 
AS
BEGIN
	DECLARE @iMinBlockNumber bigint = 0 
	SET @iMinBlockNumber = ((SELECT MAX(BlockID) FROM [Veil].[bak].[BlockData] with (nolock)) - 1440);

	SELECT
	(SELECT COUNT_BIG(*)  FROM bak.BlockData (nolock)  WHERE [BlockType]=3) AS PosBlocks,
	(SELECT COUNT_BIG(*)  FROM bak.BlockData (nolock)  WHERE [BlockType] IN (2,31,32,34)) AS PowBlocks,
	(SELECT COUNT_BIG(*)  FROM bak.BlockData (nolock)  WHERE [BlockType]=3 AND BlockID > @iMinBlockNumber) AS PosBlocks24Hr,
	(SELECT COUNT_BIG(*)  FROM bak.BlockData (nolock)  WHERE [BlockType] IN (2,31,32,34) AND BlockID > @iMinBlockNumber) AS PowBlocks24Hr

	RETURN; 
END
GO


