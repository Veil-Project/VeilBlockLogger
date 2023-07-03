using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VeilBlockToDB.ModelsJson;

namespace VeilBlockToDB.Procs
{
    public class JsonDataset
    {  
        public static string GetBlockTallyFromDB()
        {
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "sp_GetBlockTally";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                // execute the command
                var rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console

                var szDataset = "";
                while (rdr.Read())
                {
                    var oTemp = new
                    {
                        PosBlocks = (long)rdr["PosBlocks"],
                        PowBlocks = (long)rdr["PowBlocks"],
                        PosBlocks24hr = (long)rdr["PosBlocks24hr"],
                        PowBlocks24hr = (long)rdr["PowBlocks24hr"]
                    };

                    szDataset = JsonConvert.SerializeObject(oTemp, Formatting.None,
                        new JsonSerializerSettings
                        {
                            DateFormatHandling = DateFormatHandling.IsoDateFormat
                        });
                    break;
                }

                return szDataset;
            }
            catch (Exception ex)
            {
                var o = ex.Message;
                return "";
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }
        }  

        public static List<PieChartDataPoint> GetDenomSupplyDataFromDB(int dateRange)
        {
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "sp_GetDenomSupplyData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DateRange", dateRange));
                cmd.CommandTimeout = 600;

                // execute the command
                var rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console               
                var oStakeData = new DenomSupplyData();
                while (rdr.Read())
                {
                    oStakeData.MaxBlock = (long)rdr["MaxBlock"];
                    oStakeData.MaxBlockDate = (DateTime)rdr["MaxBlockDate"];
                    oStakeData.AvgZerocoinSupply = (long)rdr["AvgZerocoinSupply"];
                    oStakeData.AvgAmount10s = (long)rdr["AvgAmount10s"];
                    oStakeData.AvgAmount100s = (long)rdr["AvgAmount100s"];
                    oStakeData.AvgAmount1000s = (long)rdr["AvgAmount1000s"];
                    oStakeData.AvgAmount10000s = (long)rdr["AvgAmount10000s"];
                    oStakeData.Percent10s = (decimal)rdr["Percent10s"];
                    oStakeData.Percent100s = (decimal)rdr["Percent100s"];
                    oStakeData.Percent1000s = (decimal)rdr["Percent1000s"];
                    oStakeData.Percent10000s = (decimal)rdr["Percent10000s"];
                }

                var colDataPoints = new List<PieChartDataPoint>
                {
                    new PieChartDataPoint() { Y = oStakeData.Percent10s, label = "10s" },
                    new PieChartDataPoint() { Y = oStakeData.Percent100s, label = "100s" },
                    new PieChartDataPoint() { Y = oStakeData.Percent1000s, label = "1000s" },
                    new PieChartDataPoint() { Y = oStakeData.Percent10000s, label = "10000s" }
                };
                return colDataPoints;
            }
            catch (Exception ex)
            {
                var o = ex.Message;
                return new List<PieChartDataPoint>();
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }
        }

        public static List<PieChartDataPoint> GetWinnningDenomDataFromDB(int dateRange)
        {
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "sp_GetWinningDenomData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DateRange", dateRange));
                cmd.CommandTimeout = 600;

                // execute the command
                var rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console               
                var oStakeData = new DenomStakeData();
                while (rdr.Read())
                {
                    oStakeData.Count10s = (int)rdr["Count10s"];
                    oStakeData.Count100s = (int)rdr["Count100s"];
                    oStakeData.Count1000s = (int)rdr["Count1000s"];
                    oStakeData.Count10000s = (int)rdr["Count10000s"];
                    oStakeData.Percent10s = (decimal)rdr["Percent10s"];
                    oStakeData.Percent100s = (decimal)rdr["Percent100s"];
                    oStakeData.Percent1000s = (decimal)rdr["Percent1000s"];
                    oStakeData.Percent10000s = (decimal)rdr["Percent10000s"];
                }
                var colDataPoints = new List<PieChartDataPoint>
                {
                    new PieChartDataPoint() { Y = oStakeData.Percent10s, label = "10s" },
                    new PieChartDataPoint() { Y = oStakeData.Percent100s, label = "100s" },
                    new PieChartDataPoint() { Y = oStakeData.Percent1000s, label = "1000s" },
                    new PieChartDataPoint() { Y = oStakeData.Percent10000s, label = "10000s" }
                };
                return colDataPoints;
            }
            catch (Exception ex)
            {
                var o = ex.Message;
                return new List<PieChartDataPoint>();
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }
        }

        public static MultiSeriesLineChart GetDifficultyDataFromDB(int blockType, int dateRange)
        {
            var _dbVeilContext = new VeilContext();
            try
            {
                _dbVeilContext.Database.Connection.Open();

                var cmd = _dbVeilContext.Database.Connection.CreateCommand();
                cmd.CommandText = "sp_GetDiffLineGraphData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@BlockType", blockType)); // DiffType 1 = PoS, 2 = PoW
                cmd.Parameters.Add(new SqlParameter("@DateRange", dateRange));
                cmd.CommandTimeout = 600;

                // execute the command
                var rdr = cmd.ExecuteReader();

                // iterate through results, printing each to console      
                var oLineGraph = new MultiSeriesLineChart();

                var dLastDiff = 0m;
                var dLastMa720Blocks = 0m;
                var dLastMa1440Blocks = 0m;
                var dLastMa4320Blocks = 0m;
                while (rdr.Read())
                {
                    var dtBlockTime = ((DateTime)rdr["BlockDate"]).ToString("dd MMM yyyy HH:mm:ss UTC");
                    oLineGraph.LastBlockTime = dtBlockTime;

                    var dLastDiffTemp = Convert.ToDecimal(rdr["Diff"]);
                    if(dLastDiffTemp > 0)
                    {
                        dLastDiff = dLastDiffTemp;
                    }

                    var dLastMa720BlocksTemp = Convert.ToDecimal(rdr["Ma720Blocks"]);
                    if (dLastMa720BlocksTemp > 0)
                    {
                        dLastMa720Blocks = dLastMa720BlocksTemp;
                    }
                    var dLastMa1440BlocksTemp = Convert.ToDecimal(rdr["Ma1440Blocks"]);
                    if (dLastMa1440BlocksTemp > 0)
                    {
                        dLastMa1440Blocks = dLastMa1440BlocksTemp;
                    }
                    var dLastMa4320BlocksTemp = Convert.ToDecimal(rdr["Ma4320Blocks"]);
                    if (dLastMa4320BlocksTemp > 0)
                    {
                        dLastMa4320Blocks = dLastMa4320BlocksTemp;
                    }
                    oLineGraph.Series1.Add(new LineGraphDataPointDecimal((long)rdr["BlockID"], dLastDiff, dtBlockTime));
                    oLineGraph.Series2.Add(new LineGraphDataPointDecimal((long)rdr["BlockID"], dLastMa720Blocks, dtBlockTime));
                    oLineGraph.Series3.Add(new LineGraphDataPointDecimal((long)rdr["BlockID"], dLastMa1440Blocks, dtBlockTime));
                    oLineGraph.Series4.Add(new LineGraphDataPointDecimal((long)rdr["BlockID"], dLastMa4320Blocks, dtBlockTime));
                }

                return oLineGraph;
            }
            catch (Exception ex)
            {
                var o = ex.Message;
                return new MultiSeriesLineChart();
            }
            finally
            {
                _dbVeilContext.Database.Connection.Close();
            }
        }
    }
}
