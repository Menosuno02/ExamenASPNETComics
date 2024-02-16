using ExamenASPNETComics.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE OR REPLACE PROCEDURE SP_CREAR_COMIC
(P_NOMBRE COMICS.NOMBRE%TYPE,
P_IMAGEN COMICS.IMAGEN%TYPE,
P_DESCRIPCION COMICS.DESCRIPCION%TYPE)
AS
P_IDCOMIC COMICS.IDCOMIC%TYPE;
BEGIN
  SELECT MAX(IDCOMIC) + 1 INTO P_IDCOMIC
  FROM COMICS;
  INSERT INTO COMICS VALUES
  (P_IDCOMIC, P_NOMBRE, P_IMAGEN, P_DESCRIPCION);
  COMMIT;
END; 
*/

#endregion

namespace ExamenASPNETComics.Repositories
{
    public class RepositoryComicsOracle : IRepositoryComics
    {
        private DataTable tablaComics;
        private OracleConnection cn;
        private OracleCommand com;

        public RepositoryComicsOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User ID=SYSTEM; Password=oracle";
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand();
            this.com.Connection = this.cn;
            this.tablaComics = new DataTable();
            string sql = "SELECT * FROM COMICS";
            OracleDataAdapter adapter = new OracleDataAdapter(sql, this.cn);
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
                    Descripcion = row.Field<string>("DESCRIPCION"),
                    Imagen = row.Field<string>("IMAGEN")
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
                "(:IDCOMIC, :NOMBRE, :IMAGEN, :DESCRIPCION)";
            OracleParameter paramId = new OracleParameter(":IDCOMIC", idComic);
            this.com.Parameters.Add(paramId);
            OracleParameter paramNombre = new OracleParameter(":NOMBRE", comic.Nombre);
            this.com.Parameters.Add(paramNombre);
            OracleParameter paramImagen = new OracleParameter(":IMAGEN", comic.Imagen);
            this.com.Parameters.Add(paramImagen);
            OracleParameter paramDescripcion = new OracleParameter(":DESCRIPCION", comic.Descripcion);
            this.com.Parameters.Add(paramDescripcion);
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void CreateComicProcedure(Comic comic)
        {
            OracleParameter paramNombre = new OracleParameter(":P_NOMBRE", comic.Nombre);
            this.com.Parameters.Add(paramNombre);
            OracleParameter paramImagen = new OracleParameter(":P_IMAGEN", comic.Imagen);
            this.com.Parameters.Add(paramImagen);
            OracleParameter paramDescripcion = new OracleParameter(":P_DESCRIPCION", comic.Descripcion);
            this.com.Parameters.Add(paramDescripcion);
            this.com.CommandText = "SP_CREAR_COMIC";
            this.com.CommandType = CommandType.StoredProcedure;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void DeleteComic(int id)
        {
            string sql = "DELETE FROM COMICS WHERE IDCOMIC=:IDCOMIC";
            OracleParameter paramId = new OracleParameter(":IDCOMIC", id);
            this.com.Parameters.Add(paramId);
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
    }
}
