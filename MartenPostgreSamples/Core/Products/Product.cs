using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Core.Products
{
    public class Product : RootEntity
    {
        readonly List<string> _tags = new List<string>();

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<string> Tags => _tags;

        public void AddTag(string tag)
        {
            if (_tags.Contains(tag))
                throw new Exception($"Tag {tag} is already added. Please choose an unique tag.");
            _tags.Add(tag);
        }

        public void Remove(string tag)
        {
            _tags.Remove(tag);
        }
    }
}
