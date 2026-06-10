using CashDriver.Data;
using CashDriver.Models;
using Microsoft.EntityFrameworkCore;

namespace CashDriver.Services
{
    public class PersistenceService
    {
        private readonly AppDbContext _dbContext;

        public PersistenceService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SalvarJornadaAsync(Jornada jornada)
        {
            try
            {
                var existente = await _dbContext.Jornadas.FirstOrDefaultAsync(x => x.Id == jornada.Id);
                if (existente == null)
                {
                    await _dbContext.Jornadas.AddAsync(jornada);
                }
                else
                {
                    _dbContext.Entry(existente)
                    .CurrentValues
                    .SetValues(jornada);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Jornada?> ObterJornadaAtivaAsync()
        {
            try
            {
                var jornada = await _dbContext
                    .Jornadas
                    //.AsNoTracking()
                    .Include(j => j.Ganhos)
                    .Include(j => j.Despesas)
                    .ThenInclude(d => d.Tipo)

                    .FirstOrDefaultAsync(x => (x.Status == Models.Enums.EnumStatusJornada.Ativa || x.Status == Models.Enums.EnumStatusJornada.Pausa));

                return jornada;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task SeedPlataformasAsync()
        {
            try
            {
                var existe = await _dbContext.Plataformas.AnyAsync();
                if (!existe)
                {
                    var plataformas = new List<Plataforma>();
                    plataformas.Add(new Plataforma { Name = "Uber", CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "99", CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "InDriver", CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "Maxim" , CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "V1" , CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "XCarro" , CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "Cabify" , CreatedAt = DateTime.Now });
                    plataformas.Add(new Plataforma { Name = "Didi" , CreatedAt = DateTime.Now });
                    await _dbContext.AddRangeAsync(plataformas);
                    await _dbContext.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SeedTiposDespesaAsync()
        {
            try
            {
                var existe = await _dbContext.TiposDespesa.AnyAsync();

                if (!existe)
                {
                    var tiposDespesas = new List<TipoDespesa>();

                    tiposDespesas.Add(
                        new TipoDespesa
                        {
                            DescricaoTipo = "Combustível",
                            Tipo = Models.Enums.EnumTipoDespesa.Combustivel,
                            NomeIcone = "fuel_black_50dp.png"
                        });

                    tiposDespesas.Add(
                        new TipoDespesa
                        {
                            DescricaoTipo = "Alimentação",
                            Tipo = Models.Enums.EnumTipoDespesa.Alimentacao,
                            NomeIcone = "food_black_50dp.png"
                        });

                    tiposDespesas.Add(
                        new TipoDespesa
                        {
                            DescricaoTipo = "Manutenção",
                            Tipo = Models.Enums.EnumTipoDespesa.Manutencao,
                            NomeIcone = "build_black_50dp.png"
                        });

                    tiposDespesas.Add(
                        new TipoDespesa
                        {
                            DescricaoTipo = "Limpeza",
                            Tipo = Models.Enums.EnumTipoDespesa.Limpeza,
                            NomeIcone = "clean_black_50dp.png"
                        });

                    tiposDespesas.Add(
                        new TipoDespesa
                        {
                            DescricaoTipo = "Saúde",
                            Tipo = Models.Enums.EnumTipoDespesa.Saude,
                            NomeIcone = "health_black_50dp.png"
                        });

                    tiposDespesas.Add(
                        new TipoDespesa
                        {
                            DescricaoTipo = "Diversos",
                            Tipo = Models.Enums.EnumTipoDespesa.Diversos,
                            NomeIcone = "miscellaneous_black_50dp.png"
                        });

                    await _dbContext.AddRangeAsync(tiposDespesas);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Jornada>> ObterJornadasAsync()
        {
            return await _dbContext.Jornadas.AsNoTracking().OrderByDescending(j => j.Inicio).ToListAsync();
        }

        public async Task<List<Plataforma>> ObterPlataformasAsync()
        {
            try
            {
                await SeedPlataformasAsync();
                return await _dbContext.Plataformas.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }   

        public async Task<List<TipoDespesa>> ObterTiposDespesaAsync()
        {
            try
            {
                await SeedTiposDespesaAsync();
                return await _dbContext.TiposDespesa.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

       


        public async Task CriarTipoDespesaAsync()
        {

        }

        public async Task AdicionarGanhoAsync(Ganho ganho)
        {
            await _dbContext.Ganhos.AddAsync(ganho);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AdicionarDespesaAsync(Despesa despesa)
        {
            await _dbContext.Despesas.AddAsync(despesa);
            await _dbContext.SaveChangesAsync();   
        }

        public async Task RemoverDespesa(Despesa despesa)
        {
            _dbContext.Despesas.Remove(despesa);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoverGanho(Ganho ganho)
        {
            _dbContext.Ganhos.Remove(ganho);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ExportarBancoAsync()
        {
            var origem = DatabaseConstants.DatabasePath;

            var destino = Path.Combine(
                FileSystem.Current.AppDataDirectory,
                "cashdriver_export.db3");

            File.Copy(origem, destino, true);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Exportar banco",
                File = new ShareFile(destino)
            });
        }
    }
}
