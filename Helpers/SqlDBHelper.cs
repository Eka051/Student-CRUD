using Npgsql;
using System.Data;

namespace Student_CRUD.Helpers
{
    public class SqlDBHelper
    {
        private NpgsqlConnection connection;
        private string constr;

        public SqlDBHelper(string PConstr)
        {
            constr = PConstr;
            connection = new NpgsqlConnection(constr);
        }

        public NpgsqlCommand GetCommand(string query)
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        public void CloseConnection()
        {
            connection.Close();
        }
    }
}
