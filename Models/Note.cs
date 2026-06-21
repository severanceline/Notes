using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noots.Models
{
    public class Note
    {
        public NoteInfo NoteInfo { get; set; }
        public List<NoteImage> noteImages { get; set; }
    }
}
