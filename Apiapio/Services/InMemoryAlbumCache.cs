using Apiapio.Models;
using System.Collections.Concurrent;

namespace Apiapio.Services
{
    public class InMemoryAlbumCache
    {
        private readonly ConcurrentDictionary<int, AlbumDto> _cache = new();
        private int _nextId = 2001;

        public void Add(AlbumDto album)
        {
            album.Id = _nextId++;
            _cache.TryAdd(album.Id, album);
        }

        public bool Update(int id, AlbumDto album)
        {
            if (_cache.ContainsKey(id))
            {
                album.Id = id;
                _cache[id] = album;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            return _cache.TryRemove(id, out _);
        }

        public AlbumDto? Get(int id)
        {
            _cache.TryGetValue(id, out var album);
            return album;
        }

        public IEnumerable<AlbumDto> GetAll()
        {
            return _cache.Values;
        }

        public IEnumerable<AlbumDto> GetByUserId(int userId)
        {
            return _cache.Values.Where(a => a.UserId == userId);
        }
    }
}
