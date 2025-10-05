using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;


namespace GS.Trade.QuoteDownLoader.Works
{
    public class TickToBar : IWorkItem
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        
        [XmlIgnore]
        public WorkContainer Works { get; set; }

        public int SqlExecTimeOut { get; set; }
        public string CompressedTickDB { get; set; }
        public string TickBarDB { get; set; }
        
        public void DoWork()
        {
            var res = TickCompressedToDb();
        }

        private int TickCompressedToDb()
        {
            var ret = 0;
            using (var conn = new SqlConnection(CompressedTickDB))
            {
                using (var command = new SqlCommand
                                  {
                                      Connection = conn,
                                      CommandText = "dbo.NewCompressedTicks",
                                      CommandType = CommandType.StoredProcedure,
                                      CommandTimeout = SqlExecTimeOut
                                  })
                {
                    var retpar = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                    retpar.Direction = ParameterDirection.ReturnValue;
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        ret = (int) retpar.Value;
                        Works.EvlMessage(EvlResult.SUCCESS, "Compressed Ticks:", ret.ToString());
                    }
                    catch (Exception ex)
                    {
                        Works.EvlMessage(EvlResult.FATAL, "Compressing Ticks", ex.Message);
                    }
                }
            }
            return ret;
        }
    }
}
