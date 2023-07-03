SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_FillBlockSplit] 
AS
BEGIN
	-- Create any missing records.
	INSERT INTO [bak].[BlockDataExtended]([BlockID])
	SELECT BlockId 
	FROM bak.BlockData
	WHERE BlockId NOT IN (SELECT BlockID FROM [bak].[BlockDataExtended])
	AND BlockID > 930000

	UPDATE BD
	SET BD.PosBlocks24Hr = (SELECT COUNT(*)  
							FROM bak.BlockData (nolock)  
							WHERE [BlockType]=3 
							AND BlockID > (BD.BlockID - 1440)
							and BlockID <= BD.BlockID
						AND XID > 1879454 ),
	BD.PowBlocks24Hr = (SELECT COUNT(*)  
						FROM bak.BlockData (nolock)  
						WHERE [BlockType] IN ( 2,31,32,34)
						AND BlockID > (BD.BlockID - 1440) 
						and BlockID <= BD.BlockID
						AND XID > 1879454 ),
	BD.X16rtBlocks24Hr = (SELECT COUNT(*)  
						FROM bak.BlockData (nolock)  
						WHERE [BlockType] = 2
						AND BlockID > (BD.BlockID - 1440) 
						and BlockID <= BD.BlockID
						AND XID > 1879454),
	BD.RandomXBlocks24Hr = (SELECT COUNT(*)  
						FROM bak.BlockData (nolock)  
						WHERE [BlockType] = 32
						AND BlockID > (BD.BlockID - 1440) 
						and BlockID <= BD.BlockID
						AND XID > 1879454),
	BD.ProgPowBlocks24Hr = (SELECT COUNT(*)  
						FROM bak.BlockData (nolock)  
						WHERE [BlockType] = 34
						AND BlockID > (BD.BlockID - 1440) 
						and BlockID <= BD.BlockID
						AND XID > 1879454 ),
	BD.ShaBlocks24Hr = (SELECT COUNT(*)  
						FROM bak.BlockData (nolock)  
						WHERE [BlockType] = 31
						AND BlockID > (BD.BlockID - 1440) 
						and BlockID <= BD.BlockID 
						AND XID > 1879454)
	FROM bak.BlockDataExtended BD
	WHERE (PosBlocks24Hr IS NULL
	OR PowBlocks24Hr IS NULL
	OR X16rtBlocks24Hr IS NULL
	OR RandomXBlocks24Hr IS NULL
	OR ProgPowBlocks24Hr IS NULL
	OR ShaBlocks24Hr IS NULL)	
	AND BD.XID > 939702
	
	UPDATE BD
	SET BD.[PosBlocks24HrPercent] = (SELECT 
									CAST ((SELECT CAST (PosBlocks24Hr AS decimal(8,2))
									FROM bak.BlockData (nolock)  
									WHERE  BlockID =  BD.BlockID
									AND XID > 1879454)/1440*100 AS decimal(5,2))),
	BD.[PowBlocks24HrPercent] = (SELECT 
								CAST ((SELECT CAST (PowBlocks24Hr AS decimal(8,2))
								FROM bak.BlockData (nolock)  
								WHERE  BlockID = BD.BlockID
									AND XID > 1879454)/1440*100 AS decimal(5,2))),
	BD.X16rtBlocks24HrPercent = (SELECT 
								CAST ((SELECT CAST (X16rtBlocks24Hr AS decimal(8,2))
								FROM bak.BlockData (nolock)  
								WHERE  BlockID = BD.BlockID
									AND XID > 1879454)/1440*100 AS decimal(5,2))),
	BD.RandomXBlocks24HrPercent = (SELECT 
								CAST ((SELECT CAST (RandomXBlocks24Hr AS decimal(8,2))
								FROM bak.BlockData (nolock)  
								WHERE  BlockID = BD.BlockID
									AND XID > 1879454)/1440*100 AS decimal(5,2))),
	BD.ProgPowBlocks24HrPercent = (SELECT 
								CAST ((SELECT CAST (ProgPowBlocks24Hr AS decimal(8,2))
								FROM bak.BlockData (nolock)  
								WHERE  BlockID = BD.BlockID
									AND XID > 1879454)/1440*100 AS decimal(5,2))),
	BD.ShaBlocks24HrPercent = (SELECT 
								CAST ((SELECT CAST (ShaBlocks24Hr AS decimal(8,2))
								FROM bak.BlockData (nolock)  
								WHERE  BlockID = BD.BlockID
									AND XID > 1879454)/1440*100 AS decimal(5,2)))
	FROM bak.BlockDataExtended BD
	WHERE (BD.[PosBlocks24HrPercent] IS NULL 
	OR BD.[PowBlocks24HrPercent] IS NULL
	OR BD.X16rtBlocks24HrPercent IS NULL
	OR BD.RandomXBlocks24HrPercent IS NULL
	OR BD.ProgPowBlocks24HrPercent IS NULL
	OR BD.ShaBlocks24HrPercent IS NULL)		
	AND BD.XID > 939702
END
GO


