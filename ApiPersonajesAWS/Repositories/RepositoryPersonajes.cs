using ApiPersonajesAWS.Data;
using ApiPersonajesAWS.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPersonajesAWS.Repositories
{
    public class RepositoryPersonajes
    {
        private PersonajesContext context;

        public RepositoryPersonajes(PersonajesContext context)
        {
            this.context = context;
        }

        public async Task<List<Personaje>> GetPersonajesAsync()
        {
            return await this.context.Personajes.ToListAsync();
        }

        public async Task<Personaje> FindPersonajeAsync(int id)
        {
            return await this.context.Personajes.FirstOrDefaultAsync(p => p.IdPersonaje == id);
        }

        private async Task<int> GetMaxIdPersonajeAsync()
        {
            return await this.context.Personajes.MaxAsync(p => p.IdPersonaje) + 1;
        }

        public async Task CreatePersonajeAsync(string nombre, string imagen)
        {
            Personaje personaje = new Personaje
            {
                IdPersonaje = await GetMaxIdPersonajeAsync(),
                Nombre = nombre,
                Imagen = imagen
            };
            await this.context.Personajes.AddAsync(personaje);
            await this.context.SaveChangesAsync();
        }
    }
}
