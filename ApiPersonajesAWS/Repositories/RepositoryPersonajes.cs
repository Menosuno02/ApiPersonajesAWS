using ApiPersonajesAWS.Data;
using ApiPersonajesAWS.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

#region PROCEDURES
/*
USE television;
DROP PROCEDURE IF EXISTS SP_UPDATEPERSONAJE;

DELIMITER //
CREATE PROCEDURE SP_UPDATEPERSONAJE (
  IN PARAM_ID INT,
  IN PARAM_NOMBRE VARCHAR(60),
  IN PARAM_IMAGEN VARCHAR(250)
)
BEGIN
  UPDATE PERSONAJES
  SET PERSONAJE = PARAM_NOMBRE,
       IMAGEN = PARAM_IMAGEN
  WHERE IDPERSONAJE = PARAM_ID;

END; //
DELIMITER ;
*/
#endregion

namespace ApiPersonajesAWS.Repositories
{
    public class RepositoryPersonajes
    {
        private PersonajesContext context;
        private MySqlConnection cn;
        private MySqlCommand com;

        public RepositoryPersonajes(PersonajesContext context, IConfiguration configuration)
        {
            this.context = context;
            this.cn = new MySqlConnection(configuration.GetConnectionString("MySqlTelevision"));
            this.com = new MySqlCommand();
            this.com.Connection = this.cn;
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

        public async Task UpdatePersonajeAsync(Personaje personaje)
        {
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_UPDATEPERSONAJE";
            this.com.Parameters.Add(new MySqlParameter("@PARAM_ID", personaje.IdPersonaje));
            this.com.Parameters.Add(new MySqlParameter("@PARAM_NOMBRE", personaje.Nombre));
            this.com.Parameters.Add(new MySqlParameter("@PARAM_IMAGEN", personaje.Imagen));
            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
        }
    }
}
