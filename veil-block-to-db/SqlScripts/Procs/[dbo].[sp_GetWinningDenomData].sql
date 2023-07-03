SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetWinningDenomData] 
	@DateRange int
AS
BEGIN

	--DECLARE  @DateRange int = 1; 
	DECLARE @BlockRangeSize int = 1440; -- 1 Days

	IF(@DateRange = 2)
	BEGIN 
		SET @BlockRangeSize = 4320; -- 3 Days
	END
	IF(@DateRange = 3)
	BEGIN 
		SET @BlockRangeSize = 10080; -- 7 Days
	END

	DECLARE @iMinBlockNumber bigint = 0 
	SET @iMinBlockNumber = ((SELECT MAX(BlockID) FROM [Veil].[bak].[BlockData] with (nolock)) - @BlockRangeSize);
	--SELECT @iMinBlockNumber;

	DECLARE @iPosBlocks bigint = 0 
	SET @iPosBlocks = (SELECT COUNT_BIG(*)  FROM [Veil].[bak].BlockData (nolock)  WHERE [BlockType]=2 AND BlockID >= @iMinBlockNumber);
	--SELECT @iPosBlocks , @BlockRangeSize, @iMinBlockNumber;

	SELECT 
	((SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '10' AND BlockID >= @iMinBlockNumber )) AS 'Count10s',
	(SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '100' AND BlockID >= @iMinBlockNumber) AS 'Count100s',
	(SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '1000' AND BlockID >= @iMinBlockNumber) AS 'Count1000s',
	(SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '10000' AND BlockID >= @iMinBlockNumber) AS 'Count10000s',
	CONVERT(decimal(4,2),((SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '10' AND BlockID >= @iMinBlockNumber ) / CONVERT(decimal, @iPosBlocks))*100) AS 'Percent10s',
	CONVERT(decimal(4,2),((SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '100' AND BlockID >= @iMinBlockNumber) / CONVERT(decimal, @iPosBlocks))*100) AS 'Percent100s',
	CONVERT(decimal(4,2),((SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '1000' AND BlockID >= @iMinBlockNumber)  / CONVERT(decimal, @iPosBlocks))*100) AS 'Percent1000s',
	CONVERT(decimal(4,2),((SELECT COUNT(*) FROM bak.WinningDenom  with (nolock) WHERE StakeDenom = '10000' AND BlockID >= @iMinBlockNumber) / CONVERT(decimal, @iPosBlocks))*100) AS 'Percent10000s'

	RETURN
END
GO


