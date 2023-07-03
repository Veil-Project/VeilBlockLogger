SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetDenomSupplyData] 
	@DateRange int
AS
BEGIN

	--DECLARE @DateRange int = 1; 
	DECLARE @iBlockRange int = 1440; -- 1 Days

	IF(@DateRange = 2)
	BEGIN 
		SET @iBlockRange = 4320; -- 3 Days
	END
	IF(@DateRange = 3)
	BEGIN 
		SET @iBlockRange = 10080; -- 7 Days
	END

	DECLARE @iMinBlockNumber bigint = 0 
	SET @iMinBlockNumber = ((SELECT MAX(BlockID) FROM [Veil].[bak].[BlockData] with (nolock)) - @iBlockRange);

	
	DECLARE @dZerocoinSupply decimal(28,0);
	set @dZerocoinSupply = (SELECT AVG(ZeroSupply)
	FROM (
		SELECT SUM(Amount) AS 'ZeroSupply', BlockID
		FROM  bak.ZerocoinSupply  with (nolock) WHERE Denom IN ( 'total') AND BlockID >= @iMinBlockNumber
		GROUP BY BlockID) AS
	IT)	

	DECLARE @iPosBlocks bigint = 0 
	SET @iPosBlocks = (SELECT COUNT_BIG(*)  FROM [Veil].[bak].BlockData (nolock)  WHERE [BlockType]=2 AND BlockID >= @iMinBlockNumber);
	--SELECT @iPosBlocks , @BlockRangeSize, @iMinBlockNumber;


	DECLARE @Amount10s decimal(28,18);
	DECLARE @Amount100s decimal(28,18);
	DECLARE @Amount1000s decimal(28,18);
	DECLARE @Amount10000s decimal(28,18);

	SET @Amount10s = (SELECT AVG(Amount) FROM bak.ZerocoinSupply  with (nolock) WHERE Denom = '10' AND BlockID >= @iMinBlockNumber );
	SET @Amount100s = (SELECT AVG(Amount) FROM bak.ZerocoinSupply  with (nolock) WHERE Denom = '100' AND BlockID >= @iMinBlockNumber);
	SET @Amount1000s = (SELECT AVG(Amount) FROM bak.ZerocoinSupply  with (nolock) WHERE Denom = '1000' AND BlockID >= @iMinBlockNumber);
	SET @Amount10000s = (SELECT AVG(Amount) FROM bak.ZerocoinSupply  with (nolock) WHERE Denom = '10000' AND BlockID >= @iMinBlockNumber);

	SELECT 
	@iMinBlockNumber as 'MaxBlock',
	GETDATE() AS 'MaxBlockDate',
	CONVERT(bigint,1) AS 'AvgZerocoinSupply',
		CONVERT(bigint,1) AS 'AvgAmount10s',
		CONVERT(bigint,1) AS 'AvgAmount100s',
		CONVERT(bigint,1) AS 'AvgAmount1000s',
		CONVERT(bigint,1) AS 'AvgAmount10000s',
	   	CONVERT(decimal(6,2),(@Amount10s/@dZerocoinSupply)*100) AS 'Percent10s',
		CONVERT(decimal(6,2),(@Amount100s/@dZerocoinSupply)*100) AS 'Percent100s',
		CONVERT(decimal(6,2),(@Amount1000s/@dZerocoinSupply)*100) AS 'Percent1000s',
		CONVERT(decimal(6,2),(@Amount10000s/@dZerocoinSupply)*100) AS 'Percent10000s'

	   
	RETURN
END
GO


