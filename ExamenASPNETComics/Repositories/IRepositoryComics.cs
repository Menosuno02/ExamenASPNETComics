using ExamenASPNETComics.Models;

namespace ExamenASPNETComics.Repositories
{
    public interface IRepositoryComics
    {
        public List<Comic> GetComics();
        public void CreateComic(Comic comic);
        public void CreateComicProcedure(Comic comic);
        public Comic FindComic(int id);
        public void DeleteComic(int id);
    }
}
