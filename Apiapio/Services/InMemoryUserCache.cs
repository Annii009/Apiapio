using Apiapio.Models;
using System.Collections.Concurrent;

namespace Apiapio.Services
{
    public class InMemoryUserCache
    {
        private readonly ConcurrentDictionary<int, UserDto> _cache = new();
        private int _nextId = 1001;

        public void Add(UserDto user)
        {
            user.Id = _nextId++;
            _cache.TryAdd(user.Id, user);
        }

        public bool Update(int id, UserDto user)
        {
            if (_cache.ContainsKey(id))
            {
                user.Id = id;
                _cache[id] = user;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            return _cache.TryRemove(id, out _);
        }

        public UserDto? Get(int id)
        {
            _cache.TryGetValue(id, out var user);
            return user;
        }

        public IEnumerable<UserDto> GetAll()
        {
            return _cache.Values;
        }
    }
}
