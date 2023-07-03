using FluentFTP;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VeilBlockToDB.ModelsApi;
using VeilBlockToDB.ModelsDb;
using VeilBlockToDB.ModelsJson;
using VeilBlockToDB.ModelsRpc.Response;
using VeilBlockToDB.Procs;

namespace VeilBlockToDB
{
    public partial class frmMain : Form
    {
        private static bool m_bLocalRpc = true;
        private static string m_szRpcUsername = "veiluser";
        private static string m_szRpcPassword = "veilrpcpassword";

        private static string m_szExplorerUrl = "http://veiluser:veilrpcpassword@127.0.0.1:58812";
        
        private VeilContext _dbVeilContext = new VeilContext();
        private Timer _ApiCallTimer;

        private long _iLastBlockNumber = 0;
        private const long _iApiBatchSize = 250;
        private const long _iDbSaveBatchSize = 50;

        private int _iTimerCounter = 1;
        private int _iLogCounter = 1;

        private IEnumerable<long> _colBlockDataIds = new List<long>();
        private IEnumerable<long> _colWinningDenomIds = new List<long>();
        private IEnumerable<long> _colZerocoinSupplyIds = new List<long>();

        public frmMain()
        {
            InitializeComponent();
        }

        #region  "Form Buttons"
        private void BtnStart_Click(object sender, EventArgs e)
        {
            ProcessNetworkData();
            var colFilesToUpload = new List<DatasetUpload>();
            colFilesToUpload.AddRange(CreateRealTimeJsonDatasets());
            colFilesToUpload.AddRange(CreateZerocoinJsonDatasets());
            colFilesToUpload.AddRange(CreateHistoryJsonDatasets());
            UploadToSite(colFilesToUpload);
            colFilesToUpload.Clear();
            if (_dbVeilContext.Database.Connection.State == ConnectionState.Open)
            {
                _dbVeilContext.Database.Connection.Close();
            }
            InitTimer();
            UpdateAppStatus("Timer started...");
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            _ApiCallTimer.Stop();
            UpdateAppStatus("Timer stopped.");
        }

        private void BtnReprocessAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ApiCallTimer != null)
                {
                    _ApiCallTimer.Stop();
                }

                long iMaxBlock = GetLastDbBlockNumber();
                UpdateAppStatus("Last DB Block #: " + iMaxBlock);

                var lStopBlock = Convert.ToInt64(txtResyncBlock.Text);
                for (; iMaxBlock >= lStopBlock; iMaxBlock--)
                {
                    UpdateAppStatus("Starting to delete block ID: " + iMaxBlock.ToString());
                    DeleteBlockData(iMaxBlock);
                    UpdateAppStatus("Block ID: " + iMaxBlock.ToString() + " deleted");
                }
            }
            catch (Exception ex)
            {
                UpdateAppStatus("Error: " + ex.Message);
            }
        }
        #endregion

        private void ProcessNetworkData()
        {
            try
            {
                UpdateAppStatus("Starting ProcessNetworkData()...");
                var lTotalBlocks = GetTotalBlockCount();
                UpdateAppStatus("Total Block Count: " + lTotalBlocks);

                long lCounter = GetLastDbBlockNumber();
                UpdateAppStatus("Last DB Block #: " + lCounter);
                while (lCounter < lTotalBlocks)
                {
                    for (; lCounter <= lTotalBlocks; lCounter += _iApiBatchSize)
                    {
                        if (DownloadJson(lCounter, lTotalBlocks))
                        {
                            lCounter = GetLastDbBlockNumber();
                            break;
                        }
                    }
                    UpdateAppStatus(string.Format("Blocks {0}-{1} of {2} were downloaded and imported.",
                                        lCounter - _iApiBatchSize, lCounter, lTotalBlocks));

                    lTotalBlocks = GetTotalBlockCount() - 2;
                    UpdateAppStatus("Rechecking Total Block Count: " + lTotalBlocks);

                    UpdateAppStatus("Current block counter: " + lCounter);
                }
                UpdateAppStatus("ProcessNetworkData - data download complete.");
                UpdateAppStatus("ProcessNetworkData - complete.");
            }
            catch (Exception ex)
            {
                UpdateAppStatus(ex.Message + Environment.NewLine + ex.StackTrace);
                Application.DoEvents();
            }
        }

        public void InitTimer()
        {
            if (_ApiCallTimer == null || !_ApiCallTimer.Enabled)
            {
                _ApiCallTimer = new Timer();
                _ApiCallTimer.Tick += new EventHandler(ApiCallTimer_Tick);
                _ApiCallTimer.Interval = 60000; // Every 60 seconds.
                _ApiCallTimer.Start();
            }
        }

        private void ApiCallTimer_Tick(object sender, EventArgs e)
        {
            _ApiCallTimer.Stop();
            ProcessNetworkData();

            var colFilesToUpload = new List<DatasetUpload>();
            if (_iTimerCounter % 30 == 0) // upload to website every 30 minutes
            {
                colFilesToUpload.AddRange(CreateRealTimeJsonDatasets());
            }

            if (_iTimerCounter % 60 == 0) // upload to website every 60 minutes
            {
                colFilesToUpload.AddRange(CreateZerocoinJsonDatasets());
                colFilesToUpload.AddRange(CreateHistoryJsonDatasets());
                _iTimerCounter = 0;
            }

            if(colFilesToUpload.Count > 0)
            {
                UploadToSite(colFilesToUpload);
                colFilesToUpload.Clear();
            }
            if (_dbVeilContext.Database.Connection.State == ConnectionState.Open)
            {
                _dbVeilContext.Database.Connection.Close();
            }
            _iTimerCounter++;
            _ApiCallTimer.Start();
        }

        private Int64 GetTotalBlockCount()
        {
            var oFirstBlock = GetBlockByHeight(1);
            return oFirstBlock.confirmations + 1;
        }

        private static string GetBlockHash(long blockId)
        {
            try
            {
                if (m_bLocalRpc)
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl);
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "POST";
                    oWebR.Timeout = 60000;
                    oWebR.Credentials = new NetworkCredential(m_szRpcUsername, m_szRpcPassword);
                    using (var streamWriter = new StreamWriter(oWebR.GetRequestStream()))
                    {
                        var oBlockRequest = new ModelsRpc.Request.BlockHashRequest();
                        oBlockRequest.@params.Add(blockId);
                        streamWriter.Write(JsonConvert.SerializeObject(oBlockRequest));
                    }
                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    var oRpcResponse = JsonConvert.DeserializeObject<BlockHashResponse>(text);
                    return oRpcResponse.result;
                }
                else
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl + "/getblockhash/" + blockId.ToString());
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "GET";
                    oWebR.Timeout = 60000;

                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    return text;
                }
            }
            catch (Exception ex)
            {
                return "Error: ";
            }
        }

        private static List<ModelsApi.ZerocoinSupply> GetZerocoinSupply(long blockId)
        {
            try
            {
                if (m_bLocalRpc)
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl);
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "POST";
                    oWebR.Timeout = 60000;
                    oWebR.Credentials = new NetworkCredential(m_szRpcUsername, m_szRpcPassword);
                    using (var streamWriter = new StreamWriter(oWebR.GetRequestStream()))
                    {
                        var oBlockRequest = new ModelsRpc.Request.ZerocoinSupplyRequest();
                        oBlockRequest.@params.Add(blockId);
                        streamWriter.Write(JsonConvert.SerializeObject(oBlockRequest));
                    }
                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    var oRpcResponse = JsonConvert.DeserializeObject<ZerocoinSupplyResponse>(text);
                    return oRpcResponse.result;
                }
                else
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl + "/getblockhash/" + blockId.ToString());
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "GET";
                    oWebR.Timeout = 60000;

                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    var oRpcResponse = JsonConvert.DeserializeObject<ZerocoinSupplyResponse>(text);
                    return oRpcResponse.result;
                }
            }
            catch (Exception ex)
            {
                return new List<ModelsApi.ZerocoinSupply>();
            }
        }

        private static Block GetBlockByHeight(long blockHeight)
        {
            try
            {
                var szFirstBlockHash = GetBlockHash(blockHeight);
                return GetBlockByHash(szFirstBlockHash);
            }
            catch (Exception ex)
            {
                return new Block();
            }
        }

        private static Block GetBlockByHash(string blockHash)
        {
            try
            {
                if (m_bLocalRpc)
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl);
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "POST";
                    oWebR.Timeout = 60000;
                    oWebR.Credentials = new NetworkCredential(m_szRpcUsername, m_szRpcPassword);
                    using (var streamWriter = new StreamWriter(oWebR.GetRequestStream()))
                    {
                        var oBlockRequest = new ModelsRpc.Request.BlockRequest();
                        oBlockRequest.@params.Add(blockHash);
                        oBlockRequest.@params.Add(2);
                        streamWriter.Write(JsonConvert.SerializeObject(oBlockRequest));
                    }

                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    var oRpcResponse = JsonConvert.DeserializeObject<BlockResponse>(text);
                    return oRpcResponse.result;
                }
                else
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl + "/getblock/" + blockHash);
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "GET";
                    oWebR.Timeout = 60000;

                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<Block>(text);
                }
            }
            catch (Exception ex)
            {
                return new Block();
            }
        }

        private static BlockHeader GetBlockHeaderByHash(string blockHash)
        {
            try
            {
                if (m_bLocalRpc)
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl);
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "POST";
                    oWebR.Timeout = 60000;
                    oWebR.Credentials = new NetworkCredential(m_szRpcUsername, m_szRpcPassword);
                    using (var streamWriter = new StreamWriter(oWebR.GetRequestStream()))
                    {
                        var oBlockRequest = new ModelsRpc.Request.BlockHeaderRequest();
                        oBlockRequest.@params.Add(blockHash);
                        streamWriter.Write(JsonConvert.SerializeObject(oBlockRequest));
                    }

                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    var oRpcResponse = JsonConvert.DeserializeObject<BlockHeaderResponse>(text);
                    return oRpcResponse.result;
                }
                else
                {
                    var oWebR = WebRequest.Create(m_szExplorerUrl + "/getblockheader/" + blockHash);
                    oWebR.ContentType = "application/json; charset=utf-8;";
                    oWebR.Method = "GET";
                    oWebR.Timeout = 60000;

                    var stream = oWebR.GetResponse().GetResponseStream();
                    var reader = new StreamReader(stream);
                    var text = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<BlockHeader>(text);
                }

            }
            catch (Exception ex)
            {
                return new BlockHeader();
            }
        }

        private bool DownloadJson(long startingBlockId, long totalBlocks)
        {
            var colActiveThreads = new List<Action>();
            long iBatchSize = _iApiBatchSize;
            if ((startingBlockId + iBatchSize) > totalBlocks)
            {
                iBatchSize = totalBlocks - startingBlockId;
            }

            var colBlocks = new Block[iBatchSize];

            long iCounter = 0;
            for (; (iCounter + startingBlockId) < (startingBlockId + iBatchSize); iCounter++)
            {
                var x1 = iCounter;
                colActiveThreads.Add(() => colBlocks[x1] = GetNetworkDataThread(x1 + startingBlockId));
            }

            UpdateAppStatus("Data threads created. Starting Block ID #: " + startingBlockId);
            Application.DoEvents();

            var oThreadOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 4 };
            Parallel.Invoke(oThreadOptions, colActiveThreads.ToArray());

            UpdateAppStatus("Data downloaded to memory. Starting Block ID #: " + startingBlockId);
            Application.DoEvents();

            ProcessBlockRpc(colBlocks);
            FillPosMovingAverage();
            FillRandomXMovingAverage();
            FillProgpowMovingAverage();
            FillShaMovingAverage();
            FillBlockSplit();
            return iBatchSize < _iApiBatchSize;
        }

        private void ProcessBlockRpc(Block[] blocks)
        {
            long iCurrentBlockID = 0;
            var colBlocks = new List<Block>();

            for (; iCurrentBlockID < blocks.Length; iCurrentBlockID++)
            {
                var oBlock = GetCurrentBlockJson(blocks, iCurrentBlockID);
                colBlocks.Add(oBlock);
            }

            GetLastDbBlockNumber();  // Make sure we have the last Pos and Pow Diff.

            iCurrentBlockID = 1;
            foreach (var oBlock in colBlocks.OrderBy(o => o.height))
            {
                SaveBlockToDB(oBlock);
                if (iCurrentBlockID % _iDbSaveBatchSize == 0)
                {
                    UpdateAppStatus("Saving previous " + _iDbSaveBatchSize + " blocks to the database. Last Block: " + _iLastBlockNumber);
                    _dbVeilContext.SaveChanges();
                    UpdateAppStatus("Saved changes to DB.");
                    _dbVeilContext = new VeilContext();
                    iCurrentBlockID = 0;
                }
                iCurrentBlockID++; ;
            }

            UpdateAppStatus("Saving previous " + colBlocks.Count + " blocks to the database. Last Block: " + _iLastBlockNumber);
            _dbVeilContext.SaveChanges();
            UpdateAppStatus("Saved changes to DB. All blocks processed! Last Block: " + _iLastBlockNumber);
        }

        private Block GetCurrentBlockJson(Block[] blocks, long iCurrentBlockID)
        {
            var oCurrentBlock = blocks[iCurrentBlockID];
            if (string.IsNullOrWhiteSpace(oCurrentBlock.hash))
            {
                oCurrentBlock = RewindToGetBlock(blocks, iCurrentBlockID, iCurrentBlockID);
            }
            return oCurrentBlock;
        }

        private Block RewindToGetBlock(Block[] blocks, long iCurrentBlockID, long iSearchBlock)
        {
            var oCurrentBlock = new Block();

            if (iCurrentBlockID == 0 && iSearchBlock == 0)
            {
                var lTemp = GetLastDbBlockNumber() - 1;
                var oTemp = _dbVeilContext.BlockData.FirstOrDefault(w => w.BlockID == lTemp);
                if (oTemp != null)
                {
                    var oRewindBlock = GetNetworkDataByHashThread(oTemp.BlockHash);
                    return GetNetworkDataByHashThread(oRewindBlock.nextblockhash);
                }
            }

            if (iCurrentBlockID < blocks.Length)
            {
                iCurrentBlockID -= 1;
                oCurrentBlock = blocks[iCurrentBlockID];
                if (string.IsNullOrWhiteSpace(oCurrentBlock.hash))
                {
                    oCurrentBlock = RewindToGetBlock(blocks, iCurrentBlockID, iSearchBlock);
                }

                if (string.IsNullOrWhiteSpace(oCurrentBlock.hash))
                {
                    oCurrentBlock = RewindToGetBlock(blocks, iCurrentBlockID, iSearchBlock);
                }
                UpdateAppStatus("RewindToGetBlock Current Index: " + iCurrentBlockID + " Search Index: " + iSearchBlock);

                oCurrentBlock = GetNetworkDataByHashThread(oCurrentBlock.nextblockhash);


                if (string.IsNullOrWhiteSpace(oCurrentBlock.hash))
                {
                    oCurrentBlock = RewindToGetBlock(blocks, iCurrentBlockID, iSearchBlock);
                }

                if (string.IsNullOrWhiteSpace(oCurrentBlock.hash))
                {
                    oCurrentBlock = RewindToGetBlock(blocks, iCurrentBlockID, iSearchBlock);
                }
            }
            return oCurrentBlock;
        }

        private void SaveBlockToDB(Block block)
        {
            var oRpcBlock = block;
            var oRpcBlockHeader = GetBlockHeaderByHash(oRpcBlock.hash);
            var oRpcZerocoinSupply = GetZerocoinSupply(oRpcBlock.height);

            if (oRpcBlock.versionHex == null)
            {
                UpdateAppStatus("oNetworkData Null. Last Block: " + _iLastBlockNumber);
            }
            double dCurrentPowDiff = 0;
            double dCurrentPosDiff = 0;
            switch (oRpcBlock.versionHex)
            {
                case "20000000":
                    if (oRpcBlock.IsPos)
                    {
                        dCurrentPosDiff = oRpcBlock.difficulty;
                    }
                    else
                    {
                        dCurrentPowDiff = oRpcBlock.difficulty;
                    }
                    break;
                case "30000000":
                    dCurrentPosDiff = oRpcBlock.difficulty;
                    break;
                case "31000000":
                    dCurrentPowDiff = oRpcBlock.difficulty;
                    break;
                case "34000000":
                    dCurrentPowDiff = oRpcBlock.difficulty;
                    break;
                case "32000000":
                    dCurrentPowDiff = oRpcBlock.difficulty;
                    break;
                default:
                    break;
            }

            _iLastBlockNumber = oRpcBlock.height;

            if (!_colBlockDataIds.Contains(oRpcBlock.height))
            {
                var oBlockData = new BlockData()
                {
                    BlockID = oRpcBlock.height,
                    BlockTimestamp = oRpcBlock.time,
                    BlockDate = oRpcBlock.BlockDate,
                    BlockHash = oRpcBlock.hash,
                    PoWDiff = dCurrentPowDiff,
                    PoSDiff = dCurrentPosDiff,
                    TxCount = oRpcBlock.nTx,
                    MoneySupply = oRpcBlockHeader.moneysupply,
                };
                if (oRpcBlock.IsPos)
                {
                    oBlockData.BlockType = 3;
                }
                else
                {
                    oBlockData.BlockType = int.Parse(oRpcBlock.versionHex.Replace("0", ""));
                }
                var oDbBlockData = _dbVeilContext.GetBlockData(oRpcBlock.height);
                if (oDbBlockData == null)
                {
                    _dbVeilContext.BlockData.Add(oBlockData);
                }
            }

            foreach (var oDenom in oRpcZerocoinSupply)
            {
                if (!_colZerocoinSupplyIds.Contains(oRpcBlock.height))
                {
                    var oZeroSupply = new ModelsDb.ZerocoinSupply()
                    {
                        BlockID = oRpcBlock.height,
                        Denom = oDenom.denom,
                        Amount = oDenom.amount,
                        PercentOfSupply = oDenom.percent
                    };
                    var oDbZerocoinSupply = _dbVeilContext.GetZerocoinSupply(oRpcBlock.height);
                    if (oDbZerocoinSupply == null)
                    {
                        _dbVeilContext.ZerocoinSupply.Add(oZeroSupply);
                    }

                }
            }

            if (!_colWinningDenomIds.Contains(oRpcBlock.height))
            {
                var szWinningDenom = "0";
                if ((oRpcBlock.IsPos) &&
                   oRpcBlock.tx != null && oRpcBlock.tx.Count > 1 &&
                   oRpcBlock.tx[1].vin != null && oRpcBlock.tx[1].vin.Count > 0 &&
                   oRpcBlock.tx[1].vin[0].type == "zerocoinspend")
                {
                    szWinningDenom = oRpcBlock.tx[1].vin[0].denomination.ToString();
                }

                var oWinningDenom = new WinningDenom()
                {
                    BlockID = oRpcBlock.height,
                    BlockDate = oRpcBlock.BlockDate,
                    StakeDenom = szWinningDenom
                };
                var oDbWinningDenom = _dbVeilContext.GetWinningDenom(oRpcBlock.height, szWinningDenom);
                if (oDbWinningDenom == null)
                {
                    _dbVeilContext.WinningDenom.Add(oWinningDenom);
                }
            }
        }
        
        private static Block GetNetworkDataThread(long blockId)
        {
            try
            {
                return GetBlockByHeight(blockId);
            }
            catch (Exception ex)
            {
                return new Block();
            }
        }

        private static Block GetNetworkDataByHashThread(string hash)
        {
            try
            {
                return GetBlockByHash(hash);
            }
            catch (Exception ex)
            {
                return new Block(); ;
            }
        }

        private Int64 GetLastDbBlockNumber()
        {
            long lMaxBlock = 0;
            try { lMaxBlock = _dbVeilContext.BlockData.Max(m => m.BlockID); } catch { }
            if (lMaxBlock == 0) { return 1; }

            var oNewestBlock = _dbVeilContext.BlockData.FirstOrDefault(w => w.BlockID == lMaxBlock);
            return lMaxBlock + 1;
        }

        private void UpdateAppStatus(string message)
        {
            UpdateAppStatus(message, true);
        }

        private void UpdateAppStatus(string message, bool appendMessage)
        {
            if (_iLogCounter == 200)
            {
                appendMessage = false;
                _iLogCounter = 0;
            }

            if (appendMessage)
            {
                rtxStatus.Text = string.Format("{0}: {1}{2}{3}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss"), message,
                                                Environment.NewLine, rtxStatus.Text);
            }
            else
            {
                rtxStatus.Text = DateTime.Now.ToString("MM-dd-yy hh:mm:ss") + ":  " + message;
            }
            _iLogCounter++;
            Application.DoEvents();
        }

        #region Create Datasets
        private List<DatasetUpload> CreateRealTimeJsonDatasets()
        {
            UpdateAppStatus("Starting to create RealTime dataset...");
            var colFilesToUpload = new List<DatasetUpload>();
            colFilesToUpload.Add(
                new DatasetUpload() { Dataset = JsonDataset.GetBlockTallyFromDB(), Source = "BlockTally" });
            UpdateAppStatus("Create RealTime dataset complete");
            return colFilesToUpload;
        }

        private List<DatasetUpload> CreateZerocoinJsonDatasets()
        {
            UpdateAppStatus("Starting to create ZerocoinStake dataset...");
            var colFilesToUpload = new List<DatasetUpload>();
            for (var i = 3; i >= 1; i--)
            {
                var colStakeDenom = JsonDataset.GetWinnningDenomDataFromDB(i);
                var colSupplyDenom = JsonDataset.GetDenomSupplyDataFromDB(i);
                colFilesToUpload.Add(ToDataUpload(new { StakeData = colStakeDenom, SupplyData = colSupplyDenom }, "ZerocoinStake", i));
            }
            UpdateAppStatus("Create ZerocoinStake dataset complete");
            return colFilesToUpload;
        }

        private List<DatasetUpload> CreateHistoryJsonDatasets()
        {
            UpdateAppStatus("Starting to create History dataset...");

            var colFilesToUpload = new List<DatasetUpload>();
            UpdateAppStatus("Creating PosDiff datasets...");
            var colRecords = JsonDataset.GetDifficultyDataFromDB(3, 4);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PosDiff", 4));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PosDiff", 3));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PosDiff", 2));
            colRecords.Series1.RemoveRange(0, colRecords.Series1.Count - 4320);
            colRecords.Series2.RemoveRange(0, colRecords.Series2.Count - 4320);
            colRecords.Series3.RemoveRange(0, colRecords.Series3.Count - 4320);
            colRecords.Series4.RemoveRange(0, colRecords.Series4.Count - 4320);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PosDiff", 1));
            UpdateAppStatus("PosDiff datasets complete");

            UpdateAppStatus("Creating PowDiff Randomx datasets...");
            colRecords = new MultiSeriesLineChart();
            colRecords = JsonDataset.GetDifficultyDataFromDB(32, 4);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffRandomX", 4));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffRandomX", 3));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffRandomX", 2));
            colRecords.Series1.RemoveRange(0, colRecords.Series1.Count - 4320);
            colRecords.Series2.RemoveRange(0, colRecords.Series2.Count - 4320);
            colRecords.Series3.RemoveRange(0, colRecords.Series3.Count - 4320);
            colRecords.Series4.RemoveRange(0, colRecords.Series4.Count - 4320);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffRandomX", 1));
            UpdateAppStatus("PowDiff RandomX datasets complete");

            UpdateAppStatus("Creating PowDiff ProgPow datasets...");
            colRecords = JsonDataset.GetDifficultyDataFromDB(34, 4);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffProgPow", 4));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffProgPow", 3));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffProgPow", 2));
            colRecords.Series1.RemoveRange(0, colRecords.Series1.Count - 4320);
            colRecords.Series2.RemoveRange(0, colRecords.Series2.Count - 4320);
            colRecords.Series3.RemoveRange(0, colRecords.Series3.Count - 4320);
            colRecords.Series4.RemoveRange(0, colRecords.Series4.Count - 4320);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffProgPow", 1));
            UpdateAppStatus("PowDiff ProgPow datasets complete");


            UpdateAppStatus("Creating PowDiff SHA datasets...");
            colRecords = JsonDataset.GetDifficultyDataFromDB(31, 4);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffSha", 4));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffSha", 3));
            colRecords.Series1.RemoveRange(0, 43200);
            colRecords.Series2.RemoveRange(0, 43200);
            colRecords.Series3.RemoveRange(0, 43200);
            colRecords.Series4.RemoveRange(0, 43200);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffSha", 2));
            colRecords.Series1.RemoveRange(0, colRecords.Series1.Count - 4320);
            colRecords.Series2.RemoveRange(0, colRecords.Series2.Count - 4320);
            colRecords.Series3.RemoveRange(0, colRecords.Series3.Count - 4320);
            colRecords.Series4.RemoveRange(0, colRecords.Series4.Count - 4320);
            colFilesToUpload.Add(ToDataUpload(colRecords, "PowDiffSha", 1));
            UpdateAppStatus("PowDiff SHA datasets complete");

            UpdateAppStatus("Create History datasets complete");
            return colFilesToUpload;
        }
      
        #endregion

        #region DatasetUpload
        private void UploadToSite(List<DatasetUpload> jsonDatasets)
        {
            if (chkSaveToFileSystem.Checked)
            {
                try
                {
                    foreach (var szDataset in jsonDatasets)
                    {
                        SaveToFileSystem(szDataset);
                    }
                }
                catch (Exception ex)
                {
                    UpdateAppStatus("Save local file error: " + ex.Message);
                }
            }
            else
            {
                var ftpUrl = ConfigurationManager.AppSettings["ftpUrl"];
                var ftpUsername = ConfigurationManager.AppSettings["ftpUsername"];
                var ftpPassword = ConfigurationManager.AppSettings["ftpPassword"];

                FtpClient client = new FtpClient(ftpUrl);

                // if you don't specify login credentials, we use the "anonymous" user account
                client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                try
                {
                    // begin connecting to the server
                    client.Connect();

                    foreach (var szDataset in jsonDatasets)
                    {
                        if (chkSaveToFileSystem.Checked)
                        {
                            SaveToFileSystem(szDataset);
                        }
                        else
                        {
                            UploadToSiteSub(szDataset, client);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UpdateAppStatus("Uploading file error: " + ex.Message);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        private DatasetUpload ToDataUpload(object data, string source, int index)
        {
            var oNewDatasetUpload = new DatasetUpload();
            oNewDatasetUpload.Dataset = CompressJson(JsonConvert.SerializeObject(data, Formatting.None,
                        new JsonSerializerSettings
                        {
                            DateFormatHandling = DateFormatHandling.IsoDateFormat
                        }));
            oNewDatasetUpload.Source = source;
            oNewDatasetUpload.Index = index;
            return oNewDatasetUpload;
        }

        private string CompressJson(string jsonData)
        {
            byte[] compressedBytes;
            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                    // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                    // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, I don't want to rely on that very odd behavior should it ever change
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }

                    // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                    compressedBytes = compressedStream.ToArray();
                }
            }

            return Convert.ToBase64String(compressedBytes);
        }

        private void UploadToSiteSub(DatasetUpload jsonDataset, FtpClient client)
        {
            UpdateAppStatus("Uploading json dataset: " + jsonDataset.Source + " Index: " + jsonDataset.Index);
            var szUploadPath = "/httpdocs/JsonDatasets/" + jsonDataset.Source;
            if (jsonDataset.Index > 0)
            {
                szUploadPath += "/" + jsonDataset.Index;
            }
            szUploadPath += "/data.json";
            client.Upload(GenerateStreamFromString(jsonDataset.Dataset), szUploadPath, FtpExists.Overwrite, true);
            UpdateAppStatus("Uploading file complete: " + szUploadPath);
        }

        private void SaveToFileSystem(DatasetUpload jsonDataset)
        {
            UpdateAppStatus("Save json dataset: " + jsonDataset.Source + " Index: " + jsonDataset.Index);
            var szUploadPath = Path.Combine(txtSavePath.Text, "JsonDatasets", jsonDataset.Source);
            if (jsonDataset.Index > 0)
            {
                szUploadPath += @"\" + jsonDataset.Index;
            }
            szUploadPath += @"\data.json";
            File.WriteAllText(szUploadPath, jsonDataset.Dataset);
            UpdateAppStatus("Save file complete: " + szUploadPath);
        }
        #endregion



        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private void DeleteBlockData(long blockID)
        {
            // Delete.
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "sp_DeleteAllBlockData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@BlockID", blockID));
                cmd.CommandTimeout = 600;

                // execute the command
                var rdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                var o = ex.Message;
                UpdateAppStatus("Delete block error: " + ex.Message);
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }
        }

        public void FillPosMovingAverage()
        {
            UpdateAppStatus("Starting FillPosMovingAverage()...");
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "dbo.sp_FillPosDiff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 6000;

                // execute the command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var o = ex.Message;
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }
            UpdateAppStatus("FillPosMovingAverage() complete");
        }

        public void FillRandomXMovingAverage()
        {
            UpdateAppStatus("Starting FillRandomXMovingAverage()...");
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "dbo.sp_FillRandomXDiff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 6000;

                // execute the command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var o = ex.Message;
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }

            UpdateAppStatus("FillRandomXMovingAverage() complete");
        }

        public void FillShaMovingAverage()
        {
            UpdateAppStatus("Starting FillShaMovingAverage()...");
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "dbo.sp_FillShaDiff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 6000;

                // execute the command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var o = ex.Message;
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }

            UpdateAppStatus("FillShaMovingAverage() complete");
        }

        public void FillProgpowMovingAverage()
        {
            UpdateAppStatus("Starting FillProgpowMovingAverage()...");
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "dbo.sp_FillProgpowDiff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 6000;

                // execute the command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var o = ex.Message;
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }

            UpdateAppStatus("FillProgpowMovingAverage() complete");
        }

        public void FillBlockSplit()
        {
            UpdateAppStatus("Starting FillBlockSplit()...");
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "dbo.sp_FillBlockSplit";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 6000;

                // execute the command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var o = ex.Message;
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }

            UpdateAppStatus("FillBlockSplit() complete");
        }
    }
}

