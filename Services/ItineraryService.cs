using System;
using System.Collections.Generic;
using System.Text;
using BuenosAiresExp.Models;
using BuenosAiresExp.Data;
using Microsoft.EntityFrameworkCore;

namespace BuenosAiresExp.Services
{
    public class ItineraryService
    {
        private readonly AppDbContext _context;

        public ItineraryService()
        {
            _context = new AppDbContext();
        }

        public List<Itinerary> GetAll()
        {
            return _context.Itineraries
                .Include(r => r.Items)
                .ThenInclude(i => i.Location)
                .OrderByDescending(r => r.Date)
                .ToList();
        }

        public void Add(Itinerary itinerary)
        {
            NormalizeOrder(itinerary);
            _context.Itineraries.Add(itinerary);
            _context.SaveChanges();
        }

        public void Update(Itinerary updated)
        {
            var existing = _context.Itineraries
                .Include(r => r.Items)
                .FirstOrDefault(r => r.Id == updated.Id);

            if (existing == null) return;

            existing.Name = updated.Name;
            existing.Date = updated.Date;
            existing.Notes = updated.Notes;

            _context.ItineraryItems.RemoveRange(existing.Items);

            foreach (var item in updated.Items)
            {
                item.ItineraryId = existing.Id;
                item.Id = 0;
            }

            NormalizeOrder(updated);
            existing.Items = updated.Items;

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var itinerary = _context.Itineraries
                .Include(i => i.Items)
                .FirstOrDefault(i => i.Id == id);
            if (itinerary == null) return;

            // Remove todos os itens relacionados primeiro
            _context.ItineraryItems.RemoveRange(itinerary.Items);

            _context.Itineraries.Remove(itinerary);
        
            _context.SaveChanges();
        }

        private static void NormalizeOrder(Itinerary itinerary)
        {
            for (int i = 0; i < itinerary.Items.Count; i++)
                itinerary.Items[i].Order = i;
        }
    }
}
