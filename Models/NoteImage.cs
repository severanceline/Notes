using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noots.Models
{
    public class NoteImage
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public Guid NoteId { get; set; }
    }
}