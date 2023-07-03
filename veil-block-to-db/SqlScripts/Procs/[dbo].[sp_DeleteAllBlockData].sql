SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_DeleteAllBlockData] 
	@BlockID int
AS
BEGIN
	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.BlockDataExtended 
	WHERE BlockID = @BlockID

	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.DenomEfficiency 
	WHERE BlockID = @BlockID

	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.PosDiffHistory 
	WHERE BlockID = @BlockID

	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.PowDiffHistory 
	WHERE BlockID = @BlockID

	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.WinningDenom 
	WHERE BlockID = @BlockID

	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.ZerocoinSupply 
	WHERE BlockID = @BlockID

	--SELECT TOP 1 *
	DELETE FROM VEIL.bak.BlockData 
	WHERE BlockID = @BlockID
END
GO


