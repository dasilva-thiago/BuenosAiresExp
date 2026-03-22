using System;
using System.Collections.Generic;
using System.Text;
using BuenosAiresExp.Data;
using BuenosAiresExp.Models;
using Microsoft.EntityFrameworkCore;

namespace BuenosAiresExp.Services
{
    public class LocationService
    {
        private readonly AppDbContext _context;
        // Service precisa falar com o banco de dados, e o AppDbContext é a classe que representa essa conexão. Então, criamos uma variável privada _context do tipo AppDbContext para usar dentro da classe LocationService.
        // ela é readonly porque só queremos atribuir um valor a ela uma vez, no construtor, e depois não queremos que seja modificada.
        // o valor atribuído a _context é uma nova instância de AppDbContext, que é criada no construtor da classe LocationService.


        public LocationService()

        {
            _context = new AppDbContext();
            // Esse construtor é chamado quando criamos uma nova instância de LocationService. Ele inicializa a variável _context com uma nova instância de AppDbContext, o que significa que toda vez que criamos um LocationService, ele tem sua própria conexão com o banco de dados.
        }

        public List<Location> GetAll()
        {
            return _context.Locations.ToList();
        }
        // O método GetAll() retorna uma lista de todas as localizações armazenadas no banco de dados. Ele usa o DbSet Locations do AppDbContext para acessar os dados e o método ToList() para converter o resultado em uma lista.


        public void Add(Location location)
        {
            _context.Locations.Add(location);
            _context.SaveChanges();
            // O método Add(Location location) recebe um objeto Location como parâmetro, adiciona esse objeto à coleção Locations do AppDbContext e depois chama SaveChanges() para salvar as alterações no banco de dados. Isso significa que a nova localização será inserida na tabela correspondente no banco de dados.
        }

        public void Delete(int id)
        {
            var location = _context.Locations.Find(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
                _context.SaveChanges();
            }
            // O método Delete(int id) recebe um ID como parâmetro, usa o método Find() para localizar a localização correspondente no banco de dados e, se encontrada, remove essa localização da coleção Locations e salva as alterações. Isso resulta na exclusão da localização do banco de dados.
        }
        public void Update(Location updated)
        {
            var existing = _context.Locations.Find(updated.Id);
            if (existing == null) return;
            
            existing.Name = updated.Name;
            existing.Category = updated.Category;
            existing.Address = updated.Address;
            existing.Latitude = updated.Latitude;
            existing.Longitude = updated.Longitude;
            existing.Notes = updated.Notes;

            _context.SaveChanges();   
        }
    }
}
