using ExamenASPNETComics.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE OR ALTER PROCEDURE SP_CREATE_COMIC
(@NOMBRE NVARCHAR(150), @IMAGEN NVARCHAR(600), @DESCRIPCION NVARCHAR(500))
AS
DECLARE @IDCOMIC INT
SELECT @IDCOMIC = MAX(IDCOMIC) + 1
FROM COMICS
INSERT INTO COMICS VALUES
(@IDCOMIC, @NOMBRE, @IMAGEN, @DESCRIPCION)
GO
*/

#endregion

namespace ExamenASPNETComics.Repositories
{
    public class RepositoryComicsSQLServer : IRepositoryComics
    {
        private DataTable tablaComics;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryComicsSQLServer()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=sa;Password=MCSD2023;Encrypt=False";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.tablaComics = new DataTable();
            string sql = "SELECT * FROM COMICS";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.cn);
            adapter.Fill(this.tablaComics);
        }

        public List<Comic> GetComics()
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           select datos;
            List<Comic> comics = new List<Comic>();
            foreach (var row in consulta)
            {
                Comic comic = new Comic
                {
                    IdComic = row.Field<int>("IDCOMIC"),
                    Nombre = row.Field<string>("NOMBRE"),
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };
                comics.Add(comic);
            }
            return comics;
        }

        public void CreateComic(Comic comic)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           select datos;
            int idComic = consulta.Max(comic => comic.Field<int>("IDCOMIC")) + 1;
            string sql = "INSERT INTO COMICS VALUES" +
                "(@IDCOMIC, @NOMBRE, @IMAGEN, @DESCRIPCION)";
            this.com.Parameters.AddWithValue("@IDCOMIC", idComic);
            this.com.Parameters.AddWithValue("@NOMBRE", comic.Nombre);
            this.com.Parameters.AddWithValue("@IMAGEN", comic.Imagen);
            this.com.Parameters.AddWithValue("@DESCRIPCION", comic.Descripcion);
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void DeleteComic(int id)
        {
            string sql = "DELETE FROM COMICS WHERE IDCOMIC = @IDCOMIC";
            this.com.Parameters.AddWithValue("@IDCOMIC", id);
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public Comic FindComic(int id)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           where datos.Field<int>("IDCOMIC") == id
                           select datos;

            if (consulta.Count() == 0)
            {
                return null;
            }
            var row = consulta.First();
            Comic comic = new Comic
            {
                IdComic = row.Field<int>("IDCOMIC"),
                Nombre = row.Field<string>("NOMBRE"),
                Descripcion = row.Field<string>("DESCRIPCION"),
                Imagen = row.Field<string>("IMAGEN")
            };
            return comic;
        }

        public void CreateComicProcedure(Comic comic)
        {
            this.com.Parameters.AddWithValue("@NOMBRE", comic.Nombre);
            this.com.Parameters.AddWithValue("@IMAGEN", comic.Imagen);
            this.com.Parameters.AddWithValue("@DESCRIPCION", comic.Descripcion);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_CREATE_COMIC";
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
