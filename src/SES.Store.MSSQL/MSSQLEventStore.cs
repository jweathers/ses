using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SES.Core;

namespace SES.Store.MSSQL
{
    public class MSSQLEventStore : IEventStore,IDisposable
    {
        private readonly SqlConnection connection;
        private readonly bool connectionIsSelfOwned;

        protected MSSQLEventStore(SqlConnection connection, bool connectionIsSelfOwned)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.connectionIsSelfOwned = connectionIsSelfOwned;
        }
        public MSSQLEventStore(SqlConnection connection):this(connection,false)
        {
        }
        public MSSQLEventStore(string connectionString):this(new SqlConnection(connectionString),true)
        {           
        }
        public async Task<System.Collections.Generic.IEnumerable<SES.Core.Event>> FetchAsync(string queue, ulong startIndex, uint count)
        {
            const string sql="SELECT TOP(@count) id,data,queuename FROM SQSEvents WHERE id>=@startIndex and queuename=@queue";
            return await connection.QueryAsync<Event>(sql,new{startIndex=(long)startIndex,count=(int)count,queue})
                                    .ConfigureAwait(false);
        }

        public async Task StoreAsync(string queue, string data)
        {
            const string sql = "INSERT INTO SQSEvents (data,queuename) VALUES (@data,@queue)";
            await connection.ExecuteAsync(sql,new {data,queue});
        }

        public void Dispose()
        {
            if(connectionIsSelfOwned)
            {
                connection.Dispose();
            }
        }
    }
}
