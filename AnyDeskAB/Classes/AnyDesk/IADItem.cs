using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnyDeskAB.Classes.AnyDesk {
    public interface IADItem {
        event EventHandler NameChanged;

        string Id { get; }

        string Name {
            get;
            set;
        }

        ADItem Parent { get; }

        XElement ToXML();
        ADItem Clone(ADItem parent);
    }
}
