using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Models
{
    public class UserLabel
    {
        public Guid UserId { get; set; }
        public Guid LabelId { get; set; }
    }
}